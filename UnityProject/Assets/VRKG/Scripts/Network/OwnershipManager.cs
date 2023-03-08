using System;
using System.Collections;
using System.Collections.Generic;
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
