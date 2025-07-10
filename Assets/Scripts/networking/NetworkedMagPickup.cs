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

    public InputActionReference gripRightAction;
    public InputActionReference gripLeftAction;

    [SerializeField] private Transform leftSpawnPos;
    [SerializeField] private Transform rightSpawnPos;
    
    [SerializeField] private GameObject pistolMagPrefab;
    [SerializeField] private GameObject rifleMagPrefab;

    private GameManager GM;

    private bool leftHandInZone = false;
    private bool rightHandInZone = false;

    private void Start()
    {
        gripRightAction.action.Enable();
        gripLeftAction.action.Enable();
    }

    private void Update()
    {
        if (GM == null)
        {
            GM = FindFirstObjectByType<GameManager>();
        }

        if (GM == null || GM.Magcount <= 0)
            return;

        if (rightHandInZone && gripRightAction.action.WasPressedThisFrame())
        {
            CmdRequestMag("Right");
        }

        if (leftHandInZone && gripLeftAction.action.WasPressedThisFrame())
        {
            CmdRequestMag("Left");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name.Contains("Left"))
        {
            leftHandInZone = true;
        }

        if (other.name.Contains("Right"))
        {
            rightHandInZone = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name.Contains("Left"))
        {
            leftHandInZone = false;
        }

        if (other.name.Contains("Right"))
        {
            rightHandInZone = false;
        }
    }

    [Command]
    private void CmdRequestMag(string hand)
    {
        if (GM == null || GM.Magcount <= 0)
            return;

        Transform spawnPoint = hand == "Right" ? rightSpawnPos : leftSpawnPos;
        GameObject prefabToSpawn = SelectMagPrefab(hand);

        GameObject magInstance = Instantiate(prefabToSpawn, spawnPoint.position, spawnPoint.rotation);
        NetworkServer.Spawn(magInstance);

        GM.Magcount--;

        RpcForceGrab(magInstance.GetComponent<NetworkIdentity>().netId, hand);
    }

    private GameObject SelectMagPrefab(string hand)
    {
        BaseGun heldGun = null;

        if (hand == "Right" && rightHandDetector != null)
            heldGun = rightHandDetector.currentGun;
        else if (hand == "Left" && leftHandDetector != null)
            heldGun = leftHandDetector.currentGun;

        if (heldGun != null && heldGun.firingType == FiringType.Pistol)
        {
            return pistolMagPrefab;
        }

        return rifleMagPrefab;
    }

    [ClientRpc]
    private void RpcForceGrab(uint netId, string hand)
    {
        NetworkIdentity obj = NetworkClient.spawned[netId];
        if (obj == null) return;

        Grabbable grabbable = obj.GetComponent<Grabbable>();
        if (grabbable == null) return;

        Grabber targetGrabber = hand == "Right" ? rightGrabber : leftGrabber;

        StartCoroutine(DelayedGrab(grabbable, targetGrabber));
    }

    private IEnumerator DelayedGrab(Grabbable grabbable, Grabber grabber)
    {
        yield return null;
        grabber.GrabGrabbable(grabbable);
    }
}
