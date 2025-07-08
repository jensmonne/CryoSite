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
        }
        if (other.name.Contains("Right"))
        {
            RightHandInZone = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name.Contains("Left"))
        {
            LeftHandInZone = false;
        }
        if (other.name.Contains("Right"))
        {
            RightHandInZone = false;
        }
    }

    private void Update()
    {
        if (GM == null)
        {
            GM = FindFirstObjectByType<GameManager>();
        }
        
        if (GM.Magcount <= 0)
            return;

        if (RightHandInZone && gripRightAction.action.WasPressedThisFrame())
        {
            SpawnAndGrabMagazine("Right");
        }

        if (LeftHandInZone && gripLeftAction.action.WasPressedThisFrame())
        {
            SpawnAndGrabMagazine("Left");
        }
    }

    private void SpawnAndGrabMagazine(string hand)
    {
        if (GM.Magcount <= 0)
        {
            return;
        }

        Transform spawnPoint = hand == "Right" ? rightSpawnPos : leftSpawnPos;
        Grabber grabbingHandGrabber = hand == "Right" ? rightGrabber : leftGrabber;
        
        BaseGun heldGun = null;

        if (rightHandDetector.currentGun != null)
        {
            heldGun = rightHandDetector.currentGun;
        }
        else if (leftHandDetector.currentGun != null)
        {
            heldGun = leftHandDetector.currentGun;
        }

        // Choose mag prefab
        GameObject selectedMagPrefab = rifleMagPrefab;

        if (heldGun != null)
        {
            if (heldGun.firingType == FiringType.Pistol)
            {
                selectedMagPrefab = pistolMagPrefab;
            }
        }

        // Spawn
        GameObject magInstance = Instantiate(selectedMagPrefab, spawnPoint.position, spawnPoint.rotation);
        Grabbable grabbable = magInstance.GetComponent<Grabbable>();
        if (grabbable == null)
        {
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
