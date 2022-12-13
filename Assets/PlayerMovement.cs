using Dreamteck.Splines;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private RaceCameraSplineFollower raceCamera;
    [SerializeField] private SplineProjector _splineProjector;

    [Header("Move")]
    [SerializeField] private float _maxSpeed = 100;
    [SerializeField] private float _minSpeed = 0;
    [SerializeField] private float _acceleration = 1;
    [SerializeField] private float _deceleration = 2;
    //[SerializeField] private float _moveSpeed = 10;

    [Header("Rotation")]
    [SerializeField] private float _rotationSpeed = 1;
    [SerializeField] private float _rotationSmooth = 10;
    [SerializeField] private Transform _limiter;
    [SerializeField] private float _rotationSpeedToSpline = 5;
    [SerializeField] private bool _shouldLookAlongSplineForward = true;

    [Header("Jump")]
    [SerializeField] private float _jumpPower = 2;

    private float _currentSpeed = 0;
    private bool _isMoveButtonPressed = false;
    private float _currentAngle = 0;
    private Rigidbody _rigidbody = null;

    //---------------------------------------------------------------

    public void SetMove(bool isActive)
    {
        _isMoveButtonPressed = isActive;
    }

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
            _currentSpeed -= _deceleration * Time.deltaTime;

            if (_shouldLookAlongSplineForward)
            {
                LookAlongSplineForward();
            }
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

        var direction = transform.forward;
        var velocity = direction * Time.deltaTime * _currentSpeed;
        _rigidbody.MovePosition(_rigidbody.position + velocity);
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

    private void Rotation2() // WORKING ONE !!
    {
        var angle = Input.GetAxis("Mouse X");
        angle *= Time.deltaTime * _rotationSpeed;
        var yAxis = transform.TransformDirection(Vector3.up);
        var targetRot = _rigidbody.rotation * Quaternion.AngleAxis(angle, yAxis);

        //currentAngle += degreesRes;
        //currentAngle = Mathf.Clamp(currentAngle, -35, 35);

        //if (_rigidbody.rotation.eulerAngles.y >= 35 )
        //{
        //    targetRot = startRot * Quaternion.AngleAxis(-5, Vector3.up);
        //}
        //else if (_rigidbody.rotation.eulerAngles.y <= -35)
        //{
        //    targetRot = startRot * Quaternion.AngleAxis(5, Vector3.up);
        //}

        //var newRot = Quaternion.Slerp(startRot, targetRot, _rotationSmooth * Time.deltaTime);
        _rigidbody.rotation = targetRot;
    }

    private void LookAlongSplineForward()
    {
        var playerForward = transform.forward;
        var splineForward = _splineProjector.result.forward;
        var targetRot = _rigidbody.rotation * Quaternion.FromToRotation(playerForward, splineForward);
        targetRot = Quaternion.Slerp(_rigidbody.rotation, targetRot, Time.deltaTime * _rotationSpeedToSpline);

        _rigidbody.MoveRotation(targetRot);
    }

    private void Rotation3Extra()
    {
        //var direction = 

        var resRot = Quaternion.RotateTowards(_rigidbody.rotation, _limiter.rotation, Time.deltaTime * _rotationSmooth);
        _rigidbody.MoveRotation(resRot);
    }

    //private void MoveKeyboard()
    //{
    //    var yInput = Input.GetAxisRaw("Vertical");
    //    var xInput = Input.GetAxisRaw("Horizontal");

    //    var dir = new Vector3(xInput, 0, yInput).normalized;

    //    _rigidbody.MovePosition(_rigidbody.position + transform.TransformDirection(dir) * Time.deltaTime * _moveSpeed);
    //}

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
        _splineProjector = GetComponent<SplineProjector>();
    }

    private void Update()
    {

        Debug.DrawRay(_splineProjector.result.position, _splineProjector.result.forward * 1000, Color.yellow);

        var currentRot = transform.TransformDirection(Vector3.forward);
        Debug.DrawRay(transform.position, currentRot * 1000, Color.blue);
    }

    private void FixedUpdate()
    {
        Move();

        if (_isMoveButtonPressed)
        {
            //Rotation3();
            //Rotation3Extra();
            Rotation2();
        }
    }
}
