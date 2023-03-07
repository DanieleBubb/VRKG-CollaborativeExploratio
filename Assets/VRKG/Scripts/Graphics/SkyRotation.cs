using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Rotates skybox over time */
public class SkyRotation : MonoBehaviour {

    public float RotateSpeed = 0.0001f;

    void Update() 
    {
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * RotateSpeed);
    }
}
