using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

/* Manages the UI on the right, when a node is on focus */
public class UIParamsManager : MonoBehaviour
{
    public FocusHandler FocusHndlr;
    public Vector3 OffsetFromCamera;
    public TextMeshProUGUI DescriptionTextGUI;
    
    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void OnJoinedRoom()
    {
        Vector3 focusPoint = Camera.main.transform.position + Camera.main.transform.forward * OffsetFromCamera.z 
                                                            + Camera.main.transform.right * OffsetFromCamera.x;
        transform.position = focusPoint;
        transform.LookAt(Camera.main.transform.position);
        transform.Rotate(0f, 180f, 0f);
    }

    public void OnFocus(GameObject obj)
    {
        gameObject.SetActive(true);
        DescriptionTextGUI.text = obj.GetComponent<NodeContent>().Text;
    }

    public void OnUnfocus()
    {
        gameObject.SetActive(false);
    }
}
