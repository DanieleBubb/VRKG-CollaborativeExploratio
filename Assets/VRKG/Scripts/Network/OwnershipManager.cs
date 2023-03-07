using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

/* Handles ownership of manipulation of the graph. A user can interact with the graph only if
 no other user is interacting or has picked a node to focus on*/
public class OwnershipManager : MonoBehaviourPun
{
    public GraphContainer GraphCont;
    private int ownerID = -1;

    public bool AmITheOwner()
    {
        return PhotonNetwork.LocalPlayer.ActorNumber == ownerID;
    }
    
    public bool CanIBeTheOwner()
    {
        return ownerID == -1;
    }

    public bool TrySetMeAsOwner()
    {
        if (ownerID == -1)
        {
            GraphCont.TransferNodeOwnershipToMe();
            photonView.RPC("SetNewOwner", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber);
            return true;
        }

        return false;
    }

    public void ResetOwnership()
    {
        if (AmITheOwner())
        {
            photonView.RPC("SetNewOwner", RpcTarget.All, -1);
        }
    }

    [PunRPC]
    public void SetNewOwner(int newOwnerID)
    {
        ownerID = newOwnerID;
        OnNewOwner(newOwnerID);
    }

    public void OnNewOwner(int newOwner)
    {
        GraphCont.SetAllManipulationEnabled(newOwner == -1 || newOwner == PhotonNetwork.LocalPlayer.ActorNumber);
    }
}
