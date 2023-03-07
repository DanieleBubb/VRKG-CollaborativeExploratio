using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WebSocketSharp;

[Serializable]
public class GraphicsProfile
{
    public string Name;
    public Material SkyboxMaterial;
    public Material NodeIdleMaterial;
    public Material NodeHoverMaterial;
    public Material NodeSelectedMaterial;
    public Color NodeTitleColorTransparent;
    public Color NodeTitleColorOpaque;
    public Color32 NodeTitleOutlineColor;
    public Color EdgeColorTransparent;
    public Color EdgeColorOpaque;
}

/* Handles graphics profiles */
public class GraphicsProfileManager : MonoBehaviour
{
    public List<GraphicsProfile> Profiles;
    private GraphicsProfile currentProfile;

    public GraphicsProfile CurrentProfile
    {
        get
        {
            return currentProfile ?? Profiles[0];
        }
    }

    public void OnNewGraph(QueryEntry query)
    {
        if (query.GraphicsProfile.IsNullOrEmpty())
        {
            currentProfile = Profiles[0];
        }
        else
        {
            currentProfile = Profiles.FirstOrDefault(p => p.Name.Equals(query.GraphicsProfile));
            if(currentProfile == null)
                currentProfile = Profiles[0];
        }
    }
}
