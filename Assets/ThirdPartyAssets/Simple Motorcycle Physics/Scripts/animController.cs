using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animController : MonoBehaviour
{
    Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

        if (transform.root.GetComponent<Rigidbody>().velocity.magnitude < 1f)
        {
            anim.CrossFade("Idle",0.3f);
        }
        
        if (transform.root.GetComponent<Rigidbody>().velocity.magnitude > 1f && transform.root.GetComponent<Rigidbody>().velocity.magnitude < 5f)
        {
            anim.CrossFade("acc",0.3f);
            
        }
        
        if (transform.root.GetComponent<Rigidbody>().velocity.magnitude > 5f && transform.root.GetComponent<Rigidbody>().velocity.magnitude < 10f)
        {
            anim.CrossFade("InSpeed",0.3f);
        }

        if (transform.root.eulerAngles.z > 271 && transform.root.eulerAngles.z < 359)
        {
            anim.CrossFade ("RightTurn",0.05f,0,(359 - transform.root.eulerAngles.z)/40);
        }
        
        else if (transform.root.eulerAngles.z > 1 && transform.root.eulerAngles.z < 89)
        {
            anim.CrossFade ("LeftTurn",0.05f, 0,(transform.root.eulerAngles.z-1)/40);
        }
        else if ((transform.root.eulerAngles.z > 0 && transform.root.eulerAngles.z < 1) || (transform.root.eulerAngles.z > 359.1f && transform.root.eulerAngles.z < 359.9f))
        {
            if(transform.root.GetComponent<Rigidbody>().velocity.magnitude > 10f)
            anim.CrossFade("InSpeedCont",0.5f);
        }
    }
}
