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
    void SpawnCar() { 
            // Get the road the cars are spawning on
            roadSpawnTo = roads.GetChild(Random.Range(0, roads.transform.childCount));

            // gets which spawner to place the cars at
            currentRoute = Random.Range(0, roadSpawnTo.transform.childCount);

            // Get their "Routes" children
            routeSpawnTo = roadSpawnTo.GetChild(currentRoute);

            //Transform spawnPoint = controlPoints[index];
            FollowRoute currentCar = Instantiate(carToSpawn, routeSpawnTo.position, Quaternion.Euler(routeSpawnTo.eulerAngles), this.gameObject.transform);
            currentCar.setRoute(currentRoute, roads);

            // vehicle.GetComponentInChildren<Heads>().endScreenUI = eui;
            // vehicle.GetComponentInChildren<Heads>().alcoholUI = alcoholUI;
            //vehicle.GetComponent<forward>().getTargetChild(index, objectPosition);

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
