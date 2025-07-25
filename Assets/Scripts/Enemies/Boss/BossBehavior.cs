using System.Collections;
using UnityEngine;

public class BossBehavior : MonoBehaviour
{
    public enum BossState
    {
        Idle,
        Attacking,
        Recharging,
        StageSwap,
        Death
    }
    public enum BossStage
    {
        Stage1,
        Stage2
    }

    public BossState currentState = BossState.Idle;
    public BossStage currentStage = BossStage.Stage1;

    [SerializeField] private float stageTwoThreshold = 50f;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private float rotationUpdateInterval = 0.3f;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Animator animator;

    private float attackTimer;
    private BossHealth bossHealth;

    private bool isAttacking = false;

    [Header("Drone Spawn")]
    [SerializeField] private GameObject KamikazeDrones;
    [SerializeField] private GameObject SpawnpointDrone;
    [SerializeField] private float Timebetweenspawns;
    [SerializeField] private int droneCount = 5;

    [Header("Gun")]
    [SerializeField] private Transform[] shootpoints;
    [SerializeField] private int RangeGun;
    [SerializeField] private float FireRateGun;
    [SerializeField] private LayerMask playerlayer;
    [SerializeField] private int damageamountGun;
    [SerializeField] private float rayThickness = 0.25f;
    [SerializeField] private AudioSource Gun;
    [SerializeField] private ParticleSystem[] gunParticles;

    [Header("Lazer")]
    [SerializeField] private Transform LazerRotator;
    [SerializeField] private float rotationSpeed = 45f;
    [SerializeField] private int RangeLazer;
    [SerializeField] private Transform[] firePoints;
    [SerializeField] private float Duration;
    [SerializeField] private int DamageAmountLazer;
    private bool isFiringLazer = false;
    [SerializeField] private LineRenderer lineRendererLazer;
    [SerializeField] private float FireRateLazer = 0.2f;
    private float elapsed = 0f;
    private float LazerFireRateTimer = 0f;
    [SerializeField] private AudioSource LazerAudio;

    private int nextAttackIndex = 0;
    
    private Quaternion delayedTargetRotation;
    private float rotationUpdateTimer = 0f;
    
    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        ChangeState(BossState.Idle);

