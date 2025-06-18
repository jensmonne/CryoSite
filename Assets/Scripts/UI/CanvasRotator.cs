using UnityEngine;

public class CanvasRotator : MonoBehaviour
{
    [SerializeField] private GameObject canvas;
    [SerializeField] private GameObject centerEyeAnchor;

    [SerializeField] private float followSpeed = 2f;
    [SerializeField] private float distanceFromCamera = 2f;
    [SerializeField] private float heightOffset = 0.0f;

    private void Start()
    {
        if (centerEyeAnchor) return;
        centerEyeAnchor = GameObject.Find("CenterEyeAnchor");
    }
    
    private void LateUpdate()
    {
        Vector3 targetPosition = centerEyeAnchor.transform.position + centerEyeAnchor.transform.forward * distanceFromCamera;
        targetPosition.y += heightOffset;

        canvas.transform.position = Vector3.Lerp(canvas.transform.position, targetPosition, Time.deltaTime * followSpeed);

        Quaternion targetRotation = Quaternion.LookRotation(canvas.transform.position - centerEyeAnchor.transform.position);
        canvas.transform.rotation = Quaternion.Slerp(canvas.transform.rotation, targetRotation, Time.deltaTime * followSpeed);
    }
}
