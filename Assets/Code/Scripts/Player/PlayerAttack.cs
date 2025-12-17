using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    /// <summary>
    /// Attack Origin에서 생성된 후에 follow origin을 따라서 animation
    /// </summary>

    [Header("Component")]
    [SerializeField] GameObject weaponPrefab;
    [SerializeField] Transform attackProjectilePoint; // 투사체 애니메이션 따라감
    Animator animator;
    InputAction attackAction;

    [Header("Parameter")]
    [SerializeField] float attackPower = 50f;

    [Header("Calculation")]
    Transform currentWeapon;
    bool isAttacking = false;

    void Awake()
    {
        attackAction = InputSystem.actions.FindAction("Attack");
        animator = GetComponent<Animator>();
    }

    void OnEnable()
    {
        attackAction.Enable();
        attackAction.performed += OnAttack;
    }

    void OnDisable()
    {
        attackAction.Disable();
        attackAction.performed -= OnAttack;
    }

    [Button]
    public void SpawnWeapon()
    {
        currentWeapon = Instantiate(weaponPrefab, attackProjectilePoint.position, attackProjectilePoint.rotation, attackProjectilePoint.transform).transform;
    }

    void OnAttack(InputAction.CallbackContext context)
    {
        if (isAttacking) return;
        isAttacking = true;
        currentWeapon = Instantiate(weaponPrefab, attackProjectilePoint.position, attackProjectilePoint.rotation, attackProjectilePoint.transform).transform;
        animator.Play("Attack");
    }

    void OnAttackEnd()
    {
        isAttacking = false;

        Transform cam = Camera.main.transform;
        Vector3 camUpperForward = (cam.forward + cam.up).normalized;
        Debug.DrawRay(attackProjectilePoint.position, camUpperForward * 10f, Color.red, 2f);

        currentWeapon.parent = null;
        Rigidbody weaponRB = currentWeapon.GetComponent<Rigidbody>();
        weaponRB.isKinematic = false;
        weaponRB.AddForce(camUpperForward * attackPower, ForceMode.Impulse);
        currentWeapon = null;
    }
}