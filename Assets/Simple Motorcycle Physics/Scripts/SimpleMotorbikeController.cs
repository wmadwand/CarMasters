using UnityEngine;
using System.Collections;

public class SimpleMotorbikeController : MonoBehaviour
{

    public WheelCollider WColForward;
    public WheelCollider WColBack;

    public Transform wheelF;
    public Transform wheelB;
    [SerializeField]
    public GameObject handles;
    [SerializeField]
    public GameObject RearMudGuard;
    public Vector3 RearMudGuardSusOffset;
    [Tooltip("Lower values mean higher sensitivity")]

    public float preventGlitchAngle = 40;

    [Tooltip("Experimental Feature : Only for controlled low speeds")]
    public bool canArtificialStoppie = false;
    [Range(0.1f, 1f)]
    public float stoppieAmount = 0.3f;
    private bool fallen = false;

    public float maxSteerAngle = 45;
    public float maxMotorTorque = 500;
    [Tooltip("Adds more speed. Inaccurate from a physics standpoint. Arcade Feature. Values too high will break the realism of the system and make the bike glitch badly.")]
    public float ArtificialAcceleration = 1000f;
    [Tooltip("Adds more braking power. Inaccurate from a physics standpoint. Arcade Feature. Values too high will break the realism of the system, but it will definitely apply hard brakes")]
    [Range(0, 1)]

    public float ArtificialBrake = 0;
    public float maxForwardBrake = 400;
    public float maxBackBrake = 400;

    public float wheelRadius = 0.7f;

    public float steerSensivity = 30;
    public float controlAngle = 25;
    public float controlOmega = 30;

    public float lowSpeed = 8;
    public float highSpeed = 25;

    private WheelData[] wheels;

    private Transform thisTransform;
    public Vector3 com;
    public float comy;
    Rigidbody rb;
    float startSteerSensitivity;


    public int currentGear = 1; 
    public float revValue;
    float initialMotorTorque;


    public class WheelData
    {

        public WheelData(Transform transform, WheelCollider collider)
        {
            wheelTransform = transform;
            wheelCollider = collider;
            wheelStartPos = transform.transform.localPosition;
        }

        public Transform wheelTransform;
        public WheelCollider wheelCollider;
        public Vector3 wheelStartPos;
        public float rotation = 0f;
    }

    public struct MotorbikeInput
    {
        public float steer;
        public float acceleration;
        public float brakeForward;
        public float brakeBack;
    }

    void Start()
    {

        wheels = new WheelData[2];
        wheels[0] = new WheelData(wheelF, WColForward);
        wheels[1] = new WheelData(wheelB, WColBack);

        thisTransform = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
        startSteerSensitivity = steerSensivity;
        initialMotorTorque = maxMotorTorque;
    }


