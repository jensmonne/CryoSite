using BNG;
using UnityEngine;
using UnityEngine.UI;

namespace VRIF_Mirror_Package.Scripts.DualUIPointerScriptsVRIF
{
    public class PointerSwitching : MonoBehaviour
    {
        VRUISystem vRUI;

        [Header("Visuals")]
        public LineRenderer LaserLine;

        Color origLaserLineStartColor;
        Color origLaserLineEndColor;

        public bool useDynamicGraphics = true;

        [Tooltip("If specified will show this object at the end of the raycast")]
        public GameObject Cursor;
        Color origCursorColor;
        [Tooltip("If true the cursor object will scale based on how far away the pointer is from the origin. A cursor far away will have a larger cusor than one up close.")]
        public bool CursorScaling = true;

        [Tooltip("Minimum scale of the Cursor object if CursorScaling is enabled")]
        public float CursorMinScale = 0.6f;
        public float CursorMaxScale = 6.0f;
        private Vector3 _cursorInitialLocalScale;

        [Header("Input")]
        [Tooltip("Input determined when a button is held down or not. Should be RightTrigger or LeftTrigger (not to be confused with 'RightTriggerDown').")]
        public ControllerBinding ButtonInput = ControllerBinding.RightTrigger;

        private Ray _ray;
        private Vector3 _hitPosition;

        public bool hitFound = false;

        public bool useThisHand = true;
        public ControllerHand HandSide;

        public PointerSwitching otherUIPointer;

        public LayerMask uiLayerMask; // Assign the layer mask to filter UI layers

        [SerializeField]
        private ControllerBinding leftTriggerBinding = ControllerBinding.LeftTrigger;
        [SerializeField]
        private ControllerBinding rightTriggerBinding = ControllerBinding.RightTrigger;
    
        private void Start()
        {
            vRUI = VRUISystem.Instance;

            // store the local scale of the curser to be used scale it later
            if (Cursor)
            {
                _cursorInitialLocalScale = transform.localScale;
                origCursorColor = Cursor.GetComponentInChildren<Image>().color;
            }

            // get the LineRender component
            if (LaserLine == null)
            {
                LaserLine = GetComponent<LineRenderer>();
                origLaserLineStartColor = LaserLine.startColor;
                origLaserLineEndColor = LaserLine.endColor;
            }
        }

        private void Update()
        {
            DoGFXRaycast();
        }

        private void DoGFXRaycast()
        {
            // choose the pointer to use with triggers if both pointers are on the canvas
            if (otherUIPointer.hitFound && hitFound)
            {
                if (HandSide == ControllerHand.Right && InputBridge.Instance.RightTriggerDown)
                {
                    useThisHand = true;
                }
                else if (HandSide == ControllerHand.Left && InputBridge.Instance.RightTriggerDown)
                {
                    useThisHand = false;
                }

                if (HandSide == ControllerHand.Left && InputBridge.Instance.LeftTriggerDown)
                {
                    useThisHand = true;
                }
                else if (HandSide == ControllerHand.Right && InputBridge.Instance.LeftTriggerDown)
                {
                    useThisHand = false;

                }
            }
            // choose pointer to use if only one pointer is on the canvas, so if you have both pointed at the canvas and one leaves, the other pointer automatically becomes the pointer to be used
            else if (!otherUIPointer.hitFound && hitFound)
            {
                useThisHand = true;
                otherUIPointer.useThisHand = false;
            }

            // set VRUISystem to this hand if it isn't already the selected hand
            if (useThisHand && vRUI.SelectedHand != HandSide)
            {
                vRUI.SelectedHand = HandSide;
                vRUI.UpdateControllerHand(HandSide);
                if (HandSide == ControllerHand.Left)
                {
                    vRUI.ControllerInput[0] = leftTriggerBinding;
                }
                else if (HandSide == ControllerHand.Right)
                {
                    vRUI.ControllerInput[0] = rightTriggerBinding;
                }
                vRUI.ClearAll();

            }
            // update the Visuals of the cursor and line renderer
            UpdateVisuals();

        }

        // Draw a LineRenderer between our start point and if we hit a Canvas
        // this requires a collider to be just behind the canvas as it uses physics raycast to find the hit
        public void UpdateVisuals()
        {
            _ray = new Ray(transform.position, transform.forward);

            //make laser beam hit stuff it points at.
            if (LaserLine)
            {
                // Change the laser's length depending on where it hits
                hitFound = false;
                _hitPosition = transform.position;

                RaycastHit hit;

                if (Physics.Raycast(_ray, out hit, 10000f, uiLayerMask))
                {
                    _hitPosition = hit.point;

                    // Find if we hit a canvas
                    GraphicRaycaster graphicRaycaster = hit.collider.GetComponent<GraphicRaycaster>();

                    if (graphicRaycaster != null)
                    {
                        hitFound = true;
                    }
                    else
                    {
                        hitFound = false;
                    }
                }

                // Update the LineRenderer
                if (hitFound)
                {
                    if (LaserLine)
                    {
                        LaserLine.enabled = true;

                        float lineDistance = Vector3.Distance(transform.position, _hitPosition);
                        LaserLine.useWorldSpace = false;
                        LaserLine.SetPosition(0, Vector3.zero);
                        LaserLine.SetPosition(1, new Vector3(0, 0, lineDistance));

                        // dynamically change the color if the PointerDynamicColor component is on the UI element, for example, on a button
                        if (useThisHand && useDynamicGraphics)
                        {
                            if (vRUI.ReleasingObject != null)
                            {
                                PointerDynamicColor DynamicColor = vRUI.ReleasingObject.GetComponent<PointerDynamicColor>();

                                if (DynamicColor)
                                {
                                    LaserLine.startColor = DynamicColor.lineStartColor;
                                    LaserLine.endColor = DynamicColor.lineEndColor;
                                }
                            }
                            else if (vRUI.ReleasingObject == null)
                            {
                                LaserLine.startColor = origLaserLineStartColor;
                                LaserLine.endColor = origLaserLineEndColor;
                            }
                        }


                        // not sure why you would need world space, but here it is if you do
                        //LaserLine.useWorldSpace = true;
                        // LaserLine.SetPosition(0, transform.position);
                        // LaserLine.SetPosition(1, _hitPosition);
                    }

                    if (Cursor)
                    {
                        Cursor.SetActive(true);
                        // position the cursor
                        Cursor.transform.position = _hitPosition;
                        // Scale cursor based on distance from main camera
                        float cameraDist = Vector3.Distance(Camera.main.transform.position, Cursor.transform.position);
                        Cursor.transform.localScale = _cursorInitialLocalScale * Mathf.Clamp(cameraDist, CursorMinScale, CursorMaxScale);
                        // rotate curser to face correctly
                        Cursor.transform.rotation = Quaternion.FromToRotation(Vector3.forward, hit.normal);

                        if (useThisHand && useDynamicGraphics)
                        {
                            if (vRUI.ReleasingObject != null)
                            {
                                PointerDynamicColor dynamicColor = vRUI.ReleasingObject.GetComponent<PointerDynamicColor>();

                                if (dynamicColor)
                                {
                                    Cursor.GetComponentInChildren<Image>().color = dynamicColor.cursorColor;
                                }
                            }
                            else if (vRUI.ReleasingObject == null)
                            {
                                Cursor.GetComponentInChildren<Image>().color = origCursorColor;
                            }
                        }
                    }
                }

                // disable the laser and cursor
                else
                {
                    LaserLine.enabled = false;

                    if (Cursor)
                    {
                        Cursor.SetActive(false);
                    }
                }
            }
        }
    }
}
