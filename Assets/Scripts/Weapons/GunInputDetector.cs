using UnityEngine;
using UnityEngine.InputSystem;

public class GunInputDetector : MonoBehaviour
{
    public PlayerInput playerInput;

    private void OnTriggerEnter(Collider other)
    {
        // Try to get PlayerInput from the hand or its parents
        PlayerInput input = other.GetComponentInParent<PlayerInput>();
        if (input != null)
        {
            playerInput = input;
            Debug.Log($"PlayerInput found on {input.gameObject.name} from collider {other.gameObject.name}");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerInput input = other.GetComponentInParent<PlayerInput>();
        if (input != null && input == playerInput)
        {
            Debug.Log($"PlayerInput lost from collider {other.gameObject.name}");
            playerInput = null;
        }
    }
}