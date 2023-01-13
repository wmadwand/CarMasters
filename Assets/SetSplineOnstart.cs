using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Dreamteck.Splines;

public class SetSplineOnstart : MonoBehaviour
{
    public float rayLength = 100;
    [SerializeField] private LayerMask _groundLayerMask;

    private void Start()
    {
        RaycastHit[] hit;
        int _groundLayerIndex = _groundLayerMask.ToSingleLayer();
        int layerMask = 1 << _groundLayerIndex;

        Debug.DrawRay(transform.position, -transform.up * rayLength, Color.cyan);

        Ray ray = new Ray(transform.position, -transform.up);
        var result = Physics.RaycastAll(ray, rayLength, layerMask);

        var resultWithPathGen = result.Where(item => item.collider.GetComponent<MeshGenerator>()).ToList();
        var closest = resultWithPathGen[0];

        foreach (var item in resultWithPathGen)
        {
            if (item.distance < closest.distance)
            {
                closest = item;
            }
        }

        GetComponent<SplineProjector>().spline = closest.collider.GetComponent<MeshGenerator>().spline;
    }
}
