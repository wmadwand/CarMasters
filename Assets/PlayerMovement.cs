using Dreamteck.Splines;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private RaceCameraSplineFollower raceCamera;

    [Header("Move")]
    [SerializeField] private float _maxSpeed = 100;
    [SerializeField] private float _minSpeed = 0;
    [SerializeField] private float _acceleration = 1;
    [SerializeField] private float _deceleration = 2;

    [Header("Rotation")]
    [SerializeField] private float _rotationSpeed = 1;
    [SerializeField] private float _rotationSmooth = 10;
    [SerializeField] private float _rotationValuableRate = 3;
    [SerializeField] private float _rotationToSplineSpeed = 5;
    [SerializeField] private bool _shouldLookAlongSplineForward = true;

    [Header("Jump")]
    [SerializeField] private float _jumpPower = 2;

    private SplineProjector _splineProjector;
    private float _currentSpeed = 0;
    private float xInput = 0;
    private bool _isMoveButtonPressed = false;
    private Rigidbody _rigidbody = null;

    //---------------------------------------------------------------

    public void SetMove(bool isActive)
    {
        _isMoveButtonPressed = isActive;
    }

    //---------------------------------------------------------------

    private void CalculateMove()
    {
        if (_isMoveButtonPressed)
        {
            _currentSpeed += _acceleration * Time.deltaTime;
            _currentSpeed = Mathf.Clamp(_currentSpeed, _minSpeed, _maxSpeed);
        }
        else if (!_isMoveButtonPressed && _currentSpeed > _minSpeed)
        {
            _currentSpeed -= _deceleration * Time.deltaTime;
        }

        if (_currentSpeed <= _minSpeed)
        {
            Stop();
        }
        else
        {
            Go();
        }
    }

    void DoMove(Vector3 direction)
    {
        //var direction = transform.forward;
        //direction = _splineProjector.result.forward;
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

    private void Rotation(float angle)
    {
        angle *= Time.deltaTime;
        var targetRot = _rigidbody.rotation * Quaternion.AngleAxis(angle, transform.up);

        _rigidbody.MoveRotation(targetRot);
    }

    private void LookAlongSplineForward()
    {
        var playerForward = transform.forward;
        var splineForward = _splineProjector.result.forward;
        var targetRot = _rigidbody.rotation * Quaternion.FromToRotation(playerForward, splineForward);
        targetRot = Quaternion.Slerp(_rigidbody.rotation, targetRot, Time.deltaTime * _rotationToSplineSpeed);

        _rigidbody.MoveRotation(targetRot);
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
        _splineProjector = GetComponent<SplineProjector>();
    }

    private void Update()
    {
        xInput = Input.GetAxisRaw("Mouse X");

        CalculateMove();

        Debug.DrawRay(_splineProjector.result.position, _splineProjector.result.forward * 1000, Color.yellow);
        Debug.DrawRay(transform.position, transform.forward * 1000, Color.blue);
    }

    private void FixedUpdate()
    {
        var inputAngleRotation = xInput * _rotationSpeed;

        if (_currentSpeed > _minSpeed)
        {
            //var dir = _splineProjector.result.forward;
            //if (Mathf.Abs(inputAngleRotation) > _rotationValuableRate)
            //{
            //    dir = transform.forward;
            //}

           var dir = transform.forward;
            DoMove(dir);
        }

        if (_isMoveButtonPressed)
        {
            if (Mathf.Abs(inputAngleRotation) > _rotationValuableRate)
            {
                Rotation(inputAngleRotation);
            }
            else
            {
                if (_shouldLookAlongSplineForward)
                {
                    LookAlongSplineForward();
                }
            }
        }
        else if (!_isMoveButtonPressed && _currentSpeed > _minSpeed)
        {
            if (_shouldLookAlongSplineForward)
            {
                LookAlongSplineForward();
            }
        }
    }
}