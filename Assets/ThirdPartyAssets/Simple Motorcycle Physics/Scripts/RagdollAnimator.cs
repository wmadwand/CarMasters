using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollAnimator : MonoBehaviour
{
    GameObject riderRagdoll;
    Rigidbody rb;
    Collider col;
    Animator animator;
    RaycastHit hit;
    int rand;
    void Start()
    {
        riderRagdoll = GameObject.FindWithTag("Ragdoll");
        rb = riderRagdoll.transform.Find("Armature/Hips").GetComponent<Rigidbody>();
        col = riderRagdoll.transform.Find("Armature/Hips").GetComponent<Collider>();
        animator = GetComponent<Animator>();
        rb.maxAngularVelocity = 30f;

        InvokeRepeating("GenRandomNum",0,0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        
        // For Blend Tree usage
        // animator.SetFloat("Velocity", Mathf.Clamp(rb.velocity.magnitude -1.1f , -1,1));
        // animator.SetFloat("Angular",Mathf.Clamp(rb.angularVelocity.magnitude -1f , -1,1));
        // For simplicity's sake we will use basic animator crossfade


        //For velocity and height based animation 
        // Physics.Raycast(rb.transform.position, Vector3.down, out hit, Mathf.Infinity, 0);
        // if (rb.velocity.magnitude < 2)
        // {
        //     animator.Play("Still");
        // }

        // else if (rb.velocity.magnitude > 5 && rb.velocity.magnitude < 20)
        // {
        //     if (hit.distance < 0.2f)
        //         animator.Play("MotionProtect");
        //     else
        //         animator.Play("MotionLowSpeed");
        // }

        // else
        // {
        //     if (hit.distance > 0.2f)
        //         animator.Play("MotionHighSpeed");
        //     else
        //         animator.Play("MotionProtect");
        // }

            

        // Simple Random Motion generation algorithm based on probabilities
        if (rb.velocity.magnitude < 2)
            animator.Play("Still");

        else
        {
            //Simple Random generation of Motion
            if (rand == 1 || rand == 2)
                animator.Play("MotionProtect");
            
                //animator.CrossFade("MotionProtect", 0.2f);
            else if (rand == 3)
                animator.Play("MotionHighSpeed");
                //animator.CrossFade("MotionHighSpeed", 0.2f);
            else
                animator.Play("MotionLowSpeed");
                //animator.CrossFade("MotionLowSpeed", 0.2f);

        }



    }

    void GenRandomNum()
    {
        rand = Random.Range(1, 5);
    }
}
