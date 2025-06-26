using BNG;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

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
    private InputAction fireAction;
    
    [SerializeField] private SnapZone magSnapZone;
    public Magazine magazine;
    private bool triggerpulled = false;
    private bool previousTriggerPulled = false;
    private float lastFireTime;

    private Grabbable grabbable;
    
    public TextMeshProUGUI AmmoText;
    
    [SerializeField] private LayerMask enemyLayerMask;
    [SerializeField] private LayerMask wallsLayerMask;
    
    // Debug line for check
    [SerializeField] private GameObject hitMarkerPrefab;

    private GameObject activeHitMarker;

    private void Awake()
    {
        fireAction = playerInput.actions["Fire"];
    }

    private void OnEnable()
    {
        fireAction.Enable();
    }
    
    private void OnDisable()
    {
        
        fireAction.Disable();
    }
    
    private void Start()
    {
        grabbable = GetComponent<Grabbable>();
        magSnapZone.OnSnapEvent.AddListener(CheckMagazineSocket);
    }

    private void Update()
    {
        if (grabbable && grabbable.BeingHeld)
        {
            float triggerValue = fireAction.ReadValue<float>();
            bool isTriggerPulled = triggerValue > 0.5f;

            switch (firingType)
            {
                case  FiringType.Semi:
                    if (isTriggerPulled && !previousTriggerPulled && Time.time - lastFireTime > fireRate) TryFire();
                    break;
                
                case  FiringType.Automatic:
                    if (isTriggerPulled && Time.time - lastFireTime > fireRate) TryFire();
                    break;
            }
            previousTriggerPulled = isTriggerPulled;
        }

        AmmoText.text = magazine ? magazine.currentAmmo.ToString() : "No Mag";
    }

    private void TryFire()
    {
        if (!magazine || magazine.currentAmmo <= 0) return;

        Fire();
        magazine.consumeAmmo();
        lastFireTime = Time.time;
        
        if (slide) slide.OnFired(); 
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void Fire()
    {
        muzzleFlash.Play();
        ShootSound.Play();

        Ray ray = new Ray(muzzleTransform.position, muzzleTransform.forward);
        Vector3 endPoint = muzzleTransform.position + muzzleTransform.forward * range;

        if (Physics.Raycast(ray, out RaycastHit hit, range, enemyLayerMask))
        {
            endPoint = hit.point;

            Health health = hit.collider.gameObject.GetComponent<Health>();
            if (health) health.TakeDamage(damageAmount);
            
            BossHealth bossHealth = hit.collider.gameObject.GetComponent<BossHealth>();
            if (bossHealth) bossHealth.TakeDamage(damageAmount);
            
        } else if (Physics.Raycast(ray, out RaycastHit hits, range, wallsLayerMask))
        {
            endPoint = hits.point;
        }

        UpdateDebugRay(endPoint);
    }

    private void CheckMagazineSocket(Grabbable mag)
    {
        if (magSnapZone != null) magazine = mag.GetComponent<Magazine>(); 
    }
    
    private void UpdateDebugRay(Vector3 end)
    {
        if (hitMarkerPrefab == null) return;
        if (activeHitMarker != null) Destroy(activeHitMarker);

        activeHitMarker = Instantiate(hitMarkerPrefab, end, Quaternion.identity);
    }
    
    private void OnDrawGizmos()
    {
        if (muzzleTransform == null) return;
        
        Gizmos.color = Color.red;
        Gizmos.DrawRay(muzzleTransform.position, muzzleTransform.forward * range);
    }
    
}
