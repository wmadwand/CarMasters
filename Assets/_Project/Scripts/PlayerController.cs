using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float acceleration = 1;
    public float maxSpeed = 100;
    public float rotationSpeed = 1;
    public float smoothRotation = .5f;

    Rigidbody _rigidbody;
    float inputHorizontal;
    float currentSpeed = 0;
    float minSpeed = 0;
    bool isMoveButtonPressed = false;



    /*Auto adjust to track surface parameters*/
    public float hover_height = 3f;     //Distance to keep from the ground
    public float height_smooth = 10f;   //How fast the ship will readjust to "hover_height"
    public float pitch_smooth = 5f;     //How fast the ship will adjust its rotation to match track normal

    /*We will use all this stuff later*/
    private Vector3 prev_up;
    public float yaw;
    private float smooth_y;
    private float current_speed;


    public void Move()
    {
        if (isMoveButtonPressed)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, maxSpeed, acceleration);
        }


    }

    public void PressMoveButton(bool isActive)
    {
        isMoveButtonPressed = isActive;
    }



    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        //Physics.gravity = new Vector3(0, 9.81f, 0);
        //_rigidbody.useGravity
    }

    private void Update()
    {
        if (isMoveButtonPressed)
        {
            currentSpeed += acceleration * Time.deltaTime;

            Rotation();
            //var clamp
        }
        else if (!isMoveButtonPressed && currentSpeed > 0)
        {
            currentSpeed -= acceleration * 2 * Time.deltaTime;
        }




        Debug.Log($"currentSpeed {currentSpeed}");



        RaycastHit hit;
        if (Physics.Raycast(transform.position, -prev_up, out hit))
        {
            Debug.DrawLine(transform.position, hit.point);

            /*Here are the meat and potatoes: first we calculate the new up vector for the ship using lerp so that it is smoothed*/
            Vector3 desired_up = Vector3.Lerp(prev_up, hit.normal, Time.deltaTime * pitch_smooth);
            /*Then we get the angle that we have to rotate in quaternion format*/
            Quaternion tilt = Quaternion.FromToRotation(transform.up, desired_up);
            /*Now we apply it to the ship with the quaternion product property*/
            _rigidbody.rotation = tilt * _rigidbody.rotation;

            /*Smoothly adjust our height*/
            smooth_y = Mathf.Lerp(smooth_y, hover_height - hit.distance, Time.deltaTime * height_smooth);
            transform.localPosition += prev_up * smooth_y;
        }

        /*Finally we move the ship forward according to the speed we calculated before*/
        transform.position += transform.forward * (current_speed * Time.deltaTime);

    }

    Quaternion newRot;
    float currentAngle;

    private void Rotation()
    {
        inputHorizontal = Input.GetAxis("Mouse X");

        Debug.Log($"inputHorizontal {inputHorizontal}");

        var degrees = /*Mathf.Rad2Deg **/ inputHorizontal;

        var startRot = _rigidbody.rotation;
        var inputAngle = degrees * rotationSpeed * Time.deltaTime;
        var newAngle = currentAngle + inputAngle;
        currentAngle = Mathf.Clamp(newAngle, -35, 35);
        //newRot = startRot * Quaternion.Euler(0, degreesRes, 0);

        //var startRot = _rigidbody.rotation;
        //var deegreesRes = degrees * rotationSpeed /** Time.deltaTime*/;
        //var targetRot = startRot * Quaternion.AngleAxis(deegreesRes, Vector3.up);
        //newRot = Quaternion.Slerp(startRot, targetRot, smoothRotation * Time.deltaTime);
        //_rigidbody.MoveRotation(newRot);

        Debug.Log($"currentRotation degrees {degrees}");
    }

    private void FixedUpdate()
    {
        _rigidbody.MovePosition(transform.position + transform.forward * currentSpeed);
        //_rigidbody.MoveRotation(newRot);
        _rigidbody.rotation = Quaternion.Euler(0, currentAngle, 0);
    }
}