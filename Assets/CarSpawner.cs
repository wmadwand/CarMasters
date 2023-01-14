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

    public float respawnDistance;
    public float respawnHeight;
    public float respawnTime = 2;
    public Player playerPrefab;

    public void Respawn(Vector3 deadPosition, GameObject prevCar)
    {
        StartCoroutine(RespawnRoutine(prevCar));
    }

    IEnumerator RespawnRoutine(GameObject prevCar)
    {
        yield return new WaitForSeconds(respawnTime);

        var player = Instantiate(playerPrefab);

        camera.projector = player.GetComponent<SplineProjector>();
        camera.rb = player.GetComponent<Rigidbody>();
        speedButton.player = player;
        dragHandler.player = player.GetComponent<PlayerRotation>();
        player.GetComponent<PlayerHealth>().carSpawner = this;

        Destroy(prevCar);
    }
}
