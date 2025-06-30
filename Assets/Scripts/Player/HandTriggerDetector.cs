using UnityEngine;

public class HandTriggerDetector : MonoBehaviour
{
    public BaseGun currentGun;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<BaseGun>(out var gun))
        {
            currentGun = gun;
            Debug.Log($"{gameObject.name} entered trigger with {gun.name}");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<BaseGun>(out var gun))
        {
            if (currentGun == gun)
            {
                Debug.Log($"{gameObject.name} exited trigger from {gun.name}");
                currentGun = null;
            }
        }
    }
}