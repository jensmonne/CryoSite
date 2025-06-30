using UnityEngine;
using BNG; // For Grabbable

public class SlideLockController : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The ConfigurableJoint attached to the slide.")]
    [SerializeField] private ConfigurableJoint slideJoint;

    [Tooltip("The Grabbable script on the slide.")]
    [SerializeField] private Grabbable slideGrabbable;

    private bool wasBeingHeld = false;

    private void Update()
    {
        if (slideGrabbable == null || slideJoint == null)
            return;

        bool isHeld = slideGrabbable.BeingHeld;

        // Only update if state has changed
        if (isHeld != wasBeingHeld)
        {
            if (isHeld)
            {
                // Unlock Z so it can slide
                slideJoint.zMotion = ConfigurableJointMotion.Limited;
            }
            else
            {
                // Lock Z so it cannot move when unheld
                slideJoint.zMotion = ConfigurableJointMotion.Locked;
            }

            wasBeingHeld = isHeld;
        }
    }
}