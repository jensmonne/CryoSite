using System.Collections;
using BNG;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

public class NetworkedMagPickUp : NetworkBehaviour
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
        if (!isLocalPlayer)
            return;

        if (GM == null)
        {
            GM = FindFirstObjectByType<GameManager>();
        }

        if (GM.Magcount <= 0)
            return;

        if (RightHandInZone && gripRightAction.action.WasPressedThisFrame())
        {
            CmdRequestSpawnMagazine("Right");
        }

        if (LeftHandInZone && gripLeftAction.action.WasPressedThisFrame())
        {
            CmdRequestSpawnMagazine("Left");
        }
    }

    [Command]
    private void CmdRequestSpawnMagazine(string hand)
    {
        if (GM == null || GM.Magcount <= 0)
            return;

        GameObject selectedMagPrefab = rifleMagPrefab;

        // Check hand detectors on server — optional validation
        BaseGun heldGun = rightHandDetector?.currentGun ?? leftHandDetector?.currentGun;

        if (heldGun != null && heldGun.firingType == FiringType.Pistol)
        {
            selectedMagPrefab = pistolMagPrefab;
        }

        Transform spawnPoint = hand == "Right" ? rightSpawnPos : leftSpawnPos;

        GameObject magInstance = Instantiate(selectedMagPrefab, spawnPoint.position, spawnPoint.rotation);
        NetworkServer.Spawn(magInstance, connectionToClient); // Spawns and assigns ownership

        GM.Magcount--;

        RpcForceGrab(magInstance, hand);
    }

    [ClientRpc]
    private void RpcForceGrab(GameObject magInstance, string hand)
    {
        if (!isLocalPlayer)
            return;

        Grabber grabber = hand == "Right" ? rightGrabber : leftGrabber;
        Grabbable grabbable = magInstance.GetComponent<Grabbable>();
        if (grabber != null && grabbable != null)
        {
            StartCoroutine(ForceGrab(grabbable, grabber));
        }
    }

    private IEnumerator ForceGrab(Grabbable grabbable, Grabber grabber)
    {
        yield return null;
        grabber.GrabGrabbable(grabbable);
    }
}
