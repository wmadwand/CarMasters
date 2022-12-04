using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float acceleration = 1;
    public float maxSpeed = 100;

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
        inputHorizontal = Input.GetAxis("Horizontal");

        if (isMoveButtonPressed)
        {
            currentSpeed += acceleration * Time.deltaTime;


        }
        else if (!isMoveButtonPressed && currentSpeed > 0)
        {
            currentSpeed -= acceleration * Time.deltaTime;
        }

        Debug.Log($"currentSpeed {currentSpeed}");
    }

    private void FixedUpdate()
    {
        _rigidbody.MovePosition(transform.position + transform.forward * currentSpeed);
    }
}