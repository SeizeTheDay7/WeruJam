using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Component")]
    public PlayerMove move { get; private set; }
    public PlayerAttack attack { get; private set; }


    void Awake()
    {
        move = GetComponent<PlayerMove>();
        attack = GetComponent<PlayerAttack>();
    }

    void Update()
    {

    }
}
