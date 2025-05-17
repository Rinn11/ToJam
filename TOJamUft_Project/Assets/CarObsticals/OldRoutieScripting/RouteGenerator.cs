using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class RouteGenerator : MonoBehaviour
{
    [Header("Route Settings")]
    public Transform floor;
    public int numberOfPoints = 4;
    public float radius = 5f;
    public bool closeLoop = false;

    [Header("Point Settings")]
    public GameObject pointPrefab;

    [Range(0, 180)]
    public float curveAngleThreshold = 30f;

    public void GenerateRoute()
    {
        // Clear old points
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
#if UNITY_EDITOR
            DestroyImmediate(transform.GetChild(i).gameObject);
#else
            Destroy(transform.GetChild(i).gameObject);
#endif
        }

        Vector3 center = floor != null ? floor.position : transform.position;
        float angleStep = 360f / numberOfPoints;

        // Create points around center in a circle
        for (int i = 0; i < numberOfPoints; i++)
        {
            float angle = angleStep * i * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
            Vector3 pos = center + offset;

            GameObject point = Instantiate(pointPrefab, pos, Quaternion.identity, transform);
            point.name = $"Point {i}";
        }

        // Optional closing loop
        if (closeLoop && numberOfPoints > 1)
        {
            GameObject firstClone = Instantiate(pointPrefab, transform.GetChild(0).position, Quaternion.identity, transform);
            firstClone.name = $"Point {numberOfPoints} (loop)";
        }
    }

    private void OnDrawGizmos()
    {
        if (transform.childCount < 2) return;

        // Draw lines between points
        Gizmos.color = Color.yellow;
        for (int i = 0; i < transform.childCount - 1; i++)
        {
            Gizmos.DrawLine(transform.GetChild(i).position, transform.GetChild(i + 1).position);
        }

        if (closeLoop)
        {
            Gizmos.DrawLine(transform.GetChild(transform.childCount - 1).position, transform.GetChild(0).position);
        }

        // Curve detection and debug marker
        Gizmos.color = Color.cyan;
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform current = transform.GetChild(i);
            Transform prev = transform.GetChild((i - 1 + transform.childCount) % transform.childCount);
            Transform next = transform.GetChild((i + 1) % transform.childCount);

            Vector3 toPrev = (prev.position - current.position).normalized;
            Vector3 toNext = (next.position - current.position).normalized;

            float angle = Vector3.Angle(toPrev, toNext);

            if (angle < 180f - curveAngleThreshold)
            {
                Gizmos.DrawWireSphere(current.position, 0.5f);
            }
        }
    }
}
