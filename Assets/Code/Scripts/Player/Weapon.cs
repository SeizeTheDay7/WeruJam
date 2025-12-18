using UnityEngine;
using System;

public class Weapon : MonoBehaviour
{
    [SerializeField] LayerMask groundMask;
    [SerializeField] LayerMask enemyMask;

    public event Action<Weapon> OnDestroyed;
    Rigidbody rb;
    bool isDestroyed = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isDestroyed) return;

        int colMask = 1 << collision.gameObject.layer;
        if ((colMask & enemyMask) != 0)
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.Collapse();
                Destroy();
            }
        }
        else if ((colMask & groundMask) != 0)
        {
            // 땅에 닿으면 불이 식는다는 컨셉
            Destroy();
        }
    }

    public void Reset()
    {
        rb.isKinematic = true;
        rb.Sleep();
        isDestroyed = false;
    }

    public void Launch(Vector3 direction, float power)
    {
        rb.isKinematic = false;
        rb.WakeUp();
        rb.AddForce(direction * power, ForceMode.Impulse);
    }

    private void Destroy()
    {
        if (isDestroyed) return;
        isDestroyed = true;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;
        rb.Sleep();
        OnDestroyed?.Invoke(this);
    }
}