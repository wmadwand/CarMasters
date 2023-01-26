using Technoprosper.Gameplay.Camera;
using Technoprosper.Input.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Technoprosper.Common;

public class GameController : MonoSingleton<GameController>
{
    public RaceCamera raceCamera;
    public PlayerSpawner playerSpawner;
    public LevelController levelController;
    public PlayerInput playerInput;
    public Transform levelParent;

    public int splineNumberDebug = 0;

    //---------------------------------------------------------------

    private void Start()
    {
        //StartGame();
        //Track.OnFinish += Track_OnFinish;
    }

    private void StartGame(Action callBack = null)
    {
        StartCoroutine(StartGameRoutine(callBack));
    }

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

    private void Track_OnFinish()
    {
        levelController.SaveNextLevel();
    }

    private void OnDestroy()
    {
        LevelFinishTrigger.OnFinish -= Track_OnFinish;
    }
}