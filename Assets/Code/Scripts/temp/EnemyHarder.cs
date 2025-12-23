using UnityEngine;

public class EnemyHarder : MonoBehaviour
{
    public int spawnAmount = 15;
    public float spawnInterval = 8f;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            var enem = other.GetComponent<EnemyManager>();
            enem.spawnAmount = spawnAmount;
            enem.spawnInterval = spawnInterval;
        }
    }
}
