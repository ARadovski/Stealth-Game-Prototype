using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameStates : MonoBehaviour
{
    bool gameOver;
    bool winner;
    public GameObject gameOverPanel;
    public GameObject winnerPanel;

    void Start()
    {
        
    }

    void Update()
    {
        if (gameOver){
            gameOverPanel.SetActive(true);
        }

        if (winner)
        {
            winnerPanel.SetActive(true);
        }
    }
}
