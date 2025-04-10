using System.Collections.Generic;
using UnityEngine;

public class ChomperSpawner : MonoBehaviour
{
    public GameObject chomperPrefab;
    public List<Transform> spawnPoints; // Also used as patrol points

    void Start()
    {
        if (spawnPoints == null || spawnPoints.Count == 0)
        {
            Debug.LogError("No spawn points assigned!");
            return;
        }

        // Spawn one chomper per spawn point
        foreach (Transform spawn in spawnPoints)
        {
            GameObject newChomper = Instantiate(chomperPrefab, spawn.position, spawn.rotation);
            
            // Optional: Assign the same spawnPoints list to the patrolPoints of the ChomperAI script
            ChomperAI chomperAI = newChomper.GetComponent<ChomperAI>();
            if (chomperAI != null)
            {
                chomperAI.patrolPoints = spawnPoints;
            }
        }
    }
}
