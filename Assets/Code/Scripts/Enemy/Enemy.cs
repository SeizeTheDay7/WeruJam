using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
using System;
using System.Collections;

public class Enemy : MonoBehaviour
{
    [HideInInspector] public NavMeshAgent agent;
    [SerializeField] Transform graphic;
    [SerializeField] float collapseAfter = 2f;
    public bool isDead { get; private set; } = false;
    public event Action<Enemy> OnCollapse;
    AudioSource audioSource;
    float graphicInitY;
    Collider col;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        col = GetComponent<Collider>();
        graphicInitY = graphic.localPosition.y;
        audioSource = GetComponent<AudioSource>();
    }

    public void Init()
    {
        // underground에서 기어올라와
        isDead = false;
        col.enabled = true;
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
        agent.isStopped = true;
        isDead = true;
        col.enabled = false;
        audioSource.Play();
        graphic.DOLocalMoveY(graphicInitY, collapseAfter).SetEase(Ease.Linear);
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