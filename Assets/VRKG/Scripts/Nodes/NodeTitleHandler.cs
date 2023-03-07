using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NodeTitleHandler : MonoBehaviour
{
    private Vector3 initialLocalPosition;

    private void Awake()
    {
        initialLocalPosition = transform.localPosition;
    }

    private void Update()
    {
        transform.LookAt(Camera.main.transform.position);
        transform.Rotate(0f, 180f, 0f);
        transform.position = transform.parent.position + initialLocalPosition;
    }
}
