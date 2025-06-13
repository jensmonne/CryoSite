using UnityEngine;

public class BigKaboomSnap : MonoBehaviour
{
    public void OnBigKaboomSnap()
    {
        Debug.Log("BigKaboom has snapped");
        HandUIStuff.BigKaboomTimeStarter();
    }
}
