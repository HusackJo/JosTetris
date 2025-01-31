using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TetrisManager : MonoBehaviour
{
    public GameObject GameOverCanvas, ScoreCanvas;
    public Board board;
    public TextMeshProUGUI scoreText;
    private int playerScore;


    private void Start()
    {
        //board.StartSpawning();
    }

    public void UpdateScore(int scoreChange)
    {
        playerScore += scoreChange;
        scoreText.text = playerScore.ToString();
    }

    public void StartGame()
    {
        board.StartSpawning();
        ScoreCanvas.SetActive(true);
    }

    public void StartSpecialBoard()
    {
        board.StartSpawningSpecialSequence();
        ScoreCanvas.SetActive(true);
    }

    public void GameOver()
    {
        ScoreCanvas.SetActive(false);
        board.ClearAllLines();
        board.StopSpawning();
        GameOverCanvas.SetActive(true);
    }

    public void ChangeToSpecialBoardScene()
    {
        SceneManager.LoadScene(1);
    }

    public void ChangeToNormalTetris()
    {
        SceneManager.LoadScene(0);
    }
}
