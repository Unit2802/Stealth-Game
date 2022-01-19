using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    Player player;

    public GameObject gameLoseUI;
    public GameObject gameWinUI;
    bool gameIsOver;
    int hasPlayerWon = 0;

    GameData gd;
    
    // Start is called before the first frame update
    void Start()
    {
        Guard.OnGuardHasSpottedPlayer += ShowGameLoseUI;
        FindObjectOfType<Player>().OnReachedEndOfLevel += ShowGameWinUI;

        
    }

    // Update is called once per frame
    void Update()
    {
        if (gameIsOver)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene(0);

            }
        }
        
        if (player == null)
        {
            player = FindObjectOfType<Player>();
        }

        if(gd == null)
        {
            gd = FindObjectOfType<GameData>();
        }

        if(player.hasReachedEndOfLevel)
        {
           
            if (Input.GetKeyDown(KeyCode.Space))
            {
              SceneManager.LoadScene(hasPlayerWon);

            }
           
        }
    }

    void ShowGameWinUI()
    {
        OnGameOver(gameWinUI);
        gd.scene++;
        hasPlayerWon = gd.scene;
        gameIsOver = false;
    }

    void ShowGameLoseUI()
    {
        OnGameOver(gameLoseUI);
        gameIsOver = true;
        gd.scene = 0;
    }

    void OnGameOver(GameObject gameOverUI)
    {
        
        gameOverUI.SetActive(true);
        Guard.OnGuardHasSpottedPlayer -= ShowGameLoseUI;
        FindObjectOfType<Player>().OnReachedEndOfLevel -= ShowGameWinUI;
    }
}
