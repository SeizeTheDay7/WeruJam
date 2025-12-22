using UnityEngine;
using System.Collections;

public class Adjuster_MarchShader : MonoBehaviour
{
    private MeshRenderer _renderer;
    [SerializeField]
    private float duration;
    [SerializeField]
    private string PropertyName;

    private GameObject Socket;
    private Light pointLight;

    private bool isFinished = false;
    private float pingPongTime = 0f;
    private float intensityMax = 10f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        _renderer = GetComponent<MeshRenderer>();
        PropertyName = "_" + PropertyName;
        
        Socket = transform.GetChild(0).gameObject;
        pointLight = Socket.GetComponent<Light>();
        pointLight.intensity = 0f;
        pointLight.range = 10f;
        pointLight.color = ColorUtility.TryParseHtmlString("#FF8000", out var c) ? c : Color.white;
        
        isFinished = false;
        pingPongTime = 0f;
        intensityMax = 5f;
    }

    private void OnDisable()
    {
        _renderer.material.SetFloat(PropertyName, 0);
        pointLight.intensity = 0f;
        isFinished = false;
    }

    private void Update()
    {
        if (isFinished)
        {
            pingPongTime = Mathf.PingPong(Time.time, 1f);
            pointLight.intensity = intensityMax + pingPongTime*10.0f;
        }
    }

    private IEnumerator CallEveryFrameForSeconds(float seconds)
    {
        float elapsed = 0f;

        while (elapsed < seconds)
        {
            OnFrame(elapsed / seconds); 

            elapsed += Time.deltaTime;
            yield return null; 
        }

        OnFinished();
    }

    private void OnFrame(float t01)
    {
        _renderer.material.SetFloat(PropertyName, t01);
        pointLight.intensity = Mathf.Lerp(0f, intensityMax, t01);
    }

    private void OnFinished()
    {
        pointLight.intensity = intensityMax;
        isFinished = true;
    }

    public void OnFire()
    {
        print("OnFire");
        StartCoroutine(CallEveryFrameForSeconds(duration));
    }
}