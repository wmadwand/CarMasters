using UnityEngine;

public class PlayerGravity : MonoBehaviour
{
    [SerializeField] private float _gravity = -10;
    [SerializeField] private float _smoothRotation = 10;
    [SerializeField] private float _rayLength = 10;
    [SerializeField] private LayerMask _groundLayerMask;

    private Rigidbody _rigidbody = null;
    private int _groundLayerIndex = 0;

    //---------------------------------------------------------------

    public void Jump(float jumpPower)
    {
        //_rigidbody.AddForce(hit.normal.normalized * -gravity * jumpPower, ForceMode.Impulse);
    }

    public Vector3 GetGravityNormal => hitTemp.normal.normalized;

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

    RaycastHit hitTemp;

    private void Update()
    {
        int layerMask = 1 << _groundLayerIndex;

        Debug.DrawRay(transform.position, -transform.up * _rayLength, Color.green);

        //TODO: cast down 3 rays: face, center, back
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit, _rayLength, layerMask))
        {
            Debug.DrawRay(transform.position, -hit.normal * _rayLength, Color.red);

            var targetRot = Quaternion.FromToRotation(transform.up, hit.normal.normalized);
            targetRot *= _rigidbody.rotation;
            var newRot = Quaternion.Slerp(_rigidbody.rotation, targetRot, Time.deltaTime * _smoothRotation);


            //TODO: move all the physics to FixedUpdate
            _rigidbody.MoveRotation(newRot);
            _rigidbody.AddForce(hit.normal.normalized * _gravity);

            hitTemp = hit;
        }



        //if (Input.GetKeyDown("space"))
        //{
        //    rigidbody.AddForce(hit.normal.normalized * -gravity * jumpPower, ForceMode.Impulse);
        //}
    }
}
