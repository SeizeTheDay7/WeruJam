using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Scripter : MonoBehaviour
{
    [SerializeField] TMP_Text tmp;
    [SerializeField] string[] script;
    [SerializeField] float letterCoolTime = 0.2f;
    [SerializeField] float scriptCoolTime = 1f;
    [SerializeField] ScriptEnd scriptEnd;
    AudioSource audioSource;
    int scriptIdx;

    void Awake()
    {
        tmp.maxVisibleCharacters = 0;
        tmp.text = script[0];
        audioSource = GetComponent<AudioSource>();
        scriptIdx = 0;
        ProceedScript();
    }

    void Update()
    {
        if (Keyboard.current.anyKey.wasPressedThisFrame || Mouse.current.leftButton.wasPressedThisFrame)
        {
            SkipText();
        }
    }

    private void SkipText()
    {
        StopAllCoroutines();

        // 텍스트가 아직 모두 나오지 않았다면 모두 출력
        if (tmp.maxVisibleCharacters < tmp.text.Length)
        {
            tmp.maxVisibleCharacters = tmp.text.Length;
            if (scriptIdx >= script.Length - 1)
            {
                scriptEnd.OnScriptEnd(this);
                return;
            }
        }
        // 텍스트가 모두 나왔다면 다음 텍스트로 넘어감
        else
        {
            scriptIdx++;
            ProceedScript();
        }
    }

    /// <summary>
    /// ProceedScript()와 CoProceedScript()는 서로를 호출하는 관계
    /// </summary>
    private void ProceedScript()
    {
        if (scriptIdx > script.Length - 1)
        {
            scriptEnd.OnScriptEnd(this);
            return;
        }
        StartCoroutine(CoProceedScript());
    }

    private IEnumerator CoProceedScript()
    {
        tmp.text = script[scriptIdx];
        tmp.maxVisibleCharacters = 0;
        yield return new WaitForSeconds(letterCoolTime);
        for (int j = 0; j <= script[scriptIdx].Length; j++)
        {
            tmp.maxVisibleCharacters = j;
            audioSource.Play();
            yield return new WaitForSeconds(letterCoolTime);
        }
        yield return new WaitForSeconds(scriptCoolTime);
        scriptIdx++;
        ProceedScript();
    }
}