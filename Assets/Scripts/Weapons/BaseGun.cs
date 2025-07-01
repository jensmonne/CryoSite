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
    public FiringType firingType = FiringType.Pistol;
    [SerializeField] private Transform muzzleTransform;
    [SerializeField] private float fireRate = 0.2f;
    [SerializeField] private int range = 50;
    [SerializeField] private int damageAmount = 10;

    [Space(10)]
    [Header("Shotgun Settings")]
    [SerializeField] private int shotgunPelletCount = 8;
    [SerializeField] private float shotgunSpreadAngle = 5f;
    [SerializeField] private Transform slideTransform;
    [SerializeField] private float slideForwardPosition = 0f;
    [SerializeField] private float slidePullThreshold = 0.15f;

    [Space(10)]
    [Header("Gun Components")]
    [SerializeField] private Slider slide;
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private AudioSource ShootSound;
    [SerializeField] private AudioSource ShotGunSound;

    [Space(10)]
    [Header("Input")]
    private PlayerInput playerInput;
    private InputAction fireAction;
    private InputAction swapFireTypeAction;

    [Space(10)]
    [Header("Magazine Settings")]
    [SerializeField] private SnapZone magSnapZone;
    public Magazine magazine;

    [Space(10)]
    [Header("UI")]
    public TextMeshProUGUI AmmoText;

    [Space(10)]
    [Header("Layers")]
    [SerializeField] private LayerMask enemyLayerMask;
    [SerializeField] private LayerMask wallsLayerMask;

    [Space(10)]
    [Header("Debug")]
    [SerializeField] private GameObject hitMarkerPrefab;

    private bool previousTriggerPulled = false;
    private float lastFireTime;
    private bool isCocked = true;
    private bool slidePulledBack = false;
    private Grabbable grabbable;
    private GameObject activeHitMarker;

    private void Start()
    {
        // Find PlayerInput once (from root or scene)
        playerInput = GetComponentInParent<PlayerInput>();

        if (playerInput == null)
        {
            playerInput = FindObjectOfType<PlayerInput>();
        }

        if (playerInput == null)
        {
            Debug.LogError("PlayerInput not found on BaseGun or parents or scene!");
            return;
        }

        fireAction = playerInput.actions["Fire"];
        if (fireAction == null)
            Debug.LogError("Fire action not found in PlayerInput!");

        swapFireTypeAction = playerInput.actions["SwapFireType"];
        if (swapFireTypeAction == null)
            Debug.LogError("SwapFireType action not found in PlayerInput!");

        swapFireTypeAction.performed += OnSwapFireType;

        fireAction.Enable();
        swapFireTypeAction.Enable();

        grabbable = GetComponent<Grabbable>();
        if (magSnapZone != null)
            magSnapZone.OnSnapEvent.AddListener(CheckMagazineSocket);
    }

    private void OnDisable()
    {
        if (fireAction != null)
            fireAction.Disable();

        if (swapFireTypeAction != null)
            swapFireTypeAction.Disable();
    }

    private void Update()
    {
        if (playerInput == null) return;

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
            Vector3 localSlidePos = transform.InverseTransformPoint(slideTransform.position);

            if (!slidePulledBack && localSlidePos.z <= (slideForwardPosition - slidePullThreshold))
            {
                slidePulledBack = true;
            }
            else if (slidePulledBack && localSlidePos.z >= (slideForwardPosition - (slidePullThreshold * 0.5f)))
            {
                isCocked = true;
                slidePulledBack = false;
            }
        }

        AmmoText.text = magazine ? magazine.currentAmmo.ToString() : "";
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
    }

    private void Fire()
    {
        muzzleFlash?.Play();
        ShootSound?.Play();

        Ray ray = new Ray(muzzleTransform.position, muzzleTransform.forward);
        Vector3 endPoint = muzzleTransform.position + muzzleTransform.forward * range;

        if (Physics.Raycast(ray, out RaycastHit hit, range, enemyLayerMask))
        {
            endPoint = hit.point;
            var health = hit.collider.GetComponent<Health>();
            if (health) health.TakeDamage(damageAmount);

            var bossHealth = hit.collider.GetComponent<BossHealth>();
            if (bossHealth) bossHealth.TakeDamage(damageAmount);
        }
        else if (Physics.Raycast(ray, out RaycastHit wallHit, range, wallsLayerMask))
        {
            endPoint = wallHit.point;
        }

        UpdateDebugRay(endPoint);
    }

    private void FireShotgun()
    {
        muzzleFlash?.Play();
        ShotGunSound?.Play();

        for (int i = 0; i < shotgunPelletCount; i++)
        {
            Vector3 dir = GetShotgunPelletDirection();
            Ray ray = new Ray(muzzleTransform.position, dir);
            Vector3 endPoint = muzzleTransform.position + dir * range;

            if (Physics.Raycast(ray, out RaycastHit hit, range, enemyLayerMask))
            {
                endPoint = hit.point;
                var health = hit.collider.GetComponent<Health>();
                if (health) health.TakeDamage(damageAmount);

                var bossHealth = hit.collider.GetComponent<BossHealth>();
                if (bossHealth) bossHealth.TakeDamage(damageAmount);
            }
            else if (Physics.Raycast(ray, out RaycastHit wallHit, range, wallsLayerMask))
            {
                endPoint = wallHit.point;
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

        if (activeHitMarker != null)
            Destroy(activeHitMarker);

        activeHitMarker = Instantiate(hitMarkerPrefab, end, Quaternion.identity);
    }

    private void OnSwapFireType(InputAction.CallbackContext context)
    {
        if (firingType == FiringType.Pistol)
            return;

        if (firingType == FiringType.Rifle)
        {
            firingType = FiringType.Shotgun;
            damageAmount = 35;
            Debug.Log("Swapped to Shotgun.");
        }
        else if (firingType == FiringType.Shotgun)
        {
            firingType = FiringType.Rifle;
            damageAmount = 10;
            Debug.Log("Swapped to Rifle.");
        }
    }

    private void OnDestroy()
    {
        if (swapFireTypeAction != null)
            swapFireTypeAction.performed -= OnSwapFireType;
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

