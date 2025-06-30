using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class PoissonDiskGenerate : MonoBehaviour
{
    // Start is called before the first frame update
    public float radius = 0.3f; // Radius of the samples
    [SerializeField] Vector3 size;
    public float poissonRadius = 0.3f; // Radius for the Poisson disk sampling
    GameObject someObject;
    private float prevRadius;
    private Bounds boxBounds; // Bounds of the mesh renderer
    public GameObject volumeObject; // Object to visualize the volume
    void Start()
    {

        boxBounds = volumeObject.GetComponent<MeshRenderer>().bounds; // Get the bounds of the mesh renderer

        someObject = GameObject.CreatePrimitive(PrimitiveType.Cube); // Example object to instantiate
        

        size = boxBounds.size; // Get the size of the bounds
        ReSample(size); // Call the ReSample method to generate samples within the bounds
        volumeObject.GetComponent<MeshRenderer>().enabled = false; // Disable the mesh renderer after sampling

        prevRadius = poissonRadius; // Initialize prevRadius to the current radius value
    }




    // Update is called once per frame
    void Update()
    {

     if(poissonRadius!=prevRadius)
        {
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Debug"))
            {
                Destroy(obj); // Destroy all previously instantiated objects with the tag "Debug"
            }
            Debug.Log("Generating Poisson disk samples");
            ReSample(size); // Call ReSample on space key press
        }

     prevRadius = poissonRadius; // Store the previous radius value
    }

    void ReSample(Vector3 size)
    {
        someObject.transform.localScale = new Vector3(radius, radius, radius); // Set the scale of the object
        PoissonDiskSampler3D sampler = new PoissonDiskSampler3D(size.x, size.y, size.z, poissonRadius);
        foreach (Vector3 sample in sampler.Samples())
        {
            
            if (IsInsideMeshCollider(volumeObject.GetComponent<MeshCollider>(), sample+boxBounds.min))
            {
                Vector3 pos = sample + boxBounds.min;
                var instance =GameObject.Instantiate(someObject, pos, Quaternion.identity); // Instantiate the object at the sample position
                instance.tag = "Debug"; // Set the tag of the instantiated object to "Debug"
            }

        }

    }

    bool IsInsideMeshCollider(MeshCollider col, Vector3 point)
    {
        var temp = Physics.queriesHitBackfaces;
        Ray ray = new Ray(point, Vector3.back);

        bool hitFrontFace = false;
        RaycastHit hit = default;

        Physics.queriesHitBackfaces = true;
        bool hitFrontOrBackFace = col.Raycast(ray, out RaycastHit hit2, 100f);
        if (hitFrontOrBackFace)
        {
            Physics.queriesHitBackfaces = false;
            hitFrontFace = col.Raycast(ray, out hit, 100f);
        }
        Physics.queriesHitBackfaces = temp;

        if (!hitFrontOrBackFace)
        {
            return false;
        }
        else if (!hitFrontFace)
        {
            return true;
        }
        else
        {
            // This can happen when, for instance, the point is inside the torso but there's a part of the mesh (like the tail) that can still be hit on the front
            if (hit.distance > hit2.distance)
            {
                return true;
            }
            else
                return false;
        }

    }
}


