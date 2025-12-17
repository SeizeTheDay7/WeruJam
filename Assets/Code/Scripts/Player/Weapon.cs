using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] LayerMask groundMask;
    [SerializeField] LayerMask enemyMask;

    void OnCollisionEnter(Collision collision)
    {
        int colMask = 1 << collision.gameObject.layer;
        if ((colMask & enemyMask) != 0)
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.Die();
                Destroy(gameObject);
            }
        }
        else if ((colMask & groundMask) != 0)
        {
            // 땅에 닿으면 불이 식는다는 컨셉
            Destroy(gameObject);
        }
    }
}