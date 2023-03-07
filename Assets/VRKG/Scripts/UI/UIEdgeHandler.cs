using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class UIEdgeHandler : MonoBehaviour
{
    public UIEdgesManager UIEdges;
    public TextMeshProUGUI Text;
    public int Index;

    private void Start()
    {
        UIEdges.RegisterButton(gameObject, Text, Index);
    }

    public void OnClick()
    {
        UIEdges.OnEdgePressed(gameObject);
    }
}
