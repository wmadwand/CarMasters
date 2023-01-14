using UnityEngine;

public class PlayerGravity : MonoBehaviour
{
    [SerializeField] private float _gravity = -10;
    [SerializeField] private float _smoothRotation = 10;
    [SerializeField] Transform[] _gravityRays;
    [SerializeField] private float _rayLength = 10;
    [SerializeField] private LayerMask _groundLayerMask;
    [SerializeField] private float _jumpPower;

    private Rigidbody _rigidbody = null;
    private int _groundLayerIndex = 0;

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

    private RaycastHit CastRayDown(Vector3 startPos)
    {
        int layerMask = 1 << _groundLayerIndex;

        Debug.DrawRay(startPos, -transform.up * _rayLength, Color.green);

        var ray = new Ray(startPos, -transform.up);
        var hits = Physics.RaycastAll(ray, _rayLength, layerMask);

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

    private void Update()
    {
        var hit01 = CastRayDown(_gravityRays[0].position);
        var hit02 = CastRayDown(_gravityRays[1].position);

        var normal01 = hit01.normal;
        var normal02 = hit02.normal;
        var avarageNormal = normal01 + normal02;

        Debug.DrawRay(transform.position, avarageNormal * _rayLength, Color.blue);

        var targetRot = Quaternion.FromToRotation(transform.up, avarageNormal.normalized);
        targetRot *= _rigidbody.rotation;
        var newRot = Quaternion.Slerp(_rigidbody.rotation, targetRot, Time.deltaTime * _smoothRotation);

        //TODO: move all the physics to FixedUpdate
        _rigidbody.MoveRotation(newRot);
        _rigidbody.AddForce(avarageNormal.normalized * _gravity);
        //}

        if (Input.GetKeyDown("space"))
        {
            _rigidbody.AddForce(avarageNormal.normalized * -_gravity * _jumpPower, ForceMode.Impulse);
        }
    }
}
