using UnityEngine;

public class BigKaboomSnap : MonoBehaviour
{
    [SerializeField] private SphereCollider sphereCollider;
    [SerializeField] private AudioSource beepingSource;

    [SerializeField] private float blastDuration = 5f;
    [SerializeField] private float maxRadius = 600f;

    private float blastTimer = 0f;
    private static bool blastem;
    private bool done = false;

    private void FixedUpdate()
    {
        if (!blastem) return;
        if (!done) sphereCollider.gameObject.SetActive(true); done = true;
        
        blastTimer += Time.fixedDeltaTime;
        float t = Mathf.Clamp01(blastTimer / blastDuration);
            
        float eased = 1f - Mathf.Pow(1f - t, 3f);
        float scaleFactor = transform.lossyScale.x;
        sphereCollider.radius = (eased * maxRadius) / scaleFactor;

        if (t >= 1f)
        {
            blastem = false;
        }
    }
    
    public void OnBigKaboomSnap()
    {
        Debug.Log("BigKaboom has snapped");

        GameManager.IsBombActive = true;
        
        beepingSource.Play();
    }

    public static void ActivateBlast()
    {
        blastem = true;
    }
}
