using UnityEngine;

public class InteractableDummy : MonoBehaviour, IInteractable
{
    static readonly int OutlineOn = Shader.PropertyToID("_OutlineOn"); // 셰이더 프로퍼티명
    Renderer[] rends;
    MaterialPropertyBlock mpb;
    bool isOn;

    void Awake()
    {
        rends = GetComponentsInChildren<Renderer>(true);
        mpb = new MaterialPropertyBlock();
    }

    public void OnInteract(PlayerInteract player)
    {
        if (player.TryGetComponent<IRechargeBullet>(out var rechargeable))
        {
            rechargeable.RechargeBullet();
        }
    }

    public void OnHit(PlayerInteract player)
    {
        SetOutline(true);
    }

    public void OnExitHit(PlayerInteract player)
    {
        SetOutline(false);
    }

    public void SetOutline(bool on)
    {
        if (isOn == on) return;
        isOn = on;

        for (int i = 0; i < rends.Length; i++)
        {
            var r = rends[i];
            r.GetPropertyBlock(mpb);
            mpb.SetFloat(OutlineOn, on ? 1f : 0f);
            r.SetPropertyBlock(mpb);
        }
    }
}