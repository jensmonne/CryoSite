using UnityEngine;

public class ImageFollowPlayer : MonoBehaviour
{
    [SerializeField] private Transform targetImage;
    private Transform playerTransform;

    void Update()
    {
        if (playerTransform == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
            {
                playerTransform = playerObj.transform;
            }
        }
        
        if (playerTransform != null && targetImage != null)
        {
            targetImage.LookAt(playerTransform);

        }
    }
}