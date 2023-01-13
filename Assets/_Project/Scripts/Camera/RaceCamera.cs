using Dreamteck.Splines;
using UnityEngine;

public class RaceCamera : MonoBehaviour
{
    public PlayerRotation playerRotation;

    [SerializeField] private float _rotationSpeed = 50f;
    [SerializeField] private float _pitchRotationAngle = 15f;
    [SerializeField] private float _followingHorizontalSpeed = 50f;
    [SerializeField] private float _followTargetDistance = 8f;
    [SerializeField] private float _yOffsetPosition = 0f;

    private SplineProjector _splineProjector;
    private SplinePositioner _splinePositioner;

    //---------------------------------------------------------------

    private void Awake()
    {
        _splinePositioner = GetComponentInParent<SplinePositioner>();
        _splineProjector = playerRotation.GetComponent<SplineProjector>();
    }

    private void Update()
    {
        _splinePositioner.followTargetDistance = _followTargetDistance;

        var currentRotation = transform.rotation;
        var pitchRotation = Quaternion.Euler(_pitchRotationAngle, 0, 0);
        var targetRotation = Quaternion.LookRotation(playerRotation.SplineForward, Vector3.up) * pitchRotation;
        var resultRotation = Quaternion.RotateTowards(currentRotation, targetRotation, Time.deltaTime * _rotationSpeed);
        transform.rotation = resultRotation;

        var playerRelativePosition = playerRotation.transform.InverseTransformPoint(_splineProjector.result.position);
        var newOffsetPosition = new Vector2(-playerRelativePosition.x, _yOffsetPosition - playerRelativePosition.y);
        var newOffsetPositionLerp = Vector2.Lerp(_splinePositioner.motion.offset, newOffsetPosition, Time.deltaTime * _followingHorizontalSpeed);
        _splinePositioner.motion.offset = newOffsetPositionLerp;

        Debug.DrawRay(transform.position, playerRotation.SplineForward * 1000, Color.magenta);
    }
}