    void FixedUpdate()
    {
        uprightCheck();
        if (fallen == false)
            uprightForce();

        var input = new MotorbikeInput();

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) input.acceleration = 1;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) input.steer += 1;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) input.steer -= 1;

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            input.brakeBack = 0.3f;
            input.brakeForward = 0.5f;
        }
        if (Input.GetKey(KeyCode.Space))
        {
            input.brakeForward = 1f;
        }

        motoMove(motoControl(input));
        updateWheels();
        steerHandles();
        steerHelper();
        RearMudGuardSuspension();
        CalcGear();
        if (canArtificialStoppie) // for natural stoppie increase forward brake to 50000 and Front Wheel collider forward friction to 5. Around those values a natural stoppie can be performed.
            CalcStoppie(); //Requires prevent falling

        if (speedVal < 0.01f && thisTransform.up.y < 0.2f && Mathf.Abs(prevOmega) < 0.001f || Input.GetKeyDown(KeyCode.R) || fallen == true)
        {
            reset();
        }

        //Bug fix stationary wheel rotates on asphalt
        if (speedVal < 0.5f && !Input.GetKey(KeyCode.W)) // or UP ARROW
        {
            
        }

    }
    void Awake()
    {
        Time.timeScale = 1.15f; //Makes simulation movement more agile. You can delete this line if it interferes with your project settings.
    }

    private void reset()
    {
        Transform t = GetComponent<Transform>();
        t.position = t.position + new Vector3(0, 0.1f, 0);
        t.eulerAngles = new Vector3(0, transform.rotation.eulerAngles.y, 0);
        rb.angularDrag = 100;
        fallen = false;
    }

    private Vector3 prevPos = new Vector3();
    private float prevAngle = 0;
    private float prevOmega = 0;
    private float speedVal = 0;
    private float prevSteer = 0f;

    private MotorbikeInput motoControl(MotorbikeInput input)
    {
        var posNow = thisTransform.position;
        var speed = (posNow - prevPos) / Time.fixedDeltaTime;
        prevPos = posNow;

        speedVal = speed.magnitude;
        var moveForward = speed.normalized;

        var angle = Vector3.Dot(moveForward, Vector3.Cross(thisTransform.up, new Vector3(0, 1, 0)));
        var omega = (angle - prevAngle) / Time.fixedDeltaTime;
        prevAngle = angle;
        prevOmega = omega;


        if (speedVal < lowSpeed)
        {
            float t = speedVal / lowSpeed;
            input.steer *= t * t;
            omega *= t * t;
            angle = angle * (2 - t);
            input.acceleration += Mathf.Abs(angle) * 3 * (1 - t);
        }

        if (speedVal > highSpeed)
        {
            float t = speedVal / highSpeed;
            if (omega * angle < 0f)
            {
                omega *= t;
            }
        }
        input.steer *= (1 - 2.3f * angle * angle);
        input.steer = 1f / (speed.sqrMagnitude + 1f) * (input.steer * steerSensivity + angle * controlAngle + omega * controlOmega);
        float steerDelta = 10 * Time.fixedDeltaTime;
        input.steer = Mathf.Clamp(input.steer, prevSteer - steerDelta, prevSteer + steerDelta);
        prevSteer = input.steer;

        return input;
    }
    private void uprightForce()
    {
        rb.angularDrag -= 100 * Time.deltaTime;
        rb.angularDrag = Mathf.Clamp(rb.angularDrag, 0, 100);

        var turnAngle = transform.eulerAngles.z;
        if (transform.eulerAngles.z > 180)
            turnAngle = transform.eulerAngles.z - 360;


        com.y = comy;

        if (speedVal < 1 && !Input.GetKey(KeyCode.W))
        {

            var rot = Quaternion.FromToRotation(transform.up, Vector3.up);
            rb.AddTorque(new Vector3(rot.x, rot.y, rot.z) * Mathf.Abs(turnAngle) * 100 , ForceMode.Acceleration);
            rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezeRotationZ;

        }
        else
        {  
           rb.constraints = RigidbodyConstraints.None; 
        }
        // else if (speedVal > 1.5f)
        // {
        //     if (turnAngle > preventFallAngle || turnAngle < -preventFallAngle)
        //     {
        //         com.y = 0;
        //     }
        // }
        // if (turnAngle > preventFallAngle)
        //     com.x += Time.deltaTime*preventFallInfluence;
        // else if (turnAngle < -preventFallAngle)
        //     com.x -= Time.deltaTime*preventFallInfluence;
        // else if (turnAngle < 40 && turnAngle > -40)
        //     com.x = 0;
        // if(Mathf.Abs(turnAngle)>preventFallAngle+5)


        rb.centerOfMass = com;


    }
    public void uprightCheck()
    {
        var turnAngle = transform.eulerAngles.z;
        if (transform.eulerAngles.z > 180)
            turnAngle = transform.eulerAngles.z - 360;
        if (Mathf.Abs(turnAngle) > preventGlitchAngle)
        {
            fallen = true;
            Debug.Log("Crashed & Reseted");
        }
    }

    private void motoMove(MotorbikeInput input)
    {
        if (speedVal > 1)
            WColForward.steerAngle = Mathf.Clamp(input.steer, -1, 1) * maxSteerAngle;
        else
            WColForward.steerAngle = Mathf.Clamp(input.steer, -speedVal, speedVal);



        WColForward.brakeTorque = maxForwardBrake * input.brakeForward;
            WColBack.brakeTorque = maxBackBrake * input.brakeBack;
        WColBack.motorTorque = maxMotorTorque * input.acceleration;
        if (speedVal < highSpeed)
            rb.AddForce(transform.forward * ArtificialAcceleration * input.acceleration);

        if (Input.GetAxis("Vertical") < 0)
            rb.velocity = new Vector3(rb.velocity.x * (1 - ArtificialBrake / 10), rb.velocity.y, rb.velocity.z * (1 - ArtificialBrake / 10));


    }

    private void updateWheels()
    {
        float delta = Time.fixedDeltaTime;

        foreach (WheelData w in wheels)
        {
            WheelHit hit;

            Vector3 localPos = w.wheelTransform.localPosition;
            if (w.wheelCollider.GetGroundHit(out hit))
            {
                localPos.y -= Vector3.Dot(w.wheelTransform.position - hit.point, transform.up) - wheelRadius;
                w.wheelTransform.localPosition = localPos;
            }
            else
            {
                localPos.y = w.wheelStartPos.y;
            }

            w.rotation = Mathf.Repeat(w.rotation + delta * w.wheelCollider.rpm * 360.0f / 60.0f, 360f);
            //w.wheelTransform.localRotation = Quaternion.Euler(w.rotation, w.wheelCollider.steerAngle, 0);
            w.wheelTransform.localRotation = Quaternion.Euler(w.rotation, Mathf.Lerp(w.wheelTransform.localRotation.y, w.wheelCollider.steerAngle, Time.deltaTime * 10), 0);

        }
    }
    private void steerHandles()
    {
        handles.transform.localRotation = Quaternion.Euler(0, Mathf.Lerp(handles.transform.localRotation.y, WColForward.steerAngle, Time.deltaTime * 10), 0);
    }
    private void RearMudGuardSuspension()
    {
        WheelHit hit;
        if (WColBack.GetGroundHit(out hit))
            RearMudGuard.transform.rotation = Quaternion.LookRotation(transform.position - wheelB.transform.position - RearMudGuardSusOffset, transform.forward);
    }
    private void CalcStoppie()
    {
        var stoppieAngle = transform.eulerAngles.x;
        if (transform.eulerAngles.x > 180)
            stoppieAngle = transform.eulerAngles.x - 360;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.Space))
            com.z += (speedVal * Time.deltaTime) / 5;
        else
            com.z -= Time.deltaTime * 100;
        if (com.z < 0 || stoppieAngle > 5 + speedVal)
        {
            com.z = 0;
            if (stoppieAngle > 50)
                com.z -= stoppieAngle / 10;
        }

        else if (com.z > stoppieAmount)
            com.z = stoppieAmount;
    }

    void steerHelper()
    {

        controlAngle = Mathf.Clamp(controlAngle, 48, 60);
        if (Input.anyKey)
            controlAngle -= 1;
        else
            controlAngle += 1;
        var turnAngle = transform.eulerAngles.z;
        if (transform.eulerAngles.z > 180)
            turnAngle = transform.eulerAngles.z - 360;

        if (turnAngle > 10 && Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            rb.AddTorque(-transform.forward * 10 * turnAngle / 15, ForceMode.Acceleration);

        }
        else if (turnAngle < -10 && Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            rb.AddTorque(transform.forward * 10 * -turnAngle / 15, ForceMode.Acceleration);
        }
        if (turnAngle > 46 || turnAngle < -46)
            controlAngle += 2;
        
        //Sets Sideways friction with speed gradations
        if (speedVal < 10)
            SetWheelFriction(1.5f);
        else if(speedVal < 20 && speedVal > 10)
            SetWheelFriction(2);
        else if(speedVal < 30 && speedVal > 20)
            SetWheelFriction(3f);
        else if(speedVal < 40 && speedVal > 20)
            SetWheelFriction(6);
            else if(speedVal < 60 && speedVal > 40)
            SetWheelFriction(9f);
            else if(speedVal < 80 && speedVal > 60)
            SetWheelFriction(12);
            else if(speedVal < 100 && speedVal > 80)
            SetWheelFriction(15f);
    }
    void SetWheelFriction(float friction)
    {
        WheelFrictionCurve wfc;
        wfc = WColBack.sidewaysFriction;
        wfc.stiffness = friction;
        WColBack.sidewaysFriction = wfc;
        WColForward.sidewaysFriction = wfc;
    }
    void CalcGear()
    {
        var prevGear = currentGear;
        currentGear =  Mathf.FloorToInt(rb.velocity.magnitude/13);
        if(currentGear!=prevGear)
        StartCoroutine(MotorDisengage());
        revValue = rb.velocity.magnitude%13/13;
    }

    IEnumerator MotorDisengage()
    {
        maxMotorTorque = 0;
        yield return new WaitForSeconds(0.1f);
        maxMotorTorque = initialMotorTorque;
    }

}
