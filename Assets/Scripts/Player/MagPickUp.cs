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

    [SerializeField] private bool LeftHandInZone = false;
    [SerializeField] private bool RightHandInZone = false;

    [SerializeField] private GameManager GM;

    public InputActionReference gripRightAction;
    public InputActionReference gripLeftAction;

    [SerializeField] private GameObject magPrefab;
    [SerializeField] private Transform leftSpawnPos;
    [SerializeField] private Transform rightSpawnPos;

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

        Transform handTransform = hand == "Right" ? righthand : lefthand;
        Grabber grabber = hand == "Right" ? rightGrabber : leftGrabber;
        Transform spawnPoint = hand == "Right" ? rightSpawnPos : leftSpawnPos;

        GameObject magInstance = Instantiate(magPrefab, spawnPoint.position, spawnPoint.rotation);

        Grabbable grabbable = magInstance.GetComponent<Grabbable>();
        if (grabbable == null)
        {
            Debug.LogError("Spawned mag prefab is missing Grabbable component!");
            Destroy(magInstance);
            return;
        }

        GM.Magcount--;

        StartCoroutine(ForceGrab(grabbable, grabber));
    }

    private IEnumerator ForceGrab(Grabbable grabbable, Grabber grabber)
    {
        yield return null;
        grabber.GrabGrabbable(grabbable);
    }
}
