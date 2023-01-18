using Dreamteck.Splines;
using Dreamteck.Splines.Examples;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CarMasters.UI.Input;
using CarMasters.Gameplay.Player;

public class CarSpawner : MonoBehaviour
{
    public RaceCameraGood camera;
    public PlayerMovementInput speedButton;
    public PlayerRotationInput dragHandler;
    public Vector3 respawnOffset;

    public Player Player => _player;

    public float respawnTime = 2;
    public Player playerPrefab;

    private Track _track;
    private Player _player;

    //---------------------------------------------------------------

    public IEnumerator Init(Track track)
    {
        _track = track;
        var player = Instantiate(playerPrefab, _track.startPoint.position, Quaternion.identity);

        //TODO: freeze the car behaviour

        _player = player.GetComponent<Player>();

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

        var splinePoint = _track.GetProjectionPosition(spawnPosition);

        spawnPosition = splinePoint.position - new Vector3(0, respawnOffset.y, 0);
        player.transform.position = spawnPosition;

        camera.projector = splineProjector;
        camera.rb = player.GetComponent<Rigidbody>();
        speedButton._player = player;
        dragHandler._player = player.GetComponent<Player>();
        player.GetComponent<PlayerHealth>().carSpawner = this;

        Destroy(prevCar);
    }
}
