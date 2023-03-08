using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

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
