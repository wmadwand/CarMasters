using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowingCamera : MonoBehaviour
{
    public Transform target;
    public float speed;
    public float roationSpeed;

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * speed);

        var direction = target.position - transform.position;
        var targetRot = Quaternion.LookRotation(target.forward);
        var newrot = Quaternion.RotateTowards(transform.rotation, targetRot, Time.deltaTime * roationSpeed);

        transform.rotation = newrot;
    }
}
