using UnityEngine;
using UnityEngine.Pool;

public class WeaponPool : MonoBehaviour
{
    [SerializeField] GameObject weaponPrefab;
    [SerializeField] int defaultCapacity = 5;
    [SerializeField] int maxSize = 20;
    ObjectPool<Weapon> weaponPool;

    void Start()
    {
        weaponPool = new ObjectPool<Weapon>(
            createFunc: () =>
            {
                var weaponObj = Instantiate(weaponPrefab);
                var weapon = weaponObj.GetComponent<Weapon>();
                weapon.OnDestroyed += Release;
                return weapon;
            },
            actionOnGet: (weapon) =>
            {
                weapon.gameObject.SetActive(true);
            },
            actionOnRelease: (weapon) =>
            {
                weapon.gameObject.SetActive(false);
            },
            actionOnDestroy: (weapon) =>
            {
                // Destroy(weapon.gameObject);
            },
            collectionCheck: true,
            defaultCapacity: defaultCapacity,
            maxSize: maxSize
        );
    }

    public Weapon Get() => weaponPool.Get();
    public void Release(Weapon weapon) => weaponPool.Release(weapon);
}