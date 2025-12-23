using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenE : MonoBehaviour
{
    [SerializeField]
    private Material PPMI_InGame;
    [SerializeField]
    private Material PPMI_Dead;

    private string InGameSceneName = "Game";
    private string DeadSceneName = "Dead";
    private string EndingSceneName = "Ending";

    private float InGamePixelationMax = 100f;
    private float DeadPixelationMax = 64f;

    void Awake()
    {
        // 현재 활성 씬 가져오기
        Scene currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();

        // 씬 이름 출력
        Debug.Log("현재 씬 이름: " + currentScene.name);
        if(InGameSceneName== currentScene.name)
        {
            PPMI_InGame_Start2End(1.5f, 0f, InGamePixelationMax);
        }
        //else if(DeadSceneName== currentScene.name)
        //{
        //    PPMI_Dead_Start2End(1.5f, 0f, DeadPixelationMax);
        //}
        //else if(EndingSceneName == currentScene.name)
        //{
        //    PPMI_Dead_Start2End(1.5f, 0f, DeadPixelationMax);
        //}
    }

    // ===============================
    // PPMI_InGame Methods
    // ===============================
    public void PPMI_InGame_Start2End(float duration, float startValue, float EndValue)
    {
        print("PPMI_InGame_Start2End");
        StartCoroutine(PPMI_InGame_Easing_Start2End(duration, startValue, EndValue));
    }

    private IEnumerator PPMI_InGame_Easing_Start2End(float seconds, float startValue, float EndValue)
    {
        float elapsed = 0f;

        while (elapsed < seconds)
        {
            PPMI_InGame_Start2End_OnFrame(elapsed / seconds, startValue, EndValue);
            elapsed += Time.deltaTime;
            yield return null;
        }

        PPMI_InGame_Start2End_Finished(1f, startValue, EndValue);
    }

    private void PPMI_InGame_Start2End_OnFrame(float t01, float startValue, float EndValue)
    {
        PPMI_InGame.SetFloat("_Pixelation", Mathf.Lerp(startValue, EndValue, t01));
    }

    private void PPMI_InGame_Start2End_Finished(float t01, float startValue, float endValue)
    {
        PPMI_InGame.SetFloat("_Pixelation", Mathf.Lerp(startValue, endValue, t01));
    }

    // ===============================
    // PPMI_Dead Methods
    // ===============================
    public void PPMI_Dead_Start2End(float duration, float startValue, float EndValue)
    {
        print("PPMI_Dead_Start2End");
        StartCoroutine(PPMI_Dead_Easing_Start2End(duration, startValue, EndValue));
    }

    private IEnumerator PPMI_Dead_Easing_Start2End(float seconds, float startValue, float EndValue)
    {
        float elapsed = 0f;

        while (elapsed < seconds)
        {
            PPMI_Dead_Start2End_OnFrame(elapsed / seconds, startValue, EndValue);
            elapsed += Time.deltaTime;
            yield return null;
        }

        PPMI_Dead_Start2End_Finished(1f, startValue, EndValue);
    }

    private void PPMI_Dead_Start2End_OnFrame(float t01, float startValue, float EndValue)
    {
        PPMI_Dead.SetFloat("_Pixelation", Mathf.Lerp(startValue, EndValue, t01));
    }

    private void PPMI_Dead_Start2End_Finished(float t01, float startValue, float endValue)
    {
        PPMI_Dead.SetFloat("_Pixelation", Mathf.Lerp(startValue, endValue, t01));
    }
}