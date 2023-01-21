using Technoprosper.Gameplay.Camera;
using Technoprosper.Input.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public RaceCamera raceCamera;
    public PlayerSpawner playerSpawner;
    public LevelController levelController;
    public PlayerInput playerInput;

    public int splineNumberDebug = 0;

    private void Start()
    {
        //StartGame();
    }

    public void StartGame(Action callBack)
    {
        StartCoroutine(StartGameRoutine(callBack));
    }

    private IEnumerator StartGameRoutine(Action callBack)
    {
        yield return levelController.LoadLevelRoutine();

        var trackController = levelController?.Track;
        yield return playerSpawner.Spawn(trackController, raceCamera);
        yield return trackController.Init(playerSpawner.Player.SplineProjector, splineNumberDebug);
        yield return raceCamera.Init(playerSpawner.Player);
        yield return playerInput.Init(playerSpawner.Player); 

        callBack?.Invoke();
    }
}