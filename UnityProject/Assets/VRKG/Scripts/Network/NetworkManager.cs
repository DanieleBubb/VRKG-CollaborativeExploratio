using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
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

/* Forwards events regarding Photon Network */
public class NetworkManager : MonoBehaviourPunCallbacks
{
    public PlayersManager PlayersMan;
    public UIEdgesManager UIEdges;
    public UIParamsManager UIParams;
    public MPGraphGenerator GraphGen;
    public StartingAnimation StartAnim;
    private QueryEntry query;
    
    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster()");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarningFormat("OnDisconnected(): reason {0}", cause);
    }
    
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("OnJoinRoomFailed(): The room is not available");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom()");
        PlayersMan.OnJoinedRoom();
        UIParams.OnJoinedRoom();
        UIEdges.OnJoinedRoom();
        StartAnim.OnJoinedRoom();
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("OnCreatedRoom()");
        GraphGen.OnRoomCreated(query);
    }

    public void ConnectToServer()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public void JoinQueryRoom(QueryEntry query)
    {
        this.query = query;
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsVisible = true;
        roomOptions.IsOpen = true;
        roomOptions.MaxPlayers = 16;
        PhotonNetwork.JoinOrCreateRoom(query.Name, roomOptions, TypedLobby.Default);
    }
    
}
