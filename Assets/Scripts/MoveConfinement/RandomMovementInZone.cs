using UnityEngine;
using System.Collections;

public class RandomMovementInZone : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 3f;
    public float rotationSpeed = 120f;
    public float directionChangeInterval = 3f;
    public bool restrictXRotation = true;
    public bool restrictZRotation = true;

    [Header("Zone Detection")]
    public string zoneTag = "MoveZone";
    public float zoneSearchInterval = 2f;
    public bool searchForZoneAtStart = true;

    [Header("Debug")]
    public bool showDebugRay = true;

    private Vector3 targetDirection;
    private Collider moveZoneCollider;
    private float nextDirectionChangeTime;
    private bool isTurningBack = false;
    private bool isSearchingForZone = false;

    private void OnEnable()
    {
        if (searchForZoneAtStart)
        {
            FindMoveZoneByTag();
        }

        if (moveZoneCollider != null)
        {
            InitializeMovement();
        }
        else
        {
            StartCoroutine(PeriodicZoneSearch());
        }
    }

    private void FindMoveZoneByTag()
    {
        GameObject moveZone = GameObject.FindWithTag(zoneTag);

        if (moveZone != null)
        {
            moveZoneCollider = moveZone.GetComponent<Collider>();

            if (moveZoneCollider == null)
            {
                Debug.LogWarning($"Found object with tag '{zoneTag}' but it has no Collider component!");
                return;
            }

            if (isSearchingForZone)
            {
                InitializeMovement();
                isSearchingForZone = false;
                StopCoroutine(PeriodicZoneSearch());
            }
        }
    }

    private void InitializeMovement()
    {
        ResetDirectionChangeTime();
        ChooseNewRandomDirection();
        enabled = true;
    }

    private IEnumerator PeriodicZoneSearch()
    {
        isSearchingForZone = true;

        while (moveZoneCollider == null)
        {
            FindMoveZoneByTag();

            if (moveZoneCollider == null)
            {
                yield return new WaitForSeconds(zoneSearchInterval);
            }
        }
    }

    private void Update()
    {
        if (moveZoneCollider == null) return;

        if (Time.time >= nextDirectionChangeTime && !isTurningBack)
        {
            ChooseNewRandomDirection();
            ResetDirectionChangeTime();
        }

        MoveForward();

        if (!moveZoneCollider.bounds.Contains(transform.position))
        {
            if (!isTurningBack)
            {
                StartCoroutine(TurnBackToZone());
            }
        }
    }

    private void MoveForward()
    {
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);

        if (targetDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

            Vector3 euler = targetRotation.eulerAngles;
            if (restrictXRotation) euler.x = 0;
            if (restrictZRotation) euler.z = 0;
            targetRotation = Quaternion.Euler(euler);

            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime);
        }
    }

    private void ChooseNewRandomDirection()
    {
        Vector3 randomPoint = GetRandomPointInBounds(moveZoneCollider.bounds);
        targetDirection = (randomPoint - transform.position).normalized;

        if (showDebugRay)
        {
            Debug.DrawRay(transform.position, targetDirection * 5f, Color.green, 1f);
        }
    }

    private Vector3 GetRandomPointInBounds(Bounds bounds)
    {
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            Random.Range(bounds.min.z, bounds.max.z)
        );
    }

    private IEnumerator TurnBackToZone()
    {
        isTurningBack = true;

        Vector3 centerDirection = (moveZoneCollider.bounds.center - transform.position).normalized;
        targetDirection = centerDirection;

        if (showDebugRay)
        {
            Debug.DrawRay(transform.position, targetDirection * 5f, Color.red, 1f);
        }

        float timeout = 3f;
        float startTime = Time.time;

        while (!moveZoneCollider.bounds.Contains(transform.position)
               && Time.time < startTime + timeout)
        {
            yield return null;
        }

        ChooseNewRandomDirection();
        ResetDirectionChangeTime();
        isTurningBack = false;
    }

    private void ResetDirectionChangeTime()
    {
        nextDirectionChangeTime = Time.time + directionChangeInterval;
    }

    private void OnDrawGizmos()
    {
        if (showDebugRay && Application.isPlaying && enabled && moveZoneCollider != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, targetDirection * 2f);
        }
    }

    public void SetMoveZone(GameObject newMoveZone)
    {
        if (newMoveZone != null && newMoveZone.CompareTag(zoneTag))
        {
            moveZoneCollider = newMoveZone.GetComponent<Collider>();
            if (moveZoneCollider != null)
            {
                InitializeMovement();
                if (isSearchingForZone)
                {
                    isSearchingForZone = false;
                    StopCoroutine(PeriodicZoneSearch());
                }
            }
        }
    }
}