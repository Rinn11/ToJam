using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnCar : MonoBehaviour
{
    public GameObject cars;
    public GameObject tileSpawner;

    public float waveIntervalMin = 1f;
    public float waveIntervalMax = 2f;
    public int initialCarCount = 3;

    private void Start()
    {
        // Spawn a few cars at game start
        SpawnInitialCars();

        // Begin the infinite spawn loop
        StartCoroutine(SpawnCarWaves());
    }

    private void SpawnInitialCars()
    {
        List<Transform> allPoints = GetAllControlPoints();

        for (int i = 0; i < initialCarCount; i++)
        {
            if (allPoints.Count == 0) break;

            Transform spawnPoint = allPoints[Random.Range(0, allPoints.Count)];
            SpawnCarAtPoint(spawnPoint);
        }
    }

    private IEnumerator SpawnCarWaves()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(waveIntervalMin, waveIntervalMax));

            List<Transform> allPoints = GetAllControlPoints();
            if (allPoints.Count == 0) continue;

            Transform spawnPoint = allPoints[Random.Range(0, allPoints.Count)];
            SpawnCarAtPoint(spawnPoint);
        }
    }

    private void SpawnCarAtPoint(Transform point)
    {
        GameObject vehicle = Instantiate(cars, point.position, Quaternion.Euler(point.eulerAngles), this.transform);
        vehicle.GetComponent<forward>().getTargetChild(point.GetSiblingIndex());
    }

    private List<Transform> GetAllControlPoints()
    {
        List<Transform> points = new List<Transform>();

        foreach (Transform tile in tileSpawner.transform)
        {
            Transform routes = tile.Find("Routes");
            if (routes != null)
            {
                foreach (Transform child in routes)
                {
                    points.Add(child);
                }
            }
        }

        return points;
    }
}
