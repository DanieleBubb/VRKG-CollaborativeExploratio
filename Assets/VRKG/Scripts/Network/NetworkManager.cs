using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

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
