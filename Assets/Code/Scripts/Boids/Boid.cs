using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    [SerializeField] float rayLength = 2f;
    [SerializeField] float speed = 5f;
    [SerializeField] float turnAceel = 1f;
    [SerializeField] float avoidAccel = 10f;
    [SerializeField] float alignAccel = 1f;
    [SerializeField] float followAccel = 1f;
    Transform target;
    AudioSource audioSource;
    float nextCryTime = 0;
    [SerializeField] float shortestCooltime = 2f;
    [SerializeField] float longestCooltime = 20f;
    [SerializeField] AudioClip[] crySounds;

    HashSet<Boid> collidingBoids = new();

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        nextCryTime = Time.time + Random.Range(shortestCooltime, longestCooltime);
    }

    void Start()
    {
        float offset = Random.Range(0f, 1f);
        GetComponent<Animator>().Play("Fly", 0, offset);
    }

    public void TryCry()
    {
        if (Time.time >= nextCryTime)
        {
            audioSource.clip = crySounds[Random.Range(0, crySounds.Length)];
            audioSource.Play();
            float cooltime = Random.Range(shortestCooltime, longestCooltime);
            nextCryTime = Time.time + cooltime;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // print("Boid : OnTriggerEnter");
        if (other.TryGetComponent(out Boid otherBoid))
        {
            collidingBoids.Add(otherBoid);
        }
    }

    void OnTriggerExit(Collider other)
    {
        // print("Boid : OnTriggerExit");
        if (other.TryGetComponent(out Boid otherBoid))
        {
            collidingBoids.Remove(otherBoid);
        }
    }

    public void Init(Transform target)
    {
        this.target = target;
    }

    public void UpdateBoid()
    {
        Vector3 headDir = transform.forward;
        GetSteeringSum(out Vector3 avoidanceSum, out Vector3 alignmentSum);
        Vector3 followTargetSum = FollowTarget();

        Vector3 finalDir = (headDir
                            + avoidanceSum * avoidAccel
                            + alignmentSum * alignAccel
                            + followTargetSum * followAccel).normalized;
        if (finalDir != Vector3.zero)
            transform.forward = Vector3.Slerp(transform.forward, finalDir, Time.deltaTime * turnAceel);

        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private void GetSteeringSum(out Vector3 avoidanceSum, out Vector3 alignmentSum)
    {
        avoidanceSum = Vector3.zero; // 다른 놈 만났을 때 가까워지면 회피
        alignmentSum = Vector3.zero; // 다른 놈 만났을 때 거리 상관 없이 정렬 시작

        Vector3 pos = transform.position;
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        foreach (var boid in collidingBoids)
        {
            if (boid == null) continue;

            Vector3 toOther = boid.transform.position - pos;
            float dist = toOther.magnitude;
            if (dist < 0.01f) continue; // 너무 가까우면 무시 (계산 오류 방지)

            Vector3 dirToOther = toOther / dist;
            float inFront = Vector3.Dot(forward, dirToOther);

            // 정면에 있어야 회피
            if (inFront > 0f)
            {
                // 오른쪽에 있으면 왼쪽으로, 왼쪽에 있으면 오른쪽으로 회피
                float side = Vector3.Dot(dirToOther, right) > 0 ? -1f : 1f;
                float strength = inFront / (dist + 0.5f); // 분모에 값을 더해 급격한 변화 방지
                avoidanceSum += right * side * strength;

                alignmentSum += boid.transform.forward;
            }
        }

        alignmentSum.Normalize();
    }

    /// <summary>
    /// 타겟에 가까울수록 타겟을 쫓아가려고 해
    /// </summary>
    private Vector3 FollowTarget()
    {
        Vector3 toTarget = target.position - transform.position;
        float dist = toTarget.magnitude;
        if (dist < 0.01f) return Vector3.zero; // 너무 가까우면 무시 (계산 오류 방지)

        Vector3 targetFollowSum = toTarget.normalized / dist;
        return targetFollowSum;
    }

}
