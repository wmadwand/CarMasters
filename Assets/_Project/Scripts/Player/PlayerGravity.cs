using UnityEngine;

public class PlayerGravity : MonoBehaviour
{
    [SerializeField] private float _gravity = -10;
    [SerializeField] private float _smoothRotation = 10;
    [SerializeField] Transform[] _gravityRays;
    [SerializeField] private float _rayLength = 10;
    [SerializeField] private LayerMask _groundLayerMask;
    [SerializeField] private float _jumpPower;
    [SerializeField] private float _heightOffset = 1;

    private Rigidbody _rigidbody = null;
    private int _groundLayerIndex = 0;
    private Vector3 _defaultDownDirection = Vector3.up;

    private bool _isOnGround = true;

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

        //TODO: for all the obstacles with rigidbody set CollisionDetectionMode.ContinuousDynamic;
        //
        _rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    private void Update()
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
        var newRot = Quaternion.Slerp(_rigidbody.rotation, targetRot, Time.deltaTime * _smoothRotation);

        //TODO: move all the physics to FixedUpdate
        _rigidbody.MoveRotation(newRot);
        _rigidbody.AddForce(averageNormal.normalized * _gravity);
        //}


        var checkGroundHit = CastRay(transform.position, -averageNormal.normalized, _rayLength);
        _isOnGround = checkGroundHit.collider ? checkGroundHit.distance <= _heightOffset : false;
        Debug.Log($"_isOnGround {_isOnGround}");

        if (Input.GetKeyDown("space"))
        {
            _rigidbody.AddForce(averageNormal.normalized * -_gravity * _jumpPower, ForceMode.Impulse);
        }
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
