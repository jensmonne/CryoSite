using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;
using TMPro;

public enum FiringType
{
    Pistol,
    Rifle,
    Shotgun
}

public class BaseGun : NetworkBehaviour
{
    [Header("General Settings")]
    public FiringType firingType = FiringType.Pistol;
    [SerializeField] private Transform muzzleTransform;
    [SerializeField] private float fireRate = 0.2f;
    [SerializeField] private int range = 50;
    [SerializeField] private int damageAmount = 10;

    [Header("Shotgun Settings")]
    [SerializeField] private int shotgunPelletCount = 8;
    [SerializeField] private float shotgunSpreadAngle = 5f;

    [Header("Gun Components")]
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private AudioSource shootSound;
    [SerializeField] private AudioSource shotgunSound;

    [Header("Magazine Settings")]
    public Magazine magazine;

    [Header("UI")]
    public TextMeshProUGUI ammoText;

    [Header("Layers")]
    [SerializeField] private LayerMask enemyLayerMask;

    private PlayerInput playerInput;
    private InputAction fireAction;
    private InputAction swapFireTypeAction;

    private bool previousTriggerPulled = false;
    private float lastFireTime = 0f;
    private bool isCocked = true;

    private void Start()
    {
        Debug.Log($"BaseGun Start. isOwned={isOwned}, netId={netId}");

        if (!isOwned)
        {
            Debug.Log("BaseGun not owned on Start. Disabling script.");
            enabled = false;
            return;
        }

        playerInput = GetComponentInParent<PlayerInput>();
        if (playerInput == null)
        {
            Debug.LogError("PlayerInput not found on owner.");
            enabled = false;
            return;
        }

        fireAction = playerInput.actions["Fire"];
        if (fireAction == null)
        {
            Debug.LogError("Fire action not found.");
            enabled = false;
            return;
        }

        swapFireTypeAction = playerInput.actions["SwapFireType"];
        if (swapFireTypeAction != null)
            swapFireTypeAction.performed += OnSwapFireType;

        fireAction.Enable();
        if (swapFireTypeAction != null)
            swapFireTypeAction.Enable();
    }

    private void OnDisable()
    {
        if (fireAction != null)
            fireAction.Disable();

        if (swapFireTypeAction != null)
        {
            swapFireTypeAction.Disable();
            swapFireTypeAction.performed -= OnSwapFireType;
        }
    }

    private void Update()
    {
        if (!isOwned) return;

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
                if (isTriggerPulled && !previousTriggerPulled && Time.time - lastFireTime > fireRate && isCocked)
                    TryShotgunFire();
                break;
        }

        previousTriggerPulled = isTriggerPulled;

        if (magazine != null && ammoText != null)
            ammoText.text = magazine.currentAmmo.ToString();
    }

    private void TryFire()
    {
        if (magazine == null || magazine.currentAmmo <= 0)
        {
            Debug.Log("Cannot fire: No ammo.");
            return;
        }

        muzzleFlash?.Play();
        shootSound?.Play();

        Ray ray = new Ray(muzzleTransform.position, muzzleTransform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, range, enemyLayerMask))
        {
            Debug.Log($"Hit enemy: {hit.collider.gameObject.name}");
            CmdShootEnemy(hit.collider.gameObject.GetInstanceID(), damageAmount);
        }
        else
        {
            Debug.Log("Shot missed.");
        }

        magazine.consumeAmmo();
        lastFireTime = Time.time;
    }

    private void TryShotgunFire()
    {
        if (magazine == null || magazine.currentAmmo <= 0)
        {
            Debug.Log("Cannot fire shotgun: No ammo.");
            return;
        }

        muzzleFlash?.Play();
        shotgunSound?.Play();

        for (int i = 0; i < shotgunPelletCount; i++)
        {
            Vector3 dir = GetShotgunPelletDirection();
            Ray ray = new Ray(muzzleTransform.position, dir);

            if (Physics.Raycast(ray, out RaycastHit hit, range, enemyLayerMask))
            {
                Debug.Log($"Shotgun pellet hit: {hit.collider.gameObject.name}");
                CmdShootEnemy(hit.collider.gameObject.GetInstanceID(), damageAmount);
            }
        }

        magazine.consumeAmmo();
        lastFireTime = Time.time;
        isCocked = false;
    }

    private Vector3 GetShotgunPelletDirection()
    {
        float yaw = Random.Range(-shotgunSpreadAngle, shotgunSpreadAngle);
        float pitch = Random.Range(-shotgunSpreadAngle, shotgunSpreadAngle);
        return Quaternion.Euler(pitch, yaw, 0) * muzzleTransform.forward;
    }

    private void OnSwapFireType(InputAction.CallbackContext context)
    {
        if (firingType == FiringType.Pistol) return;

        if (firingType == FiringType.Rifle)
        {
            firingType = FiringType.Shotgun;
            damageAmount = 35;
            Debug.Log("Swapped to Shotgun");
        }
        else if (firingType == FiringType.Shotgun)
        {
            firingType = FiringType.Rifle;
            damageAmount = 15;
            Debug.Log("Swapped to Rifle");
        }
    }

    [Command]
    private void CmdShootEnemy(int enemyInstanceID, int damage)
    {
        Debug.Log($"CmdShootEnemy called for instanceID {enemyInstanceID} with damage {damage}");

        GameObject enemy = FindObjectByInstanceID(enemyInstanceID);
        if (enemy == null)
        {
            Debug.LogWarning("Enemy not found on server.");
            return;
        }

        Health health = enemy.GetComponent<Health>();
        if (health != null)
        {
            health.CmdTakeDamage(damage);
        }
    }

    private GameObject FindObjectByInstanceID(int instanceID)
    {
        Health[] allHealth = FindObjectsOfType<Health>();
        foreach (var h in allHealth)
        {
            if (h.gameObject.GetInstanceID() == instanceID)
                return h.gameObject;
        }
        return null;
    }
}
