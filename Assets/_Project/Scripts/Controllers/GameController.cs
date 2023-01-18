using CarMasters.UI.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public RaceCameraGood raceCamera;
    public CarSpawner carSpawner;
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
        //yield return levelController.LoadLevelRoutine();

        var trackController = levelController?.Track;
        yield return carSpawner.Init(trackController, raceCamera);
        yield return trackController.Init(carSpawner.Player.SplineProjector, splineNumberDebug);
        yield return raceCamera.Init(carSpawner.Player);
        yield return playerInput.Init(carSpawner.Player);

        callBack?.Invoke();
    }
}