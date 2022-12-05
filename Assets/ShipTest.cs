using UnityEngine;
using System.Collections;

public class ShipTest : MonoBehaviour
{
    /*Ship handling parameters*/
    public float fwd_accel = 100f;
    public float fwd_max_speed = 200f;
    public float brake_speed = 200f;
    public float turn_speed = 50f;

    /*Auto adjust to track surface parameters*/
    public float hover_height = 3f;     //Distance to keep from the ground
    public float height_smooth = 10f;   //How fast the ship will readjust to "hover_height"
    public float pitch_smooth = 5f;     //How fast the ship will adjust its rotation to match track normal

    /*We will use all this stuff later*/
    private Vector3 prev_up;
    public float yaw;
    private float smooth_y;
    private float current_speed;


    void Update()
    {
        /*Here we get user input to calculate the speed the ship will get*/
        if (Input.GetKey(KeyCode.W))
        {
            /*Increase our current speed only if it is not greater than fwd_max_speed*/
            current_speed += (current_speed >= fwd_max_speed) ? 0f : fwd_accel * Time.deltaTime;
        }
        else
        {
            if (current_speed > 0)
            {
                /*The ship will slow down by itself if we dont accelerate*/
                current_speed -= brake_speed * Time.deltaTime;
            }
            else
            {
                current_speed = 0f;
            }
        }

        /*We get the user input and modifiy the direction the ship will face towards*/
        yaw += turn_speed * Time.deltaTime * Input.GetAxis("Horizontal");
        /*We want to save our current transform.up vector so we can smoothly change it later*/
        prev_up = transform.up;
        /*Now we set all angles to zero except for the Y which corresponds to the Yaw*/
        transform.rotation = Quaternion.Euler(0, yaw, 0);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, -prev_up, out hit))
        {
            Debug.DrawLine(transform.position, hit.point);

            /*Here are the meat and potatoes: first we calculate the new up vector for the ship using lerp so that it is smoothed*/
            Vector3 desired_up = Vector3.Lerp(prev_up, hit.normal, Time.deltaTime * pitch_smooth);
            /*Then we get the angle that we have to rotate in quaternion format*/
            Quaternion tilt = Quaternion.FromToRotation(transform.up, desired_up);
            /*Now we apply it to the ship with the quaternion product property*/
            transform.rotation = tilt * transform.rotation;

            /*Smoothly adjust our height*/
            smooth_y = Mathf.Lerp(smooth_y, hover_height - hit.distance, Time.deltaTime * height_smooth);
            transform.localPosition += prev_up * smooth_y;
        }

        /*Finally we move the ship forward according to the speed we calculated before*/
        transform.position += transform.forward * (current_speed * Time.deltaTime);
    }
}
