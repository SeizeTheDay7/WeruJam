using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;

public class PlayerAttack : MonoBehaviour
{
    /// <summary>
    /// Attack Origin에서 생성된 후에 follow origin을 따라서 animation
    /// </summary>

    [Header("Component")]
    [SerializeField] GameObject[] weaponPrefabs;
    [SerializeField] Transform attackProjectilePoint; // 투사체 애니메이션 따라감
    [SerializeField] TMP_Text tmp_bullet;
    Animator animator;

    [Header("Action")]
    InputAction attackAction;
    InputAction prevWeaponAction;
    InputAction nextWeaponAction;

    [Header("Parameter")]
    [SerializeField] float attackPower = 50f;
    [SerializeField] int defaultCapacity = 5;
    [SerializeField] int maxSize = 20;
    [SerializeField] int maxWeaponCount = 15;

    [Header("Calculation")]
    int currentWeaponCount;
    Transform currentWeapon;
    bool isAttacking = false;
    int weaponIdx = 0;

    public ObjectPool<Weapon>[] weaponPools { get; private set; }

    void Awake()
    {
        currentWeaponCount = maxWeaponCount;
        SetCurrentBulletUI();

        attackAction = InputSystem.actions.FindAction("Attack");
        prevWeaponAction = InputSystem.actions.FindAction("PrevWeapon");
        nextWeaponAction = InputSystem.actions.FindAction("NextWeapon");
        animator = GetComponent<Animator>();

        // 무기 풀 초기화
        weaponPools = new ObjectPool<Weapon>[weaponPrefabs.Length];

        for (int i = 0; i < weaponPrefabs.Length; i++)
        {
            int poolIndex = i; // Closure를 피하기 위해 로컬 변수에 할당
            weaponPools[i] = new ObjectPool<Weapon>(
                createFunc: () => OnCreatePoolWeapon(poolIndex),
                actionOnGet: OnGetWeaponFromPool,
                actionOnRelease: OnReleaseWeaponToPool,
                actionOnDestroy: OnDestroyPoolWeapon,
                collectionCheck: true,
                defaultCapacity: defaultCapacity,
                maxSize: maxSize
            );
        }
    }

    private void SetCurrentBulletUI()
    {
        tmp_bullet.text = currentWeaponCount + " / " + maxWeaponCount;
    }

    void OnEnable()
    {
        attackAction.Enable();
        attackAction.performed += OnAttack;

        prevWeaponAction.Enable();
        nextWeaponAction.Enable();
        prevWeaponAction.performed += PrevWeapon;
        nextWeaponAction.performed += NextWeapon;
    }

    void OnDisable()
    {
        attackAction.Disable();
        attackAction.performed -= OnAttack;

        prevWeaponAction.Disable();
        nextWeaponAction.Disable();
        prevWeaponAction.performed -= PrevWeapon;
        nextWeaponAction.performed -= NextWeapon;
    }

    void PrevWeapon(InputAction.CallbackContext context)
    {
        weaponIdx--;
        weaponIdx = (weaponIdx + weaponPrefabs.Length) % weaponPrefabs.Length;
    }

    void NextWeapon(InputAction.CallbackContext context)
    {
        weaponIdx++;
        weaponIdx = weaponIdx % weaponPrefabs.Length;
    }

    private Weapon OnCreatePoolWeapon(int poolIndex)
    {
        Weapon weapon = Instantiate(weaponPrefabs[poolIndex]).GetComponent<Weapon>();
        weapon.OnDestroyed += OnWeaponDestroyed;
        return weapon;
    }

    private void OnWeaponDestroyed(Weapon weapon)
    {
        // 적절한 풀에 반환하기 위해 weaponPrefabs와 비교
        for (int i = 0; i < weaponPrefabs.Length; i++)
        {
            if (weapon.gameObject.name.Contains(weaponPrefabs[i].name))
            {
                weaponPools[i].Release(weapon);
                return;
            }
        }
    }

    /// <summary>
    /// 무기를 풀에서 가져올 때 호출됨
    /// </summary>
    private void OnGetWeaponFromPool(Weapon weapon)
    {
        weapon.gameObject.SetActive(true);
    }

    /// <summary>
    /// 무기가 파괴될 때 Release로 호출됨
    /// </summary>
    private void OnReleaseWeaponToPool(Weapon weapon)
    {
        weapon.gameObject.SetActive(false);
    }

    private void OnDestroyPoolWeapon(Weapon weapon)
    {
        if (weapon == null) return;
        weapon.OnDestroyed -= OnWeaponDestroyed;
        Destroy(weapon.gameObject);
    }

    public Weapon GetWeapon(int weaponIndex, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        if (weaponIndex < 0 || weaponIndex >= weaponPools.Length)
        {
            Debug.LogError($"Invalid weapon index: {weaponIndex}");
            return null;
        }

        Weapon weapon = weaponPools[weaponIndex].Get();
        weapon.transform.SetPositionAndRotation(position, rotation);
        weapon.ResetWeapon(attackProjectilePoint);
        return weapon;
    }

    void OnAttack(InputAction.CallbackContext context)
    {
        if (isAttacking) return;
        if (currentWeaponCount == 0) return;
        isAttacking = true;
        currentWeapon = GetWeapon(weaponIdx, attackProjectilePoint.position, attackProjectilePoint.rotation, attackProjectilePoint).transform;
        animator.Play("Attack");
    }

    void OnAttackEnd()
    {
        currentWeaponCount--;
        SetCurrentBulletUI();

        Transform cam = Camera.main.transform;
        Vector3 camUpperForward = (cam.forward + cam.up).normalized;
        Debug.DrawRay(attackProjectilePoint.position, camUpperForward * 10f, Color.red, 2f);

        currentWeapon.GetComponent<Weapon>().Launch(camUpperForward, attackPower);
        currentWeapon = null;

        isAttacking = false;
    }
}