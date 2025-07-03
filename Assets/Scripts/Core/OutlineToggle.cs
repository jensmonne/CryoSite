using UnityEngine;
using BNG;

public class OutlineToggle : MonoBehaviour
{
    public string outlineLayerName = "OutlineObjects";
    public string noHitPlayerLayerName = "Nohitplayer";

    private int outlineLayer;
    private int noHitPlayerLayer;

    private Grabbable grabbable;

    private bool wasHeldLastFrame = false;

    void Awake()
    {
        outlineLayer = LayerMask.NameToLayer(outlineLayerName);
        noHitPlayerLayer = LayerMask.NameToLayer(noHitPlayerLayerName);

        grabbable = GetComponent<Grabbable>();
    }

    void Update()
    {
        if (grabbable == null) return;

        bool isHeld = grabbable.BeingHeld;

        if (isHeld && !wasHeldLastFrame)
        {
            SetLayerRecursively(gameObject, noHitPlayerLayer);
        }
        else if (!isHeld && wasHeldLastFrame)
        {
            SetLayerRecursively(gameObject, outlineLayer);
        }

        wasHeldLastFrame = isHeld;
    }

    void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }
}