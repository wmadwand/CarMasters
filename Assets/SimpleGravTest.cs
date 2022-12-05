using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleGravTest : MonoBehaviour
{
    Rigidbody rigidbody;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        //var xInput = Input.GetAxis("Vertical");

        //rigidbody.MovePosition(rigidbody.position + transform.forward * xInput);

        Physics.Raycast(transform.position, -transform.up, out RaycastHit hit);

        Vector3 interpolatedNormal = BarycentricCoordinateInterpolator.GetInterpolatedNormal(hit);

        rigidbody.MoveRotation(Quaternion.FromToRotation(transform.up, interpolatedNormal) * rigidbody.rotation);
    }
}
