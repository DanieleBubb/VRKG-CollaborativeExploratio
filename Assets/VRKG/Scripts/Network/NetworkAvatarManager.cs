using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Photon.Pun;
using UnityEngine;

/* synchronizes avatar positions and rotations among the clients */
public class NetworkAvatarManager : LocalAvatarManager, IPunObservable
{
    private void Start()
    {
        PlayersManager playersManager = FindObjectOfType<PlayersManager>();
        transform.parent = playersManager.transform;
        PhotonView photonView = GetComponent<PhotonView>();
        if (photonView.IsMine)
        {
            Head.SetActive(false);
            Body.SetActive(false);
            LeftHand.SetActive(false);
            RightHand.SetActive(false);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            FillAvatarValues();
            stream.Serialize(ref headPos);
            stream.Serialize(ref headEuler);
            stream.Serialize(ref lHandPresent);
            stream.Serialize(ref lHandPos);
            stream.Serialize(ref lHandEuler);
            stream.Serialize(ref rHandPresent);
            stream.Serialize(ref rHandPos);
            stream.Serialize(ref rHandEuler);
        }
        else
        {
            stream.Serialize(ref headPos);
            stream.Serialize(ref headEuler);
            stream.Serialize(ref lHandPresent);
            stream.Serialize(ref lHandPos);
            stream.Serialize(ref lHandEuler);
            stream.Serialize(ref rHandPresent);
            stream.Serialize(ref rHandPos);
            stream.Serialize(ref rHandEuler);
            UpdateAvatar();
        }
    }

    protected override void Update()
    {
    }
}
