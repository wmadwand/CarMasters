using Dreamteck.Splines;
using Dreamteck.Splines.Examples;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.UI.Input;
using Game.Gameplay.Player;

public class CarSpawner : MonoBehaviour, IController
{
    public BallCamera camera;
    public PlayerMovementInput speedButton;
    public PlayerRotationInput dragHandler;
    public Vector3 respawnOffset;

    public float respawnTime = 2;
    public Player playerPrefab;

    //---------------------------------------------------------------

    public IEnumerator Init()
    {
        yield return null;
    }



    public void Respawn(Vector3 deadPosition, GameObject prevCar)
    {
        StartCoroutine(RespawnRoutine(deadPosition, prevCar));
    }

    //---------------------------------------------------------------

    private IEnumerator RespawnRoutine(Vector3 deadPosition, GameObject prevCar)
    {
        yield return new WaitForSeconds(respawnTime);

        var spawnPosition = deadPosition - respawnOffset;
        var player = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
        var splineProjector = player.GetComponent<SplineProjector>();

        yield return new WaitUntil(() => splineProjector.spline);

        SplineSample resSplinePoint = new SplineSample();
        splineProjector.Project(spawnPosition, ref resSplinePoint);

        spawnPosition = resSplinePoint.position - new Vector3(0, respawnOffset.y, 0);
        player.transform.position = spawnPosition;

        camera.projector = splineProjector;
        camera.rb = player.GetComponent<Rigidbody>();
        speedButton._player = player;
        dragHandler._player = player.GetComponent<Player>();
        player.GetComponent<PlayerHealth>().carSpawner = this;

        Destroy(prevCar);
    }
}
