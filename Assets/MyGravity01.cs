using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyGravity01 : MonoBehaviour
{
    Rigidbody rigidbody;
    public float smoothRotation;
    public float gravity = -10;
    public float moveSpeed = 10;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        rigidbody.useGravity = false;
        rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
    }

    private void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit))
        {
            Debug.DrawRay(transform.position, hit.normal, Color.red);

            var targetRot = Quaternion.FromToRotation(transform.up, hit.normal.normalized);
            targetRot *= rigidbody.rotation;
            var newRot = Quaternion.Slerp(rigidbody.rotation, targetRot, Time.deltaTime * smoothRotation);

            rigidbody.MoveRotation(newRot);

            rigidbody.AddForce(hit.normal.normalized * gravity);
        }

        Move();
    }

    void Move()
    {
        var yInput = Input.GetAxisRaw("Vertical");
        var xInput = Input.GetAxisRaw("Horizontal");

        var dir = new Vector3(xInput, 0, yInput).normalized;

        rigidbody.MovePosition(rigidbody.position + transform.TransformDirection(dir) * Time.deltaTime * moveSpeed);
    }
}
