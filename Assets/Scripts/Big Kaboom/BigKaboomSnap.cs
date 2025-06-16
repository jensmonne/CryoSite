using UnityEngine;

public class BigKaboomSnap : MonoBehaviour
{
    [SerializeField] private AudioSource beepingSource;
    
    public void OnBigKaboomSnap()
    {
        Debug.Log("BigKaboom has snapped");

        GameManager.IsBombActive = true;
        
        beepingSource.Play();
    }
}
