
using System.Collections.Generic;
using UnityEngine;



public class FlyingToCurve : MonoBehaviour
{
    public Vector3 direction;
    public float speed = 2f;

    [SerializeField] private Vector3 offset = new Vector3(4.58f, 4.52f, 0);//determines arch of the curve
    private int steps; // Number of steps for the curve
    public float turnSpeed = 2f; // Speed of rotation towards the movement direction
    [SerializeField]private List<Vector3> curve = new List<Vector3>();
    private int nearestPointIndex;
    private int currentCurveIndex = 0; // Index of the current curve

    void Start()
    {
        direction = transform.forward.normalized;
        FindCurve(); // Find a curve to follow at the start
    }

    void Update()
    {

        Vector3 dir = curve[(currentCurveIndex+1)%(steps)] - curve[currentCurveIndex];

        if (Vector3.Distance(transform.position, curve[currentCurveIndex]) < 0.1f) currentCurveIndex++;
        if (currentCurveIndex >= steps) currentCurveIndex = 0; // Reset counter if it exceeds the number of steps

        transform.position = Vector3.MoveTowards(transform.position, curve[currentCurveIndex], speed * Time.deltaTime);

        // Smoothly rotate toward movement direction
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), turnSpeed * Time.deltaTime);

 
    }

    private void FindCurve()
    {
        int randCurveIndex = Random.Range(0, PoissonDiskPath.freeCurves.Count);
        
        
        curve = PoissonDiskPath.freeCurves[randCurveIndex]; // Select a random curve from the remaining freeCurves
        PoissonDiskPath.freeCurves.RemoveAt(randCurveIndex); // Remove the current curve from the freeCurves list

        PoissonDiskPath.vizualizeCurves();
        steps = curve.Count - 1; // Set steps based on the number of points in the curve
        // If the current curve is taken, find the nearest point again
        FindNearest();
        currentCurveIndex = nearestPointIndex;
        
    }


    void FindNearest()
    {
        nearestPointIndex = 0;
        float minDistance = (curve[0] - curve[nearestPointIndex]).sqrMagnitude;
        for (int i = 1; i < curve.Count; i++)
        {
            float distance = (curve[i] - curve[nearestPointIndex]).sqrMagnitude;
            if (distance<minDistance)
            {
                minDistance = distance;
                nearestPointIndex = i;
            }
        }
    }
}