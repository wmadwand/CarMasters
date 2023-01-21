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

    public IEnumerator Spawn(Track track, RaceCamera raceCamera)
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

    //---------------------------------------------------------------

    private IEnumerator RespawnRoutine(Vector3 deadPosition)
    {
        _raceCamera.SetActive(false);

        yield return new WaitForSeconds(respawnTime);

        var spawnPosition = deadPosition - respawnOffset;
        var splinePoint = _track.GetProjectionPosition(spawnPosition);

        spawnPosition = splinePoint.position - new Vector3(0, respawnOffset.y, 0);
        _player.transform.position = spawnPosition;
        _player.SplineProjector.motion.rotationOffset = Vector3.zero;
        _player.ActivateMovement();

        _raceCamera.SetActive(true);
    }
}