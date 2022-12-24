using Dreamteck.Splines;
using UnityEngine;
using UnityEngine.UI;

public class PlayerRotation02 : MonoBehaviour
{
    public Image rotationDetector;

    [SerializeField] private float _speedRot = 1;
    [SerializeField] private float _smoothRot = 10;
    [SerializeField] private float _sensitivity = 3;
    [SerializeField] private float _autoRotationToSplineSpeed = 5;
    [SerializeField] private float _manualRotationToSplineSpeed = 5;
    [SerializeField] private bool _useAutoRotationToSpline = true;

    private float _currentSpeed = 0;
    private Rigidbody _rigidbody = null;
    private SplineProjector _splineProjector = null;
    private float _xInput = 0;
    private bool _isMoveButtonPressed = false;
    private bool _isRotatingByUser = false;

    PlayerMovement playerMovement;

    //---------------------------------------------------------------

    public Vector3 SplineForward => _splineProjector.result.forward;

    public void SetXInput(float value)
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
        _currentSpeed = _autoRotationToSplineSpeed;

        ApplyProjectorPosition(true);
        playerMovement.SetDirection(_splineProjector.result.forward);
    }

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _splineProjector = GetComponent<SplineProjector>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    public float offsetLerpSpeed = 10;
    public float offsetMoveSpeed = 10;
    public Vector2 AngleMinMax = new Vector2(-30, 30);
    public Vector2 roadWidthMinMax = new Vector2(-10, 10);

    private void Update()
    {
        if (_isMoveButtonPressed)
        {
            //_xInput = Input.GetAxis("Mouse X");
            var inputAngleRotation = _xInput * offsetMoveSpeed;

            if (Mathf.Abs(inputAngleRotation) > _sensitivity)
            {
                playerMovement.SetDirection(transform.forward);
                ApplyProjectorPosition(false);
                //ManualRotation(inputAngleRotation);

            }
            else
            {
                playerMovement.SetDirection(_splineProjector.result.forward);
                ApplyProjectorPosition(true);
            }

            //var targetPos = _splineProjector.motion.offset + new Vector2(inputAngleRotation, _splineProjector.motion.offset.y);
            //var targetPosXClamped = Mathf.Clamp(targetPos.x, roadWidthMinMax.x, roadWidthMinMax.y);
            //targetPos = new Vector2(targetPosXClamped, targetPos.y);
            //_splineProjector.motion.offset = Vector2.Lerp(_splineProjector.motion.offset, targetPos, offsetLerpSpeed * Time.deltaTime);


            //var targetRot = _splineProjector.motion.rotationOffset + new Vector3(_splineProjector.motion.rotationOffset.x, inputAngleRotation, _splineProjector.motion.rotationOffset.z);
            //var targetRotYClamped = Mathf.Clamp(targetRot.y, AngleMinMax.x, AngleMinMax.y);
            //targetRot = new Vector3(targetRot.x, targetRotYClamped, targetRot.z);
            //_splineProjector.motion.rotationOffset = Vector3.Lerp(_splineProjector.motion.rotationOffset, targetRot, _speedRot * Time.deltaTime);


            Debug.Log($"inputAngleRotation {inputAngleRotation}");
        }

        //if (Mathf.Abs(inputAngleRotation) > _sensitivity)
        //{
        //    _isRotatingByUser = true;
        //    _currentSpeed = _manualRotationToSplineSpeed;
        //    rotationDetector.color = Color.green;
        //}
        //else if (Mathf.Abs(inputAngleRotation) < _sensitivity)
        //{
        //    _isRotatingByUser = false;
        //    rotationDetector.color = Color.red;

        //    _xInput = 0;
        //}
        //else if (!_isMoveButtonPressed)
        //{
        //    _isRotatingByUser = false;
        //    _currentSpeed = _autoRotationToSplineSpeed;

        //    _xInput = 0;
        //}



        //if (_isMoveButtonPressed)
        //{
        //    if (_isRotatingByUser)
        //    {
        //        ManualRotation(_xInput);
        //    }
        //    else
        //    {
        //        if (_useAutoRotationToSpline)
        //        {
        //            AutoRotation(_currentSpeed);
        //        }
        //    }
        //}
        //else if (!_isMoveButtonPressed && GetComponent<PlayerMovement>().IsMoving)
        //{
        //    if (_useAutoRotationToSpline)
        //    {
        //        AutoRotation(_currentSpeed);
        //    }
        //}

        //if (Vector3.Dot(transform.forward, _splineProjector.result.forward) == 1 && !_isRotatingByUser)
        //{            
        //    _currentSpeed = _autoRotationToSplineSpeed;
        //    rotationDetector.color = Color.red;
        //}

        //Debug.Log($"_isRotatingByUser {_isRotatingByUser}");
        //Debug.DrawRay(_splineProjector.result.position, _splineProjector.result.forward * 1000, Color.yellow);
        //Debug.Log($"Current rotation speed {_currentSpeed}");
    }

    private void ApplyProjectorPosition(bool value)
    {
        _splineProjector.motion.applyPositionX = value;
        _splineProjector.motion.applyPositionZ = value;
    }

    private void ManualRotation(float angle)
    {
        angle *= _speedRot;
        var targetRot = _rigidbody.rotation * Quaternion.AngleAxis(angle, transform.up);
        var res = Quaternion.Slerp(_rigidbody.rotation, targetRot, Time.deltaTime * _smoothRot);

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