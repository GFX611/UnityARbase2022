using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GetDistanceValue : MonoBehaviour
{
    [SerializeField] Transform ARtarget;
    [SerializeField] Transform ARCamera;
    [SerializeField] TMP_Text m_TextComponent;
    // Start is called before the first frame update
  

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(ARtarget.position, ARCamera.position);
        Debug.Log(distance);
        m_TextComponent.text = "Distance Value: " + distance.ToString();
    }
}
