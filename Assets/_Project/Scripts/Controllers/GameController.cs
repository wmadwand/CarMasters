using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public RaceCameraGood raceCamera;
    public CarSpawner carSpawner;
    public LevelController levelController;

    private void Start()
    {
        //StartGame();
    }

    public void StartGame()
    {
        StartCoroutine(StartGameRoutine());
    }

    private IEnumerator StartGameRoutine()
    {
        //yield return levelController.LoadLevelRoutine();

        var trackController = levelController?.Track;
        yield return carSpawner.Init(trackController);        
        yield return trackController.Init(carSpawner.Player.SplineProjector);
        yield return raceCamera.Init(carSpawner.Player);
    }
}