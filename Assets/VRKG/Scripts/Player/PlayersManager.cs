using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.MixedReality.Toolkit;
using Photon.Pun;
using UnityEngine;

[Serializable]
public class StartingPosRot
{
    public Vector3 Position;
    public int ActorNumber;
}

/* Handles users' spawning position */
public class PlayersManager : MonoBehaviour
{
    public FocusHandler FocusHndlr;
    public Transform AvatarPrefab;
    public List<StartingPosRot> StartingPosRots;
    public StartingPosRot BackupPosRot;

    public void OnJoinedRoom()
    {
        StartingPosRot playerStart =
            StartingPosRots.FirstOrDefault(s => s.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber);
        if (playerStart == null)
        {
            Debug.LogWarning("Unable to find starting position");
            playerStart = BackupPosRot;
        }

        MixedRealityPlayspace.Transform.position = playerStart.Position;
        MixedRealityPlayspace.Transform.LookAt(FocusHndlr.FocusPoint);
        PhotonNetwork.Instantiate(AvatarPrefab.name, playerStart.Position, Quaternion.identity);
    }
}
