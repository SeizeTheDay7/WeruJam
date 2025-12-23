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
    [SerializeField] LayerMask interactMask;
    [SerializeField] float rayLength = 10f;
    IInteractable curInteract;
    InputAction interactAction;
    [SerializeField] GameObject interactUI;
    [SerializeField] AudioSource rechargeAudio;

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
        if (curInteract != null)
        {
            curInteract.OnInteract(this);
            rechargeAudio.Play();
        }
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit rayHit;
        Debug.DrawRay(ray.origin, ray.direction * rayLength, Color.red);
        if (Physics.Raycast(ray.origin, ray.direction, out rayHit, rayLength, interactMask)
        && rayHit.transform.TryGetComponent<IInteractable>(out var raycastHandler))
        {
            if (raycastHandler != curInteract)
            {
                curInteract?.OnExitHit(this);
                curInteract = raycastHandler;
                interactUI.SetActive(true);
                curInteract.OnHit(this);
            }
        }
        else
        {
            curInteract?.OnExitHit(this);
            curInteract = null;
            interactUI.SetActive(false);
        }
    }
}