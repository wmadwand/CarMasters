using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagneticScript : MonoBehaviour
{
    public GameObject Car;
    public float magnetDistance = 100f;     //Maximum distance that a vehicle can be pulled by the track
    public float magnetForce = 50f;     //Magnitude of the magnet force

    private int LayerTrack;

    void Update()
    {
        RaycastHit hit;

        LayerTrack = LayerMask.NameToLayer("GravTrack");

        Rigidbody rb = Car.GetComponent<Rigidbody>();

        if (Physics.Raycast(Car.transform.position, Car.transform.TransformDirection(Vector3.down), out hit, magnetDistance))
        {
            if (hit.transform.gameObject.layer == LayerTrack)
            {
                Debug.Log("YES");
                rb.useGravity = false;
                rb.AddForce(magnetForce * Car.transform.TransformDirection(Vector3.down));
            }
            else
            {
                Debug.Log("Nah");
                rb.useGravity = true;
            }
        }

    }

}