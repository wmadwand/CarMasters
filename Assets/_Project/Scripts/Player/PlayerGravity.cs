using UnityEngine;

public class PlayerGravity : MonoBehaviour
{
    [SerializeField] private float _gravity = -10;
    [SerializeField] private float _smoothRotation = 10;
    [SerializeField] Transform[] _gravityRays;
    [SerializeField] private float _rayLength = 10;
    [SerializeField] private LayerMask _groundLayerMask;
    [SerializeField] private float _jumpPower;
    [SerializeField] private float _groundedOffset;
    [SerializeField] private float _groundedRadius;

    private Rigidbody _rigidbody = null;
    private int _groundLayerIndex = 0;
    private Vector3 _defaultDownDirection = Vector3.up;

    private bool _isGrounded = true;
    private Vector3 _averageGravityNormal;

    //---------------------------------------------------------------

    public void SetGravity(float value)
    {
        _gravity = value;
    }

    public void Jump(float jumpPower)
    {
        //_rigidbody.AddForce(hit.normal.normalized * -gravity * jumpPower, ForceMode.Impulse);
    }

    //---------------------------------------------------------------

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _groundLayerIndex = _groundLayerMask.ToSingleLayer();
    }

    private void Start()
    {
        _rigidbody.useGravity = false;
        _rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        _rigidbody.maxDepenetrationVelocity = 1000;
        _rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }

    private void Update()
    {
        CheckGrounded();
    }

    private void FixedUpdate()
    {
        _averageGravityNormal = CheckGravity();

#if DEVELOPMENT
        if (Input.GetKeyDown("space") && _isGrounded)
        {
            _rigidbody.AddForce(_averageGravityNormal.normalized * -_gravity * _jumpPower, ForceMode.Impulse);
        }
#endif
    }

    private Vector3 CheckGravity()
    {
        var averageNormal = Vector3.zero;
        foreach (var item in _gravityRays)
        {
            var hit = CastRay(item.position, -transform.up, _rayLength);
            averageNormal += hit.normal;
        }

        Debug.DrawRay(transform.position, -averageNormal.normalized * _rayLength, Color.cyan);

        var targetRot = Quaternion.FromToRotation(transform.up, averageNormal.normalized);
        targetRot *= _rigidbody.rotation;
        var newRot = Quaternion.Slerp(_rigidbody.rotation, targetRot, Time.fixedDeltaTime  * _smoothRotation);

        _rigidbody.MoveRotation(newRot);
        _rigidbody.AddForce(averageNormal.normalized * _gravity);

        return averageNormal;
    }

    private void CheckGrounded()
    {
        var spherePos = new Vector3(transform.position.x, transform.position.y - _groundedOffset, transform.position.z);
        _isGrounded = Physics.CheckSphere(spherePos, _groundedRadius, _groundLayerMask, QueryTriggerInteraction.Ignore);

        Debug.Log($"_isOnGround {_isGrounded}");
    }

    private void OnDrawGizmosSelected()
    {
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        if (_isGrounded) Gizmos.color = transparentGreen;
        else Gizmos.color = transparentRed;

        var spherePos = new Vector3(transform.position.x, transform.position.y - _groundedOffset, transform.position.z);
        Gizmos.DrawSphere(spherePos, _groundedRadius);
    }

    //TODO: to get the best experience of smoothing rotation to the surface
    //use Physics.BoxCastAll and so on
    private RaycastHit CastRay(Vector3 startPos, Vector3 direction, float length)
    {
        int layerMask = 1 << _groundLayerIndex;

        Debug.DrawRay(startPos, direction * length, Color.green);

        var ray = new Ray(startPos, direction);
        var hits = Physics.RaycastAll(ray, length, layerMask);

        RaycastHit closestHit = default;

        if (hits.Length < 1)
        {
            return closestHit;
        }

        closestHit = hits[0];

        foreach (var item in hits)
        {
            if (item.distance < closestHit.distance)
            {
                closestHit = item;
            }
        }

        return closestHit;
    }
}