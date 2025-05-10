using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class spawnCar : MonoBehaviour
{
    public GameObject cars;

    public GameObject tileSpawner;
    public GameObject _FrountTile;
    public GameObject _BackTile;

    public Transform[] currentRoutes;

    //public Transform[] controlPoints;
    private bool _SpawnCar = true;

    // Update is called once per frame
    void Update()
    {
        //spawns cars after a 1-3 secounds randomly between 3 points
        if (_SpawnCar)
        {
            // Get the tiles
            GameObject _BackTile = tileSpawner.transform.GetChild(0).gameObject;
            GameObject _FrontTile = tileSpawner.transform.GetChild(tileSpawner.transform.childCount - 1).gameObject;

            // Get their "Routes" children
            Transform backRoutes = _BackTile.transform.Find("Routes");
            Transform frontRoutes = _FrontTile.transform.Find("Routes");

            // Assign control points: [0, 1] from backRoutes, [2, 3] from frontRoutes
            Transform[] controlPoints = new Transform[4];
            controlPoints[0] = backRoutes.GetChild(0);
            controlPoints[1] = backRoutes.GetChild(1);
            controlPoints[2] = frontRoutes.GetChild(2);
            controlPoints[3] = frontRoutes.GetChild(3);

            StartCoroutine(_SpawnCarTimer());

            List<int> usedIndices = new List<int>();

            int numToSpawn = Random.Range(1, controlPoints.Length - 2);

            for (int i = 0; i < numToSpawn; i++)
            {
                int index;
                do
                {
                    index = Random.Range(0, controlPoints.Length);
                } while (usedIndices.Contains(index));

                usedIndices.Add(index);

                Transform spawnPoint = controlPoints[index];
                GameObject vehicle = Instantiate(cars, spawnPoint.position, Quaternion.Euler(spawnPoint.eulerAngles), this.gameObject.transform);
                vehicle.GetComponent<forward>().getTargetChild(index);
            }

            _SpawnCar = false;
        }
    }

    private IEnumerator _SpawnCarTimer()
    {
        // rantom time to spawn
        yield return new WaitForSeconds(Random.Range(0.2f, 1));
        _SpawnCar = true;
    }
}