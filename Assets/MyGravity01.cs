using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyGravity01 : MonoBehaviour
{
    Rigidbody rigidbody;
    public float smoothRotation;
    public float gravity = -10;
    public float moveSpeed = 10;


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
        }
        else if (!isMoveButtonPressed && currentSpeed > 0)
        {
            currentSpeed -= acceleration * 2 * Time.deltaTime;
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
    }

    private void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit))
        {
            Debug.DrawRay(transform.position, -hit.normal * 50, Color.red);

            var targetRot = Quaternion.FromToRotation(transform.up, hit.normal.normalized);
            targetRot *= rigidbody.rotation;
            var newRot = Quaternion.Slerp(rigidbody.rotation, targetRot, Time.deltaTime * smoothRotation);

            rigidbody.MoveRotation(newRot);
            rigidbody.AddForce(hit.normal.normalized * gravity);
        }

        Move();

        if (isMoveButtonPressed)
        {
            Rotation2();
        }

        if (Input.GetKeyDown("space"))
        {
            rigidbody.AddForce(hit.normal.normalized * -gravity, ForceMode.Impulse);
        }
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
        var newRot = Quaternion.Slerp(startRot, targetRot, smoothRotation * Time.deltaTime);
        rigidbody.MoveRotation(newRot);


    }

}
