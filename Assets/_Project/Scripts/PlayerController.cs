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
        //if (isMoveButtonPressed)
        //{
        //    currentSpeed += acceleration * Time.deltaTime;


        //    //var clamp
        //}
        //else if (!isMoveButtonPressed && currentSpeed > 0)
        //{
        //    currentSpeed -= acceleration * Time.deltaTime;
        //}

        Rotation();


        Debug.Log($"currentSpeed {currentSpeed}");

    }

    private void Rotation()
    {
        inputHorizontal = Input.GetAxis("Mouse X") * rotationSpeed;

        Debug.Log($"inputHorizontal {inputHorizontal}");

        var degrees = /*Mathf.Rad2Deg **/ inputHorizontal;
        //var targetRot = transform.rotation * Quaternion.AngleAxis(degrees * Time.deltaTime, Vector3.up);
        //var targetRot = Quaternion.Euler(0, degrees * Time.deltaTime, 0) * transform.rotation;
        //_rigidbody.MoveRotation(targetRot);

        var originRot = _rigidbody.rotation;
        var targetRot = Quaternion.AngleAxis(degrees, Vector3.up);
        var resRot = Quaternion.Slerp(originRot, targetRot, smoothRotation * Time.deltaTime);
        _rigidbody.MoveRotation(resRot);
        //_rigidbody.rotation = resRot;

        Debug.Log($"currentRotation degrees {degrees}");
    }

    private void FixedUpdate()
    {
        _rigidbody.MovePosition(transform.position + transform.forward * currentSpeed);

    }
}