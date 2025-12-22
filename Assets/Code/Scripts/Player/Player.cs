using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Component")]
    public PlayerMove move { get; private set; }
    public PlayerAttack attack { get; private set; }
    public PlayerInteract interact { get; private set; }
    public bool isDead { get; private set; } = false;


    void Awake()
    {
        move = GetComponent<PlayerMove>();
        attack = GetComponent<PlayerAttack>();
        interact = GetComponent<PlayerInteract>();
    }

    public void Shutdown(Transform enemy)
    {
        isDead = true;
        move.Shutdown(enemy.GetComponent<Enemy>());
        attack.Shutdown();
        attack.enabled = false;
        interact.enabled = false;
    }
}
