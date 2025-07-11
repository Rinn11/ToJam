using System.Collections.Generic;
using UnityEngine;

// This script is to be attached to the Directed Graph representing the car pathing system.
// It will handle the spawning of NPC cars at designated nodes in the graph.
public class NPCCarSpawner : MonoBehaviour
{
    [Header("Car Prefabs List")]
    public List<GameObject> carPrefabs; // List of car prefabs to spawn

    [Header("Car Spawn Settings")]
    public int numberOfCarsToSpawn; // Number of cars to spawn
    public float initialCarSpeed;

    [Header("Optional Settings")]
    public Transform cloneParent; // Parent object for cloned cars (optional, can be used for organization)
    [SerializeField]
    private int maxIterations; // Maximum iterations to prevent infinite loops

    List<int> GetShuffledIndices(int count)
    {
        List<int> indices = new List<int>();
        for (int i = 0; i < count; i++)
        {
            indices.Add(i);
        }

        // Fisher–Yates shuffle
        for (int i = 0; i < count; i++)
        {
            int j = Random.Range(i, count);
            int temp = indices[i];
            indices[i] = indices[j];
            indices[j] = temp;
        }

        return indices;
    }

    // Spawn cars
    private void spawnCars()
    { 
        Debug.Log("Spawning Cars");

        // Grab the children of the graph (this object) and randomly spawn a car at one of those nodes.
        List<int> shuffledIndices = GetShuffledIndices(transform.childCount);

        // numberOfCarsToSpawn should not exceed the number of available spawn points
        numberOfCarsToSpawn = Mathf.Min(numberOfCarsToSpawn, transform.childCount);

        int j = 0;
        int k = 0;
        while (cloneParent.childCount < numberOfCarsToSpawn)
        {
            if (k >= maxIterations)
            {
                Debug.LogWarning("Max iterations reached while trying to spawn cars. Stopping to prevent infinite loop.");
                break;
            }
            // Randomly select a car prefab from the list
            GameObject carPrefab = carPrefabs[Random.Range(0, carPrefabs.Count)];

            // Randomly select a spawn point from the children of this object
            Transform spawnPoint = transform.GetChild(shuffledIndices[j]);
            j = (j + 1) % shuffledIndices.Count; // Cycle through the shuffled indices

            // Set the destination for the car to the next travel point
            TravelPoint travelPoint = spawnPoint.GetComponent<TravelPoint>();
            if (travelPoint != null && !travelPoint.triggered)
            {
                // Instantiate the car at the spawn point
                GameObject carInstance = Instantiate(carPrefab, spawnPoint.position, spawnPoint.rotation, cloneParent);
                CarAgent carAgent = carInstance.GetComponent<CarAgent>();
                if (carAgent != null)
                {
                    // Set the speed
                    carAgent.speed = initialCarSpeed;
                    carAgent.destination = travelPoint; // Set the car's destination to the next travel point
                }
            }
            k++;
        }
    }

    private void Update()
    {
        // Add any update logic here if needed, such as spawning new cars dynamically or managing existing ones.
        if (cloneParent.childCount < numberOfCarsToSpawn)
        {
            // Logic to spawn more cars if needed
            spawnCars();
        }
    }
}
