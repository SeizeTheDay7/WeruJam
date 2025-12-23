using System.Collections.Generic;
using UnityEngine;

public class InteractableDummy : MonoBehaviour, IInteractable
{
    static readonly int OutlineOn = Shader.PropertyToID("_OutlineOn"); // 셰이더 프로퍼티명
    List<Renderer> rends;
    MaterialPropertyBlock mpb;
    bool isOn;

    void Awake()
    {
        rends = new List<Renderer>(GetComponentsInChildren<Renderer>(true));
        rends.Add(GetComponent<Renderer>());
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

        for (int i = 0; i < rends.Count; i++)
        {
            var r = rends[i];
            r.GetPropertyBlock(mpb);
            mpb.SetFloat(OutlineOn, on ? 1f : 0f);
            r.SetPropertyBlock(mpb);
        }
    }
}