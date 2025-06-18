using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GetDistanceMultipe : MonoBehaviour
{
    [SerializeField] Transform[] ARtargets; // Array to store multiple ARtarget GameObjects
    [SerializeField] Transform ARCamera;
    [SerializeField] TMP_Text m_TextComponent;

    // Update is called once per frame
    void Update()
    {
        // Find the active ARtarget (the one that is currently active/visible)
        Transform activeTarget = null;
        foreach (Transform target in ARtargets)
        {
            if (target.gameObject.activeSelf)
            {
                activeTarget = target;
                break;
            }
        }

        // Calculate and display the distance if an active ARtarget is found
        if (activeTarget != null)
        {
           
            float distance = Vector3.Distance(activeTarget.position, ARCamera.position);
            Debug.Log("Distance to Active Target: " + distance);
            m_TextComponent.text = "Distance Value: " + distance.ToString();
        }
        else
        {
            Debug.Log("No Active ARtarget found.");
            m_TextComponent.text = "No Active Target";
        }
    }
}
