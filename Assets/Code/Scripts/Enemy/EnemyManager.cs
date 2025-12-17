using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.AI;

public class EnemyManager : MonoBehaviour
{
    [Header("Component")]
    [SerializeField] Player player;
    [SerializeField] SplineContainer splineContainer;
    [SerializeField] GameObject enemyPrefab;

    [Header("Parameter")]
    [SerializeField] int spawnAmount = 8;
    [SerializeField] float spawnInterval = 3f;
    [SerializeField] float followTargetInterval = 0.1f;
    [SerializeField] float navmeshSampleRadius = 5f;

    [Header("Calculation")]
    [SerializeField] float mapMaxHeight = 90f;
    [SerializeField] float checkRayLength = 100f;
    int navmeshAreaMask = NavMesh.AllAreas;
    bool canUpdateTarget = true;
    bool canSpawn = true;
    List<Enemy> enemies = new();

    void Update()
    {
        if (canUpdateTarget) StartCoroutine(CoUpdateTarget());
        if (canSpawn) StartCoroutine(CoSpawn());
    }

    private IEnumerator CoUpdateTarget()
    {
        canUpdateTarget = false;
        foreach (Enemy enemy in enemies)
        {
            if (enemy == null) continue;
            if (!enemy.agent.enabled) continue;
            enemy.SetTarget(player.transform);
        }
        yield return new WaitForSeconds(followTargetInterval);
        canUpdateTarget = true;

    }

    private IEnumerator CoSpawn()
    {
        canSpawn = false;

        for (int i = 0; i < spawnAmount; i++)
        {
            Vector3 spawnPosition = GetSpawnPoint(i);
            if (spawnPosition == Vector3.zero) continue;
            Enemy enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity).GetComponent<Enemy>();
            enemy.Init();
            enemies.Add(enemy);
        }

        yield return new WaitForSeconds(spawnInterval);
        canSpawn = true;
    }

    private Vector3 GetSpawnPoint(int i)
    {
        float t = (float)i / spawnAmount + Random.Range(-0.05f, 0.05f);
        Vector3 point = splineContainer.EvaluatePosition(t);
        point.y = mapMaxHeight;
        Ray ray = new Ray(point, Vector3.down);
        Debug.DrawRay(ray.origin, ray.direction * checkRayLength, Color.green, 1f);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, checkRayLength))
        {
            if (NavMesh.SamplePosition(hitInfo.point, out NavMeshHit navmeshHit, navmeshSampleRadius, navmeshAreaMask))
            {
                return navmeshHit.position;
            }
        }
        return Vector3.zero;
    }
}