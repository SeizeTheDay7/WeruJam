using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IInteractable
{
    void OnInteract(PlayerInteract player); // e 누름
    void OnHit(PlayerInteract player); // ray 닿음
    void OnExitHit(PlayerInteract player); // ray 안 닿음
}

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] float rayLength = 10f;
    IInteractable curInteract;
    InputAction interactAction;

    void Awake()
    {
        interactAction = InputSystem.actions.FindAction("Interact");
    }

    void OnEnable()
    {
        interactAction.Enable();
        interactAction.performed += OnInteract;
    }

    void OnDisable()
    {
        interactAction.Disable();
        interactAction.performed -= OnInteract;
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        curInteract?.OnInteract(this);
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit rayHit;
        Debug.DrawRay(ray.origin, ray.direction * rayLength, Color.red);
        if (Physics.Raycast(ray.origin, ray.direction * rayLength, out rayHit)
        && rayHit.transform.TryGetComponent<IInteractable>(out var raycastHandler))
        {
            if (raycastHandler != curInteract)
            {
                curInteract?.OnExitHit(this);
                curInteract = raycastHandler;
                curInteract.OnHit(this);
            }
        }
        else
        {
            curInteract?.OnExitHit(this);
            curInteract = null;
        }
    }
}