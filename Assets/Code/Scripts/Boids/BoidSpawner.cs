using UnityEngine;
using System.Collections.Generic;

public class BoidSpawner : MonoBehaviour
{
    [SerializeField] GameObject spawnerCube;
    [SerializeField] GameObject crowPrefab;
    [SerializeField] int crowAmount = 10;

    List<Boid> boids;

    Vector2 XLimit;
    Vector2 YLimit;
    Vector2 ZLimit;

    void Awake()
    {
        boids = new List<Boid>(crowAmount);

        Vector3 scale = spawnerCube.transform.localScale;
        Vector3 position = spawnerCube.transform.position;

        XLimit = new Vector2(position.x - scale.x / 2, position.x + scale.x / 2);
        // YLimit = new Vector2(position.y - scale.y / 2, position.y + scale.y / 2);
        YLimit = new Vector2(position.y, position.y);
        ZLimit = new Vector2(position.z - scale.z / 2, position.z + scale.z / 2);
    }

    void Start()
    {

        for (int i = 0; i < crowAmount; i++)
        {
            Vector3 spawnPosition = new Vector3(
                Random.Range(XLimit.x, XLimit.y),
                Random.Range(YLimit.x, YLimit.y),
                Random.Range(ZLimit.x, ZLimit.y)
            );

            Quaternion spawnRotation = Quaternion.Euler(
                0f,
                Random.Range(0f, 360f),
                0f
            );
            // Quaternion spawnRotation = Quaternion.identity;

            var boid = Instantiate(crowPrefab, spawnPosition, spawnRotation).GetComponent<Boid>();
            boids.Add(boid);
            boid.Init(boids);
        }
    }

    void Update()
    {
        Vector3 flockDir = Vector3.zero;
        foreach (var boid in boids)
        {
            flockDir += boid.transform.forward;
        }
        flockDir.Normalize();

        foreach (var boid in boids)
        {
            boid.UpdateBoid(flockDir);
            BoundToBox(boid.transform);
        }
    }

    private void BoundToBox(Transform boid)
    {
        Vector3 pos = boid.position;
        if (pos.x < XLimit.x) pos.x = XLimit.y;
        else if (pos.x > XLimit.y) pos.x = XLimit.x;
        if (pos.y < YLimit.x) pos.y = YLimit.y;
        else if (pos.y > YLimit.y) pos.y = YLimit.x;
        if (pos.z < ZLimit.x) pos.z = ZLimit.y;
        else if (pos.z > ZLimit.y) pos.z = ZLimit.x;
        boid.position = pos;
    }
}
