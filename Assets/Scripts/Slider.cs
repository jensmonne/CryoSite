using UnityEngine;

public class Slider : MonoBehaviour
{

    public Transform slideTransform;        
    public Vector3 slideLocalOffset = new Vector3(-0.05f, 0, 0); 
    public float slideSpeed = 10f;

    private Vector3 initialLocalPosition;
    private Vector3 targetLocalPosition;
    private bool isRecoiling = false;
    private float slideProgress = 0f;

    void Start()
    {
        if (slideTransform == null)
            slideTransform = transform;

        initialLocalPosition = slideTransform.localPosition;
        targetLocalPosition = initialLocalPosition + slideLocalOffset;
    }

    void Update()
    {
        if (isRecoiling)
        {
            slideProgress += Time.deltaTime * slideSpeed;
            
            float t = Mathf.PingPong(slideProgress, 1f);
            slideTransform.localPosition = Vector3.Lerp(initialLocalPosition, targetLocalPosition, t);


            if (slideProgress >= 2f)
            {
                slideTransform.localPosition = initialLocalPosition;
                isRecoiling = false;
                slideProgress = 0f;
            }
        }
    }
    
    public void OnFired()
    {
        isRecoiling = true;
        slideProgress = 0f;
    }
}

