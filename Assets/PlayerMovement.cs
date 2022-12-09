using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _acceleration = 1;
    [SerializeField] private float _maxSpeed = 100;
    [SerializeField] private float _minSpeed = 0;
    [SerializeField] private float _rotationSpeed = 1;
    [SerializeField] private float _smoothRotation = 10;
    [SerializeField] private float _moveSpeed = 10;
    [SerializeField] private float _jumpPower = 2;

    private float _currentSpeed = 0;

    private bool _isMoveButtonPressed;
    private float _currentAngle;
    private Rigidbody _rigidbody;

    //---------------------------------------------------------------

    private void Move()
    {
        if (_isMoveButtonPressed)
        {
            _currentSpeed += _acceleration * Time.deltaTime;

            _currentSpeed = Mathf.Clamp(_currentSpeed, _minSpeed, _maxSpeed);
        }
        else if (!_isMoveButtonPressed && _currentSpeed > _minSpeed)
        {
            _currentSpeed -= _acceleration * 2 * Time.deltaTime;
        }

        if (_currentSpeed <= _minSpeed)
        {
            Stop();
            return;
        }
        else
        {
            Go();
        }

        var dir = Vector3.forward * _currentSpeed;
        _rigidbody.MovePosition(_rigidbody.position + transform.TransformDirection(dir) * Time.deltaTime);
    }

    private void Go()
    {
        //rigidbody.constraints = rigidbody.constraints ^ RigidbodyConstraints.FreezePosition;
        _rigidbody.constraints = _rigidbody.constraints ^ RigidbodyConstraints.FreezeRotationY;
    }

    private void Stop()
    {
        _rigidbody.constraints = _rigidbody.constraints | RigidbodyConstraints.FreezeRotationY;
        //rigidbody.velocity = Vector3.zero;
        //rigidbody.angularVelocity = Vector3.zero;


        //rigidbody.constraints = rigidbody.constraints | RigidbodyConstraints.FreezePosition;
        //TODO freeze position
    }

    private void Rotation()
    {
        var inputHorizontal = Input.GetAxis("Mouse X");

        Debug.Log($"inputHorizontal {inputHorizontal}");

        var degrees = /*Mathf.Rad2Deg **/ inputHorizontal;
        var inputAngle = degrees * _rotationSpeed * Time.deltaTime;
        var newAngle = _currentAngle + inputAngle;
        _currentAngle = Mathf.Clamp(newAngle, -35, 35);

        var newRotInput = Quaternion.Euler(0, _currentAngle, 0);
        //targetRot = targetRot * rigidbody.rotation;

        //newRotInput = Quaternion.Slerp(transform.rotation, targetRot, smoothRotation * Time.deltaTime);

        Debug.Log($"currentRotation degrees {degrees}");
        //rigidbody.MoveRotation(newRot);
        //transform.rotation = newRotInput;
    }

    private void Rotation2()
    {
        var degrees = Input.GetAxis("Mouse X");

        var startRot = _rigidbody.rotation;
        var deegreesRes = degrees * _rotationSpeed /** Time.deltaTime*/;
        var targetRot = startRot * Quaternion.AngleAxis(deegreesRes, Vector3.up);
        var newRot = Quaternion.Slerp(startRot, targetRot, _smoothRotation * Time.deltaTime);
        _rigidbody.MoveRotation(newRot);
    }

    private void MoveKeyboard()
    {
        var yInput = Input.GetAxisRaw("Vertical");
        var xInput = Input.GetAxisRaw("Horizontal");

        var dir = new Vector3(xInput, 0, yInput).normalized;

        _rigidbody.MovePosition(_rigidbody.position + transform.TransformDirection(dir) * Time.deltaTime * _moveSpeed);
    }

    private void Jump()
    {
        if (Input.GetKeyDown("space"))
        {
            GetComponent<PlayerGravity>().Jump(_jumpPower);
        }
    }

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Move();

        if (_isMoveButtonPressed)
        {
            Rotation2();
            //Rotation();
        }
    }
}
