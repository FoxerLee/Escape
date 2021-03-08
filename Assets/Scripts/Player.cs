using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Player : MovingObject
{
    public int wallDamage = 1;
    public int pointsPerFood = 10;
    public int pointsPerSoda = 20;
    public int sanityUpdateTime = 2;
    public Vector3 uiStartPosition;
    public Vector3 uiEndPosition;
    public Transform bloodMask;
    public Transform sanMask;
    public float restartLevelDelay = 1f;

    Animator animator;
    bool isInLightArea = true;
    float totalUILength;
    int food;
    int san;

    protected override void Start()
    {
        animator = GetComponent<Animator>();
        food = GameController.instance.playerFoodPoints;
        san = GameController.instance.playerSanPoints;
        totalUILength = uiStartPosition.x - uiEndPosition.x;
        base.Start();
        UpdateBlood();
        StartCoroutine(SanCheck());
    }

    protected override void AttempMove<T> (int xDir, int yDir)
    {
        food--;
        UpdateBlood();
        base.AttempMove<T>(xDir, yDir);
        CheckIfGameOver();
        GameController.instance.playerTurn = false;
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log(collider.tag);
        if (collider.tag == "Dark Floor")
            isInLightArea = false;
        if (collider.tag == "Light Floor")
            isInLightArea = true;
        if (collider.tag == "Food")
        {
            food += pointsPerFood;
            collider.gameObject.SetActive(false);
        }
        else if (collider.tag == "Soda")
        {
            food += pointsPerSoda;
            collider.gameObject.SetActive(false);
        }
        else if (collider.tag == "Exit")
        {
            Invoke("Restart", restartLevelDelay);
            enabled = false;
        }

        UpdateBlood();
        //Debug.Log(food);
    }

    void OnDisable()
    {
        GameController.instance.playerFoodPoints = food;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameController.instance.playerTurn)
        {
            return;
        }

        int horizotal = 0;
        int vertical = 0;

        horizotal = (int)Input.GetAxisRaw("Horizontal");
        vertical = (int)Input.GetAxisRaw("Vertical");

        if (horizotal != 0)
        {
            vertical = 0;
        }

        if (horizotal != 0 || vertical != 0)
        {
            // GameController.instance.playerTurn = false;
            AttempMove<Wall>(horizotal, vertical);
        }
    }

    void Restart()
    {
        SceneManager.LoadScene(0);
    }

    protected override void OnCantMove<T> (T component)
    {
        Wall hitWall = component as Wall;
        hitWall.DamageWall(wallDamage);
        animator.SetTrigger("playerChop");
    }

    public void LoseFood(int loss)
    {
        animator.SetTrigger("playerHit");
        food -= loss;
        UpdateBlood();
        CheckIfGameOver();
        
    }

    private void CheckIfGameOver()
    {
        if (food <= 0 || san <= 0)
        {
            GameController.instance.GameOver();
        }
    }

    IEnumerator SanCheck() {
        while (true) {
            Debug.Log("san check");
            if (isInLightArea)
            {
                if (san < GameController.instance.playerSanPoints)
                    san++;
            }else {
                san--;
                CheckIfGameOver();
            }

            UpdateSan();
            yield return new WaitForSeconds(sanityUpdateTime);
        }
    }

    void UpdateSan() {
        float percetageSan;
        percetageSan = Mathf.Round(((float)san / (float)GameController.instance.playerSanPoints) * 100.0f) / 100.0f;
        float newX = uiEndPosition.x + percetageSan * totalUILength;
        Vector3 newMaskPostion = new Vector3(newX, uiStartPosition.y, uiStartPosition.x);
        sanMask.localPosition = newMaskPostion;
        print("san: "+san);
    }

    void UpdateBlood() {

        float percentageFood;
        percentageFood = Mathf.Round(((float)food / (float)GameController.instance.playerFoodPoints) * 100.0f) / 100.0f;
        float newX = uiEndPosition.x + percentageFood * totalUILength;
        Vector3 newMaskPostion = new Vector3(newX, uiStartPosition.y, uiStartPosition.x);
        bloodMask.localPosition = newMaskPostion;
    }
}
