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
    public float restartDelay = 1.5f;

    public AudioClip moveSound1;
    public AudioClip moveSound2;
    public AudioClip eatSound1;
    public AudioClip eatSound2;
    public AudioClip drinkSound1;
    public AudioClip drinkSound2;
    public AudioClip gameOverSound;

    Animator animator;
    bool isInLightArea = true;
    float totalUILength;
    int food;
    int san;
    bool gameovered = false;

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
        //Debug.Log(food);
        //Debug.Log(GameController.instance.playerFoodPoints);
        food--;
        UpdateBlood();
        base.AttempMove<T>(xDir, yDir);

        RaycastHit2D hit;
        if (Move(xDir, yDir, out hit))
        {
            SoundManager.instance.RandomizeSfx(moveSound1, moveSound2);
        }

        CheckIfGameOver();
        GameController.instance.playerTurn = false;
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        // Debug.Log(collider.tag);
        if (collider.tag == "Dark Floor")
            isInLightArea = false;
        if (collider.tag == "Light Floor")
            isInLightArea = true;
        if (collider.tag == "Food")
        {
            food += pointsPerFood;
            SoundManager.instance.RandomizeSfx(eatSound1, eatSound2);
            collider.gameObject.SetActive(false);
        }
        else if (collider.tag == "Soda")
        {
            food += pointsPerSoda;
            SoundManager.instance.RandomizeSfx(drinkSound1, drinkSound2);
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
        // food = GameController.instance.playerFoodPoints;
        // san = GameController.instance.playerSanPoints;
        SceneManager.LoadScene(0);
        if (gameovered)
        {
            SoundManager.instance.musicSource.Play();
        }

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

    public void SeedRestart() {
        SoundManager.instance.musicSource.Stop();
        GameController.instance.SeedRestart();
        gameovered = true;
        Invoke("Restart", restartDelay);
    }

    private void CheckIfGameOver()
    {
        if (gameovered)
            return;
        if (food <= 0 || san <= 0)
        {
            SoundManager.instance.musicSource.Stop();
            SoundManager.instance.PlaySingle(gameOverSound);
            GameController.instance.GameOver();
            gameovered = true;

            // GameController.instance.level = 1;
            // GameController.instance.playerFoodPoints = 100;
            // GameController.instance.playerSanPoints = 100;
            Invoke("Restart",  restartDelay);
        }
    }

    IEnumerator SanCheck() {
        while (true) {
            // Debug.Log("san check");
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
        percetageSan = Mathf.Round(((float)san / (float)GameController.instance.playerWholeSanPoints) * 100.0f) / 100.0f;
        if (percetageSan > 1.0f)
        {
            percetageSan = 1.0f;
        }
        float newX = uiEndPosition.x + percetageSan * totalUILength;
        Vector3 newMaskPostion = new Vector3(newX, uiStartPosition.y, uiStartPosition.x);
        sanMask.localPosition = newMaskPostion;
        // print("san: "+san);
    }

    void UpdateBlood() {

        float percentageFood;
        percentageFood = Mathf.Round(((float)food / (float)GameController.instance.playerWholeFoodPoints) * 100.0f) / 100.0f;
        if (percentageFood > 1.0f) 
        {
            percentageFood = 1.0f;
        }
        float newX = uiEndPosition.x + percentageFood * totalUILength;
        Vector3 newMaskPostion = new Vector3(newX, uiStartPosition.y, uiStartPosition.x);
        bloodMask.localPosition = newMaskPostion;
    }
}
