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

    void Rotate()
    {


        var currRot = transform.rotation;
        var speedRes = xInput * Time.deltaTime * speed;
        var targetRot = transform.rotation * Quaternion.AngleAxis(speedRes, Vector3.up);

        transform.rotation = targetRot;
    }

    private void Update()
    {
        Rotate();
    }
}
