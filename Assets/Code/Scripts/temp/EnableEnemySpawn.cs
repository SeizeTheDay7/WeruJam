using UnityEngine;

public class EnableEnemySpawn : MonoBehaviour
{
    [SerializeField] EnemyManager enemyManager;

    void Start()
    {
        enemyManager.enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player") return;
        enemyManager.enabled = true;
        Destroy(gameObject);
    }
}
