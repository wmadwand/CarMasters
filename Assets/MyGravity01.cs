using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayer
{
    void PressMoveButton(bool isActive);
}

public class MyGravity01 : MonoBehaviour, IPlayer
{
    Rigidbody rigidbody;
    public float smoothRotation;
    public float gravity = -10;
    public float moveSpeed = 10;
    public int groundLayer;
    public float rayLength = 10;
    public float jumpPower = 2;

    public float acceleration = 1;
    public float maxSpeed = 100;
    public float rotationSpeed = 1;

    float currentSpeed = 0;
    float minSpeed = 0;
    float currentAngle;
    bool isMoveButtonPressed;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    void Move()
    {
        if (isMoveButtonPressed)
        {
            currentSpeed += acceleration * Time.deltaTime;

            currentSpeed = Mathf.Clamp(currentSpeed, 0, maxSpeed);
        }
        else if (!isMoveButtonPressed && currentSpeed > 0)
        {
            currentSpeed -= acceleration * 2 * Time.deltaTime;
        }

        if (currentSpeed <= 0)
        {
            Stop();
            return;
        }
        else
        {
            Go();
        }

        var dir = Vector3.forward * currentSpeed;
        rigidbody.MovePosition(rigidbody.position + transform.TransformDirection(dir) * Time.deltaTime);
    }

    public void PressMoveButton(bool isActive)
    {
        isMoveButtonPressed = isActive;
    }

    private void Start()
    {
        rigidbody.useGravity = false;
        rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        rigidbody.maxDepenetrationVelocity = 1000;
    }

    private void Update()
    {
        int layerMask = 1 << groundLayer;

        Debug.DrawRay(transform.position, -transform.up * rayLength, Color.green);

        //TODO: cast down 3 rays: face, center, back
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit, rayLength, layerMask))
        {
            Debug.DrawRay(transform.position, -hit.normal * rayLength, Color.red);

            var targetRot = Quaternion.FromToRotation(transform.up, hit.normal.normalized);
            targetRot *= rigidbody.rotation;
            var newRot = Quaternion.Slerp(rigidbody.rotation, targetRot, Time.deltaTime * smoothRotation);


            //TODO: move all the physics to FixedUpdate
            rigidbody.MoveRotation(newRot);
            rigidbody.AddForce(hit.normal.normalized * gravity);
        }



        if (Input.GetKeyDown("space"))
        {
            rigidbody.AddForce(hit.normal.normalized * -gravity * jumpPower, ForceMode.Impulse);
        }
    }

    private void FixedUpdate()
    {
        Move();

        if (isMoveButtonPressed)
        {
            Rotation2();
        }
    }

    void Go()
    {
        //rigidbody.constraints = rigidbody.constraints ^ RigidbodyConstraints.FreezePosition;
        rigidbody.constraints = rigidbody.constraints ^ RigidbodyConstraints.FreezeRotationY;
    }

    void Stop()
    {
        rigidbody.constraints = rigidbody.constraints | RigidbodyConstraints.FreezeRotationY;
        //rigidbody.velocity = Vector3.zero;
        //rigidbody.angularVelocity = Vector3.zero;


        //rigidbody.constraints = rigidbody.constraints | RigidbodyConstraints.FreezePosition;
        //TODO freeze position
    }

    void Move2()
    {
        var yInput = Input.GetAxisRaw("Vertical");
        var xInput = Input.GetAxisRaw("Horizontal");

        var dir = new Vector3(xInput, 0, yInput).normalized;

        rigidbody.MovePosition(rigidbody.position + transform.TransformDirection(dir) * Time.deltaTime * moveSpeed);
    }

    private void Rotation()
    {
        var inputHorizontal = Input.GetAxis("Mouse X");

        Debug.Log($"inputHorizontal {inputHorizontal}");

        var degrees = /*Mathf.Rad2Deg **/ inputHorizontal;

        var startRot = rigidbody.rotation;
        var inputAngle = degrees * rotationSpeed * Time.deltaTime;
        var newAngle = currentAngle + inputAngle;
        currentAngle = Mathf.Clamp(newAngle, -35, 35);

        Debug.Log($"currentRotation degrees {degrees}");

        rigidbody.rotation = rigidbody.rotation * Quaternion.Euler(0, currentAngle, 0);
    }

    private void Rotation2()
    {
        var degrees = Input.GetAxis("Mouse X");

        var startRot = rigidbody.rotation;
        var deegreesRes = degrees * rotationSpeed /** Time.deltaTime*/;
        var targetRot = startRot * Quaternion.AngleAxis(deegreesRes, Vector3.up);

        var currentAngle = Quaternion.Angle(Quaternion.identity, rigidbody.rotation);
        Debug.Log($"Current angle {currentAngle}");

        //if (rigidbody.rotation.eulerAngles.y > 30 || rigidbody.rotation.eulerAngles.y < -30)
        //{
        //    return;
        //}

        var newRot = Quaternion.Slerp(startRot, targetRot, smoothRotation * Time.deltaTime);
        rigidbody.MoveRotation(newRot);


    }

}
