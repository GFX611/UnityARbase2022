using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestInstantiate : MonoBehaviour
{

    public GameObject prefabToInstantiate;
    // Start is called before the first frame update
    void Start()
    {
        Instantiate(prefabToInstantiate,transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
