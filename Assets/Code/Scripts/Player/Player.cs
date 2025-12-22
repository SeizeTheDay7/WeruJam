using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Component")]
    public PlayerMove move { get; private set; }
    public PlayerAttack attack { get; private set; }
    public PlayerInteract interact { get; private set; }


    void Awake()
    {
        move = GetComponent<PlayerMove>();
        attack = GetComponent<PlayerAttack>();
        interact = GetComponent<PlayerInteract>();
    }

    public void Shutdown(Transform enemy)
    {
        move.Shutdown(enemy.GetComponent<Enemy>());
        attack.Shutdown();
        attack.enabled = false;
        interact.enabled = false;
    }
}
