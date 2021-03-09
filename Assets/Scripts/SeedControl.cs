using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SeedControl : MonoBehaviour
{
    public Player player;
    public Text input;
    Button btn;
    BoardManager boardManager;
    // Start is called before the first frame update
    void Start()
    {
        
        btn = GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        if (input.text != "")
            btn.interactable = true;
    }

    public void restartWithNewSeed() {
        boardManager = GameObject.Find("GameManager(Clone)").GetComponent<BoardManager>();
        boardManager.seed = int.Parse(input.text);
        player.SeedRestart();
    }
}
