using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectSpawner : MonoBehaviour
{
    [System.Serializable]
    public class SpawnableObject
    {
        public GameObject prefab;
        public int count;
    }

    [Header("Spawn Settings")]
    public List<SpawnableObject> objectsToSpawn; // List of objects to spawn with their counts
    public float spawnHeightOffset = 0f; // Height offset to place objects above the terrain

    private Terrain terrain; // Reference to the terrain

    void Start()
    {
        // Start the coroutine to wait for the terrain to be generated
        StartCoroutine(WaitForTerrainAndSpawnObjects());
    }

    IEnumerator WaitForTerrainAndSpawnObjects()
    {
        // Wait until the terrain is generated
        while (Terrain.activeTerrain == null)
        {
            yield return null;
        }

        // Get the active terrain
        terrain = Terrain.activeTerrain;

        // Check if the terrain is found
        if (terrain != null)
        {
            // Spawn objects across the entire terrain
            SpawnObjects();
        }
        else
        {
            Debug.LogError("No active terrain found in the scene.");
        }
    }

    void SpawnObjects()
    {
        if (terrain == null)
        {
            Debug.LogError("Terrain is not assigned.");
            return;
        }

        Vector3 terrainPosition = terrain.transform.position;
        Vector3 terrainSize = terrain.terrainData.size;

        foreach (var spawnableObject in objectsToSpawn)
        {
            for (int i = 0; i < spawnableObject.count; i++)
            {
                Vector3 spawnPosition = Vector3.zero; // Initialize with a default value
                bool validPosition = false;

                // Try to find a valid position within the terrain bounds
                for (int attempts = 0; attempts < 10; attempts++)
                {
                    // Generate a random position within the terrain bounds
                    float x = Random.Range(terrainPosition.x, terrainPosition.x + terrainSize.x);
                    float z = Random.Range(terrainPosition.z, terrainPosition.z + terrainSize.z);

                    // Check if the position is within the terrain bounds
                    if (x >= terrainPosition.x && x <= terrainPosition.x + terrainSize.x &&
                        z >= terrainPosition.z && z <= terrainPosition.z + terrainSize.z)
                    {
                        float y = terrain.SampleHeight(new Vector3(x, 0, z)) + spawnHeightOffset;
                        spawnPosition = new Vector3(x, y, z);
                        validPosition = true;
                        break;
                    }
                }

                if (validPosition)
                {
                    // Instantiate the object at the calculated position
                    GameObject spawnedObject = Instantiate(spawnableObject.prefab, spawnPosition, Quaternion.identity);

                    // Ensure the spawned object has a collider
                    if (spawnedObject.GetComponent<Collider>() == null)
                    {
                        spawnedObject.AddComponent<BoxCollider>();
                    }
                }
                else
                {
                    Debug.LogWarning("Failed to find a valid spawn position within the terrain bounds.");
                }
            }
        }
    }
}