using UnityEngine;
using System;
using System.Collections;

public enum WeaponState
{
    None,
    Holding,
    Extinguishing
}

public class Weapon : MonoBehaviour
{
    [SerializeField] LayerMask groundMask;
    [SerializeField] LayerMask enemyMask;
    [SerializeField] Adjuster_MarchShader shader;
    [SerializeField] Light weaponLight;
    [SerializeField] AnimationCurve holdingCurve;
    [SerializeField] float holdingCurveDuration = 3f;
    float holdingStartTime;
    [SerializeField] AnimationCurve extinguishCurve;
    [SerializeField] float extinguishCurveDuration = 5f;
    float extinguishStartTime;

    public event Action<Weapon> OnDestroyed;
    Rigidbody rb;
    bool isTerminated = false;
    Transform currnetFollow;
    WeaponState state;
    float initLightIntensity;

    public void HoldingLight()
    {
        initLightIntensity = weaponLight.intensity;
        state = WeaponState.Holding;
        holdingStartTime = Time.time;
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        state = WeaponState.None;
    }


    void Update()
    {
        if (currnetFollow)
        {
            transform.position = currnetFollow.position;
            transform.rotation = currnetFollow.rotation;
        }

        switch (state)
        {
            case WeaponState.Holding:
                {
                    float t = (Time.time - holdingStartTime) / holdingCurveDuration;
                    weaponLight.intensity = holdingCurve.Evaluate(t) * initLightIntensity;
                    break;
                }

            case WeaponState.Extinguishing:
                {
                    float t = (Time.time - extinguishStartTime) / extinguishCurveDuration;
                    weaponLight.intensity = extinguishCurve.Evaluate(t) * initLightIntensity;
                    break;
                }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isTerminated) return;

        int colMask = 1 << collision.gameObject.layer;
        if ((colMask & enemyMask) != 0)
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.Die();
                Terminate();
            }
        }
        else if ((colMask & groundMask) != 0)
        {
            // 땅에 닿으면 불이 식는다는 컨셉
            Terminate();
        }
    }

    public void ResetWeapon(Transform follow)
    {
        currnetFollow = follow;
        isTerminated = false;
        rb.isKinematic = true;
    }

    public void LightUp()
    {
        shader.OnFire();
    }

    public void Launch(Vector3 direction, float power)
    {
        rb.isKinematic = false;
        ResetVelocity();

        currnetFollow = null;
        rb.AddForce(direction * power, ForceMode.Impulse);
    }

    private void Terminate()
    {
        if (isTerminated) return;
        isTerminated = true;

        ResetVelocity();
        rb.isKinematic = true;

        StartCoroutine(CoDestroy());
    }

    private void ResetVelocity()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    private IEnumerator CoDestroy()
    {
        state = WeaponState.Extinguishing;
        extinguishStartTime = Time.time;
        yield return new WaitForSeconds(extinguishCurveDuration);

        OnDestroyed?.Invoke(this);
    }
}