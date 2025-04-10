using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HeartSpawner : MonoBehaviour
{
    [System.Serializable]
    public class HeartObject
    {
        public GameObject prefab;
        public int count;
    }

    [Header("Heart Spawn Settings")]
    public List<HeartObject> heartsToSpawn; // List of heart objects to spawn with their counts
    public float spawnHeightOffset = 1f; // Height offset to place hearts above the plane
    public float heartSize = 1f; // Size of the heart
    public Vector3 heartRotation = Vector3.zero; // Rotation of the heart

    public GameObject plane; // Reference to the plane

    void Start()
    {
        // Start spawning hearts
        SpawnHearts();
    }

    void SpawnHearts()
    {
        if (plane == null)
        {
            Debug.LogError("Plane is not assigned.");
            return;
        }

        Vector3 planePosition = plane.transform.position;
        Vector3 planeScale = plane.transform.localScale;

        foreach (var heartObject in heartsToSpawn)
        {
            if (heartObject.prefab == null)
            {
                Debug.LogError("Heart prefab is not assigned.");
                continue;
            }

            for (int i = 0; i < heartObject.count; i++)
            {
                Vector3 spawnPosition = Vector3.zero; // Initialize with a default value
                bool validPosition = false;

                // Try to find a valid position within the plane bounds
                for (int attempts = 0; attempts < 10; attempts++)
                {
                    // Generate a random position within the plane bounds
                    float x = Random.Range(planePosition.x - planeScale.x * 5, planePosition.x + planeScale.x * 5);
                    float z = Random.Range(planePosition.z - planeScale.z * 5, planePosition.z + planeScale.z * 5);

                    // Check if the position is within the plane bounds
                    if (x >= planePosition.x - planeScale.x * 5 && x <= planePosition.x + planeScale.x * 5 &&
                        z >= planePosition.z - planeScale.z * 5 && z <= planePosition.z + planeScale.z * 5)
                    {
                        float y = planePosition.y + spawnHeightOffset;
                        spawnPosition = new Vector3(x, y, z);

                        // Check for collisions with other objects
                        Collider[] colliders = Physics.OverlapSphere(spawnPosition, heartSize / 2);
                        if (colliders.Length == 0)
                        {
                            validPosition = true;
                            break;
                        }
                        else
                        {
                            Debug.Log("Collision detected, trying another position.");
                        }
                    }
                }

                if (validPosition)
                {
                    Debug.Log($"Spawning heart at position: {spawnPosition}");
                    // Instantiate the heart object at the calculated position with the specified rotation
                    GameObject spawnedHeart = Instantiate(heartObject.prefab, spawnPosition, Quaternion.Euler(heartRotation));
                    spawnedHeart.transform.localScale = Vector3.one * heartSize; // Set the size of the heart

                    // Ensure the spawned heart object has a collider
                    if (spawnedHeart.GetComponent<Collider>() == null)
                    {
                        spawnedHeart.AddComponent<BoxCollider>();
                    }
                }
                else
                {
                    Debug.LogWarning("Failed to find a valid spawn position within the plane bounds.");
                }
            }
        }
    }
}