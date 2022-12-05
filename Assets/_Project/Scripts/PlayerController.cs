using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float acceleration = 1;
    public float maxSpeed = 100;
    public float rotationSpeed = 1;
    public float smoothRotation = .5f;

    Rigidbody _rigidbody;
    float inputHorizontal;
    float currentSpeed = 0;
    float minSpeed = 0;
    bool isMoveButtonPressed = false;


    public void Move()
    {
        if (isMoveButtonPressed)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, maxSpeed, acceleration);
        }


    }

    public void PressMoveButton(bool isActive)
    {
        isMoveButtonPressed = isActive;
    }



    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (isMoveButtonPressed)
        {
            currentSpeed += acceleration * Time.deltaTime;

            Rotation();
            //var clamp
        }
        else if (!isMoveButtonPressed && currentSpeed > 0)
        {
            currentSpeed -= acceleration * Time.deltaTime;
        }




        Debug.Log($"currentSpeed {currentSpeed}");

    }

    Quaternion newRot;
    float currdegr;

    private void Rotation()
    {
        inputHorizontal = Input.GetAxis("Mouse X");

        Debug.Log($"inputHorizontal {inputHorizontal}");

        var degrees = /*Mathf.Rad2Deg **/ inputHorizontal;

        var startRot = _rigidbody.rotation;
        var degreesRes = degrees * rotationSpeed * Time.deltaTime;
        var newB = currdegr + degreesRes;
        currdegr = Mathf.Clamp(newB, -35, 35);
        //newRot = startRot * Quaternion.Euler(0, degreesRes, 0);

        //var startRot = _rigidbody.rotation;
        //var deegreesRes = degrees * rotationSpeed /** Time.deltaTime*/;
        //var targetRot = startRot * Quaternion.AngleAxis(deegreesRes, Vector3.up);
        //newRot = Quaternion.Slerp(startRot, targetRot, smoothRotation * Time.deltaTime);
        //_rigidbody.MoveRotation(newRot);

        Debug.Log($"currentRotation degrees {degrees}");
    }

    private void FixedUpdate()
    {
        _rigidbody.MovePosition(transform.position + transform.forward * currentSpeed);
        //_rigidbody.MoveRotation(newRot);
        _rigidbody.rotation = Quaternion.Euler(0, currdegr, 0);
    }
}