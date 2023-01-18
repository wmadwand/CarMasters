using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrakeLights : MonoBehaviour
{
    [Tooltip("The Gameobject that has the Tail light material")]
    public GameObject TailLightObject;
    public Material Toff, Ton, ToffSecondary, TonSecondary;
    int indexMaterial = 0;
    int indexMaterialSecondary = 0;

    Renderer mrender;
    Material[] m;
    void Start()
    {
        mrender = TailLightObject.GetComponent<Renderer>();
        m = mrender.sharedMaterials;
        if (Ton != null || Toff != null)
            for (int i = 0; i < m.Length; i++)
            {
                if (m[i] == Toff || m[i] == Ton)
                    indexMaterial = i;
            }
        if (TonSecondary != null || ToffSecondary != null)
            for (int i = 0; i < m.Length; i++)
            {
                if (m[i] == ToffSecondary || m[i] == TonSecondary)
                    indexMaterialSecondary = i;
            }
    }

    // Update is called once per frame
    void Update()
    {
        if (Ton != null)
        {
            if (Input.GetAxisRaw("Vertical") < 0 || Input.GetKey(KeyCode.Space))
            {
                m[indexMaterial] = Ton;
                if (TonSecondary != null)
                    m[indexMaterialSecondary] = TonSecondary;
                mrender.sharedMaterials = m;
            }

            else
            {
                m[indexMaterial] = Toff;
                if (ToffSecondary != null)
                    m[indexMaterialSecondary] = ToffSecondary;
                mrender.sharedMaterials = m;

            }
        }

    }
}

