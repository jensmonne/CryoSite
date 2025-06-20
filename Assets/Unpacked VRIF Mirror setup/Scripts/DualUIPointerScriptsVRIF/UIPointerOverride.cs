using BNG;

public class UIPointerOverride : UIPointer
{
    public override void UpdatePointer()
    {
        base.UpdatePointer();

        if (lineRenderer != null)
        {
            lineRenderer = null;
        }
    }

    public override void HidePointer()
    {
        return;
    }
}
