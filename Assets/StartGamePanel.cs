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

    private void Awake()
    {
        loadLevelButton.onClick.AddListener(() => levelController.LoadLevel());
        startLevelButton.onClick.AddListener(() => gameController.StartGame());
    }

    private void OnDestroy()
    {
        loadLevelButton.onClick.RemoveAllListeners();
        startLevelButton.onClick.RemoveAllListeners();
    }
}