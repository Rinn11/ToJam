using System.Collections.Generic;
using UnityEngine;

public class CarPathFollower : MonoBehaviour
{
    /*
    public PathNode currentNode;
    public float speed = 5f;
    public float reachThreshold = 0.2f;
    public float steeringSmooth = 5f;
    public float lookaheadDistance = 1f;

    private PathNode lastNode;

    void Update()
    {
        if (currentNode == null) return;

        Vector3 target = GetLookaheadPoint();

        Vector3 moveDir = (target - transform.position).normalized;
        transform.position += moveDir * speed * Time.deltaTime;
        transform.forward = Vector3.Lerp(transform.forward, moveDir, Time.deltaTime * steeringSmooth);

        if (Vector3.Distance(transform.position, currentNode.position) < reachThreshold)
        {
            AdvanceToNextNode();
        }
    }

    Vector3 GetLookaheadPoint()
    {
        if (currentNode.nextNodes.Count > 0)
        {
            PathNode next = GetNextBestNode(excludeLast: true);
            if (next != null)
                return Vector3.Lerp(currentNode.position, next.position, lookaheadDistance);
        }

        return currentNode.position;
    }

    void AdvanceToNextNode()
    {
        if (currentNode.nextNodes == null || currentNode.nextNodes.Count == 0)
        {
            Debug.Log($"{gameObject.name} reached a dead end at {currentNode.name}");
            enabled = false;
            return;
        }

        PathNode next = GetNextBestNode(excludeLast: true);
        if (next == null)
            next = GetNextBestNode(excludeLast: false); // fallback to reverse

        lastNode = currentNode;
        currentNode = next;
    }

    PathNode GetNextBestNode(bool excludeLast)
    {
        List<PathNode> options = new();

        foreach (var node in currentNode.nextNodes)
        {
            if (excludeLast && node == lastNode)
                continue;

            // Add multiple entries based on weight to bias selection
            int weightEntries = Mathf.Max(1, Mathf.RoundToInt(node.weight));
            for (int i = 0; i < weightEntries; i++)
                options.Add(node);
        }

        if (options.Count == 0) return null;

        return options[Random.Range(0, options.Count)];
    }

    private void OnDrawGizmosSelected()
    {
        if (currentNode == null) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(currentNode.position, 0.2f);

        Gizmos.color = Color.magenta;
        foreach (var next in currentNode.nextNodes)
        {
            Gizmos.DrawLine(currentNode.position, next.position);
        }
    }
    */
}
