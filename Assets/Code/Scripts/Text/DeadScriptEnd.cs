using UnityEngine;

public abstract class ScriptEnd : MonoBehaviour
{
    public abstract void OnScriptEnd(Scripter scripter);
}

public class DeadScriptEnd : ScriptEnd
{
    [SerializeField] GameObject buttonDiv;

    public override void OnScriptEnd(Scripter scripter)
    {
        buttonDiv.SetActive(true);
    }
}