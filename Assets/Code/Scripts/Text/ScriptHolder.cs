using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections;

public class ScriptHolder : MonoBehaviour
{
    [SerializeField] TMP_Text tmp;
    [SerializeField] string[] script;
    [SerializeField] bool canSkip;
    [SerializeField] float textCoolTime = 0.1f;
    [SerializeField] GameObject buttonDiv;
    AudioSource audioSource;
    // 오버엔지니어링
    // [SerializeField] bool autoEnd;
    [SerializeField] float scriptEndWaitTime = 2f;
    int scriptIdx;
    bool nowReading;
    bool flag = false;

    // 처음엔 아무 글자도 안 보여주다가 한 글자씩 출력
    // 클릭하면 텍스트 스킵되거나, 텍스트 이미 다 보여줬다면 다음 텍스트로 넘어감
    // 텍스트 다 출력했는데도 클릭 안 했다면 일정 시간 이후 다음 텍스트

    void Awake()
    {
        tmp.maxVisibleCharacters = 0;
        scriptIdx = 0;
        tmp.text = script[scriptIdx];
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (canSkip && Keyboard.current.anyKey.wasPressedThisFrame)
        {
            SkipText();
        }
        else if (tmp.maxVisibleCharacters < tmp.text.Length)
        {
            ProceedText();
        }
        else if (!flag && scriptIdx == script.Length - 1)
        {
            flag = true;
            StartCoroutine(CoEnableButton());
        }
    }

    private void SkipText()
    {
        // 다음 텍스트로
        if (tmp.maxVisibleCharacters == tmp.text.Length)
        {
            if (scriptIdx == script.Length - 1) return;
            tmp.maxVisibleCharacters = 0;
            tmp.text = script[++scriptIdx];
        }
        // 이번 텍스트 끝내기
        else
        {
            tmp.maxVisibleCharacters = tmp.text.Length;
        }
    }

    private void ProceedText()
    {
        if (!nowReading)
        {
            tmp.maxVisibleCharacters++;
            audioSource.Play();
            StartCoroutine(CoTextCoolDown());
        }
    }

    private IEnumerator CoTextCoolDown()
    {
        nowReading = true;
        yield return new WaitForSeconds(textCoolTime);
        nowReading = false;
    }

    private IEnumerator CoEnableButton()
    {
        yield return new WaitForSeconds(textCoolTime * 3);
        buttonDiv.SetActive(true);
    }
}