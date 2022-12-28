using Dreamteck.Splines;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _maxSpeed = 100f;
    [SerializeField] private float _minSpeed = 0f;
    [SerializeField] private float _acceleration = 1f;
    [SerializeField] private float _deceleration = 2f;

    public bool IsMoving => _currentSpeed > _minSpeed;

    private float _currentSpeed = 0f;
    private bool _isMoveButtonPressed = false;
    private Rigidbody _rigidbody = null;
    SplineProjector _projector;

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
            //Stop();
        }
        else
        {
            //Go();
        }
    }

    private void DoMove(Vector3 direction)
    {
        var velocity = direction /** Time.deltaTime*/ * _currentSpeed;
        //_rigidbody.MovePosition(_rigidbody.position + velocity);

        _rigidbody.AddForce(velocity);
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

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _projector = GetComponent<SplineProjector>();
    }

    private void Update()
    {
        CalculateMove();

        Debug.DrawRay(transform.position, transform.forward * 1000, Color.blue);
    }

    private void Start()
    {
        var _projector = GetComponent<SplineProjector>();
        var startPosition = new Vector3(_projector.result.position.x, _rigidbody.position.y, _rigidbody.position.z);
        
        _rigidbody.position = _projector.result.position;
    }

    public void SetDirection(Vector3 value)
    {
        _direction = value;
    }

    private Vector3 _direction;

    private void FixedUpdate()
    {
        if (_currentSpeed > _minSpeed)
        {
            _direction = transform.forward;

            //var dir = _projector.result.forward;
            DoMove(_direction);
        }        
    }
}