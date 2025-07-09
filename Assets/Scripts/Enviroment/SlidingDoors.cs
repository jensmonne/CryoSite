using System.Collections;
using UnityEngine;

public class SlidingDoors : MonoBehaviour, IUnlockableDoor
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

    public void Unlock()
    {
        canAutoOpen = true;
    }
    
    private void Start()
    {
        leftClosedPos = leftDoor.localPosition;
        rightClosedPos = rightDoor.localPosition;
        
        leftOpenPos = leftClosedPos + leftDoor.forward * slideDistance;
        rightOpenPos = rightClosedPos - leftDoor.forward * slideDistance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!canAutoOpen) return;
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        moveCoroutine = StartCoroutine(MoveDoors(leftOpenPos, rightOpenPos));
        doorSound.Play();
    }

    private void OnTriggerExit(Collider other)
    {
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        moveCoroutine = StartCoroutine(MoveDoors(leftClosedPos, rightClosedPos));
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
}
