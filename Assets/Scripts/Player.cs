using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Player : MovingObject
{
    public int wallDamage = 1;
    public int pointsPerFood = 10;
    public int pointsPerSoda = 20;
    public float restartLevelDelay = 1f;

    Animator animator;
    int food;

    protected override void Start()
    {
        animator = GetComponent<Animator>();
        food = GameController.instance.playerFoodPoints;
        base.Start();
    }

    protected override void AttempMove<T> (int xDir, int yDir)
    {
        food--;
        base.AttempMove<T>(xDir, yDir);
        CheckIfGameOver();
        GameController.instance.playerTurn = false;
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
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
        Debug.Log(food);
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
        CheckIfGameOver();
        
    }

    private void CheckIfGameOver()
    {
        if (food <= 0)
        {
            GameController.instance.GameOver();
        }
    }
}
