using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{

    [SerializeField] float rayLength = 2f;
    [SerializeField] float speed = 5f;
    [SerializeField] float avoidAccel = 10f;
    [SerializeField] float alignAccel = 1f;

    List<Boid> allBoids;
    HashSet<Boid> collidingBoids = new();

    void OnTriggerEnter(Collider other)
    {
        print("Boid : OnTriggerEnter");
        if (other.TryGetComponent(out Boid otherBoid))
        {
            collidingBoids.Add(otherBoid);
        }
    }

    void OnTriggerExit(Collider other)
    {
        print("Boid : OnTriggerExit");
        if (other.TryGetComponent(out Boid otherBoid))
        {
            collidingBoids.Remove(otherBoid);
        }
    }

    public void Init(List<Boid> boids)
    {
        allBoids = boids;
    }

    public void UpdateBoid(Vector3 flockDir)
    {
        Vector3 headDir = transform.forward;
        Vector3 avoidanceSum = Avoidance();

        Vector3 targetDir = (headDir + avoidanceSum * avoidAccel + flockDir * alignAccel).normalized;
        if (targetDir != Vector3.zero)
            transform.forward = Vector3.Slerp(transform.forward, targetDir, Time.deltaTime * avoidAccel);

        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private Vector3 Avoidance()
    {
        Vector3 avoidanceSum = Vector3.zero;

        foreach (var boid in collidingBoids)
        {
            if (boid == null) continue;

            Vector3 toOther = boid.transform.position - transform.position;
            float dist = toOther.magnitude;
            if (dist < 0.01f) continue; // 너무 가까우면 무시 (계산 오류 방지)

            Vector3 dirToOther = toOther / dist;
            float inFront = Vector3.Dot(transform.forward, dirToOther);

            // 정면에 있어야 회피
            if (inFront > 0f)
            {
                // 오른쪽에 있으면 왼쪽으로, 왼쪽에 있으면 오른쪽으로 회피
                Vector3 right = transform.right;
                float side = Vector3.Dot(dirToOther, right) > 0 ? -1f : 1f;
                float strength = inFront / (dist + 0.5f); // 분모에 값을 더해 급격한 변화 방지
                avoidanceSum += right * side * strength;
            }
        }

        return avoidanceSum;
    }

}
