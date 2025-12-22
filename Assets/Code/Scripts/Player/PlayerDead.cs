using Unity.Cinemachine;
using UnityEngine;

public class PlayerDead : MonoBehaviour
{
    [SerializeField] CinemachineCamera fpsCam;
    [SerializeField] LayerMask enemyLayer;
    Player player;

    void Awake()
    {
        player = GetComponent<Player>();
    }

    void OnTriggerEnter(Collider hit)
    {
        int colMask = 1 << hit.gameObject.layer;
        if ((colMask & enemyLayer) != 0)
        {
            player.Shutdown(hit.transform);
            EnemyManager.Instance.Shutdown();
            SoundManager.Instance.MuteAllExceptScream();
        }
    }
}