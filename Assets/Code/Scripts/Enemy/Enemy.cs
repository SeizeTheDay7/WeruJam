using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
using System;

public class Enemy : MonoBehaviour
{
    [HideInInspector] public NavMeshAgent agent;
    [SerializeField] Transform graphic;
    public event Action<Enemy> OnCollapse;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void Init()
    {
        // underground에서 기어올라와
        graphic.DOLocalMoveY(0, 1f).SetEase(Ease.Linear);
    }

    public void SetTarget(Transform target)
    {
        agent.SetDestination(target.position);
    }

    public void Die()
    {
        // 눈 빛내는거 없애고 이동 멈추기
        agent.isStopped = true;
    }

    public void Collapse()
    {
        OnCollapse?.Invoke(this);
    }
}