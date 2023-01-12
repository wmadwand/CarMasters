using Dreamteck.Splines;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerRotationNew : MonoBehaviour
{
    public SplineComputer nextSpline;

    public Image rotationDetector;
    public Transform localRotator;

    [SerializeField] private float _angleRate = 1;
    [SerializeField] private float _smooth = 10;
    [SerializeField] private float _manualRotationToSplineSpeed = 5;
    [SerializeField] private bool _useAutoRotationToSpline = true;

    [SerializeField] private float _disatnceAfterManualTurn = 2;


    // TODO: Replace Projector with Follower !!
    private SplineProjector _splineProjector = null;


    private float _xInput = 0;
    private bool _isMoveButtonPressed = false;

    //---------------------------------------------------------------

    public Vector3 SplineForward => _splineProjector.result.forward;

    public void SetXInput(float value, Action callback)
    {
        _xInput = value;
    }

    public void SetMove(bool isActive)
    {
        _isMoveButtonPressed = isActive;
    }

    //---------------------------------------------------------------

    private void Start()
    {
        _splineProjector.onEndReached += _splineProjector_onEndReached;
        _splineProjector.onBeginningReached += _splineProjector_onBeginningReached;
    }


    private void Awake()
    {
        _splineProjector = GetComponent<SplineProjector>();

    }

    private void OnDestroy()
    {
        _splineProjector.onEndReached -= _splineProjector_onEndReached;
        _splineProjector.onBeginningReached -= _splineProjector_onBeginningReached;
    }

    private void _splineProjector_onBeginningReached()
    {
        Debug.Log("_splineProjector_onBeginningReached");
    }


    private void _splineProjector_onEndReached()
    {
        _splineProjector.spline = nextSpline;
        _splineProjector.RebuildImmediate();
        _splineProjector.SetPercent(0d, false, false);

        //float deltaTime = _splineProjector.updateMethod == SplineUser.UpdateMethod.FixedUpdate ? Time.fixedDeltaTime : Time.deltaTime;
        ////float distance = _splineProjector. * deltaTime;
        //float travelled = _splineProjector.CalculateLength(0, 1d);
        //float remainder = 40 - travelled;

        //_splineProjector.spline = nextSpline;
        //_splineProjector.RebuildImmediate();
        //_splineProjector.SetPercent(0d, false, false);
        //_splineProjector.SetDistance(remainder, true, true);
    }

    SplineFollower follower;

    void OnEndReached(double last)
    {
        //Detect when the wagon has reached the end of the spline
        List<SplineComputer> computers = new List<SplineComputer>();
        List<int> connections = new List<int>();
        List<int> connected = new List<int>();
        follower.spline.GetConnectedComputers(computers, connections, connected, 1.0, follower.direction, true); //Get the avaiable connected computers at the end of the spline
        if (computers.Count == 0) return;
        //Do not select computers that are not connected at the first point so that we don't reverse direction
        for (int i = 0; i < computers.Count; i++)
        {
            if (connected[i] != 0)
            {
                computers.RemoveAt(i);
                connections.RemoveAt(i);
                connected.RemoveAt(i);
                i--;
                continue;
            }
        }
        float distance = follower.CalculateLength(0.0, follower.result.percent); //Get the excess distance after looping
        follower.spline = computers[UnityEngine.Random.Range(0, computers.Count)]; //Change the spline computer to the new spline
        follower.SetDistance(distance); //Set the excess distance along the new spline
    }


    float _inputAngleRotation = 0;

    Vector3 startPosAfterManualTurnFinished;
    bool isManualTurnFinished = false;
    bool isAutoTurning = false;

    private void Update()
    {
        if (_isMoveButtonPressed)
        {
            //_inputAngleRotation = _inputAngleRotation == 0 ? _xInput * _angleRate : _inputAngleRotation;
            _inputAngleRotation = _xInput * _angleRate;

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
                    ResetRotation(_manualRotationToSplineSpeed, out Quaternion targetRotation);

                    var checkThatAngle = Quaternion.Angle(startRot, targetRotation);
                    Debug.Log($"checkThatAngle = {checkThatAngle}");

                    if (checkThatAngle <= 0)
                    {
                        isManualTurnFinished = false;
                        isAutoTurning = false;
                    }
                }
            }
        }
    }

    private void ResetRotation(float speed, out Quaternion targetRottt)
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
}