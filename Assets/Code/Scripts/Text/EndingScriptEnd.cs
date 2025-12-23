using UnityEngine;
using DG.Tweening;

public class EndingScriptEnd : ScriptEnd
{
    [SerializeField] GameObject buttonDiv;
    [SerializeField] GameObject creditDiv;

    public override void OnScriptEnd(Scripter scripter)
    {
        buttonDiv.SetActive(true);
        Camera.main.transform.DORotate(new Vector3(0, -70, 0), 2f).OnComplete(() =>
        {
            creditDiv.SetActive(true);
        });
    }
}