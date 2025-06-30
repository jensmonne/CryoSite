using System.Collections;
using BNG;
using UnityEngine;
using UnityEngine.InputSystem;

public class MagPickUp : MonoBehaviour
{
    public Transform lefthand;
    public Transform righthand;
    public Grabber leftGrabber;
    public Grabber rightGrabber;
    public HandTriggerDetector leftHandDetector;
    public HandTriggerDetector rightHandDetector;

    [SerializeField] private bool LeftHandInZone = false;
    [SerializeField] private bool RightHandInZone = false;

    [SerializeField] private GameManager GM;

    public InputActionReference gripRightAction;
    public InputActionReference gripLeftAction;
    
    
    [SerializeField] private Transform leftSpawnPos;
    [SerializeField] private Transform rightSpawnPos;
    
    [SerializeField] private GameObject pistolMagPrefab;
    [SerializeField] private GameObject rifleMagPrefab;
    
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.name.Contains("Left"))
        {
            LeftHandInZone = true;
            Debug.Log("Left hand entered mag pickup zone.");
        }
        if (other.name.Contains("Right"))
        {
            RightHandInZone = true;
            Debug.Log("Right hand entered mag pickup zone.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name.Contains("Left"))
        {
            LeftHandInZone = false;
            Debug.Log("Left hand exited mag pickup zone.");
        }
        if (other.name.Contains("Right"))
        {
            RightHandInZone = false;
            Debug.Log("Right hand exited mag pickup zone.");
        }
    }

    private void Update()
    {
        if (GM == null)
        {
            FindFirstObjectByType<GameManager>();
        }
        
        if (GM.Magcount <= 0)
            return;

        if (RightHandInZone && gripRightAction.action.WasPressedThisFrame())
        {
            Debug.Log("Right hand grab pressed, spawning mag.");
            SpawnAndGrabMagazine("Right");
        }

        if (LeftHandInZone && gripLeftAction.action.WasPressedThisFrame())
        {
            Debug.Log("Left hand grab pressed, spawning mag.");
            SpawnAndGrabMagazine("Left");
        }
    }

    private void SpawnAndGrabMagazine(string hand)
    {
        if (GM.Magcount <= 0)
        {
            Debug.Log("No mags left to spawn.");
            return;
        }

        Transform spawnPoint = hand == "Right" ? rightSpawnPos : leftSpawnPos;
        Grabber grabbingHandGrabber = hand == "Right" ? rightGrabber : leftGrabber;
        
        BaseGun heldGun = null;

        if (rightHandDetector.currentGun != null)
        {
            heldGun = rightHandDetector.currentGun;
            Debug.Log("Right hand trigger is overlapping " + heldGun.name);
        }
        else if (leftHandDetector.currentGun != null)
        {
            heldGun = leftHandDetector.currentGun;
            Debug.Log("Left hand trigger is overlapping " + heldGun.name);
        }
        else
        {
            Debug.Log("Neither hand is overlapping a gun. Defaulting to rifle mag.");
        }

        // Choose mag prefab
        GameObject selectedMagPrefab = rifleMagPrefab;

        if (heldGun != null)
        {
            if (heldGun.firingType == FiringType.Pistol)
            {
                selectedMagPrefab = pistolMagPrefab;
                Debug.Log("Held gun is a pistol, spawning pistol mag.");
            }
            else
            {
                Debug.Log("Held gun is rifle or shotgun, spawning rifle mag.");
            }
        }

        // Spawn
        GameObject magInstance = Instantiate(selectedMagPrefab, spawnPoint.position, spawnPoint.rotation);
        Grabbable grabbable = magInstance.GetComponent<Grabbable>();
        if (grabbable == null)
        {
            Debug.LogError("Spawned mag is missing Grabbable component!");
            Destroy(magInstance);
            return;
        }

        GM.Magcount--;

        StartCoroutine(ForceGrab(grabbable, grabbingHandGrabber));
    }

    private IEnumerator ForceGrab(Grabbable grabbable, Grabber grabber)
    {
        yield return null;
        grabber.GrabGrabbable(grabbable);
    }
}
