using Technoprosper.Gameplay.Player;
using System.Collections;
using UnityEngine;
using Technoprosper.Gameplay.Camera;

public class PlayerSpawner : MonoBehaviour
{    
    public Vector3 respawnOffset;
    public Player Player => _player;

    public float respawnTime = 2;
    public Player playerPrefab;

    private Track _track;
    private Player _player;
    private RaceCamera _raceCamera;

    //---------------------------------------------------------------

    public IEnumerator Init(Track track, RaceCamera raceCamera)
    {
        _track = track;
        _raceCamera = raceCamera;
        var player = Instantiate(playerPrefab, _track.startPoint.position, Quaternion.identity);

        //TODO: freeze the car behaviour

        _player = player.GetComponent<Player>();
        _player.callToRespawn = Respawn;

        yield return null;
    }

    public void Respawn(Vector3 deadPosition)
    {
        StartCoroutine(RespawnRoutine(deadPosition));
    }

    public void Respawn(Vector3 deadPosition, GameObject prevCar)
    {
        //StartCoroutine(RespawnRoutine(deadPosition, prevCar));
    }

    //---------------------------------------------------------------

    private IEnumerator RespawnRoutine(Vector3 deadPosition)
    {
        _raceCamera.SetActive(false);

        yield return new WaitForSeconds(respawnTime);

        var spawnPosition = deadPosition - respawnOffset;
        var splinePoint = _track.GetProjectionPosition(spawnPosition);

        spawnPosition = splinePoint.position - new Vector3(0, respawnOffset.y, 0);
        _player.transform.position = spawnPosition;
        _player.ActivateMovement();

        _raceCamera.SetActive(true);
    }

    //private IEnumerator RespawnRoutine(Vector3 deadPosition, GameObject prevCar)
    //{
    //    yield return new WaitForSeconds(respawnTime);

    //    var spawnPosition = deadPosition - respawnOffset;
    //    var player = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
    //    var splineProjector = player.GetComponent<SplineProjector>();

    //    yield return new WaitUntil(() => splineProjector.spline);

    //    var splinePoint = _track.GetProjectionPosition(spawnPosition);

    //    spawnPosition = splinePoint.position - new Vector3(0, respawnOffset.y, 0);
    //    player.transform.position = spawnPosition;

    //    camera.projector = splineProjector;
    //    camera.rb = player.GetComponent<Rigidbody>();
    //    //speedButton._player = player;
    //    //dragHandler._player = player.GetComponent<Player>();
    //    player.GetComponent<PlayerHealth>().carSpawner = this;

    //    Destroy(prevCar);
    //}
}