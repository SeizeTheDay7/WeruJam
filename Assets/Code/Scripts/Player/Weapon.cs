using UnityEngine;
using System;

public class Weapon : MonoBehaviour
{
    [SerializeField] LayerMask groundMask;
    [SerializeField] LayerMask enemyMask;

    public event Action<Weapon> OnDestroyed;
    Rigidbody rb;
    bool isTerminated = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isTerminated) return;

        int colMask = 1 << collision.gameObject.layer;
        if ((colMask & enemyMask) != 0)
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.Collapse();
                Terminate();
            }
        }
        else if ((colMask & groundMask) != 0)
        {
            // 땅에 닿으면 불이 식는다는 컨셉
            Terminate();
        }
    }

    public void Reset()
    {
        isTerminated = false;
    }

    public void Launch(Vector3 direction, float power)
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        gameObject.SetActive(false);
        gameObject.SetActive(true);

        rb.useGravity = true;
        rb.isKinematic = false;
        rb.AddForce(direction * power, ForceMode.Impulse);
    }

    private void Terminate()
    {
        if (isTerminated) return;
        isTerminated = true;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.useGravity = false;
        rb.isKinematic = true;
        OnDestroyed?.Invoke(this);
    }
}