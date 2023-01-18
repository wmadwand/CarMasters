using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollJointConfig : MonoBehaviour
{

    public bool invert;

    public float torqueForce;
    public float angularDamping;
    public float maxForce;
    public float springForce;
    public float springDamping;

    public Vector3 targetVel;

    public Transform target;
    private GameObject limb;
    private JointDrive drive;
    private SoftJointLimitSpring spring;
    private ConfigurableJoint joint;
    private Quaternion startingRotation;
    RagdollJointImitation ragdollJointImitation;
    bool onAnimatorIK;

    void Start()
    {
        ragdollJointImitation = FindObjectOfType<RagdollJointImitation>();
        maxForce = 1000f;

        springForce = 0f;
        springDamping = 0f;

        targetVel = new Vector3(0f, 0f, 0f);

        drive.positionSpring = torqueForce;
        drive.positionDamper = angularDamping;
        drive.maximumForce = maxForce;

        spring.spring = springForce;
        spring.damper = springDamping;

        joint = gameObject.GetComponent<ConfigurableJoint>();

        joint.slerpDrive = drive;

        joint.linearLimitSpring = spring;
        joint.rotationDriveMode = RotationDriveMode.Slerp;
        joint.targetAngularVelocity = targetVel;
        joint.configuredInWorldSpace = false;
        joint.swapBodies = true;

        if (joint.angularXMotion != ConfigurableJointMotion.Limited || joint.angularYMotion != ConfigurableJointMotion.Limited || joint.angularZMotion != ConfigurableJointMotion.Limited)
        {
            joint.angularXMotion = ConfigurableJointMotion.Free;
            joint.angularYMotion = ConfigurableJointMotion.Free;
            joint.angularZMotion = ConfigurableJointMotion.Free;
        }
        joint.xMotion = ConfigurableJointMotion.Locked;
        joint.yMotion = ConfigurableJointMotion.Locked;
        joint.zMotion = ConfigurableJointMotion.Locked;

        startingRotation = Quaternion.Inverse(target.localRotation);
    }

    void LateUpdate()
    {
        if (!onAnimatorIK)
        {
            ragdollJointImitation.CopyInitialPositions();
            onAnimatorIK = true;
        }
        joint.targetRotation = target.localRotation * startingRotation;
    }


}
