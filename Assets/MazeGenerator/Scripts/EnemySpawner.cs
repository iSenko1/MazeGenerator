using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner instance;

    public Transform player;
    public GameObject enemyPrefab;
    public float spawnDistance = 10f;

    private bool enemySpawned = false;

    private void Awake()
    {
        // If an EnemySpawner already exists, destroy this one
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        // Else, this becomes the instance
        instance = this;
        DontDestroyOnLoad(this.gameObject);

    }

    void Start()
    {
        if (!enemySpawned)
        {
            SpawnEnemyNearPlayer();
            enemySpawned = true;
        }
    }

    void SpawnEnemyNearPlayer()
    {
        // Calculate random direction
        Vector3 randomDirection = Random.onUnitSphere;
        randomDirection.y = 0; // Keep it on the same horizontal plane
        randomDirection.Normalize(); // Normalize to ensure it's a unit vector

        // Calculate spawn position
        Vector3 spawnPosition = player.position + randomDirection * spawnDistance;

        // Add an offset in Y to prevent spawning inside the floor (optional)
        spawnPosition.y += 1f;

        // Instantiate enemy at spawn position
        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
    }
}


