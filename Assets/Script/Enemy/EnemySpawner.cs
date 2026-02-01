using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Configuration of Spawner")]
    public float playerDetectionRange = 5f;
    public float spawnRange = 5f;
    public int maxEnemyCount = 3;
    public int spawningDelay = 10;

    [Header("Reward Settings")]
    public RotatingHazard spinner;
    public DialogueManager dialogueManager;

    [Header("Enemy to Spawn")]
    public GameObject enemyPrefab;

    [Header("Spawn Limits")]
    public int totalSpawnLimit = 3; 
    private int totalSpawnedSoFar = 0;

    private Transform player;
    private float lastTimeSpawn;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void FixedUpdate()
    {
        CheckSpawnable();
    }

    private void CheckSpawnable()
    {
        if (AreAllSpawnersDepleted())
        {
            if (CountTotalEnemiesInWorld() == 0)
            {
                if (spinner != null && spinner.enabled)
                {

                    StopHazardAndDialogue();
                }

                this.enabled = false; 
                return;
            }
        }

        if (totalSpawnedSoFar < totalSpawnLimit)
        {
            if (Vector3.Distance(transform.position, player.position) <= playerDetectionRange)
            {
                if (CountEnemiesNearThis(spawnRange) < maxEnemyCount)
                {
                    if ((Time.time - lastTimeSpawn) >= spawningDelay)
                    {
                        SpawnEnemy();
                        totalSpawnedSoFar++;
                        lastTimeSpawn = Time.time;
                        Debug.Log($"{gameObject.name} Task Progress {totalSpawnedSoFar}/{totalSpawnLimit}");
                    }
                }
            }
        }
    }

    private bool AreAllSpawnersDepleted()
    {
        EnemySpawner[] spawners = FindObjectsOfType<EnemySpawner>();
        foreach (var s in spawners)
        {
            if (s.totalSpawnedSoFar < s.totalSpawnLimit) return false;
        }
        return true;
    }

    private int CountTotalEnemiesInWorld()
    {
        return FindObjectsOfType<Bot>().Length;
    }

    private int CountEnemiesNearThis(float radius)
    {
        int count = 0;
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
        foreach (var c in colliders)
        {
            if (c.GetComponentInParent<Bot>() != null) count++;
        }
        return count;
    }

    private void StopHazardAndDialogue()
    {
        if (spinner != null && spinner.enabled)
        {
            Debug.Log("Mission Accomplished: Rotating hazard stopped.");
            dialogueManager.GlobalShowMessage("Artemis: Bio-signals terminated. Hazard offline. The Access Card is now reachable");
            dialogueManager.GlobalShowMessage("Artemis: Bio-signals terminated. Hazard offline. The Access Card is now reachable");
            spinner.DeactivateHazard();
        }
    }

    private void SpawnEnemy()
    {
        Vector3 spawnPosition = transform.position + Random.insideUnitSphere;
        spawnPosition.y = 0.5f;
        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
    }
}