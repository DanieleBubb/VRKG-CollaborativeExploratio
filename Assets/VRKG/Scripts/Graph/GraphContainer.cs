using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;

/* stores the references to all nodes and edges present in the scene,
        forwards events and manipulates physics parameters*/
public class GraphContainer : MonoBehaviour
{
    public UIEdgesManager UIEdges;
    public StartingAnimation StartAnim;
    public OwnershipManager OwnershipMan;
    private List<GameObject> Nodes;
    private List<GameObject> Edges;

    private void Awake()
    {
        Nodes = new List<GameObject>();
        Edges = new List<GameObject>();
    }

    public void RegisterNode(GameObject node)
    {
        Nodes.Add(node);
        StartAnim.OnNodeRegistered(node);
    }
    
    public void UnregisterNode(GameObject node)
    {
        Nodes.Remove(node);
    }

    public void OnNodeGrabbed(GameObject node)
    {
        if (OwnershipMan.TrySetMeAsOwner())
        {
            foreach (GameObject curNode in Nodes)
            {
                if (curNode != node)
                {
                    curNode.GetComponent<NodeManipulationEvents>().OnOtherNodeGrabbed();
                }
            }
        }
    }

    public void OnNodeReleased(GameObject node)
    {
        OwnershipMan.ResetOwnership();
    }
    
    public void ResetAllMasses()
    {
        foreach (GameObject curNode in Nodes)
        {
            curNode.GetComponent<NodeManipulationEvents>().ResetInitialMass();
        }
    }
    
    public void SetAllManipulationEnabled(bool enabled)
    {
        foreach (GameObject curNode in Nodes)
        {
            curNode.GetComponent<NodeManipulationEvents>().SetManipulationEnabled(enabled);
        }
    }

    public void TransferNodeOwnershipToMe()
    {
        foreach (GameObject curNode in Nodes)
        {
            PhotonView view = curNode.GetPhotonView();
            if(!view.AmOwner)
                view.TransferOwnership(PhotonNetwork.LocalPlayer);
        }
    }

    public void RegisterEdge(GameObject edge)
    {
        Edges.Add(edge);
        UIEdges.RegisterEdge(edge.GetComponent<EdgeManager>());
        StartAnim.OnEdgeRegistered(edge);
    }
    
    public void UnregisterEdge(GameObject edge)
    {
        Edges.Remove(edge);
        UIEdges.UnregisterEdge(edge.GetComponent<EdgeManager>());
    }

    
}
