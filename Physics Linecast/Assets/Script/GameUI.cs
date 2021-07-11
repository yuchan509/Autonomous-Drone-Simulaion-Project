using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameUI : MonoBehaviour
{
    public GameObject gameLoseUI;
    public GameObject gameWinUI;
    bool gameIsOver;

    void Start()
    {
        Guard.OnGuardHasSpottedPlayer += showGameLoseUI;
        FindObjectOfType<Player>().OnReachedEndOfLevel += showGameWinUI;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameIsOver)
        { 
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(0);
            }
        }
    }

    void showGameWinUI()
    {
        OnGameOver(gameWinUI);
    }

    void showGameLoseUI()
    {
        OnGameOver(gameLoseUI);
    }

    void OnGameOver(GameObject gameOverUI)
    {
        gameOverUI.SetActive(true);
        gameIsOver = true;
        Guard.OnGuardHasSpottedPlayer -= showGameLoseUI;
        FindObjectOfType<Player>().OnReachedEndOfLevel -= showGameWinUI;
    }
}
