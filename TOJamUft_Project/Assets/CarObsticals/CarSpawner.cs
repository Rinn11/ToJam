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
        //if (lane.points.Count == 0) return;

        Vector3 spawnPoint = lane.points[Random.Range(0, spawnLanes.Count)];
        Vector3 offset = new Vector3(Random.Range(-spawnRadius, spawnRadius), 0, Random.Range(-spawnRadius, spawnRadius));
        GameObject car = Instantiate(carPrefab, spawnPoint + offset, Quaternion.identity);

        PathFollower movement = car.GetComponent<PathFollower>();
        if (movement != null)
        {
            movement.AssignLane(lane);
        }

        activeCars.Add(car);
    }

    void CleanupCars()
    {
        activeCars.RemoveAll(car => car == null);
    }
}
