using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ObjectManip = Microsoft.MixedReality.Toolkit.UI.ObjectManipulator;
using Random = UnityEngine.Random;

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

public class SpawnedNode
{
    public KGNode Node;
    public GameObject GO;
}

public class SpawnedEdge
{
    public KGEdge Edge;
    public GameObject GO;
}

/* Handles local graph generation in the scene*/
public class KnowledgeGraphGenerator : MonoBehaviour
{
    public float FirstNodeDistance;
    public float AdjacentNodesDistance;
    public float OtherNodesDistance;
    public float OtherNodesMaxAngle;
    public SpringParameters SpringParams;
    
    public GameObject Parent;
    public Transform NodePrefab;
    public Transform EdgePrefab;
    public FocusHandler FocusHndlr;
    public GraphContainer GraphCont;

    public KnowledgeGraphImporter KgImporter;
    private List<SpawnedNode> spawnedNodes;
    private List<KGNode> toSpawnNodes;
    private List<KGEdge> toSpawnEdges;
    private List<SpawnedNode> openNodes;
    private List<SpawnedNode> closedNodes;

    Vector3 GetCentralPosition()
    {
        return Camera.main.transform.position + Camera.main.transform.forward * FirstNodeDistance;
    }
    
    void InitDataStructures()
    {
        spawnedNodes = new List<SpawnedNode>();
        toSpawnNodes = new List<KGNode>();      
        toSpawnNodes.AddRange(KgImporter.Graph.Nodes);
        toSpawnEdges = new List<KGEdge>();
        toSpawnEdges.AddRange(KgImporter.Graph.Edges);
        openNodes = new List<SpawnedNode>();
        closedNodes = new List<SpawnedNode>();
    }
    
    void DeleteCurrentGraph()
    {
        for (int i = 0; i < Parent.transform.childCount; ++i)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }

    SpawnedNode SpawnNode(KGNode node, Vector3 pos)
    {
        GameObject firstNodeGO = Instantiate(NodePrefab, pos, Quaternion.identity).gameObject;
        InitNode(firstNodeGO, node.Label, node.Comment);
        SpawnedNode newNodeSpawned = new SpawnedNode();
        newNodeSpawned.Node = node;
        newNodeSpawned.GO = firstNodeGO;
        spawnedNodes.Add(newNodeSpawned);
        toSpawnNodes.Remove(node);
        return newNodeSpawned;
    }

    SpawnedEdge SpawnEdge(KGEdge edge)
    {
        GameObject curEdge = Instantiate(EdgePrefab).gameObject;
        GameObject edgeNode1 = spawnedNodes.First(n => n.Node.ID == edge.IDNode1).GO;
        GameObject edgeNode2 = spawnedNodes.First(n => n.Node.ID == edge.IDNode2).GO;
        InitEdge(curEdge, edge.Label, edgeNode1, edgeNode2);
        AddSpring(edgeNode2, edgeNode1);
        SpawnedEdge newEdgeSpawned = new SpawnedEdge();
        newEdgeSpawned.Edge = edge;
        newEdgeSpawned.GO = curEdge;
        toSpawnEdges.Remove(edge);
        return newEdgeSpawned;
    }

    SpawnedNode SpawnMainNodeSubject()
    {
        KGNode firstNode = KgImporter.Graph.Nodes.First();
        return SpawnNode(firstNode, GetCentralPosition());
    }

    List<KGNodesEdge> FilterOutClosedNodes(List<KGNodesEdge> nodesEdges, KGNode baseNode)
    {
        List<KGNodesEdge> filteredNodesEdges = new List<KGNodesEdge>();
        foreach (var curNodesEdge in nodesEdges)
        {
            if (curNodesEdge.Node1 == baseNode)
            {
                if (closedNodes.FirstOrDefault(n => n.Node == curNodesEdge.Node2) == null)
                {
                    filteredNodesEdges.Add(curNodesEdge);
                }
            }
            else if (curNodesEdge.Node2 == baseNode)
            {
                if (closedNodes.FirstOrDefault(n => n.Node == curNodesEdge.Node1) == null)
                {
                    filteredNodesEdges.Add(curNodesEdge);
                }
            }
        }

        return filteredNodesEdges;
    }

