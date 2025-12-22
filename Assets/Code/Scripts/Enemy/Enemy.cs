using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
using System;
using System.Collections;

public class Enemy : MonoBehaviour
{
    [HideInInspector] public NavMeshAgent agent;
    [SerializeField] Transform graphic;
    [SerializeField] float collapseAfter = 3f;
    public bool isDead { get; private set; } = false;
    public event Action<Enemy> OnCollapse;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void Init()
    {
        // underground에서 기어올라와
        isDead = false;
        graphic.DOLocalMoveY(0, 1f).SetEase(Ease.Linear);
    }

    public void SetTarget(Transform target)
    {
        agent.SetDestination(target.position);
    }

    /// <summary>
    /// 안 바라볼 때 추격 시작
    /// </summary>
    public void Chase()
    {
        agent.isStopped = false;
    }

    /// <summary>
    /// 바라봤을 때 멈춤
    /// </summary>
    public void Stop()
    {
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
    }

    /// <summary>
    /// 머리에 성냥을 맞았을 때 눈 꺼지고 이동 멈춤. n초 뒤에 pool로 반환
    /// </summary>
    public void Die()
    {
        // 눈 빛내는거 없애고 이동 멈추기
        agent.isStopped = true;
        isDead = true;
        StartCoroutine(CoCollapse());
    }

    public IEnumerator CoCollapse()
    {
        yield return new WaitForSeconds(collapseAfter);
        OnCollapse?.Invoke(this);
    }

    public void Shutdown()
    {
        StopAllCoroutines();
        agent.isStopped = true;
    }
}