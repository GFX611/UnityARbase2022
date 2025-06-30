using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PoissonDiskPath: MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Vector3 size;
    public float poissonRadius = 0.3f; // Radius for the Poisson disk sampling
    
    private Bounds boxBounds; // Bounds of the mesh renderer
    
    private List<Vector3> samples; // List to store the sampled points
    private List<Vector3> curvePoints; // Array to store the sampled points
    public int numberOfPoints = 10; // Number of points to sample along the curve
    private List<Vector3> remainingPoints; // List to store the sampled points
    static Color hue; // Generate a random color for visualization
    public static List<List<Vector3>> curves;
    public static List<List<Vector3>> freeCurves; // List to store the curves that have been taken
    public GameObject volumeObject; // Object to visualize the volume

    [Range(0f, 0.5f)]
    public float curveSmoothRatio;
    private static float curveSmooth = 0.25f;

    public int divisions = 3; // Number of divisions for the Chaikin's algorithm
    private static int max;

    void Awake()
    {
        curveSmooth = curveSmoothRatio; // Set the curve smooth ratio
        curves = new List<List<Vector3>>(); // Initialize the list of curves
        boxBounds = volumeObject.GetComponent<MeshRenderer>().bounds; // Get the bounds of the mesh renderer

        //someObject = GameObject.CreatePrimitive(PrimitiveType.Cube); // Example object to instantiate
        

        size = boxBounds.size; // Get the size of the bounds
        
        //volumeObject.GetComponent<MeshRenderer>().enabled = false; // Disable the mesh renderer after sampling
        samples = initializePoissonDisk(); // Initialize the Poisson disk samples
        remainingPoints = initializePoissonDisk();

        max = Mathf.FloorToInt(samples.Count/numberOfPoints); // Calculate the maximum number of curves to create based on the number of points
        
        for (int i = 0; i < max; i++)
        {
            
      
            createCurve(); // Create the curve at the center of the bounds
        }
        freeCurves = curves; // Store the curves that have been created

        vizualizeCurves(); // Visualize the curves created

    }


    void createCurve()
    {
        curvePoints = new List<Vector3>();

        if (remainingPoints.Count < numberOfPoints)
        {
            Debug.LogWarning("Not enough points to create the curve.");
            return;
        }

        for (int i = 0; i < numberOfPoints; i++)
        {
            
                int seed = Random.Range(0, remainingPoints.Count-1);
                curvePoints.Add(remainingPoints[seed]);
                remainingPoints.RemoveAt(seed); // Remove the point from the list to avoid duplicates
            

        }
        curvePoints = ApplyChaikinLoop(curvePoints, divisions); // Apply Chaikin's algorithm to smooth the curve

        curves.Add(curvePoints); // Add the curve points to the list of curves
    }



    public static void vizualizeCurves()
    {
        for (int i =0; i<curves.Count;i++)
        {
            Debug.Log("Curve " + i + " has " + curves[i].Count + " points.");
            hue = Color.HSVToRGB((float)i /curves.Count,1f,1f); // Use black color for curves that are not taken
                vizualizeCurve(curves[i]); // Visualize the curve
        }
        
    }

    static void vizualizeCurve(List<Vector3> points)
    {
        for (int i = 0; i < points.Count; i++)
        {

            if (i == points.Count - 1) // If it's the last point, connect it to the first point
                Debug.DrawLine(points[points.Count - 1], points[0], hue, 100f); // Connect the last point to the first point to close the loop
            else
                // Otherwise, connect the current point to the next point
                Debug.DrawLine(points[i], points[i + 1], hue, 100f); // Draw a line between the current point and the next point

        }
    }

    List<Vector3> initializePoissonDisk()
    {
        PoissonDiskSampler3D sampler = new PoissonDiskSampler3D(size.x, size.y, size.z, poissonRadius);
        List<Vector3> samples = new List<Vector3>();
        foreach (Vector3 sample in sampler.Samples())
        {
            if (IsInsideMeshCollider(volumeObject.GetComponent<MeshCollider>(), sample + boxBounds.min))
            {

                Vector3 pos = sample + boxBounds.min;  
                samples.Add(pos); // Add the sample position to the list of samples
            }
           
        }
        return samples;
    }

    public static List<Vector3> ApplyChaikin(List<Vector3> controlPoints, int iterations)
    {
        if (controlPoints == null || controlPoints.Count < 2)
            return controlPoints;

        List<Vector3> result = new List<Vector3>(controlPoints);

        for (int iter = 0; iter < iterations; iter++)
        {
            List<Vector3> newPoints = new List<Vector3>();

            for (int i = 0; i < result.Count - 1; i++)
            {
                Vector3 p0 = result[i];
                Vector3 p1 = result[i + 1];

                Vector3 q = Vector3.Lerp(p0, p1, 0.25f); // 0.75 * p0 + 0.25 * p1
                Vector3 r = Vector3.Lerp(p0, p1, 0.75f); // 0.25 * p0 + 0.75 * p1

                newPoints.Add(q);
                newPoints.Add(r);
            }

            result = newPoints;
        }

        return result;
    }


    public static List<Vector3> ApplyChaikinLoop(List<Vector3> controlPoints, int iterations)
    {
        if (controlPoints == null || controlPoints.Count < 3)
            return controlPoints;

        List<Vector3> result = new List<Vector3>(controlPoints);

        for (int iter = 0; iter < iterations; iter++)
        {
            List<Vector3> newPoints = new List<Vector3>();

            int count = result.Count;
            for (int i = 0; i < count; i++)
            {
                Vector3 p0 = result[i];
                Vector3 p1 = result[(i + 1) % count]; // wrap-around

                Vector3 q = Vector3.Lerp(p0, p1, curveSmooth);
                Vector3 r = Vector3.Lerp(p0, p1, 1-curveSmooth);

                newPoints.Add(q);
                newPoints.Add(r);
            }

            result = newPoints;
        }

        return result;
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


