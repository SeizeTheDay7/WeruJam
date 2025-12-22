using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;
using DG.Tweening;
using System.Collections;

public interface IRechargeBullet
{
    public void RechargeBullet();
}

public enum AttackPhase
{
    NotArmed,
    Grabbing,
    Holding,
    Throwing
}

public class PlayerAttack : MonoBehaviour, IRechargeBullet
{
    /// <summary>
    /// Attack Origin에서 생성된 후에 follow origin을 따라서 animation
    /// </summary>

    [Header("Component")]
    [SerializeField] Transform hand_right; // 들고 있는 손, weapon의 부모가 된다
    [SerializeField] Transform hand_right_pinpoint; // 들고 있는 손의 특정 지점, weapon의 local position의 ref가 된다.
    [SerializeField] DummyWeaponBox dummyBox;
    [SerializeField] WeaponPool weaponPool;
    Animator animator;
    Transform cam;

    [Header("Action")]
    InputAction attackAction;

    [Header("Parameter")]
    [SerializeField] float attackPower = 50f;

    [Header("Calculation")]
    Weapon currentWeapon;
    AttackPhase phase;

    void Awake()
    {
        phase = AttackPhase.NotArmed;
        cam = Camera.main.transform;
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

    public void RechargeBullet()
    {
        dummyBox.Recharge();
    }

    #region Attack Event
    void OnAttack(InputAction.CallbackContext context)
    {
        switch (phase)
        {
            case AttackPhase.NotArmed:
                StartGrab();
                break;
            case AttackPhase.Grabbing: // 집고있는 도중, 공격 불가
                break;
            case AttackPhase.Holding: // 집은 상태, 공격 가능
                StartThrow();
                break;
            case AttackPhase.Throwing:
                break;
        }
    }

    private void StartGrab()
    {
        if (dummyBox.isEmpty()) return;
        animator.Play("GrabWeapon");
        phase = AttackPhase.Grabbing;
        StartCoroutine(CoStartHold(1.25f));
    }

    /// <summary>
    /// GrabWeapon 애니메이션 이벤트가 호출
    /// </summary>
    private void StartHold()
    {
        phase = AttackPhase.Holding;
        // TODO :: 십자선 UI 등장
    }

    /// <summary>
    /// Holding 상태에서 공격 누르면 호출됨
    /// </summary>
    private void StartThrow()
    {
        phase = AttackPhase.Throwing;
        animator.Play("Attack");
    }

    /// <summary>
    /// 보여주기용 성냥 하나 끄고, pool에서 가져온 성냥을 그 위치에서부터 animation
    /// </summary>
    void OnGrabWeapon()
    {
        dummyBox.DisableDummyWeapon();
        currentWeapon = weaponPool.Get();
        currentWeapon.ResetWeapon(hand_right_pinpoint);
    }

    private IEnumerator CoStartHold(float animTime)
    {
        yield return new WaitForSeconds(animTime);
        StartHold();
    }

    void OnLightMatch()
    {
        currentWeapon.LightUp();
    }

    void OnAttackEnd()
    {
        Vector3 camUpperForward = (cam.forward + cam.up).normalized;


        currentWeapon.Launch(camUpperForward, attackPower);
        currentWeapon.transform.SetParent(null);
        currentWeapon = null;

        // TODO :: 십자선 UI 해제
        phase = AttackPhase.NotArmed;
    }
    #endregion
}