using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class EnemyManager : MonoBehaviour
{
    [Header("Component")]
    [SerializeField] Player player;
    [SerializeField] SplineContainer splineContainer;
    [SerializeField] GameObject enemyPrefab;

    [Header("Parameter")]
    [SerializeField] float spawnAmount = 8;
    [SerializeField] float spawnInterval = 3f;

    [Header("Calculation")]
    bool canSpawn = true;
    List<Enemy> enemies = new();

    void Update()
    {
        UpdateTarget();
        if (!canSpawn) return;
        StartCoroutine(CoSpawn());
    }

    private void UpdateTarget()
    {
        foreach (Enemy enemy in enemies)
        {
            if (enemy == null) continue;
            enemy.SetTarget(player.transform);
        }
    }

    private IEnumerator CoSpawn()
    {
        canSpawn = false;

        for (int i = 0; i < spawnAmount; i++)
        {
            float t = i / spawnAmount + Random.Range(-0.05f, 0.05f);
            Vector3 spawnPosition = splineContainer.EvaluatePosition(t);
            Enemy enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity).GetComponent<Enemy>();
            enemy.SetTarget(player.transform);
            enemies.Add(enemy);
        }

        yield return new WaitForSeconds(spawnInterval);
        canSpawn = true;
    }
}