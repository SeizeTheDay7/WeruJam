using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.AI;
using UnityEngine.Pool;

public class EnemyManager : Singleton<EnemyManager>
{
    [Header("Component")]
    [SerializeField] Player player;
    [SerializeField] SplineContainer splineContainer;
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] GameObject[] enemyPrefabs;

    [Header("Parameter - Spawn")]
    [SerializeField] int spawnAmount = 8;
    [SerializeField] float spawnInterval = 3f;
    [SerializeField] float mapMaxHeight = 90f;
    [SerializeField] float checkRayLength = 100f;

    [Header("Parameter - Performance")]
    [SerializeField] float followTargetInterval = 0.1f;
    [SerializeField] float visibilityCheckInterval = 0.05f;
    [SerializeField] float navmeshSampleRadius = 5f;

    [Header("Calculation")]
    bool canUpdateTarget = true;
    bool canSpawn = true;
    bool canCheckVisible = true;

    int navmeshAreaMask = NavMesh.AllAreas;
    HashSet<Enemy> enemies = new(); // 현재 스폰된 눈사람들
    public ObjectPool<Enemy> enemyPool { get; private set; }

    Vector3 enemySpawnPosition;
    Camera cam;
    float cosThreshold;
    bool isShutdown = false;

    public void Clear()
    {
        var snapshot = new List<Enemy>(enemies);

        foreach (var enemy in snapshot)
        {
            if (enemy == null) continue;
            enemyPool.Release(enemy);
        }
        enemies.Clear();
    }

    public void Shutdown()
    {
        isShutdown = true;
        foreach (Enemy enemy in enemies)
        {
            if (enemy == null) continue;
            enemy.Shutdown();
        }
    }

    protected override void OnAwake()
    {
        cam = Camera.main;
        cosThreshold = Mathf.Cos(180 * Mathf.Deg2Rad);

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
        enemy.agent.Warp(enemySpawnPosition);
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
        if (isShutdown) return;
        if (canSpawn) StartCoroutine(CoSpawn());
        if (canCheckVisible) StartCoroutine(CoCheckVisible());
        if (canUpdateTarget) StartCoroutine(CoUpdateTarget());
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

    private IEnumerator CoCheckVisible()
    {
        canCheckVisible = false;
        foreach (Enemy enemy in enemies)
        {
            if (enemy == null) continue;
            if (enemy.isDead) continue;

            if (IsEnemyVisible(enemy)) { enemy.Stop(); }
            else { enemy.Chase(); }
        }
        yield return new WaitForSeconds(visibilityCheckInterval);
        canCheckVisible = true;
    }

    private bool IsEnemyVisible(Enemy enemy)
    {
        Vector3 camPos = cam.transform.position;
        Vector3 enemyPos = enemy.transform.position + Vector3.up * enemy.agent.height / 2;
        Vector3 toEnemy = enemyPos - camPos;
        float distance = toEnemy.magnitude;
        if (distance <= 0.001f) { return false; }

        bool inFront = Vector3.Dot(cam.transform.forward, toEnemy.normalized) >= cosThreshold;
        if (!inFront) { return false; }

        Vector3 viewportPos = cam.WorldToViewportPoint(enemyPos);
        bool inViewport = viewportPos.x >= 0 && viewportPos.x <= 1 &&
                          viewportPos.y >= 0 && viewportPos.y <= 1 &&
                          viewportPos.z > 0;

        if (inViewport) { return true; }
        else { return false; }
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

}