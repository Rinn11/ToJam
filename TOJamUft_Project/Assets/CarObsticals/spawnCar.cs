using System.Collections;
using UnityEngine;

public class spawnCar : MonoBehaviour
{
    public GameObject cars;
    public Transform[] controlPoints;
    private bool _SpawnCar = true;

    // Update is called once per frame
    void Update()
    {
        //spawns cars after a 1-3 secounds randomly between 3 points
        if (_SpawnCar)
        {
            StartCoroutine(_SpawnCarTimer());
            Instantiate(cars, controlPoints[Random.Range(0, 3)]);
            _SpawnCar = false;
        }
    }

    private IEnumerator _SpawnCarTimer()
    {
        // rantom time to spawn
        yield return new WaitForSeconds(Random.Range(0.5f, 3));
        _SpawnCar = true;
    }
}
