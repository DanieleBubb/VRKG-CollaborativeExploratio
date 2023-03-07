using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class NodeContent : MonoBehaviour
{
    public TextMeshPro Title;
    [TextArea]
    public string Text;

    private void Awake()
    {
        Title.text = gameObject.name;
    }
}
