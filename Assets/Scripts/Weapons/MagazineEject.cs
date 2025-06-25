using System;
using UnityEngine;
using BNG;
using UnityEngine.InputSystem;

public class MagazineEject : MonoBehaviour
{
    public SnapZone magSnapZone; 
    public ControllerHand ejectHand = ControllerHand.Left;
    public float gripThreshold = 0.9f;
    public PlayerInput playerInput;
    private InputAction EjectMag;
    public Magazine mag;

    private bool gripWasPressed = false;

    private void Awake()
    {
        EjectMag = playerInput.actions["EjectMag"];
    }
    
    void OnEnable()
    {
        EjectMag.Enable();
    }
    
    void OnDisable()
    {
        
        EjectMag.Disable();
    }

    void Update() {
        if (mag == null)
        {
            mag = magSnapZone.GetComponentInChildren<Magazine>();
        }
        bool ejectButtonPressed = EjectMag.IsPressed();
        float gripValue = GetGripValue();

        // Check grip rising edge
        bool gripPressedThisFrame = gripValue > gripThreshold && !gripWasPressed;

        gripWasPressed = gripValue > gripThreshold;

        if (ejectButtonPressed) {
            mag.EjectMag();
            TryEjectMagazine();
        }
    }
    
    float GetGripValue() {
        switch (ejectHand) {
            case ControllerHand.Left:
                return InputBridge.Instance.LeftGrip;
            case ControllerHand.Right:
                return InputBridge.Instance.RightGrip;
            default:
                return 0f;
        }
    }

    void TryEjectMagazine() {
        if (magSnapZone == null) {
            Debug.LogWarning("Magazine SnapZone not assigned.");
            return;
        }

        Grabbable mag = magSnapZone.HeldItem;

        if (mag != null) {
            magSnapZone.ReleaseAll();

            Rigidbody rb = mag.GetComponent<Rigidbody>();
            if (rb != null) {
                rb.AddForce(transform.right * 3f + transform.up * 0.5f, ForceMode.Impulse);
            }

            Debug.Log("Magazine ejected.");
        }
    }
}
