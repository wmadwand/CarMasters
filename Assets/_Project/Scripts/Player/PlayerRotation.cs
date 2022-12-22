using Dreamteck.Splines;
using UnityEngine;
using UnityEngine.UI;

public class PlayerRotation : MonoBehaviour
{
    public Image rotationDetector;

    [SerializeField] private float _speed = 1;
    [SerializeField] private float _smooth = 10;
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
    }

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _splineProjector = GetComponent<SplineProjector>();
    }

    private void Update()
    {
        var inputAngleRotation = _xInput * _speed;

        if (Mathf.Abs(inputAngleRotation) > _sensitivity)
        {
            _isRotatingByUser = true;
            _currentSpeed = _manualRotationToSplineSpeed;
            rotationDetector.color = Color.green;
        }
        else if (Mathf.Abs(inputAngleRotation) < _sensitivity)
        {
            _isRotatingByUser = false;
            rotationDetector.color = Color.red;

            _xInput = 0;
        }
        else if (!_isMoveButtonPressed)
        {
            _isRotatingByUser = false;
            _currentSpeed = _autoRotationToSplineSpeed;

            _xInput = 0;
        }

        if (!_isMoveButtonPressed)
        {
            _xInput = 0;
        }

        if (_isMoveButtonPressed)
        {
            if (_isRotatingByUser)
            {
                ManualRotation(_xInput);
            }
            else
            {
                if (_useAutoRotationToSpline)
                {
                    AutoRotation(_currentSpeed);
                }
            }
        }
        else if (!_isMoveButtonPressed && GetComponent<PlayerMovement>().IsMoving)
        {
            if (_useAutoRotationToSpline)
            {
                AutoRotation(_currentSpeed);
            }
        }

        if (Vector3.Dot(transform.forward, _splineProjector.result.forward) == 1 && !_isRotatingByUser)
        {            
            _currentSpeed = _autoRotationToSplineSpeed;
            rotationDetector.color = Color.red;
        }

        Debug.Log($"_isRotatingByUser {_isRotatingByUser}");
        Debug.DrawRay(_splineProjector.result.position, _splineProjector.result.forward * 1000, Color.yellow);
    }

    private void ManualRotation(float angle)
    {
        angle *= _speed;
        var targetRot = _rigidbody.rotation * Quaternion.AngleAxis(angle, transform.up);
        var res = Quaternion.Slerp(_rigidbody.rotation, targetRot, Time.deltaTime * _smooth);

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