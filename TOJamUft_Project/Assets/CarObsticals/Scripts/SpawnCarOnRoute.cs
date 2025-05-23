using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCarOnRoute : MonoBehaviour
{
    [SerializeField] private FollowRoute carToSpawn;
    [SerializeField] private Transform roads;

    public GameObject eui;
    public GameObject alcoholUI;

    private Transform roadSpawnTo; // route object the car spawns to
    private Transform routeSpawnTo; //route object the car spawns to
    private int currentRoute; // used to find routes: routes F are 0-1, routes B are 2-3

    //Transform[] controlPoints = new Transform[4]; // the route the player will follow

    bool spawnCarAfterTime = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(SpawnCarTimer());
    }

    // Update is called once per frame
    void SpawnCar()
    {
        // Pick a random road group (FRouteA, BRouteB, etc.)
        roadSpawnTo = roads.GetChild(Random.Range(0, roads.transform.childCount));

        // Pick a random lane (child of the road group)
        currentRoute = Random.Range(0, roadSpawnTo.transform.childCount);

        // This is the actual Transform where the car spawns
        routeSpawnTo = roadSpawnTo.GetChild(currentRoute);

        // Spawn the car at that position/rotation
        FollowRoute currentCar = Instantiate(
            carToSpawn,
            routeSpawnTo.position,
            Quaternion.Euler(routeSpawnTo.eulerAngles),
            this.transform
        );

        // Tell the car which route and position it starts from
        currentCar.setRoute(currentRoute, roads, routeSpawnTo);
    }


    private IEnumerator SpawnCarTimer()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(1, 2));
            SpawnCar();

            spawnCarAfterTime = true;
        }
    }
}
