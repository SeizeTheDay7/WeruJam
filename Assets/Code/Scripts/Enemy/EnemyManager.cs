using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.AI;
using UnityEngine.Pool;

public class EnemyManager : MonoBehaviour
{
    [Header("Component")]
    [SerializeField] Player player;
    [SerializeField] SplineContainer splineContainer;
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] GameObject[] enemyPrefabs;

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
    HashSet<Enemy> enemies = new(); // 현재 스폰된 눈사람들
    public ObjectPool<Enemy> enemyPool { get; private set; }
    Vector3 enemySpawnPosition;

    void Awake()
    {
        enemyPool = new ObjectPool<Enemy>(
            createFunc: OnCreatePoolEnemy,
            actionOnGet: OnGetEnemyFromPool,
            actionOnRelease: OnReleaseEnemyToPool,
            actionOnDestroy: OnDestroyPoolEnemy,
            collectionCheck: true,
            defaultCapacity: 20,
            maxSize: 100
        );
    }

    private Enemy OnCreatePoolEnemy()
    {
        int index = Random.Range(0, enemyPrefabs.Length);
        GameObject prefabToSpawn = enemyPrefabs[index];

        Enemy enemy = Instantiate(prefabToSpawn).GetComponent<Enemy>();
        // Enemy enemy = Instantiate(enemyPrefab).GetComponent<Enemy>();
        enemy.OnCollapse += OnEnemyCollapse;
        return enemy;
    }

    private void OnEnemyCollapse(Enemy enemy)
    {
        enemyPool.Release(enemy);
    }

    /// <summary>
    /// 눈사람 새로 만들어낼 때 호출됨
    /// </summary>
    private void OnGetEnemyFromPool(Enemy enemy)
    {
        enemy.Init();
        enemy.transform.position = enemySpawnPosition;
        enemy.gameObject.SetActive(true);
        enemies.Add(enemy);
    }

    /// <summary>
    /// 아예 눈사람 무너져서 사라질 때 Release로 호출됨
    /// </summary>
    private void OnReleaseEnemyToPool(Enemy enemy)
    {
        enemy.gameObject.SetActive(false);
        enemies.Remove(enemy);
    }

    private void OnDestroyPoolEnemy(Enemy enemy)
    {
        enemies.Remove(enemy);
        enemy.OnCollapse -= OnEnemyCollapse;
        Destroy(enemy);
    }

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
            if (enemy.agent.isStopped) continue;
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
            enemySpawnPosition = GetSpawnPoint(i);
            if (enemySpawnPosition == Vector3.zero) continue;
            enemyPool.Get(); // 초기화는 OnGet에서 전부 처리
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