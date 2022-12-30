using Dreamteck.Splines;
using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerRotationNew : MonoBehaviour
{
    public Image rotationDetector;
    public Transform localRotator;

    [SerializeField] private float _speed = 1;
    [SerializeField] private float _smooth = 10;    
    [SerializeField] private float _autoRotationToSplineSpeed = 5;
    [SerializeField] private float _manualRotationToSplineSpeed = 5;
    [SerializeField] private bool _useAutoRotationToSpline = true;

    [SerializeField] private float _disatnceAfterManualTurn = 2;
    
    private SplineProjector _splineProjector = null;
    private float _xInput = 0;
    private bool _isMoveButtonPressed = false;

    //---------------------------------------------------------------

    public Vector3 SplineForward => _splineProjector.result.forward;


    private Action _callBackToDragHandler;

    public void SetXInput(float value, Action callback)
    {
        _xInput = value;
        _callBackToDragHandler = callback;
    }

    public void SetMove(bool isActive)
    {
        _isMoveButtonPressed = isActive;
    }

    //---------------------------------------------------------------

    private void Awake()
    {        
        _splineProjector = GetComponent<SplineProjector>();
    }

    float _inputAngleRotation = 0;

    Vector3 startPosAfterManualTurnFinished;
    bool isManualTurnFinished = false;
    bool isAutoTurning = false;

    private void Update()
    {
        if (_isMoveButtonPressed)
        {
            _inputAngleRotation = _inputAngleRotation == 0 ? _xInput * _speed : _inputAngleRotation;

            if (_inputAngleRotation != 0 && !isAutoTurning)
            {
                isManualTurnFinished = false;

                var angleClamp = Mathf.Clamp(_inputAngleRotation, -45, 45);
                var startRot = localRotator.localRotation;
                var targetRot = Quaternion.AngleAxis(angleClamp, localRotator.transform.up);
                var resRot = Quaternion.RotateTowards(startRot, targetRot, Time.deltaTime * _smooth);

                localRotator.localRotation = resRot;
                _splineProjector.motion.rotationOffset = localRotator.localRotation.eulerAngles;

                //var offset = _splineProjector.motion.rotationOffset;
                //var yy = Mathf.Lerp(offset.y, angleClamp, Time.deltaTime * _smooth);
                //_splineProjector.motion.rotationOffset = new Vector3(offset.x, yy, offset.z);

                var checkAngle = Quaternion.Angle(startRot, targetRot);
                Debug.Log($"checkAngle = {checkAngle}");

                if (checkAngle <= 0)
                {
                    _xInput = 0;
                    _inputAngleRotation = 0;

                    _callBackToDragHandler?.Invoke();

                    isManualTurnFinished = true;
                    startPosAfterManualTurnFinished = localRotator.position;
                }

                Debug.Log($"_inputAngleRotation = {angleClamp}");
            }

            if (isManualTurnFinished)
            {
                if (Vector3.Distance(startPosAfterManualTurnFinished, localRotator.position) >= _disatnceAfterManualTurn)
                {
                    isAutoTurning = true;

                    var startRot = localRotator.localRotation;
                    AutoRotationNew(_manualRotationToSplineSpeed, out Quaternion targetRotation);

                    var checkThatAngle = Quaternion.Angle(startRot, targetRotation);
                    Debug.Log($"checkThatAngle = {checkThatAngle}");

                    if (checkThatAngle <= 0)
                    {
                        isManualTurnFinished = false;
                        isAutoTurning = false;
                    }
                }
            }

            //if (_useAutoRotationToSpline && _inputAngleRotation == 0 && !isManualTurnFinished && !isAutoTurning)
            //{
            //    //AutoRotationNew(_autoRotationToSplineSpeed, out Quaternion targetRotation);
            //}
        }
    }


    private void AutoRotationNew(float speed, out Quaternion targetRottt)
    {
        //var playerForward = transform.forward;

        //var splineForward = Vector3.forward;
        //if (_splineProjector != null)
        //{
        //    splineForward = _splineProjector.result.forward;
        //}

        //var targetRot = Quaternion. LookRotation(Quaternion.identity.fo, transform.up);
        //var targetRot = Quaternion.identity;

        //var resRot = Quaternion.RotateTowards(localRotator.rotation, targetRot, Time.deltaTime * speed);
        var targetRot = Quaternion.Euler(0, 0, 0);
        // TODO: Better use RotateTowards
        var resRot = Quaternion.Lerp(localRotator.localRotation, targetRot, Time.deltaTime * speed);

        localRotator.localRotation = resRot;
        _splineProjector.motion.rotationOffset = localRotator.localRotation.eulerAngles;

        //_rigidbody.MoveRotation(resRot);

        targetRottt = targetRot;
    }


    //private void ManualRotation(float angle)
    //{
    //    angle *= _speed;
    //    var targetRot = _rigidbody.rotation * Quaternion.AngleAxis(angle, transform.up);
    //    var res = Quaternion.Slerp(_rigidbody.rotation, targetRot, Time.deltaTime * _smooth);

    //    _rigidbody.MoveRotation(res);
    //}

    //private void AutoRotation(float speed)
    //{
    //    var playerForward = transform.forward;
    //    var splineForward = _splineProjector.result.forward;
    //    var targetRot = _rigidbody.rotation * Quaternion.FromToRotation(playerForward, splineForward);
    //    targetRot = Quaternion.Slerp(_rigidbody.rotation, targetRot, Time.deltaTime * speed);

    //    _rigidbody.MoveRotation(targetRot);
    //}

    //private void Update_old()
    //{
    //    var inputAngleRotation = _xInput * _speed;

    //    if (Mathf.Abs(inputAngleRotation) > _sensitivity)
    //    {
    //        _isRotatingByUser = true;
    //        _currentSpeed = _manualRotationToSplineSpeed;
    //        rotationDetector.color = Color.green;
    //    }
    //    else if (Mathf.Abs(inputAngleRotation) < _sensitivity)
    //    {
    //        _isRotatingByUser = false;
    //        rotationDetector.color = Color.red;

    //        _xInput = 0;
    //    }
    //    else if (!_isMoveButtonPressed)
    //    {
    //        _isRotatingByUser = false;
    //        _currentSpeed = _autoRotationToSplineSpeed;

    //        _xInput = 0;
    //    }

    //    if (!_isMoveButtonPressed)
    //    {
    //        _xInput = 0;
    //    }

    //    if (_isMoveButtonPressed)
    //    {
    //        if (_isRotatingByUser)
    //        {
    //            ManualRotation(_xInput);
    //        }
    //        else
    //        {
    //            if (_useAutoRotationToSpline)
    //            {
    //                AutoRotation(_currentSpeed);
    //            }
    //        }
    //    }
    //    else if (!_isMoveButtonPressed && GetComponent<PlayerMovementNew>().IsMoving)
    //    {
    //        if (_useAutoRotationToSpline)
    //        {
    //            AutoRotation(_currentSpeed);
    //        }
    //    }

    //    if (Vector3.Dot(transform.forward, _splineProjector.result.forward) == 1 && !_isRotatingByUser)
    //    {
    //        _currentSpeed = _autoRotationToSplineSpeed;
    //        rotationDetector.color = Color.red;
    //    }

    //    Debug.Log($"_isRotatingByUser {_isRotatingByUser}");
    //    Debug.DrawRay(_splineProjector.result.position, _splineProjector.result.forward * 1000, Color.yellow);
    //    Debug.Log($"Current rotation speed {_currentSpeed}");
    //}
}