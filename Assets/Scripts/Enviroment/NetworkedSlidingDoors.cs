using System.Collections;
using Mirror;
using UnityEngine;

public class NetworkedSlidingDoors : NetworkBehaviour, IUnlockableDoor
{
    [SerializeField] public bool canAutoOpen;
    [SerializeField] private Transform leftDoor;
    [SerializeField] private Transform rightDoor;
    [SerializeField] private AudioSource doorSound;
    private float slideDistance = 0.013f;
    private float slideSpeed = 0.01f;
    
    private Vector3 leftClosedPos;
    private Vector3 rightClosedPos;
    private Vector3 leftOpenPos;
    private Vector3 rightOpenPos;
    private Coroutine moveCoroutine;

    public void Unlock() => canAutoOpen = true;
    
    private void Start()
    {
        leftClosedPos = leftDoor.localPosition;
        rightClosedPos = rightDoor.localPosition;
        
        leftOpenPos = leftClosedPos - leftDoor.forward * slideDistance;
        rightOpenPos = rightClosedPos + leftDoor.forward * slideDistance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!canAutoOpen) return;
        
        if (!isServer)
        {
            // Client with authority triggers request
            CmdRequestOpen();
        }
        else
        {
            // Server opens directly (host or authoritative server)
            OpenDoor();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!isServer)
        {
            // Client with authority triggers request
            CmdRequestClose();
        }
        else
        {
            // Server opens directly (host or authoritative server)
            CloseDoor();
        }
    }
    
    [Command(requiresAuthority = false)]
    private void CmdRequestOpen()
    {
        OpenDoor(); // Server executes
    }
    
    [Command(requiresAuthority = false)]
    private void CmdRequestClose()
    {
        CloseDoor(); // Server executes
    }
    
    [Server]
    private void OpenDoor()
    {
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        moveCoroutine = StartCoroutine(MoveDoors(leftOpenPos, rightOpenPos));
        RpcPlayDoorSound();
    }
    
    [Server]
    private void CloseDoor()
    {
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        moveCoroutine = StartCoroutine(MoveDoors(leftClosedPos, rightClosedPos));
    }
    
    [ClientRpc]
    private void RpcPlayDoorSound() => doorSound.Play();
    
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
}
