using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public PlayerRotationNew player;
    public float speed;
    public VariableJoystick variableJoystick;
    public Rigidbody rb;

    public void Update()
    {
        player.SetXInput(variableJoystick.Horizontal, null);

        //Vector3 direction = Vector3.forward * variableJoystick.Vertical + Vector3.right * variableJoystick.Horizontal;
        //rb.AddForce(direction * speed * Time.fixedDeltaTime, ForceMode.VelocityChange);
    }
}
