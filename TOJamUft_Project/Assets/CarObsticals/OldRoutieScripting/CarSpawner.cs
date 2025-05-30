using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class CarSpawner : MonoBehaviour
{
    //float _speed;

    //public List<Vector3> points = new List<Vector3>();
    //Vector3 spawnPoint;

    [SerializeField]
    private GameObject route;
    private Transform[] controlPoints;

    private Vector3 gizmosPosition;

    private void Awake()
    {
        //route = GameObject.Find("ControlPoints");
    }

    private void OnDrawGizmos()
    {
        if (controlPoints == null || controlPoints.Length < 2)
            return;

        for (float t = 0; t <= 1; t += 0.05f)
        {
            gizmosPosition = CalculateBezierPoint(t, controlPoints);
            Gizmos.DrawSphere(gizmosPosition, 0.25f);
        }

        for (int i = 0; i < controlPoints.Length - 1; i++)
        {
            Gizmos.DrawLine(controlPoints[i].position, controlPoints[i + 1].position);
        }
    }

    [SerializeField]
    private Transform[] routes;

    private int routeToGo;
    private float tParam;
    private Vector3 objectPosition;
    private float speedModifier;
    private bool coroutineAllowed;

    void Start()
    {
        routeToGo = 0;
        tParam = 0f;
        speedModifier = 0.5f;
        coroutineAllowed = true;
    }

    void Update()
    {
        if (coroutineAllowed)
        {
            StartCoroutine(GoByTheRoute(routeToGo));
        }
    }

    private IEnumerator GoByTheRoute(int routeNum)
    {
        coroutineAllowed = false;

        Transform route = routes[routeNum];
        int pointCount = route.childCount;

        if (pointCount < 2)
        {
            Debug.LogWarning("Route has fewer than 2 points.");
            yield break;
        }

        Transform[] dynamicControlPoints = new Transform[pointCount];
        for (int i = 0; i < pointCount; i++)
        {
            dynamicControlPoints[i] = route.GetChild(i);
        }

        while (tParam < 1)
        {
            tParam += Time.deltaTime * speedModifier;

            objectPosition = CalculateBezierPoint(tParam, dynamicControlPoints);

            transform.position = objectPosition + new Vector3(0, transform.localScale.y / 2, 0);

            yield return new WaitForEndOfFrame();
        }

        tParam = 0;
        speedModifier *= 0.90f;
        routeToGo++;

        if (routeToGo > routes.Length - 1)
        {
            routeToGo = 0;
        }

        coroutineAllowed = true;
    }

    private Vector3 CalculateBezierPoint(float t, Transform[] controlPoints)
    {
        int n = controlPoints.Length - 1;
        Vector3 point = Vector3.zero;

        float[] binomials = GetBinomialCoefficients(n);

        for (int i = 0; i <= n; i++)
        {
            float coefficient = binomials[i] * Mathf.Pow(1 - t, n - i) * Mathf.Pow(t, i);
            point += coefficient * controlPoints[i].position;
        }

        return point;
    }

    private float[] GetBinomialCoefficients(int n)
    {
        float[] coeffs = new float[n + 1];
        coeffs[0] = 1;

        for (int i = 1; i <= n; i++)
        {
            coeffs[i] = coeffs[i - 1] * (n - i + 1) / i;
        }

        return coeffs;
    }
}
