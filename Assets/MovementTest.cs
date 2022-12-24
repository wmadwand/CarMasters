using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementTest : MonoBehaviour
{
    public float speed = 10;

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
        var xInput = Input.GetAxis("Vertical");

        var velocity = splineProjector.result.forward * yInput * speed/* * Time.deltaTime*/;
        rigidbody.AddForce(velocity);

        //rigidbody.MovePosition(rigidbody.position + velocity);
    }
}
