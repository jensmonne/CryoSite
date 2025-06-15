using BNG;
using UnityEngine;
using UnityEngine.InputSystem;

public enum FiringType
{
    Semi,
    Automatic
}

public class BaseGun : MonoBehaviour
{
    [SerializeField] private FiringType firingType = FiringType.Semi;
    
    [SerializeField] Transform muzzleTransform;
    [SerializeField] float fireRate;
    [SerializeField] private int range;
    [SerializeField] int damageAmount;
    
    [SerializeField] Slider slide;

    [SerializeField] ParticleSystem muzzleFlash;
    [SerializeField] private AudioSource ShootSound;
    
    public PlayerInput playerInput;
    private InputAction FireAction;
    

    [SerializeField] private SnapZone magSnapZone;
    private Magazine magazine;
    private bool triggerpulled = false;
    private bool previousTriggerPulled = false;

    private float lastFireTime;

    private Grabbable grabbable;

    private void Awake()
    {
        FireAction = playerInput.actions["Fire"];
    }

    void OnEnable()
    {
        FireAction.Enable();
    }
    
    void OnDisable()
    {
        
        FireAction.Disable();
    }
    private void Start()
    {
        grabbable = GetComponent<Grabbable>();
        magSnapZone.OnSnapEvent.AddListener(CheckMagzineSocket);
    }

    void Update()
    {
        if (grabbable && grabbable.BeingHeld)
        {
            float triggerValue = FireAction.ReadValue<float>();
            bool isTriggerPulled = triggerValue > 0.5f;

            switch (firingType)
            {
                case  FiringType.Semi:
                    if (isTriggerPulled && !previousTriggerPulled && Time.time - lastFireTime > fireRate)
                    {
                        TryFire();
                    }
                    break;
                
                case  FiringType.Automatic:
                    if (isTriggerPulled && Time.time - lastFireTime > fireRate)
                    {
                        TryFire();
                    }
                    break;
            }
            previousTriggerPulled = isTriggerPulled;
        }
    }

    void TryFire()
    {
        Debug.Log("Firing!");
        if (magazine == null || magazine.currentAmmo <= 0)
        {
            return;
        }

        Fire();
        magazine.consumeAmmo();
        lastFireTime = Time.time;
        
        if (slide != null)
        {
            slide.OnFired(); 
        }
    }

    void Fire()
    {
        muzzleFlash.Play();
        ShootSound.Play();
        
        Ray ray = new Ray(muzzleTransform.position, muzzleTransform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, range))
        {
            Health health = hit.collider.gameObject.GetComponent<Health>();
            health.TakeDamage(damageAmount);
        }
    }

    void CheckMagzineSocket(Grabbable mag)
    {
        if (magSnapZone != null)
        {
           magazine = mag.GetComponent<Magazine>(); 
        }
        else
        {
            magazine = null;
        }
    }
    
    private void OnDrawGizmos()
    {
        if (muzzleTransform != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(muzzleTransform.position, muzzleTransform.forward * range);
        }
    }
    
}
