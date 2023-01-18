using UnityEngine;
using System.Collections;

public class HelicopterCamera : MonoBehaviour
{

    Transform target;
    public GameObject PrimaryTarget;
    public GameObject SecondaryTarget;
    public float distance = 20.0f;
    public float height = 5.0f;
    public float heightDamping = 2.0f;

    public float lookAtHeight = 0.0f;

    public Rigidbody parentRigidbody;

    public float rotationSnapTime = 0.3F;

    public float distanceSnapTime;
    public float distanceMultiplier;
    float initialdistanceMultiplier;

    private Vector3 lookAtVector;

    private float usedDistance;

    float wantedRotationAngle;
    float wantedHeight;

    float currentRotationAngle;
    float currentHeight;

    Vector3 wantedPosition;

    private float yVelocity = 0.0F;
    private float zVelocity = 0.0F;
    PerfectMouseLook perfectMouseLook;
    float LateRot;
    public bool counterRotation;
    MotorbikeController motorbikeController;
    bool changed,prevFallen;



    void Start()
    {
        perfectMouseLook = GetComponent<PerfectMouseLook>();
        initialdistanceMultiplier = distanceMultiplier;
        target = PrimaryTarget.transform;
        parentRigidbody = PrimaryTarget.GetComponent<Rigidbody>();
        motorbikeController = FindObjectOfType<MotorbikeController>();
    }

    void LateUpdate()
    {
        if(motorbikeController.fallen!=prevFallen)
        changed = false;
        prevFallen = motorbikeController.fallen;
        
        if (motorbikeController.fallen && changed == false)
        {
            if(SecondaryTarget == null)
            target = GameObject.FindGameObjectWithTag("Ragdoll").transform.Find("Armature/Hips");
            parentRigidbody = target.gameObject.GetComponent<Rigidbody>();
            changed = true;
        }

        else if(motorbikeController.fallen == false && changed == false)
        {
            target = PrimaryTarget.transform;
            parentRigidbody = PrimaryTarget.GetComponent<Rigidbody>();
            changed = true;
        }

        wantedHeight = target.position.y + height;
        currentHeight = transform.position.y;

        wantedRotationAngle = target.eulerAngles.y;
        currentRotationAngle = transform.eulerAngles.y;
        if (counterRotation)
        {
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                LateRot += 0.333f;
            else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                LateRot -= 0.333f;
            else
                LateRot = Mathf.Lerp(LateRot, 0, 0.1f);
            LateRot = Mathf.Clamp(LateRot, -50 / (parentRigidbody.velocity.magnitude / 20) + 1, 50 / (parentRigidbody.velocity.magnitude / 20) + 1);
        }
        if (perfectMouseLook.movement == false)
            currentRotationAngle = Mathf.SmoothDampAngle(currentRotationAngle, wantedRotationAngle + LateRot, ref yVelocity, rotationSnapTime);

        currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightDamping * Time.deltaTime);

        wantedPosition = target.position;
        wantedPosition.y = currentHeight;

        if (Input.GetKey(KeyCode.W))
            distanceMultiplier = initialdistanceMultiplier;
        else
            distanceMultiplier = 0;

        usedDistance = Mathf.SmoothDampAngle(usedDistance, distance + (parentRigidbody.velocity.magnitude * distanceMultiplier), ref zVelocity, distanceSnapTime);

        wantedPosition += Quaternion.Euler(0, currentRotationAngle, 0) * new Vector3(0, 0, -usedDistance);
        lookAtVector = new Vector3(0, lookAtHeight, 0);


        transform.position = wantedPosition;

        transform.LookAt(target.position + lookAtVector);

    }

}