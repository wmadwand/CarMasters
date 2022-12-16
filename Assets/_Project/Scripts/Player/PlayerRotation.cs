using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerRotation : MonoBehaviour
{
    public Image rotationDetector;

    [SerializeField] private float _rotationSpeed = 1;
    [SerializeField] private float _rotationSmooth = 10;
    [SerializeField] private float _rotationValuableRate = 3;
    [SerializeField] private float _rotationToSplineSpeedAutopilot = 5;
    [SerializeField] private float _rotationToSplineSpeedAfterManulaTurn = 5;
    [SerializeField] private bool _shouldLookAlongSplineForward = true;

    private float _currentSpeed = 0;
    private Rigidbody _rigidbody = null;
    private SplineProjector _splineProjector = null;
    private float xInput = 0;
    private bool _isMoveButtonPressed = false;
    private bool _isRotatingByUser = false;

    //---------------------------------------------------------------

    public Vector3 SplineForward => _splineProjector.result.forward;

    public void SetXInput(float value)
    {
        xInput = value;
    }

    public void SetMove(bool isActive)
    {
        _isMoveButtonPressed = isActive;
    }

    //---------------------------------------------------------------

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _splineProjector = GetComponent<SplineProjector>();
    }

    private void Update()
    {
        //xInput = Input.GetAxis("Mouse X");
        var inputAngleRotation = xInput * _rotationSpeed;

        if (Mathf.Abs(inputAngleRotation) > _rotationValuableRate && _isMoveButtonPressed)
        {
            _isRotatingByUser = true;
            _currentSpeed = _rotationToSplineSpeedAfterManulaTurn;
            rotationDetector.color = Color.green;
        }
        else if (Mathf.Abs(inputAngleRotation) < _rotationValuableRate || !_isMoveButtonPressed)
        {
            _isRotatingByUser = false;
            _currentSpeed = _rotationToSplineSpeedAutopilot;
            rotationDetector.color = Color.red;
        }

        //Debug.Log($"inputAngleRotation {inputAngleRotation}");

        if (_isMoveButtonPressed)
        {
            if (_isRotatingByUser)
            {
                ManualRotation(xInput);
            }
            else
            {
                AutoRotation(_currentSpeed);
            }
        }
        else if (!_isMoveButtonPressed && GetComponent<PlayerMovement>().IsMoving)
        {
            if (_shouldLookAlongSplineForward)
            {
                AutoRotation(_currentSpeed);
            }
        }

        //if (Vector3.Dot(transform.forward, _splineProjector.result.forward) == 1)
        //{
        //    _isRotatingByUser = false;
        //    _currentSpeed = _rotationToSplineSpeedAutopilot;
        //}

        Debug.Log($"_isRotatingByUser {_isRotatingByUser}");
        Debug.DrawRay(_splineProjector.result.position, _splineProjector.result.forward * 1000, Color.yellow);
    }

    private void ManualRotation(float angle)
    {
        //if (Mathf.Abs(angle) <= _rotationValuableRate) return;

        angle *= _rotationSpeed;
        var targetRot = _rigidbody.rotation * Quaternion.AngleAxis(angle, transform.up);
        var res = Quaternion.Slerp(_rigidbody.rotation, targetRot, Time.deltaTime * _rotationSmooth);

        _rigidbody.MoveRotation(res);
    }

    private void AutoRotation(float speed)
    {
        var playerForward = transform.forward;
        var splineForward = _splineProjector.result.forward;
        var targetRot = _rigidbody.rotation * Quaternion.FromToRotation(playerForward, splineForward);
        targetRot = Quaternion.Slerp(_rigidbody.rotation, targetRot, Time.deltaTime * speed);

        _rigidbody.MoveRotation(targetRot);
    }
}