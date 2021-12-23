using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class GameStates : MonoBehaviour
{
    public bool gameOver;
    public bool winner;
    public GameObject gameOverPanel;
    public GameObject winnerPanel;
    public event Action OnGameOver;

    void Update()
    {
        if (gameOver)
        {
            if (OnGameOver != null)
            {
                OnGameOver();
            }
            gameOverPanel.SetActive(true);
        }
        
        if (winner)
        {
            if (OnGameOver != null)
            {
                OnGameOver();
            }
            winnerPanel.SetActive(true);
        }
    }

    public void ReloadScene()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

}
