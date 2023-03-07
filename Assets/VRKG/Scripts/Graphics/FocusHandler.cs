using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.UI;
using Photon.Pun;

/* Handles the focus of the node */

public class FocusHandler : MonoBehaviourPun
{
    public float PullingForce;
    public float PushingForce;
    public Vector3 FocusPoint;
    public GraphContainer GraphCont;
    public OwnershipManager OwnershipMan;
    public UIParamsManager UIParams;
    public UIEdgesManager UIEdges;
    private GameObject selectedNode;
    private bool focused;
    private bool focusSelected;

    public bool IsFocused
    {
        get
        {
            return focused;
        }
    }
    
    #region EVENTS
    public void OnNodeHover(ManipulationEventData data)
    {
        if(!focused)
            selectedNode = data.ManipulationSource;
    }
    
    public void OnNodeUnhover(ManipulationEventData data)
    {
        if(!focused)
            selectedNode = null;
    }

    public void OnEdgePressed(EdgeManager edge)
    {
        Rigidbody rb = selectedNode.GetComponent<Rigidbody>();
        rb.AddForce(Camera.main.transform.forward * PushingForce, ForceMode.Impulse);
        GameObject nextNode = edge.Node1 == selectedNode ? edge.Node2 : edge.Node1;
        photonView.RPC("OnEdgePressedNetwork", RpcTarget.All, selectedNode.GetPhotonView().ViewID);
        selectedNode = nextNode;
        OnNodeSelected(selectedNode);
    }

    void OnNodeSelected(GameObject node)
    {
        photonView.RPC("OnNodeSelectedNetwork", RpcTarget.All, node.GetPhotonView().ViewID);
        OwnershipMan.TrySetMeAsOwner();
    }

    [PunRPC]
    void OnNodeSelectedNetwork(int nodeViewID)
    {
        focused = true;
        PhotonView nodeView = PhotonNetwork.GetPhotonView(nodeViewID);
        selectedNode = nodeView.gameObject;
        selectedNode.GetComponent<NodeMaterialController>().OnSelect(true);
        UIParams.OnFocus(selectedNode);
        if (nodeView.IsMine)
        {
            UIEdges.OnNodeSelected(selectedNode);
        }
    }

    void OnNodeUnselected()
    {
        photonView.RPC("OnNodeUnselectedNetwork", RpcTarget.All);
        OwnershipMan.ResetOwnership();
    }
    
    [PunRPC]
    void OnNodeUnselectedNetwork()
    {
        focused = false;
        if(OwnershipMan.AmITheOwner())
            selectedNode.GetComponent<NodeMaterialController>().OnUnselect(true);
        else
            selectedNode.GetComponent<NodeMaterialController>().OnHoverEnd();
        UIParams.OnUnfocus();
        UIEdges.OnNodeUnselected();
    }
    
    [PunRPC]
    void OnEdgePressedNetwork(int nodeID)
    {
        PhotonView nodeView = PhotonNetwork.GetPhotonView(nodeID);
        nodeView.GetComponent<NodeMaterialController>().OnHoverEnd(true);
    }

    public void OnFocusSelected()
    {
        focusSelected = true;
    }
    #endregion

    /* Focuses on the node pointed by the controller, using the button 1 of the keyboard or the button A of the controller */
    void Update()
    {
        bool wasFocused = false;
        if (Input.GetKeyDown(KeyCode.Alpha1) || focusSelected)
        {
            focusSelected = false;
            if (focused && OwnershipMan.AmITheOwner())
            {
                wasFocused = true;
                focused = false;
                GraphCont.SetAllManipulationEnabled(true);
                OnNodeUnselected();
            }
            else
            {
                if (selectedNode != null && OwnershipMan.CanIBeTheOwner())
                {
                    focused = true;
                    GraphCont.ResetAllMasses();
                    GraphCont.SetAllManipulationEnabled(false);
                    OnNodeSelected(selectedNode);
                }
            }
        }
        if (focused)
        {
            float distanceFromFocus = Vector3.Distance(selectedNode.transform.position, FocusPoint);
            Vector3 forceDirection = (FocusPoint - selectedNode.transform.position).normalized;
            Rigidbody rb = selectedNode.GetComponent<Rigidbody>();
            rb.AddForce(forceDirection * distanceFromFocus * PullingForce, ForceMode.Force);
        }
        else if (wasFocused)
        {
            Rigidbody rb = selectedNode.GetComponent<Rigidbody>();
            rb.AddForce(Camera.main.transform.forward * PushingForce, ForceMode.Impulse);
        }
    }
    
}
