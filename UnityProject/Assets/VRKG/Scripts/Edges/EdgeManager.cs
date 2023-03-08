using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
