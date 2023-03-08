using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Photon.Pun;
using UnityEngine;

/*
 * MIT License

Copyright (c) 2023 Alberto Accardo, Daniele Monaco, Maria Angela Pellegrino, Vittorio Scarano, Carmine Spagnuolo

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

 */

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
