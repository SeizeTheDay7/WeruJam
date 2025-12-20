using UnityEngine;
using System;

public class Weapon : MonoBehaviour
{
    [SerializeField] LayerMask groundMask;
    [SerializeField] LayerMask enemyMask;

    public event Action<Weapon> OnDestroyed;
    Rigidbody rb;
    bool isTerminated = false;
    Transform currnetFollow;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (currnetFollow)
        {
            // rb.MovePosition(currnetFollow.position);
            // rb.MoveRotation(currnetFollow.rotation);
            transform.position = currnetFollow.position;
            transform.rotation = currnetFollow.rotation;
        }
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

    public void ResetWeapon(Transform follow)
    {
        currnetFollow = follow;
        isTerminated = false;
        rb.isKinematic = true;
    }

    public void Launch(Vector3 direction, float power)
    {
        rb.isKinematic = false;
        ResetVelocity();

        currnetFollow = null;
        rb.AddForce(direction * power, ForceMode.Impulse);
    }

    private void Terminate()
    {
        if (isTerminated) return;
        isTerminated = true;

        ResetVelocity();
        rb.isKinematic = true;

        OnDestroyed?.Invoke(this);
    }

    private void ResetVelocity()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
}