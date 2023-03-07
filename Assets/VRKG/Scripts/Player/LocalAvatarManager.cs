using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Photon.Pun;
using UnityEngine;

/* Handles avatar tracking, from camera and controllers to head, body and hands of the avatar */
public class LocalAvatarManager : MonoBehaviour
{
    public GameObject Head;
    public GameObject Body;
    public GameObject LeftHand;
    public GameObject RightHand;
    protected Vector3 headPos;
    protected Vector3 headEuler;
    protected bool lHandPresent;
    protected Vector3 lHandPos;
    protected Vector3 lHandEuler;
    protected bool rHandPresent;
    protected Vector3 rHandPos;
    protected Vector3 rHandEuler;

    protected void UpdateAvatar()
    {
        transform.position = headPos;
        Head.transform.eulerAngles = headEuler;
        Vector3 bodyEuler = headEuler;
        bodyEuler.x = 0f;
        bodyEuler.z = 0f;
        Body.transform.eulerAngles = bodyEuler;
        LeftHand.SetActive(lHandPresent);
        if (lHandPresent)
        {
            LeftHand.transform.position = lHandPos;
            LeftHand.transform.eulerAngles = lHandEuler;
        }
        RightHand.SetActive(rHandPresent);
        if (rHandPresent)
        {
            RightHand.transform.position = rHandPos;
            RightHand.transform.eulerAngles = rHandEuler;
        }
    }

    protected void FillAvatarValues()
    {
        Ray ray;
        if (InputRayUtils.TryGetRay(InputSourceType.Head, Handedness.Any, out ray))
        {
            headPos = ray.origin;
            headEuler = Camera.main.transform.eulerAngles;
        }

        lHandPresent = false;
        rHandPresent = false;
        foreach (var controller in CoreServices.InputSystem.DetectedControllers)
        {
            if (controller.InputSource.SourceType == InputSourceType.Controller)
            {
                if (controller.ControllerHandedness == Handedness.Left)
                {
                    foreach (MixedRealityInteractionMapping inputMapping in controller.Interactions)
                    {
                        if (inputMapping.InputType == DeviceInputType.SpatialPointer)
                        {
                            lHandPresent = true;
                            lHandPos = inputMapping.PositionData;
                            lHandEuler = inputMapping.RotationData.eulerAngles;
                        }
                    }
                }
                else if (controller.ControllerHandedness == Handedness.Right)
                {
                    foreach (MixedRealityInteractionMapping inputMapping in controller.Interactions)
                    {
                        if (inputMapping.InputType == DeviceInputType.SpatialPointer)
                        {
                            rHandPresent = true;
                            rHandPos = inputMapping.PositionData;
                            rHandEuler = inputMapping.RotationData.eulerAngles;
                        }
                    }
                }
            }
        }
    }

    protected virtual void Update()
    {
        FillAvatarValues();
        UpdateAvatar();
    }
}
