using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementTest : MonoBehaviour
{
    public float speed = 10;
    public float xInputSpeed = 10;

    Rigidbody rigidbody;

    SplineProjector splineProjector;

    // Start is called before the first frame update
    void Start()
    {

    }

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        splineProjector = GetComponent<SplineProjector>();
    }

    // Update is called once per frame
    void Update()
    {
        var yInput = Input.GetAxis("Vertical");
        var xInput = Input.GetAxis("Mouse X");

        var velocity = splineProjector.result.forward * yInput * speed/* * Time.deltaTime*/;
        rigidbody.AddForce(velocity);

        splineProjector.motion.offset += new Vector2(xInput * xInputSpeed * Time.deltaTime, splineProjector.motion.offset.y);

        //rigidbody.MovePosition(rigidbody.position + velocity);
    }
}
