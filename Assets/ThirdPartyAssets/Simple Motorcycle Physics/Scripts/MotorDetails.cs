using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MotorDetails : MonoBehaviour
{
    MotorbikeController motorbikeController;
    GameObject t1,t2,t3,t4,t5;
    GameObject c1,c2;
    WheelHit hit;
    void Start()
    {
        motorbikeController = FindObjectOfType<MotorbikeController>();
        t1 = GameObject.Find("Velocity");
        t2 = GameObject.Find("Lean");
        t3 = GameObject.Find("Omega");
        t4 = GameObject.Find("Steer");
        t5 = GameObject.Find("Rear");
        c1 = GameObject.Find("/MotorcycleWRider/WheelHolderForward/WCollider");
        c2 = GameObject.Find("/MotorcycleWRider/WheelHolderBack/WCollider");

    }

    // Update is called once per frame
    void Update()
    {
        var realSpeed = motorbikeController.GetComponent<Rigidbody>().velocity.magnitude*2;
        t1.GetComponent<Text>().text = "Velocity [Km/h] : " + realSpeed;
        if(realSpeed>90)
        t1.GetComponent<Text>().color = Color.red;
        else
        t1.GetComponent<Text>().color = Color.white;
        var lean = motorbikeController.transform.eulerAngles.z<180?motorbikeController.transform.eulerAngles.z:motorbikeController.transform.eulerAngles.z-360;
        t2.GetComponent<Text>().text = "Lean Angle [deg] : " + lean;
        if(lean>40||lean<-40)
        t2.GetComponent<Text>().color = Color.red;
        else
        t2.GetComponent<Text>().color = Color.white;
        t3.GetComponent<Text>().text = "Control ω [rad⋅s−1] : " + motorbikeController.controlAngle;
        t4.GetComponent<Text>().text = "Steer Angle [deg] : " + c1.GetComponent<WheelCollider>().steerAngle;
        c2.GetComponent<WheelCollider>().GetGroundHit(out hit);
        t5.GetComponent<Text>().text = "Rear Grip Force [N] : " + hit.force;
    }
}
