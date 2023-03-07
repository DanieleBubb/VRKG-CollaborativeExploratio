using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* updates an edge transform to be connected with its adjacent nodes */
public class EdgeManager : MonoBehaviour
{
    public GameObject Node1;
    public GameObject Node2;
    public string Title;
    public GraphContainer GraphCont;
    public GameObject GraphicsBase;

    private void Start()
    {
        GraphCont.RegisterEdge(gameObject);
    }

    private void OnDestroy()
    {
        GraphCont.UnregisterEdge(gameObject);
    }

    private void Update()
    {
        transform.position = (Node1.transform.position + Node2.transform.position) * 0.5f;
        GraphicsBase.transform.up = Node1.transform.position - Node2.transform.position;
        float newScale = (Vector3.Distance(Node1.transform.position, Node2.transform.position) - Node1.transform.localScale.x) * 0.5f;
        GraphicsBase.transform.localScale = new Vector3(GraphicsBase.transform.localScale.x, newScale, GraphicsBase.transform.localScale.z);
    }
}
