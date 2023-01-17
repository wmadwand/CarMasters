using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public TrackController trackController;
    public CarSpawner carSpawner;

    private void Start()
    {
        StartGame();
    }

    public void StartGame()
    {

    }

    private IEnumerator StartGameRoutine()
    {
        yield return trackController.Init();
        yield return carSpawner.Init();
    }
}