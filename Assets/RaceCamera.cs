using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceCamera : MonoBehaviour
{
    private SplineFollower splineFollower;
    public float distanceMove = 1;
    public float speedFactor = 2;
    public float _speed = 2;

    private void Awake()
    {
        splineFollower = GetComponent<SplineFollower>();
    }

    public void Move(float speed)
    {
        splineFollower.direction = Spline.Direction.Forward;
        splineFollower.followSpeed = -_speed;
        splineFollower.Move(Time.deltaTime * speed * speedFactor);
    }
}
