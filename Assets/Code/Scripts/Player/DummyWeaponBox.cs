using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// 던지기 전까지의 가짜 성냥들을 관리한다
/// </summary>
public class DummyWeaponBox : MonoBehaviour
{
    [SerializeField] GameObject[] dummies;
    [SerializeField] TMP_Text tmp_bullet;
    int currentWeaponIdx;

    void Awake()
    {
        currentWeaponIdx = dummies.Length - 1;
        SetCurrentWeaponUI();
    }

    public bool isEmpty() => currentWeaponIdx < 0;

    public GameObject DisableDummyWeapon()
    {
        if (currentWeaponIdx < 0) return null;
        var dummy = dummies[currentWeaponIdx];
        dummy.SetActive(false);
        currentWeaponIdx--;
        SetCurrentWeaponUI();
        return dummy;
    }

    /// <summary>
    /// 공격 중엔 호출 못하게 PlayerAttack에서 막을 것
    /// </summary>
    [Button]
    public void Recharge()
    {
        currentWeaponIdx = dummies.Length - 1;
        SetCurrentWeaponUI();

        foreach (var dummy in dummies)
        {
            dummy.SetActive(true);
        }
    }

    private void SetCurrentWeaponUI()
    {
        tmp_bullet.text = (currentWeaponIdx + 1).ToString() + " / " + dummies.Length.ToString();
    }
}