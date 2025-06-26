using BNG;

namespace VRIF_Mirror_Package.Scripts.DualUIPointerScriptsVRIF
{
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
}
