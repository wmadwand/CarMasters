using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartGamePanel : MonoBehaviour
{
    public GameController gameController;

    //TODO: remove from here and use only gameController
    public LevelController levelController;

    public Button loadLevelButton;
    public Button startLevelButton;
    public Button startDriveButton;
    public Button resetGame;

    private Action _callback;

    private void Awake()
    {
        resetGame.onClick.AddListener(() => OnResetGame());
    }

    private void OnDestroy()
    {
        resetGame.onClick.RemoveAllListeners();
    }

    private void OnResetGame()
    {
        levelController.ResetGame();
        SceneLoader.Instance.LoadNextLevel();
    }

    public void SetActive(bool value)
    {
        gameObject.SetActive(value);
    }
}