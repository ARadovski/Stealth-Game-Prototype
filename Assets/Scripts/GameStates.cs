using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class GameStates : MonoBehaviour
{
    public GameObject gameOverPanel;
    public GameObject winnerPanel;
    public event Action OnGameOver;

    private void Start()
    {
        Guard.OnPlayerSpotted += GameOver;
        PlayerController.OnGoalReached += Victory;
    }

    public void ReloadScene()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    void GameOver()
    {
        EndGame(gameOverPanel);
    }

    void Victory()
    {
        EndGame(winnerPanel);
    }

    void EndGame(GameObject panelUI)
    {
        if (OnGameOver != null)
        {
            OnGameOver();
        }
        panelUI.SetActive(true);
        PlayerController.OnGoalReached -= Victory;
        Guard.OnPlayerSpotted -= GameOver;
    }

}
