using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FinishGamePanel : MonoBehaviour
{
    public GameController gameController;
    //TODO: remove from here and use only gameController
    public LevelController levelController;
    public Button nextLevelButton;

    //---------------------------------------------------------------

    private void Awake()
    {
        //loadLevelButton.onClick.AddListener(() => levelController.LoadLevel());
        //startLevelButton.onClick.AddListener(() => gameController.StartGame(AfterStart));
        nextLevelButton.onClick.AddListener(() => SceneLoader.Instance.LoadNextLevel());
        //startDriveButton.onClick.AddListener(() => AfterStart());
    }

    private void OnDestroy()
    {
        nextLevelButton.onClick.RemoveAllListeners();
    }

    public void SetActive(bool value)
    {
        gameObject.SetActive(value);
    }
}