using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/* data structurs for nodes, edges and tables */

[Serializable]
public class KGNode : IEquatable<KGNode>
{
    public string ID;
    public string Label;
    public string Comment;
    
    public bool Equals(KGNode other)
    {
        if (other == null)
            return false;
        if (ID.Equals(other.ID))
            return true;
        return false;
    }

    public override int GetHashCode()
    {
        return ID.GetHashCode();
    }
}

[Serializable]
public class KGEdge
{
    public string IDNode1;
    public string IDNode2;
    public string Label;
}

public class KGNodesEdge : IEquatable<KGNodesEdge>
{
    public KGEdge Edge;
    public KGNode Node1;
    public KGNode Node2;
    
    public bool Equals(KGNodesEdge other)
    {
        if (other == null)
            return false;
        if (Node1.ID.Equals(other.Node1.ID) && Node2.ID.Equals(other.Node2.ID))
            return true;
        if (Node1.ID.Equals(other.Node2.ID) && Node2.ID.Equals(other.Node1.ID))
            return true;
        return false;
    }

    public override int GetHashCode()
    {
        return Node1.GetHashCode() ^ Node2.GetHashCode();
    }
}

[Serializable]
public class KGDescriptor
{
    public List<KGNode> Nodes;
    public List<KGEdge> Edges;

    public List<KGNodesEdge> GetNodesAndEdgesAdjacentToNode(KGNode node)
    {
        List<KGEdge> allEdges = Edges.Where(e => e.IDNode1.Equals(node.ID) || e.IDNode2.Equals(node.ID)).ToList();
        List<KGNodesEdge> connectedEdges = new List<KGNodesEdge>();
        foreach (var curEdge in allEdges)
        {
            KGNode otherNode = null;
            if (curEdge.IDNode1.Equals(node.ID))
            {
                otherNode = Nodes.FirstOrDefault(n => n.ID.Equals(curEdge.IDNode2));
            }
            else if(curEdge.IDNode2.Equals(node.ID))
            {
                otherNode = Nodes.FirstOrDefault(n => n.ID.Equals(curEdge.IDNode1));
            }

            if (otherNode != null)
            {
                KGNodesEdge nodesEdge = new KGNodesEdge();
                nodesEdge.Edge = curEdge;
                nodesEdge.Node1 = node;
                nodesEdge.Node2 = otherNode;
                connectedEdges.Add(nodesEdge);
            }
        }

        return connectedEdges;

        /*return Edges.Where(e => e.IDNode1.Equals(node.ID) || e.IDNode2.Equals(node.ID))
            .Select(e => new KGNodesEdge()
            {
                Edge = e,
                Node1 = Nodes.First(n => n.ID.Equals(e.IDNode1)),
                Node2 = Nodes.First(n => n.ID.Equals(e.IDNode2))
            }).ToList();*/
    }
}

[Serializable]
public class KGTableEntry
{
    public string Subject;
    public string SubjectLabel;
    public string SubjectComment;
    public string Predicate;
    public string PredicateLabel;
    public string Object;
    public string ObjectLabel;
}

[Serializable]
public class KGTable
{
    public List<KGTableEntry> Entries;
}
