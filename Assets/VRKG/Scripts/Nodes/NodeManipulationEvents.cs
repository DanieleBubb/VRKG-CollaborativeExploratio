using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.MixedReality.Toolkit.Utilities;

/* handles manipulation events of a node */
public class NodeManipulationEvents : MonoBehaviour
{
    public Microsoft.MixedReality.Toolkit.UI.ObjectManipulator Manipulator;
    public FocusHandler FocusHndlr;
    public GraphContainer GraphCont;
    public float GrabbedNodeMass;
    public float GrabbedNodeDrag;
    private float initialMass;
    private float initialDrag;

    private void Start()
    {
        if (GraphCont != null)
        {
            Init();
        }
    }

    public void Init()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        initialMass = rb.mass;
        initialDrag = rb.drag;
    }

    private void OnDestroy()
    {
        if (GraphCont != null)
            GraphCont.UnregisterNode(gameObject);
    }

    public void OnGrabStarted()
    {
        if (!FocusHndlr.IsFocused)
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.mass = GrabbedNodeMass;
            rb.drag = GrabbedNodeDrag;
            GraphCont.OnNodeGrabbed(gameObject);
        }
    }
    
    public void OnGrabReleased()
    {
        if (!FocusHndlr.IsFocused)
        {
            GraphCont.OnNodeReleased(gameObject);
        }
    }

    public void OnOtherNodeGrabbed()
    {
        ResetInitialMass();
    }

    public void ResetInitialMass()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.mass = initialMass;
        rb.drag = initialDrag;
    }

    public void SetManipulationEnabled(bool enabled)
    {
        Manipulator.ManipulationType = enabled ? ManipulationHandFlags.OneHanded : 0;
    }

}
