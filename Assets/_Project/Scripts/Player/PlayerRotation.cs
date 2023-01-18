using Dreamteck.Splines;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CarMasters.Gameplay.Player
{
    public class PlayerRotation : MonoBehaviour
    {
        [SerializeField] private Transform localRotator;
        [SerializeField] private float _angleRate = 1;
        [SerializeField] private float _smooth = 10;
        [SerializeField] private float _manualRotationToSplineSpeed = 5;
        [SerializeField] private float _disatnceAfterManualTurn = 2;

        private SplineProjector _splineProjector = null;
        private float _xInput = 0;
        private bool _isMoving = false;
        private float _inputAngleRotation = 0;
        private Vector3 _startPosAfterManualTurnFinished;
        private bool _isManualTurnFinished = false;
        private bool _isAutoTurning = false;

        //---------------------------------------------------------------

        public Vector3 SplineForward => _splineProjector.result.forward;
        public SplineProjector SplineProjector => _splineProjector;

        public void StopRightThere()
        {
            _isManualTurnFinished = false;
        }

        public void Init()
        {

        }

        public void RotateBy(float value)
        {

            _xInput = value;
        }

        public void SetMove(bool isActive)
        {
            _isMoving = isActive;
        }

        //---------------------------------------------------------------

        private void Awake()
        {
            _splineProjector = GetComponent<SplineProjector>();
        }

        private void Update()
        {
            if (_isMoving)
            {
                _inputAngleRotation = _xInput * _angleRate;

                if (_inputAngleRotation != 0 && !_isAutoTurning)
                {
                    _isManualTurnFinished = false;

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

                        _isManualTurnFinished = true;
                        _startPosAfterManualTurnFinished = localRotator.position;
                    }

                    Debug.Log($"_inputAngleRotation = {angleClamp}");
                }

                if (_isManualTurnFinished)
                {
                    if (Vector3.Distance(_startPosAfterManualTurnFinished, localRotator.position) >= _disatnceAfterManualTurn)
                    {
                        _isAutoTurning = true;

                        var startRot = localRotator.localRotation;
                        ResetRotation(_manualRotationToSplineSpeed, out Quaternion targetRotation);

                        var checkThatAngle = Quaternion.Angle(startRot, targetRotation);
                        Debug.Log($"checkThatAngle = {checkThatAngle}");

                        if (checkThatAngle <= 0)
                        {
                            _isManualTurnFinished = false;
                            _isAutoTurning = false;
                        }
                    }
                }
            }
        }

        private void ResetRotation(float speed, out Quaternion targetRottt)
        {
            var targetRot = Quaternion.Euler(0, 0, 0);
            // TODO: Better use RotateTowards
            var resRot = Quaternion.Slerp(localRotator.localRotation, targetRot, Time.deltaTime * speed);

            localRotator.localRotation = resRot;
            _splineProjector.motion.rotationOffset = localRotator.localRotation.eulerAngles;

            targetRottt = targetRot;
        }
    } 
}