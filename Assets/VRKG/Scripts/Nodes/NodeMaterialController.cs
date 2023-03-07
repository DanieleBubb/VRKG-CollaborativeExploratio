using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

/* handles node material change, based on events */
public class NodeMaterialController : MonoBehaviourPun
{
    public GraphicsProfileManager ProfilesManager;
    public MeshRenderer Renderer;
    public FocusHandler FocusHndlr;

    public Material IdleMaterial
    {
        get
        {
            return ProfilesManager.CurrentProfile.NodeIdleMaterial;
        }
    }

    public Material HoverMaterial
    {
        get
        {
            return ProfilesManager.CurrentProfile.NodeHoverMaterial;
        }
    }
    public Material SelectedMaterial 
    {
        get
        {
            return ProfilesManager.CurrentProfile.NodeSelectedMaterial;
        }
    }

    public void Start()
    {
        SetMaterial(IdleMaterial);
    }

    public void SetMaterial(Material mat)
    {
        Renderer.material = mat;
    }

    public void OnHoverStart()
    {
        if(!FocusHndlr.IsFocused && FocusHndlr.OwnershipMan.CanIBeTheOwner())
            SetMaterial(HoverMaterial);
    }

    public void OnHoverEnd(bool force = false)
    {
        if(force || !FocusHndlr.IsFocused)
            SetMaterial(IdleMaterial);
    }

    public void OnSelect(bool force = false)
    {
        if(force || !FocusHndlr.IsFocused)
            SetMaterial(SelectedMaterial);
    }

    public void OnUnselect(bool force = false)
    {
        if(force || !FocusHndlr.IsFocused)
            SetMaterial(HoverMaterial);
    }
}
