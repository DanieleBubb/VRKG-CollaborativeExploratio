using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class EdgeContent : MonoBehaviour
{
    public TextMeshPro TitleUi;
    public MeshRenderer MainRenderer;
    public string Text;

    private void Awake()
    {
        TitleUi.text = Text;
    }
}