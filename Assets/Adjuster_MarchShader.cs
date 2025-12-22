using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Adjuster_MarchShader : MonoBehaviour
{
    // Components and References variables
    Weapon weapon;
    private MeshRenderer _renderer;
    [SerializeField]
    private string PropertyName;
    private GameObject Socket;
    private Light pointLight;
    // Internal calculation variables
    private bool isLoopingFire = false;
    private float intensityMax = 5f;
    private float intensityOffset = 10f;
    private float timer = 0f;
    // Unity Methods
    private void Awake()
    {
        weapon = GetComponent<Weapon>();
        _renderer = GetComponent<MeshRenderer>();
        PropertyName = "_" + PropertyName;
        Socket = transform.GetChild(0).gameObject;
        pointLight = Socket.GetComponent<Light>();
        pointLight.intensity = 0f;
        pointLight.range = 10f;
        pointLight.color = ColorUtility.TryParseHtmlString("#FF8000", out var c) ? c : Color.white;
        isLoopingFire = false;
    }

    private void OnDisable()
    {
        _renderer.material.SetFloat(PropertyName, 0f);
        pointLight.intensity = 0f;
        isLoopingFire = false;
    }

    private void Update()
    {
        if (!isLoopingFire) { return; }

        timer += Time.deltaTime;
        pointLight.intensity = intensityMax + Mathf.PingPong(timer, intensityOffset);
    }

    // User Methods OnFire
    public void OnFire(float EndTime)
    {
        print("OnFire");
        StartCoroutine(OnFire_CallEveryFrameForSeconds(EndTime));
        timer = 0f;
    }
    private IEnumerator OnFire_CallEveryFrameForSeconds(float seconds)
    {
        float elapsed = 0f;

        while (elapsed < seconds)
        {
            OnFire_OnFrame(elapsed / seconds);

            elapsed += Time.deltaTime;
            yield return null;
        }

        OnFire_Finished();
    }
    private void OnFire_OnFrame(float t01)
    {
        _renderer.material.SetFloat(PropertyName, t01);
        pointLight.intensity = Mathf.Lerp(0f, intensityMax, t01);
    }
    private void OnFire_Finished()
    {
        _renderer.material.SetFloat(PropertyName, 1f);
        pointLight.intensity = intensityMax;
        timer = 0f;
    }
    // User Methods OffFire
    public void OffFire(float Endtime)
    {
        print("OffFire");
        isLoopingFire = false;
        StartCoroutine(OffFire_CallEveryFrameForSeconds(Endtime, intensityMax+Mathf.PingPong(timer, intensityOffset)));
    }
    private IEnumerator OffFire_CallEveryFrameForSeconds(float seconds, float tempintensity)
    {
        float elapsed = 0f;

        while (elapsed < seconds)
        {
            OffFire_OnFrame(elapsed / seconds, tempintensity);

            elapsed += Time.deltaTime;
            yield return null;
        }

        OffFire_Finished();
    }
    private void OffFire_OnFrame(float t01, float tempintensity)
    {
        _renderer.material.SetFloat(PropertyName, 1f-t01);
        pointLight.intensity = Mathf.Lerp(tempintensity, 0f, t01);
    }
    private void OffFire_Finished()
    {
        _renderer.material.SetFloat(PropertyName, 0f);
        pointLight.intensity = 0f;
        timer = 0f;
    }

}