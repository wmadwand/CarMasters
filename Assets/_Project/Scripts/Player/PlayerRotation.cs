using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotation : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed = 1;
    [SerializeField] private float _rotationSmooth = 10;
    [SerializeField] private float _rotationValuableRate = 3;
    [SerializeField] private float _rotationToSplineSpeedAutopilot = 5;
    [SerializeField] private float _rotationToSplineSpeedAfterManulaTurn = 5;
    [SerializeField] private bool _shouldLookAlongSplineForward = true;

    private float _currentRotationToSplineSpeed = 0;
    private Rigidbody _rigidbody = null;
    private SplineProjector _splineProjector = null;
    private float xInput = 0;
    private bool _isMoveButtonPressed = false;

    //---------------------------------------------------------------

    public void Rotation(float angle)
    {
        //if (Mathf.Abs(angle) <= _rotationValuableRate) return;

        angle *= _rotationSpeed;
        var targetRot = _rigidbody.rotation * Quaternion.AngleAxis(angle, transform.up);
        var res = Quaternion.Slerp(_rigidbody.rotation, targetRot, Time.deltaTime * _rotationSmooth);

        _rigidbody.MoveRotation(res);
    }

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
        xInput = Input.GetAxis("Mouse X");
        var inputAngleRotation = xInput * _rotationSpeed;

        if (_isMoveButtonPressed)
        {
            if (Mathf.Abs(inputAngleRotation) > _rotationValuableRate)
            {
                Rotation(xInput);
                _currentRotationToSplineSpeed = _rotationToSplineSpeedAfterManulaTurn;
            }
            else
            {
                xInput = 0;

                if (_shouldLookAlongSplineForward)
                {
                    _currentRotationToSplineSpeed = _rotationToSplineSpeedAutopilot;
                    LookAlongSplineForward(_currentRotationToSplineSpeed);
                }
            }

            Rotation(xInput);
        }
        else if (!_isMoveButtonPressed && GetComponent<PlayerMovement>().IsMoving)
        {
            if (_shouldLookAlongSplineForward)
            {
                _currentRotationToSplineSpeed = _rotationToSplineSpeedAutopilot;
                LookAlongSplineForward(_currentRotationToSplineSpeed);
            }
        }

        Debug.DrawRay(_splineProjector.result.position, _splineProjector.result.forward * 1000, Color.yellow);
    }

    private void LookAlongSplineForward(float speed)
    {
        var playerForward = transform.forward;
        var splineForward = _splineProjector.result.forward;
        var targetRot = _rigidbody.rotation * Quaternion.FromToRotation(playerForward, splineForward);
        targetRot = Quaternion.Slerp(_rigidbody.rotation, targetRot, Time.deltaTime * speed);

        _rigidbody.MoveRotation(targetRot);
    }
}