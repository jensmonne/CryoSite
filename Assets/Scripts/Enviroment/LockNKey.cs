using UnityEngine;

/// <summary>
/// Add IUnlockableDoor with inheritance to any door script as to make it work with the LockNKey. Just add the fucking following shit to it to make it work:
/// A boolean that checks if the door can open (the key has entered the thingie)
/// and this script:
/// TODO
/// public void Unlock()
/// {
///    canAutoOpen = true;
/// }
/// </summary>

public interface IUnlockableDoor
{
    void Unlock();
}

public class LockNKey : MonoBehaviour
{
    [SerializeField] private MonoBehaviour doorScript;

    private IUnlockableDoor door;

    private void Start()
    {
        door = doorScript as IUnlockableDoor;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Key")) return;
        
        door?.Unlock();
    }
}
