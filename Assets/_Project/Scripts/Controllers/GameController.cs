using System;
using System.Collections;
using Technoprosper.Common;
using Technoprosper.Gameplay.Camera;
using Technoprosper.Input.UI;
using UnityEngine;

public class GameController : MonoSingleton<GameController>
{
    public RaceCamera raceCamera;
    public PlayerSpawner playerSpawner;
    public LevelController levelController;
    public PlayerInput playerInput;
    public Transform levelParent;

    public int splineNumberDebug = 0;

    //---------------------------------------------------------------

    public IEnumerator StartGameRoutine(Action callBack = null)
    {
        LevelFinishTrigger.OnFinish += Track_OnFinish;

        yield return levelController.LoadLevelRoutine(levelParent);
        var trackController = levelController?.Track;
        yield return playerSpawner.Spawn(trackController, raceCamera, levelParent);
        yield return trackController.Init(playerSpawner.Player, splineNumberDebug);
        yield return raceCamera.Init(playerSpawner.Player);
        yield return playerInput.Init(playerSpawner.Player);

        //callBack?.Invoke();
    }

    //---------------------------------------------------------------

    private void Start()
    {
        // For Debuggin
        //StartGame();        
    }

    private void StartGame(Action callBack = null)
    {
        StartCoroutine(StartGameRoutine(callBack));
    }

    private void Track_OnFinish()
    {
        playerInput.SetActive(false);
        playerSpawner.Player.SetFinishSpeed();
        levelController.SaveNextLevel();
    }

    private void OnDestroy()
    {
        LevelFinishTrigger.OnFinish -= Track_OnFinish;
    }
}