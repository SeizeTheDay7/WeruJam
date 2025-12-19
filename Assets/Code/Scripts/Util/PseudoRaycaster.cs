using UnityEngine;
using UnityEngine.InputSystem;

public interface IRaycastHandler
{
    public void OnHit();
    public void OnExitHit();
}

/// <summary>
/// 긴 막대로 raycast 흉내
/// </summary>
public class PseudoRaycaster : MonoBehaviour
{
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        transform.rotation = Quaternion.LookRotation(ray.direction);
    }

    void OnTriggerEnter(Collider other)
    {
        IRaycastHandler raycastHitComponent = other.GetComponent<IRaycastHandler>();
        raycastHitComponent?.OnHit();
    }

    void OnTriggerExit(Collider other)
    {
        IRaycastHandler raycastHitComponent = other.GetComponent<IRaycastHandler>();
        raycastHitComponent?.OnExitHit();
    }
}