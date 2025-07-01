using UnityEngine;
using BNG;
using UnityEngine.InputSystem;

public class MagazineEject : MonoBehaviour
{
    public SnapZone magSnapZone;
    public ControllerHand ejectHand = ControllerHand.Left;
    public float gripThreshold = 0.9f;

    private PlayerInput playerInput;
    private InputAction ejectMagAction;
    public Magazine mag;

    private bool gripWasPressed = false;

    private void Start()
    {
        // Find the first PlayerInput in the scene (or modify to find a specific one)
        playerInput = FindObjectOfType<PlayerInput>();

        if (playerInput == null)
        {
            Debug.LogError("No PlayerInput found in scene!");
            return;
        }

        ejectMagAction = playerInput.actions["EjectMag"];

        if (ejectMagAction == null)
        {
            Debug.LogError("'EjectMag' action not found on PlayerInput!");
            return;
        }

        ejectMagAction.Enable();
    }

    private void OnEnable()
    {
        if (ejectMagAction != null)
            ejectMagAction.Enable();
    }

    private void OnDisable()
    {
        if (ejectMagAction != null)
            ejectMagAction.Disable();
    }

    private void Update()
    {
        if (mag == null)
            mag = magSnapZone.GetComponentInChildren<Magazine>();

        if (ejectMagAction == null)
            return;

        bool ejectButtonPressed = ejectMagAction.IsPressed();
        float gripValue = GetGripValue();

        bool gripPressedThisFrame = gripValue > gripThreshold && !gripWasPressed;
        gripWasPressed = gripValue > gripThreshold;

        if (ejectButtonPressed)
        {
            if (mag != null)
            {
                mag.EjectMag();
            }
            TryEjectMagazine();
        }
    }

    private float GetGripValue()
    {
        switch (ejectHand)
        {
            case ControllerHand.Left:
                return InputBridge.Instance.LeftGrip;
            case ControllerHand.Right:
                return InputBridge.Instance.RightGrip;
            default:
                return 0f;
        }
    }

    private void TryEjectMagazine()
    {
        if (magSnapZone == null)
        {
            Debug.LogWarning("Magazine SnapZone not assigned.");
            return;
        }

        Grabbable grabbedMag = magSnapZone.HeldItem;

        if (grabbedMag != null)
        {
            magSnapZone.ReleaseAll();

            Rigidbody rb = grabbedMag.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(transform.right * 3f + transform.up * 0.5f, ForceMode.Impulse);
            }

            Debug.Log("Magazine ejected.");
        }
    }
}
