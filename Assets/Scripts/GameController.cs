using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{

    public static GameController instance = null;
    public int playerFoodPoints;
    public int playerSanPoints;
    public int playerWholeFoodPoints;
    public int playerWholeSanPoints;
    public int level = 1;

    [HideInInspector] public bool playerTurn = true;
    public float turnDelay = 0.1f;
    public float levelStartDelay = 1.5f;
    public float restartDelay = 2.5f;

    BoardManager boardManager;
    List<Enemy> enemies;
    bool enemyMoving;
    bool doingSetup;
    GameObject levelImage;
    Animator levelTextAnimator;
    Text levelText;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

        
        enemies = new List<Enemy>();
        boardManager = GetComponent<BoardManager>();
        
        InitGame();
    }

    void InitGame()
    {
        doingSetup = true;
        levelImage = GameObject.Find("LevelImage");
        levelText = GameObject.Find("LevelText").GetComponent<Text>();
        levelTextAnimator = GameObject.Find("LevelText").GetComponent<Animator>();

        levelTextAnimator.SetBool("show",true);
        levelText.text = "Day " + level;
        // levelText.SetActive(true);
        levelImage.SetActive(true);
        Invoke("HideLevelImage", levelStartDelay);

        enemies.Clear();
        boardManager.SetupScene(level);
    }

    private void HideLevelImage()
    {
        levelImage.SetActive(false);
        levelTextAnimator.SetBool("show", false);
        // levelText.SetActive(false);
        doingSetup = false;
    }
    // Start is called before the first frame update
    void Start()
    {
        SceneManager.sceneLoaded += LevelWasLoaded;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerTurn || enemyMoving || doingSetup)
        {
            return;
        }
        StartCoroutine(MoveEnemys());
    }

    private void LevelWasLoaded(Scene s, LoadSceneMode mode)
    {
        if (level == 0) {
            playerFoodPoints = playerWholeFoodPoints;
            playerSanPoints = playerWholeSanPoints;
        }
        level++;
        Debug.Log("scene name: " + s.name + " level: " + level);
        InitGame();
    }

    public void GameOver()
    {
        levelTextAnimator.SetBool("show", true);
        levelText.text = "You died after " + level + " days.";
        levelImage.SetActive(true);
        level = 0;
        //enabled = false;
    }


    public void AddEnemyToList(Enemy script)
    {
        enemies.Add(script);
    }

    IEnumerator MoveEnemys()
    {
        enemyMoving = true;
        yield return new WaitForSeconds(turnDelay);
        if (enemies.Count == 0)
        {
            yield return new WaitForSeconds(turnDelay);
        }

        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].MoveEnemy();
            yield return new WaitForSeconds(turnDelay);
        }
        enemyMoving = false;
        playerTurn = true;
    }
}
