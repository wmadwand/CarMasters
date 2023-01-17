using Dreamteck.Splines;
using Dreamteck.Splines.Examples;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    public BallCamera camera;
    public SpeedButton speedButton;
    public DragHandler dragHandler;
    public Vector3 respawnOffset;

    public float respawnDistance;
    public float respawnHeight;
    public float respawnTime = 2;
    public Player playerPrefab;

    //---------------------------------------------------------------

    public void Respawn(Vector3 deadPosition, GameObject prevCar)
    {
        StartCoroutine(RespawnRoutine(deadPosition, prevCar));
    }

    IEnumerator RespawnRoutine(Vector3 deadPosition, GameObject prevCar)
    {
        yield return new WaitForSeconds(respawnTime);

        //var splineProjector = prevCar.GetComponent<SplineProjector>();



        var spawnPosition = deadPosition - respawnOffset;
        var player = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);


        var splineProjector = player.GetComponent<SplineProjector>();

        SplineSample resSplinePoint = new SplineSample();
        splineProjector.Project(spawnPosition, ref resSplinePoint);

        //player.transform.position = resSplinePoint.position;

        camera.projector = splineProjector;
        camera.rb = player.GetComponent<Rigidbody>();
        speedButton.player = player;
        dragHandler.player = player.GetComponent<PlayerRotation>();
        player.GetComponent<PlayerHealth>().carSpawner = this;

        Destroy(prevCar);
    }
}
