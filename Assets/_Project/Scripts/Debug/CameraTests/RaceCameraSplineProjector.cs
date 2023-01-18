using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceCameraSplineProjector : MonoBehaviour
{
    public Transform target;
    public float zOffset = 5;
    SplineProjector projector;

    private void Awake()
    {
        projector = GetComponent<SplineProjector>();
    }

    private void Update()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, target.position.z /*+ zOffset/*//** projector.result.forward.z*/);
    }
}
