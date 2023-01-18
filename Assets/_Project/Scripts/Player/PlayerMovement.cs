using Dreamteck.Splines;
using UnityEngine;

namespace CarMasters.Gameplay.Player
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private float _maxSpeed = 100f;
        [SerializeField] private float _minSpeed = 0f;
        [SerializeField] private float _acceleration = 1f;
        [SerializeField] private float _deceleration = 2f;

        private float _currentSpeed = 0f;
        private bool _isMoving = false;
        private Rigidbody _rigidbody = null;

        //---------------------------------------------------------------

        public void SetMove(bool value)
        {
            // But isMoving when _currentSpeed > _minSpeed!!
            // maybe set isDriving?

            _isMoving = value;
        }

        public void StopRightThere()
        {
            _currentSpeed = 0;
            _isMoving = false;
        }

        //---------------------------------------------------------------

        private void CalculateMove()
        {
            if (_isMoving)
            {
                _currentSpeed += _acceleration * Time.deltaTime;
                _currentSpeed = Mathf.Clamp(_currentSpeed, _minSpeed, _maxSpeed);
            }
            else if (!_isMoving && _currentSpeed > _minSpeed)
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

        private void DoMove(Vector3 direction)
        {
            var velocity = direction * Time.deltaTime * _currentSpeed;
            _rigidbody.MovePosition(_rigidbody.position + velocity);
        }

        private void Go()
        {
            //rigidbody.constraints = rigidbody.constraints ^ RigidbodyConstraints.FreezePosition;
            _rigidbody.constraints = _rigidbody.constraints ^ RigidbodyConstraints.FreezeRotationY;
        }

        public void Stop()
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
        }

        private void Update()
        {
            CalculateMove();

            Debug.DrawRay(transform.position, transform.forward * 1000, Color.blue);
        }

        private void FixedUpdate()
        {
            if (_currentSpeed > _minSpeed)
            {
                var dir = transform.forward;
                DoMove(dir);
            }
        }
    } 
}