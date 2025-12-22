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
    }

    private void OnDisable()
    {
        _renderer.material.SetFloat(PropertyName, 0);
        pointLight.intensity = 0f;
    }

    private IEnumerator CallEveryFrameForSeconds(float seconds)
    {
        float elapsed = 0f;

        while (elapsed < seconds)
        {
            //매 프레임 실행할 코드
            OnFrame(elapsed / seconds); // 0~1 정규화 값

            elapsed += Time.deltaTime;
            yield return null; // 다음 프레임
        }

        //종료 시 한 번 실행
        OnFinished();
    }

    private void OnFrame(float t01)
    {
        _renderer.material.SetFloat(PropertyName, t01);
        pointLight.intensity = Mathf.Lerp(0f, 15f, t01);
    }

    private void OnFinished()
    {
        // 종료 처리
    }

    public void OnFire()
    {
        StartCoroutine(CallEveryFrameForSeconds(duration));
    }
}