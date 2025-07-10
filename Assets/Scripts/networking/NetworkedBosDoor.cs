using System.Collections;
using UnityEngine;
using Mirror;

public class NetworkedBosDoor : NetworkBehaviour
{
    [SerializeField] private Transform leftDoor;
    [SerializeField] private Transform rightDoor;

    private float slideDistance = 0.013f;
    private float slideSpeed = 0.01f;

    private Vector3 leftClosedPos;
    private Vector3 rightClosedPos;
    private Vector3 leftOpenPos;
    private Vector3 rightOpenPos;

    private Coroutine moveCoroutine;

    [SyncVar(hook = nameof(OnCanAutoOpenChanged))]
    public bool canAutoOpen = false;

    [SyncVar(hook = nameof(OnDoorStateChanged))]
    private bool isOpen = false;

    private void Awake()
    {
        leftClosedPos = leftDoor.localPosition;
        rightClosedPos = rightDoor.localPosition;

        leftOpenPos = leftClosedPos + leftDoor.forward * slideDistance;
        rightOpenPos = rightClosedPos - leftDoor.forward * slideDistance;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        SetDoorState(isOpen); // Ensure correct door state on client start
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isServer || !canAutoOpen) return;

        if (!isOpen)
        {
            isOpen = true; // Will trigger hook
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!isServer) return;

        if (isOpen)
        {
            isOpen = false; // Will trigger hook
        }
    }

    private void SetDoorState(bool open)
    {
        Vector3 leftTarget = open ? leftOpenPos : leftClosedPos;
        Vector3 rightTarget = open ? rightOpenPos : rightClosedPos;

        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        moveCoroutine = StartCoroutine(MoveDoors(leftTarget, rightTarget));
    }

    private IEnumerator MoveDoors(Vector3 leftTarget, Vector3 rightTarget)
    {
        while (Vector3.Distance(leftDoor.localPosition, leftTarget) > 0.0001f)
        {
            leftDoor.localPosition = Vector3.MoveTowards(leftDoor.localPosition, leftTarget, slideSpeed * Time.deltaTime);
            rightDoor.localPosition = Vector3.MoveTowards(rightDoor.localPosition, rightTarget, slideSpeed * Time.deltaTime);
            yield return null;
        }

        leftDoor.localPosition = leftTarget;
        rightDoor.localPosition = rightTarget;
    }

    private void OnDoorStateChanged(bool oldValue, bool newValue)
    {
        SetDoorState(newValue);
    }

    private void OnCanAutoOpenChanged(bool oldVal, bool newVal)
    {
        // Optional: add VFX/SFX/UI feedback
    }

    [Command(requiresAuthority = false)]
    public void CmdSetCanAutoOpen(bool value)
    {
        canAutoOpen = value;
    }

    [Server]
    public void SetCanAutoOpen(bool value)
    {
        canAutoOpen = value;
    }
}
