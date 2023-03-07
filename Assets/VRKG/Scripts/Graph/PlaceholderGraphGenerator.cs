using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine.Events;
using ObjectManip = Microsoft.MixedReality.Toolkit.UI.ObjectManipulator;
using Random = UnityEngine.Random;

[Serializable]
public class SpringParameters
{
    public Vector3 Anchor;
    public float Spring;
    public float Damper;
    public float MinDistance;
    public float MaxDistance;
}

public class PlaceholderGraphGenerator : MonoBehaviour
{
    public int EdgesFirstNode;
    public int OtherNodes;

    public float FirstNodeDistance;
    public float AdjacentNodesDistance;
    public float OtherNodesDistance;
    public SpringParameters SpringParams;
    
    public GameObject Parent;
    public Transform NodePrefab;
    public Transform EdgePrefab;
    public FocusHandler FocusHndlr;
    public GraphContainer GraphCont;

    [ContextMenu ("TestAngles")]
    public void TestAngles()
    {
        for (int j = 0; j < 10; j++)
        {
            
            Debug.Log(Mathf.Floor((float)j / 2) + 0.5f);
        }
    }

    [ContextMenu ("Generate Graph")]
    public void GenerateGraph()
    {
        // Delete previous Graph
        for (int i = 0; i < Parent.transform.childCount; ++i)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
        // Spawn First node at the center
        Vector3 firstNodePos = Camera.main.transform.position + Camera.main.transform.forward * FirstNodeDistance;
        GameObject firstNode = Instantiate(NodePrefab, firstNodePos, Quaternion.identity).gameObject;
        InitNode(firstNode, "Central Node", "Central Node");
        // Spawn Adjacent nodes splitting the angle
        List<GameObject> adjacentNodes = new List<GameObject>();
        for (int i = 0; i < EdgesFirstNode; ++i)
        {
            Vector3 curNodeDirection = Quaternion.AngleAxis(360f / EdgesFirstNode * i, Camera.main.transform.forward) * Camera.main.transform.right;
            Vector3 curNodePosition = firstNodePos + curNodeDirection * AdjacentNodesDistance;
            GameObject curNode = Instantiate(NodePrefab, curNodePosition, Quaternion.identity).gameObject;
            InitNode(curNode, "Node " + i, "Node " + i);
            adjacentNodes.Add(curNode);

            // Spawn edges between first and adjacent nodes
            GameObject curEdge = Instantiate(EdgePrefab).gameObject;
            InitEdge(curEdge, "Central - " + i, firstNode, curNode);
            
            AddSpring(curNode, firstNode);
        }
        
        // Spawn other nodes + edges
        int[] otherNodeAdjIndices = new int[EdgesFirstNode];
        for (int i = 0; i < OtherNodes; ++i)
        {
            int newNodeIndex = Random.Range(0, EdgesFirstNode);
            otherNodeAdjIndices[newNodeIndex]++;
        }

        List<GameObject> otherNodes = new List<GameObject>();
        for (int i = 0; i < EdgesFirstNode; ++i)
        {
            for (int j = 0; j < otherNodeAdjIndices[i]; ++j)
            {
                Vector3 adjEdgeDirection = (adjacentNodes[i].transform.position - firstNode.transform.position).normalized;
                float edgeAngleFactor = otherNodeAdjIndices[i] % 2 == 0 ? Mathf.Floor((float)j / 2) + 0.5f : Mathf.Ceil((float)j / 2); 
                float edgeAngle = (360f / adjacentNodes.Count) / (otherNodeAdjIndices[i] * 0.5f) * edgeAngleFactor;
                if (j % 2 == 0)
                    edgeAngle *= -1;
                Vector3 curNodeDirection =
                    Quaternion.AngleAxis(edgeAngle, Camera.main.transform.forward) * adjEdgeDirection;
                Vector3 curNodePosition = adjacentNodes[i].transform.position + curNodeDirection * OtherNodesDistance;
                GameObject curNode = Instantiate(NodePrefab, curNodePosition, Quaternion.identity).gameObject;
                InitNode(curNode, "Node " + i + "_" + j, "Node " + i + "_" + j);
                otherNodes.Add(curNode);
                
                GameObject curEdge = Instantiate(EdgePrefab).gameObject;
                InitEdge(curEdge, i + " - " + i + "_" + j, adjacentNodes[i], curNode);
            
                AddSpring(curNode, adjacentNodes[i]);
            }
        }
    }

    void InitNode(GameObject node, string title, string content)
    {
        node.transform.parent = Parent.transform;
        NodeManipulationEvents manipEvents = node.GetComponent<NodeManipulationEvents>();
        manipEvents.FocusHndlr = FocusHndlr;
        manipEvents.GraphCont = GraphCont;
        NodeContent nodeContent = node.GetComponent<NodeContent>();
        nodeContent.Title.text = title;
        nodeContent.Text = content;
        node.name = title;
        ObjectManip manipulator = node.GetComponent<ObjectManip>();
        manipulator.OnHoverEntered.AddListener(FocusHndlr.OnNodeHover);
        manipulator.OnHoverExited.AddListener(FocusHndlr.OnNodeUnhover);
    }

    void AddSpring(GameObject nodeSource, GameObject nodeDest)
    {
        SpringJoint newSpring = nodeSource.AddComponent<SpringJoint>();
        newSpring.connectedBody = nodeDest.GetComponent<Rigidbody>();
        newSpring.anchor = SpringParams.Anchor;
        newSpring.spring = SpringParams.Spring;
        newSpring.damper = SpringParams.Damper;
        newSpring.minDistance = SpringParams.MinDistance;
        newSpring.maxDistance = SpringParams.MaxDistance;
        newSpring.autoConfigureConnectedAnchor = true;
    }

    void InitEdge(GameObject edge, string edgeName, GameObject node1, GameObject node2)
    {
        edge.name = node1.name + "-" + node2.name;
        edge.transform.parent = Parent.transform;
        EdgeManager edgeMan = edge.GetComponent<EdgeManager>();
        edgeMan.Node1 = node1;
        edgeMan.Node2 = node2;
        edgeMan.GraphCont = GraphCont;
        edgeMan.Title = edge.name;
        EdgeContent edgeContent = edge.GetComponent<EdgeContent>();
        edgeContent.Text = edgeName;
        edgeContent.TitleUi.text = edgeName;
    }
}
