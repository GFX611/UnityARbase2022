using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelizeMesh : MonoBehaviour
{
    [SerializeField] private Bounds bounds;
    public int divisions = 10;
    GameObject voxel;
    // Start is called before the first frame update
    void Start()
    {
        voxel = GameObject.CreatePrimitive(PrimitiveType.Cube);
        voxel.AddComponent<Rigidbody>(); // Add a Rigidbody to the voxel
        voxel.GetComponent<MeshRenderer>().material=GetComponent<MeshRenderer>().material; // Copy the material from the mesh renderer

        bounds = GetComponent<MeshRenderer>().bounds; // Get the bounds of the mesh renderer



        float size = (bounds.size.x / divisions)*0.99f;

        voxel.transform.localScale = new Vector3(size,size,size); // Set the size of the voxel
        
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Voxelizing mesh collider");
            Voxelize(voxel);
            GetComponent<MeshRenderer>().enabled = false; // Disable the mesh collider after voxelization
        }
    }
    
    void Voxelize(GameObject voxel)
    {
       
        float step = bounds.size.x / divisions;

        for (float x = bounds.min.x; x < bounds.max.x; x +=step)
        {
            for (float y = bounds.min.y; y < bounds.max.y; y += step)
            {
                for (float z = bounds.min.z; z < bounds.max.z; z += step)
                {
                    if (IsInsideMeshCollider(GetComponent<MeshCollider>(), new Vector3(x, y, z)))
                    {
                        
                        Instantiate(voxel, new Vector3(x, y, z), Quaternion.identity);
                        continue; // Skip if the point is outside the mesh collider
                    }
                    
                   
                }
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