        // Initialize delayed rotation
        if (playerTransform != null)
        {
            Vector3 direction = playerTransform.position - transform.position;
            direction.y = 0f;
            if (direction != Vector3.zero)
                delayedTargetRotation = Quaternion.LookRotation(direction);
            else
                delayedTargetRotation = transform.rotation;
        }
    }

    private void Update()
    {
        if (bossHealth == null)
        {
            bossHealth = GetComponent<BossHealth>();
        }

        if (playerTransform != null && currentState != BossState.Death)
        {
            rotationUpdateTimer -= Time.deltaTime;
            if (rotationUpdateTimer <= 0f)
            {
                Vector3 direction = playerTransform.position - transform.position;
                direction.y = 0f;
                if (direction != Vector3.zero)
                {
                    delayedTargetRotation = Quaternion.LookRotation(direction);
                }
                rotationUpdateTimer = rotationUpdateInterval;
            }

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                delayedTargetRotation,
                Time.deltaTime * 3f 
            );
        }

        elapsed += Time.deltaTime;
        SetState();
        HandleState();
    }

    public void ChangeState(BossState newState)
    {
        currentState = newState;
        Debug.Log($"Boss state changed to: {newState}");
    }

    private void SetState()
    {
        if (currentState == BossState.Death) return;

        if (bossHealth.currentHealth <= stageTwoThreshold && currentStage == BossStage.Stage1)
        {
            ChangeState(BossState.StageSwap);
            return;
        }

        if (bossHealth.currentHealth <= 0)
        {
            ChangeState(BossState.Death);
            return;
        }
    }

    private void HandleState()
    {
        switch (currentState)
        {
            case BossState.Idle:
                attackTimer += Time.deltaTime;
                if (attackTimer >= attackCooldown)
                {
                    attackTimer = 0f;
                    ChangeState(BossState.Attacking);
                }
                break;

            case BossState.Attacking:
                if (!isAttacking)
                    StartCoroutine(AttackSequence());
                break;

            case BossState.Recharging:
                elapsed = 0f;
                Invoke(nameof(ResetToIdle), 1f);
                break;

            case BossState.StageSwap:
                Debug.Log("Boss is evolving to Stage 2!");
                currentStage = BossStage.Stage2;
                Stageswap();
                break;

            case BossState.Death:
                Debug.Log("Boss died");
                Death();
                break;
        }
    }

    private IEnumerator AttackSequence()
    {
        isAttacking = true;

        switch (currentStage)
        {
            case BossStage.Stage1:
                switch (nextAttackIndex)
                {
                    case 0:
                        Debug.Log("Attack 1: DroneSpawn");
                        yield return StartCoroutine(DroneAttackRoutine());
                        break;
                    case 1:
                        Debug.Log("Attack 2: Gun");
                        yield return StartCoroutine(GunAttackRoutine(1f));
                        break;
                    case 2:
                        Debug.Log("Attack 3: Lazer");
                        yield return StartCoroutine(LazerShoot());
                        break;
                }
                nextAttackIndex = (nextAttackIndex + 1) % 3;
                break;

            case BossStage.Stage2:
                Debug.Log("Stage 2 bonus: More aggressive attack!");
                switch (nextAttackIndex)
                {
                    case 0:
                        Debug.Log("Attack: Lazer + Drones spawn");
                        yield return StartCoroutine(Stage2ComboAttackRoutine());
                        break;
                    case 1:
                        Debug.Log("Attack: Gun 2x speed!");
                        yield return StartCoroutine(GunAttackRoutine(2f));
                        break;
                }
                nextAttackIndex = (nextAttackIndex + 1) % 2;
                break;
        }

        ChangeState(BossState.Recharging);
        isAttacking = false;
    }

    private IEnumerator DroneAttackRoutine()
    {
        for (int i = 0; i < droneCount; i++)
        {
            Instantiate(KamikazeDrones, SpawnpointDrone.transform.position, SpawnpointDrone.transform.rotation);
            yield return new WaitForSeconds(Timebetweenspawns);
        }
        yield return new WaitForSeconds(1f);
    }

    private IEnumerator GunAttackRoutine(float speedMultiplier)
    {
        Gun.Play();
        float attackDuration = 1.5f / speedMultiplier;
        float elapsedGun = 0f;
        float[] fireTimer = new float[shootpoints.Length];

        for (int i = 0; i < fireTimer.Length; i++)
            fireTimer[i] = 0f;

        while (elapsedGun < attackDuration)
        {
            for (int i = 0; i < shootpoints.Length; i++)
            {
                fireTimer[i] -= Time.deltaTime;

                if (fireTimer[i] <= 0f)
                {
                    Ray ray = new Ray(shootpoints[i].position, shootpoints[i].forward);
                    Debug.DrawRay(ray.origin, ray.direction * RangeGun, Color.red, 0.1f);
                    if (Physics.SphereCast(ray, rayThickness, out RaycastHit hit, RangeGun, playerlayer))
                    {
                        NetworkPlayerHealth playerHp = hit.collider.GetComponent<NetworkPlayerHealth>();
                        PlayerHealth playerHealth = hit.collider.GetComponent<PlayerHealth>();
                        if (playerHealth != null) playerHealth.TakeDamage(damageamountGun);
                        if (playerHp != null) playerHealth.TakeDamage(damageamountGun);
                        fireTimer[i] = FireRateGun;
                    }

                    for (int j = 0; j < gunParticles.Length; j++)
                    {
                        gunParticles[j].Play();
                    }
                }
            }

            elapsedGun += Time.deltaTime;
            yield return null;
        }
        Gun.Stop();
    }

    private IEnumerator LazerShoot()
    {
        isFiringLazer = true;
        LazerAudio.Play();

        LineRenderer[] lasers = new LineRenderer[firePoints.Length];
        float[] tickTimers = new float[firePoints.Length];

        for (int i = 0; i < firePoints.Length; i++)
        {
            lasers[i] = Instantiate(lineRendererLazer, transform);
            lasers[i].enabled = true;
            tickTimers[i] = 0f;
        }

        float lazerElapsed = 0f;

        while (lazerElapsed < Duration)
        {
            for (int i = 0; i < firePoints.Length; i++)
            {
                Vector3 start = firePoints[i].position;
                Vector3 direction = firePoints[i].forward;

                lasers[i].SetPosition(0, start);

                if (Physics.Raycast(start, direction, out RaycastHit hit, RangeLazer, playerlayer))
                {
                    lasers[i].SetPosition(1, hit.point);

                    if (tickTimers[i] <= 0f)
                    {
                        NetworkPlayerHealth playerHp = hit.collider.GetComponent<NetworkPlayerHealth>();
                        PlayerHealth playerHealth = hit.collider.GetComponent<PlayerHealth>();
                        if (playerHealth != null) playerHealth.TakeDamage(DamageAmountLazer);
                        if (playerHp != null) playerHealth.TakeDamage(DamageAmountLazer);
                        tickTimers[i] = FireRateLazer;
                    }
                }
                else
                {
                    lasers[i].SetPosition(1, start + direction * RangeLazer);
                }

                tickTimers[i] -= Time.deltaTime;
            }

            LazerRotator.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

            lazerElapsed += Time.deltaTime;
            yield return null;
        }

        for (int i = 0; i < lasers.Length; i++)
        {
            if (lasers[i] != null)
            {
                lasers[i].enabled = false;
            }
        }
        LazerAudio.Stop();
        isFiringLazer = false;
    }

    private IEnumerator Stage2ComboAttackRoutine()
    {
        Coroutine lazerCoroutine = StartCoroutine(LazerShoot());
        Coroutine droneCoroutine = StartCoroutine(DroneAttackRoutine());

        yield return lazerCoroutine;
        yield return droneCoroutine;
    }

    private void ResetToIdle()
    {
        if (currentState != BossState.Death)
            ChangeState(BossState.Idle);
    }

    private void Stageswap()
    {
        StartCoroutine(StageSwapcor());
        
    }
    private IEnumerator StageSwapcor()
    {
        Collider[] colliders = GetComponents<Collider>();
        foreach (Collider col in colliders)
        {
            col.enabled = false;
        }
        
        animator.SetBool("StageSwap", true);
        
        yield return new WaitForSeconds(2f);

        foreach (Collider col in colliders)
        {
            col.enabled = true;
        }
        
        animator.SetBool("StageSwap", false);
        bossHealth.enabled = true;
        
        currentStage = BossStage.Stage2;
        ChangeState(BossState.Idle);
    }


    private void Death()
    {
        Destroy(gameObject, 1f);
    }
}
