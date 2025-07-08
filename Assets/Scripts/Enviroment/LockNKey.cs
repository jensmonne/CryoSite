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
    [Header("Door Setup")]
    [SerializeField] private MonoBehaviour doorScript;
    
    [Header("Visual Feedback")]
    [SerializeField] private GameObject light; // Shows that the door is unlocked
    [SerializeField] private Material material; // The material to apply to the light when unlocked
    
    [Header("Boss & Arena")]
    [SerializeField] private OnBossTriggerEnter obte; // Script that handles boss spawning
    [SerializeField] private GameObject arena; // Arena GameObject that becomes active on unlocking

    private IUnlockableDoor _door;

    private void Start()
    {
        // Lets us call Unlock().
        _door = doorScript as IUnlockableDoor;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Key")) return;
        
        // Unlock the door
        _door.Unlock();
        // Turn on the boss arena
        arena.SetActive(true);
        // Spawn the boss
        obte.SpawnBoss();
        
        // Change the material of the light so the player knows the door is unlocked.
        var component = light.GetComponent<Renderer>();
        if (component != null)
        {
            component.material = material;
        }
    }
}
