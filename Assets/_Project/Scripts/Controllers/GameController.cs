using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public Track trackController;
    public CarSpawner carSpawner;
    public LevelController levelController;

    private void Start()
    {
        StartGame();
    }

    public void StartGame()
    {
        StartCoroutine(StartGameRoutine());
    }

    private IEnumerator StartGameRoutine()
    {
        //yield return levelController.LoadLevelRoutine();
        yield return carSpawner.Init(levelController.Track);
        yield return trackController.Init(carSpawner.Player.SplineProjector);

    }
}