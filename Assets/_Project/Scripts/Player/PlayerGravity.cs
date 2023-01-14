using UnityEngine;

public class PlayerGravity : MonoBehaviour
{
    [SerializeField] private float _gravity = -10;
    [SerializeField] private float _smoothRotation = 10;
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

    private void Update()
    {
        int layerMask = 1 << _groundLayerIndex;

        Debug.DrawRay(transform.position, -transform.up * _rayLength, Color.green);

        var ray = new Ray(transform.position, -transform.up);
        var hits = Physics.RaycastAll(ray, _rayLength, layerMask);

        if (hits.Length < 1)
        {
            return;
        }

        var closestHit = hits[0];

        foreach (var item in hits)
        {
            if (item.distance < closestHit.distance)
            {
                closestHit = item;
            }
        }





        //TODO: cast down 3 rays: face, center, back
        //RaycastHit hit;
        //if (Physics.Raycast(transform.position, -transform.up, out hit, _rayLength, layerMask))
        //{
        Debug.DrawRay(transform.position, -closestHit.normal * _rayLength, Color.red);

        var targetRot = Quaternion.FromToRotation(transform.up, closestHit.normal.normalized);
        targetRot *= _rigidbody.rotation;
        var newRot = Quaternion.Slerp(_rigidbody.rotation, targetRot, Time.deltaTime * _smoothRotation);

        //TODO: move all the physics to FixedUpdate
        _rigidbody.MoveRotation(newRot);
        _rigidbody.AddForce(closestHit.normal.normalized * _gravity);
        //}

        if (Input.GetKeyDown("space"))
        {
            _rigidbody.AddForce(closestHit.normal.normalized * -_gravity * _jumpPower, ForceMode.Impulse);
        }
    }
}
