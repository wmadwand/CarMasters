using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationHandlerTest : MonoBehaviour
{
    private float xInput;

    public float speed = 10;

    public void SetXInput(float value)
    {
        xInput = value;
    }

    public void Rotate(float angle)
    {
        var speedRes = angle /** Time.deltaTime*/ * speed;

        speedRes = Mathf.Clamp(speedRes, -35, 35);

        var targetRot = transform.rotation * Quaternion.AngleAxis(speedRes, Vector3.up);

        transform.rotation = targetRot;
    }

    private void Update()
    {
        //Rotate();
    }
}
