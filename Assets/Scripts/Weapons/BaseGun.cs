using BNG;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public enum FiringType
{
    Pistol,
    Rifle,
    Shotgun
}

public class BaseGun : MonoBehaviour
{
    [Header("General Settings")]
    [Tooltip("Select the firing type for this gun.")]
    [SerializeField] private FiringType firingType = FiringType.Pistol;

    [Tooltip("Point where bullets originate.")]
    [SerializeField] private Transform muzzleTransform;

    [Tooltip("Time between shots.")]
    [SerializeField] private float fireRate = 0.2f;

    [Tooltip("Maximum shooting distance.")]
    [SerializeField] private int range = 50;

    [Tooltip("Damage dealt per hit.")]
    [SerializeField] private int damageAmount = 10;

    [Space(10)]
    [Header("Shotgun Settings")]
    [Tooltip("Number of pellets per shotgun blast.")]
    [SerializeField] private int shotgunPelletCount = 8;

    [Tooltip("Spread angle of shotgun pellets.")]
    [SerializeField] private float shotgunSpreadAngle = 5f;

    [Tooltip("Slide transform (pump action).")]
    [SerializeField] private Transform slideTransform;

    [Tooltip("Local Z position when slide is fully forward.")]
    [SerializeField] private float slideForwardPosition = 0f;

    [Tooltip("Distance slide must be pulled back to re-cock.")]
    [SerializeField] private float slidePullThreshold = 0.15f;

    [Space(10)]
    [Header("Gun Components")]
    [Tooltip("Optional slide animation reference.")]
    [SerializeField] private Slider slide;

    [Tooltip("Muzzle flash particle system.")]
    [SerializeField] private ParticleSystem muzzleFlash;

    [Tooltip("Audio source for gun shot.")]
    [SerializeField] private AudioSource ShootSound;

    [Space(10)]
    [Header("Input")]
    [Tooltip("Player input reference.")]
    public PlayerInput playerInput;

    [Space(10)]
    [Header("Magazine Settings")]
    [Tooltip("Snap zone for magazine insertion.")]
    [SerializeField] private SnapZone magSnapZone;

    [Tooltip("Current magazine.")]
    public Magazine magazine;

    [Space(10)]
    [Header("UI")]
    [Tooltip("Text displaying ammo count.")]
    public TextMeshProUGUI AmmoText;

    [Space(10)]
    [Header("Layers")]
    [Tooltip("Layer mask for enemies.")]
    [SerializeField] private LayerMask enemyLayerMask;

    [Tooltip("Layer mask for walls.")]
    [SerializeField] private LayerMask wallsLayerMask;

    [Space(10)]
    [Header("Debug")]
    [Tooltip("Prefab to show hit marker.")]
    [SerializeField] private GameObject hitMarkerPrefab;

    private InputAction fireAction;
    private bool previousTriggerPulled = false;
    private float lastFireTime;
    private bool isCocked = true;
    private Grabbable grabbable;
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
                case FiringType.Pistol:
                    if (isTriggerPulled && !previousTriggerPulled && Time.time - lastFireTime > fireRate)
                        TryFire();
                    break;

                case FiringType.Rifle:
                    if (isTriggerPulled && Time.time - lastFireTime > fireRate)
                        TryFire();
                    break;

                case FiringType.Shotgun:
                    if (isTriggerPulled && !previousTriggerPulled && Time.time - lastFireTime > fireRate)
                        TryShotgunFire();
                    break;
            }

            previousTriggerPulled = isTriggerPulled;
        }

        if (firingType == FiringType.Shotgun && slideTransform != null)
        {
            float slideZ = slideTransform.localPosition.z;
            if (slideZ <= (slideForwardPosition - slidePullThreshold))
            {
                isCocked = true;
            }
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

    private void TryShotgunFire()
    {
        if (!magazine || magazine.currentAmmo <= 0) return;
        if (!isCocked) return;

        FireShotgun();
        magazine.consumeAmmo();
        lastFireTime = Time.time;
        isCocked = false;

        if (slide) slide.OnFired();
    }

    private void Fire()
    {
        muzzleFlash.Play();
        ShootSound.Play();

        Ray ray = new Ray(muzzleTransform.position, muzzleTransform.forward);
        Vector3 endPoint = muzzleTransform.position + muzzleTransform.forward * range;

        if (Physics.Raycast(ray, out RaycastHit hit, range, enemyLayerMask))
        {
            endPoint = hit.point;
            Health health = hit.collider.GetComponent<Health>();
            if (health) health.TakeDamage(damageAmount);

            BossHealth bossHealth = hit.collider.GetComponent<BossHealth>();
            if (bossHealth) bossHealth.TakeDamage(damageAmount);
        }
        else if (Physics.Raycast(ray, out RaycastHit hits, range, wallsLayerMask))
        {
            endPoint = hits.point;
        }

        UpdateDebugRay(endPoint);
    }

    private void FireShotgun()
    {
        muzzleFlash.Play();
        ShootSound.Play();

        for (int i = 0; i < shotgunPelletCount; i++)
        {
            Vector3 dir = GetShotgunPelletDirection();
            Ray ray = new Ray(muzzleTransform.position, dir);
            Vector3 endPoint = muzzleTransform.position + dir * range;

            if (Physics.Raycast(ray, out RaycastHit hit, range, enemyLayerMask))
            {
                endPoint = hit.point;
                Health health = hit.collider.GetComponent<Health>();
                if (health) health.TakeDamage(damageAmount);

                BossHealth bossHealth = hit.collider.GetComponent<BossHealth>();
                if (bossHealth) bossHealth.TakeDamage(damageAmount);
            }
            else if (Physics.Raycast(ray, out RaycastHit hits, range, wallsLayerMask))
            {
                endPoint = hits.point;
            }

            UpdateDebugRay(endPoint);
        }
    }

    private Vector3 GetShotgunPelletDirection()
    {
        float yaw = Random.Range(-shotgunSpreadAngle, shotgunSpreadAngle);
        float pitch = Random.Range(-shotgunSpreadAngle, shotgunSpreadAngle);
        return Quaternion.Euler(pitch, yaw, 0) * muzzleTransform.forward;
    }

    private void CheckMagazineSocket(Grabbable mag)
    {
        if (magSnapZone != null)
            magazine = mag.GetComponent<Magazine>();
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

        if (firingType == FiringType.Shotgun)
        {
            Gizmos.color = Color.yellow;
            for (int i = 0; i < shotgunPelletCount; i++)
            {
                Vector3 dir = GetShotgunPelletDirection();
                Gizmos.DrawRay(muzzleTransform.position, dir * range);
            }
        }
    }
}
