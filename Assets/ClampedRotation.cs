using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClampedRotation : MonoBehaviour
{
    private void Update()
    {
       var xDegrees = Input.GetAxisRaw("Horizontal");

        var curRotation = transform.rotation;
        //var targetRot = Quaternion.LookRotation()
    }
}
