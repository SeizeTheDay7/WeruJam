using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;

public class Enemy : MonoBehaviour
{
    [HideInInspector] public NavMeshAgent agent;
    [SerializeField] Transform graphic;

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
        Destroy(gameObject);
    }
}