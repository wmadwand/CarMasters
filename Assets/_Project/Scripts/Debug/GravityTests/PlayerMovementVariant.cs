using UnityEngine;
using System.Collections;

public class PlayerMovementVariant : MonoBehaviour
{

    public float walkAcceleration = 1000;
    public GameObject cameraObject;
    public float maxWalkSpeed = 20;
    Vector3 horizontalMovement;

    public float jumpVelocity = 600;
    public float maxSlope = 45;

    public float walkDecelleration = 1;
    public float airAccMod = .5f;
    float walkDecX;
    float walkDecY;
    float walkDecZ;
    public bool grounded = true;
    public float grav = 750;

    //MouseLookFpsScript cameraMouseLook;

    public float lookSmoothDamp = .1f;

    public float xLookSensitivity = 320f;
    float currYRotation = 0;
    float yRotation;
    float yRotationVel;

    public float sler = .1f;

    float diffY;
    public int layerNumber;


    void Start()
    {
        //cameraMouseLook = (MouseLookFpsScript)cameraObject.GetComponent("MouseLookFpsScript");
    }

    Rigidbody rigidbody;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        //Setting limit to Rigidbody velocity
        horizontalMovement = new Vector3(rigidbody.velocity.x, rigidbody.velocity.y,
                                                                    rigidbody.velocity.z);
        if (horizontalMovement.magnitude > maxWalkSpeed)
        {
            horizontalMovement = horizontalMovement.normalized;
            horizontalMovement *= maxWalkSpeed;
            print("kittens");
        }
        rigidbody.velocity = new Vector3(horizontalMovement.x, horizontalMovement.y,
                                                               horizontalMovement.z);



        //Adding friction
        if (grounded)
        {
            rigidbody.velocity = new Vector3(Mathf.SmoothDamp(rigidbody.velocity.x, 0, ref
                                                                                        walkDecX, walkDecelleration),
             Mathf.SmoothDamp(rigidbody.velocity.y, 0, ref walkDecY, walkDecelleration),
             Mathf.SmoothDamp(rigidbody.velocity.z, 0, ref walkDecZ, walkDecelleration));
        }


        //Setting up raycast
        RaycastHit groundRay;
        Debug.DrawRay(transform.position, transform.up * -50, Color.red);
        bool hitGround = Physics.Raycast(transform.position, -1 * transform.up, out groundRay);



        //Rotating player based on grounds normal
        int gravLayMask = (1 << layerNumber);
        if (Physics.Raycast(transform.position, -1 * transform.up, out groundRay, 100,
      gravLayMask))
        {
            Quaternion temp = Quaternion.FromToRotation(transform.up, groundRay.normal);
            temp = temp * transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, temp, sler);
        }



        //Rotate the player to look, change "y" 
        yRotation += Input.GetAxis("Mouse X") * xLookSensitivity;
        diffY = yRotation - diffY;
        currYRotation = Mathf.SmoothDamp(currYRotation, diffY, ref yRotationVel,
                                                                                   lookSmoothDamp);
        transform.Rotate(0, currYRotation * Time.deltaTime, 0);
        diffY = yRotation;



        //Moving the player
        if (grounded)
        {
            rigidbody.AddRelativeForce(Input.GetAxis("Horizontal") * walkAcceleration * Time.deltaTime, -1 * grav * Time.deltaTime, Input.GetAxis("Vertical") * walkAcceleration * Time.deltaTime);
        }
        else
        {
            rigidbody.AddRelativeForce(Input.GetAxis("Horizontal") * walkAcceleration * Time.deltaTime * airAccMod, -1 * grav * Time.deltaTime, Input.GetAxis("Vertical") * walkAcceleration * Time.deltaTime * airAccMod);
        }

        //Jump
        if (Input.GetButtonDown("Jump") && grounded)
        {
            rigidbody.AddRelativeForce(0, jumpVelocity, 0);
        }

    }

    //Checks if the collider is touching ground beneath the player. 
    void OnCollisionStay(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            if (Vector3.Angle(contact.normal, transform.up) < maxSlope)
            {
                grounded = true;
            }
        }
    }


    void OnCollisionExit()
    {
        grounded = false;
    }

}
