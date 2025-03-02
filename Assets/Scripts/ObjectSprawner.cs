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
    public float spawnRadius = 100f; // Radius of the area to spawn objects
    public float spawnInterval = 5f; // Time interval between spawns

    [Header("Character Settings")]
    public Transform characterTransform; // Reference to the character's transform

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
            // Start spawning objects at intervals
            StartCoroutine(SpawnObjectsAtIntervals());
        }
        else
        {
            Debug.LogError("No active terrain found in the scene.");
        }
    }

    IEnumerator SpawnObjectsAtIntervals()
    {
        while (true)
        {
            SpawnObjects();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnObjects()
    {
        if (characterTransform == null)
        {
            Debug.LogError("Character transform is not assigned.");
            return;
        }

        Vector3 characterPosition = characterTransform.position;

        foreach (var spawnableObject in objectsToSpawn)
        {
            for (int i = 0; i < spawnableObject.count; i++)
            {
                // Generate a random position within the specified radius around the character
                float angle = Random.Range(0f, Mathf.PI * 2);
                float distance = Random.Range(0f, spawnRadius);
                float x = characterPosition.x + Mathf.Cos(angle) * distance;
                float z = characterPosition.z + Mathf.Sin(angle) * distance;
                float y = terrain.SampleHeight(new Vector3(x, 0, z)) + spawnHeightOffset;

                // Instantiate the object at the calculated position
                Vector3 spawnPosition = new Vector3(x, y, z);
                GameObject spawnedObject = Instantiate(spawnableObject.prefab, spawnPosition, Quaternion.identity);

                // Ensure the spawned object has a collider
                if (spawnedObject.GetComponent<Collider>() == null)
                {
                    spawnedObject.AddComponent<BoxCollider>();
                }
            }
        }
    }
}