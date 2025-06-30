using System.Collections.Generic;
using UnityEngine;


public class FlyingInsect : MonoBehaviour
{
    public Vector3 direction;
    public float speed = 2f;
    
    [SerializeField] private Vector3 offset = new Vector3(4.58f, 4.52f, 0);//determines arch of the curve
    public float distance = 1f;
    private int steps = 100; // Number of steps for the curve
    public float turnSpeed = 2f; // Speed of rotation towards the movement direction
    [SerializeField]  private List<Vector3> curvePos = new List<Vector3>();
    [SerializeField] private int counter = 0;


    void Start()
    {
        direction = transform.forward.normalized;
        CurvedPosGenerate(direction, curvePos, offset, steps);

    }

    void Update()
    {

        // Raycast to avoid obstacles
        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, 1f))
        {
            direction = Vector3.Reflect(direction, hit.normal);
            counter = 0; // Reset counter when hitting an obstacle
            CurvedPosGenerate(direction, curvePos, offset, steps); // Regenerate curve positions
        }

        if (counter < steps)
        {
            transform.position = Vector3.MoveTowards(transform.position, curvePos[counter], Time.deltaTime);
            if (Vector3.Distance(transform.position, curvePos[counter]) < 0.1f) counter++;
        }


        
        Vector3 dir = curvePos[counter+1]-curvePos[counter];
        // Smoothly rotate toward movement direction
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), turnSpeed * Time.deltaTime);

        


    }


    void CurvedPosGenerate(Vector3 direction,List<Vector3> curvePos, Vector3 offset, int steps)
    {

        curvePos.Clear(); // Clear previous positions
        // Define control points for the cubic Bezier curve
        Vector3 start = transform.position;

        RaycastHit hit;

        Vector3 goalPos;

        if (Physics.Raycast(transform.position, direction, out hit, 10f))
        {
            // If the raycast hits an obstacle, adjust the start position
            goalPos = hit.point;
        }
        else
        {
            Debug.Log("No obstacle detected, using goal position directly.");
            // If no obstacle is hit, use the goal position directly
            goalPos = start + direction.normalized * distance;
        }

        
        for (var i = 0; i < steps; i++)
        {
            
            var newPosition = CubicCurve(start, start + offset, start + offset,
                goalPos, (float)i / steps);
            curvePos.Add(newPosition);
        }
    }

    private Vector3 CubicCurve(Vector3 start, Vector3 control1, Vector3 control2, Vector3 end, float t)
    {
        return (((-start + 3 * (control1 - control2) + end) * t + (3 * (start + control2) - 6 * control1)) * t +
                3 * (control1 - start)) * t + start;
    }
}