    List<SpawnedNode> SpawnNodesAndEdgesAdjacentToNode(SpawnedNode node)
    {
        List<KGNodesEdge> adjacentNodes = KgImporter.Graph.GetNodesAndEdgesAdjacentToNode(node.Node);
        List<KGNodesEdge> filteredAdjacentNodes = FilterOutClosedNodes(adjacentNodes, node.Node);
        List<SpawnedNode> newNodes = new List<SpawnedNode>();
        for (int i = 0; i < filteredAdjacentNodes.Count; ++i)
        {
            Vector3 curNodeDirection = Quaternion.AngleAxis(360f / filteredAdjacentNodes.Count * i, Camera.main.transform.forward) * Camera.main.transform.right;
            Vector3 curNodePosition = node.GO.transform.position + curNodeDirection * AdjacentNodesDistance;
            KGNode curNode = filteredAdjacentNodes[i].Node1 == node.Node ? filteredAdjacentNodes[i].Node2 : filteredAdjacentNodes[i].Node1;
            SpawnedNode newSpawnedNode = SpawnNode(curNode, curNodePosition);
            newNodes.Add(newSpawnedNode);

            SpawnEdge(filteredAdjacentNodes[i].Edge);
        }
        return newNodes;
    }

    List<SpawnedNode> SpawnNonCentralNodesAndEdgesAdjacentToNode(SpawnedNode node)
    {
        List<KGNodesEdge> adjacentNodes = KgImporter.Graph.GetNodesAndEdgesAdjacentToNode(node.Node);
        List<KGNodesEdge> filteredAdjacentNodes = FilterOutClosedNodes(adjacentNodes, node.Node);
        List<SpawnedNode> newNodes = new List<SpawnedNode>();
        for (int i = 0; i < filteredAdjacentNodes.Count; ++i)
        {
            Vector3 adjEdgeDirection = (node.GO.transform.position - GetCentralPosition()).normalized;
            float edgeAngleFactor = filteredAdjacentNodes.Count % 2 == 0 ? Mathf.Floor((float)i / 2) + 0.5f : Mathf.Ceil((float)i / 2); 
            float edgeAngle = (OtherNodesMaxAngle / filteredAdjacentNodes.Count) * edgeAngleFactor;
            if (i % 2 == 0)
                edgeAngle *= -1;
            Vector3 curNodeDirection =
                Quaternion.AngleAxis(edgeAngle, Camera.main.transform.forward) * adjEdgeDirection;
            Vector3 curNodePosition = node.GO.transform.position + curNodeDirection * OtherNodesDistance;
            
            KGNode curNode = filteredAdjacentNodes[i].Node1 == node.Node ? filteredAdjacentNodes[i].Node2 : filteredAdjacentNodes[i].Node1;
            SpawnedNode newSpawnedNode = SpawnNode(curNode, curNodePosition);
            newNodes.Add(newSpawnedNode);

            SpawnEdge(filteredAdjacentNodes[i].Edge);
        }
        return newNodes;
    }
    
    [ContextMenu ("Generate Graph")]
    public void GenerateGraph()
    {
        InitDataStructures();
        DeleteCurrentGraph();
        SpawnedNode firstNode = SpawnMainNodeSubject();
        openNodes.Add(firstNode);
        while (openNodes.Count > 0)
        {
            SpawnedNode curNode = openNodes[0];
            List<SpawnedNode> newNodes;
            if (closedNodes.Count == 0)
            {
                newNodes = SpawnNodesAndEdgesAdjacentToNode(curNode);
            }
            else
            {
                newNodes = SpawnNonCentralNodesAndEdgesAdjacentToNode(curNode);
            }

            openNodes.RemoveAt(0);
            openNodes.AddRange(newNodes);
            closedNodes.Add(curNode);
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
        edge.name = edgeName;
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
