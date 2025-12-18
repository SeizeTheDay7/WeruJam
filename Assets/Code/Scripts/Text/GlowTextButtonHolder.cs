using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class GlowTextButtonHolder : MonoBehaviour, IRaycastHandler
{
    [SerializeField] Material normalMat;
    [SerializeField] Material glowMat;
    [SerializeField] TextMeshPro tmp;
    InputAction clickAction;
    [SerializeField] UnityEvent clickEvent;

    void Awake()
    {
        tmp.fontMaterial = normalMat;
        clickAction = InputSystem.actions.FindAction("Attack");
    }

    void OnEnable()
    {
        clickAction.Enable();
    }

    void OnDisable()
    {
        clickAction.Disable();
        clickAction.performed -= OnClickButton;
    }

    public void OnHit()
    {
        tmp.fontMaterial = glowMat;
        clickAction.performed += OnClickButton;
    }

    public void OnExitHit()
    {
        tmp.fontMaterial = normalMat;
        clickAction.performed -= OnClickButton;
    }

    private void OnClickButton(InputAction.CallbackContext context)
    {
        clickEvent.Invoke();
    }
}