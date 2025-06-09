using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class CarSpawner : MonoBehaviour
{
    public GameObject carPrefab;
    public List<LanePath> spawnLanes;
    public float spawnInterval = 3f;
    public float spawnRadius = 1f;
    public int maxCars = 10;

    private float timer;
    private List<GameObject> activeCars = new();

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            TrySpawnCar();
            timer = 0f;
        }

        CleanupCars();
    }

    void TrySpawnCar()
    {
        if (activeCars.Count >= maxCars || spawnLanes.Count == 0)
            return;

        LanePath lane = spawnLanes[Random.Range(0, spawnLanes.Count)];
        if (lane.points.Count == 0) return;

        Vector3 spawnPoint = lane.points[Random.Range(0, lane.points.Count)];
        Vector3 offset = new Vector3(Random.Range(-spawnRadius, spawnRadius), 0, Random.Range(-spawnRadius, spawnRadius));
        Vector3 finalPos = spawnPoint + offset;

        GameObject car = Instantiate(carPrefab, finalPos, Quaternion.identity);

        PathFollower movement = car.GetComponent<PathFollower>();
        if (movement != null)
        {
            //movement.AssignLane(lane);

            // Optional: Rotate car to face the next point in the lane if possible
            if (lane.points.Count > 1)
            {
                Vector3 direction = (lane.points[1] - lane.points[0]).normalized;
                if (direction != Vector3.zero)
                {
                    car.transform.rotation = Quaternion.LookRotation(direction);
                }
            }
        }

        activeCars.Add(car);
    }

    void CleanupCars()
    {
        activeCars.RemoveAll(car => car == null);
    }
}
