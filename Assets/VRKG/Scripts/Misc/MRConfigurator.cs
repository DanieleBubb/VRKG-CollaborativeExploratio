using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Input;
using Photon.Pun;
using UnityEngine;

public class MRConfigurator : MonoBehaviour
{
    private void Awake()
    {
        PointerUtils.SetGazePointerBehavior(PointerBehavior.AlwaysOff);
        PhotonNetwork.SendRate = 30;
        PhotonNetwork.SerializationRate = 30;
    }
}
