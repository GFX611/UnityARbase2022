using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulatePlacingImages : MonoBehaviour
{

    public Vector3 offsetToOrigin = new Vector3(-0.12f, -0.905f, 0f);
    Quaternion wallToWorldRotation = Quaternion.Euler(90f, 0f, 0f);
    public Vector3 worldPosition;
    public Quaternion worldRotation;
    public GameObject[] ArPrefabs;
    // Start is called before the first frame update
    void Start()
    {
        

        worldPosition = transform.transform.TransformPoint(wallToWorldRotation * offsetToOrigin);

        worldRotation = transform.transform.rotation * wallToWorldRotation;

        var newPrefab = Instantiate(ArPrefabs[0], worldPosition, worldRotation);

        
    
        
        var newPrefab2 = Instantiate(ArPrefabs[1], worldPosition, worldRotation);


    }

    
}
