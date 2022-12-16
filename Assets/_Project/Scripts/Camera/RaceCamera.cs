using UnityEngine;

public class RaceCamera : MonoBehaviour
{
    public PlayerRotation playerRotation;

    [SerializeField] private float _rotationSpeed = 50f;
    [SerializeField] private float _pitchRotationAngle = 15f;
    [SerializeField] private float _followingHorizontalSpeed = 50f;

    private void Update()
    {
        var currentRotation = transform.rotation;
        var pitchRotation = Quaternion.Euler(_pitchRotationAngle, 0, 0);
        var targetRotation = Quaternion.LookRotation(playerRotation.SplineForward, Vector3.up) * pitchRotation;
        var resultRotation = Quaternion.RotateTowards(currentRotation, targetRotation, Time.deltaTime * _rotationSpeed);
        transform.rotation = resultRotation;

        var xLerp = Mathf.Lerp(transform.position.x, playerRotation.transform.position.x, Time.deltaTime * _followingHorizontalSpeed);
        transform.position = new Vector3(xLerp, transform.position.y, transform.position.z);

        Debug.DrawRay(transform.position, playerRotation.SplineForward * 1000, Color.magenta);
    }
}