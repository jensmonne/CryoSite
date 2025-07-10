using System.Collections;
using Mirror;
using UnityEngine;

public class NetworkedBossBehavior : NetworkBehaviour
{
    public enum BossState { Idle, Attacking, Recharging, StageSwap, Death }
    public enum BossStage { Stage1, Stage2 }

    [SyncVar(hook = nameof(OnStateChanged))] public BossState currentState = BossState.Idle;
    [SyncVar(hook = nameof(OnStageChanged))] public BossStage currentStage = BossStage.Stage1;

    [SerializeField] private float stageTwoThreshold = 50f;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private Transform playerTransform;

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

    [Header("Lazer")]
    [SerializeField] private Transform LazerRotator;
    [SerializeField] private float rotationSpeed = 45f;
    [SerializeField] private int RangeLazer;
    [SerializeField] private Transform[] firePoints;
    [SerializeField] private float Duration;
    [SerializeField] private int DamageAmountLazer;
    [SerializeField] private LineRenderer lineRendererLazer;
    [SerializeField] private float FireRateLazer = 0.2f;

    private float attackElapsed = 0f;
    private int nextAttackIndex = 0;

    private void Start()
    {
        if (isServer)
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
            ChangeState(BossState.Idle);
        }
    }

    private void Update()
    {
        if (!isServer) return;

        if (bossHealth == null)
        {
            bossHealth = GetComponent<BossHealth>();
        }

        if (playerTransform != null && currentState != BossState.Death)
        {
            Vector3 direction = playerTransform.position - transform.position;
            direction.y = 0f;
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 3f);
            }
        }

        attackElapsed += Time.deltaTime;
        SetState();
        HandleState();
    }

    public void ChangeState(BossState newState)
    {
        currentState = newState;
        Debug.Log($"[Server] Boss state changed to: {newState}");
    }

    private void OnStateChanged(BossState oldState, BossState newState)
    {
        Debug.Log($"[Client] Boss state changed to: {newState}");
    }

    private void OnStageChanged(BossStage oldStage, BossStage newStage)
    {
        Debug.Log($"[Client] Boss stage changed to: {newStage}");
    }

    private void SetState()
    {
        if (currentState == BossState.Death) return;

        if (bossHealth.currentHealth <= 0)
        {
            ChangeState(BossState.Death);
            return;
        }

        if (bossHealth.currentHealth <= stageTwoThreshold && currentStage == BossStage.Stage1)
        {
            ChangeState(BossState.StageSwap);
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
                attackElapsed = 0f;
                Invoke(nameof(ResetToIdle), 1f);
                break;

            case BossState.StageSwap:
                currentStage = BossStage.Stage2;
                StartCoroutine(StageSwapcor());
                break;

            case BossState.Death:
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
                        yield return DroneAttackRoutine();
                        break;
                    case 1:
                        yield return GunAttackRoutine(1f);
                        break;
                    case 2:
                        yield return LazerShoot();
                        break;
                }
                nextAttackIndex = (nextAttackIndex + 1) % 3;
                break;

            case BossStage.Stage2:
                switch (nextAttackIndex)
                {
                    case 0:
                        yield return Stage2ComboAttackRoutine();
                        break;
                    case 1:
                        yield return GunAttackRoutine(2f);
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
            GameObject drone = Instantiate(KamikazeDrones, SpawnpointDrone.transform.position, SpawnpointDrone.transform.rotation);
            NetworkServer.Spawn(drone);
            yield return new WaitForSeconds(Timebetweenspawns);
        }

        yield return new WaitForSeconds(1f);
    }

    private IEnumerator GunAttackRoutine(float speedMultiplier)
    {
        float attackDuration = 1.5f / speedMultiplier;
        float elapsedGun = 0f;
        float[] fireTimer = new float[shootpoints.Length];

        RpcPlayGunAttack();

        while (elapsedGun < attackDuration)
        {
            for (int i = 0; i < shootpoints.Length; i++)
            {
                fireTimer[i] -= Time.deltaTime;

                if (fireTimer[i] <= 0f)
                {
                    Ray ray = new Ray(shootpoints[i].position, shootpoints[i].forward);
                    if (Physics.SphereCast(ray, rayThickness, out RaycastHit hit, RangeGun, playerlayer))
                    {
                        PlayerHealth playerHealth = hit.collider.GetComponent<PlayerHealth>();
                        if (playerHealth != null)
                        {
                            playerHealth.TakeDamage(damageamountGun);
                        }
                    }
                    fireTimer[i] = FireRateGun;
                }
            }

            elapsedGun += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator LazerShoot()
    {
        RpcStartLazerVisuals();
        float elapsed = 0f;
        float[] tickTimers = new float[firePoints.Length];

        while (elapsed < Duration)
        {
            for (int i = 0; i < firePoints.Length; i++)
            {
                Vector3 start = firePoints[i].position;
                Vector3 direction = firePoints[i].forward;

                if (Physics.Raycast(start, direction, out RaycastHit hit, RangeLazer, playerlayer))
                {
                    if (tickTimers[i] <= 0f)
                    {
                        PlayerHealth playerHealth = hit.collider.GetComponent<PlayerHealth>();
                        if (playerHealth != null)
                        {
                            playerHealth.TakeDamage(DamageAmountLazer);
                            tickTimers[i] = FireRateLazer;
                        }
                    }
                }

                tickTimers[i] -= Time.deltaTime;
            }

            LazerRotator.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator HandleLazerVisuals()
    {
        LineRenderer[] lasers = new LineRenderer[firePoints.Length];

        for (int i = 0; i < firePoints.Length; i++)
        {
            lasers[i] = Instantiate(lineRendererLazer, transform);
            lasers[i].enabled = true;
        }

        float elapsed = 0f;
        while (elapsed < Duration)
        {
            for (int i = 0; i < firePoints.Length; i++)
            {
                Vector3 start = firePoints[i].position;
                Vector3 direction = firePoints[i].forward;

                lasers[i].SetPosition(0, start);
                lasers[i].SetPosition(1, start + direction * RangeLazer);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        foreach (var laser in lasers)
        {
            if (laser != null) laser.enabled = false;
        }
    }

    [ClientRpc]
    private void RpcStartLazerVisuals()
    {
        StartCoroutine(HandleLazerVisuals());
    }

    [ClientRpc]
    private void RpcPlayGunAttack()
    {
        // Add muzzle flash or sound here if needed
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

    private IEnumerator StageSwapcor()
    {
        bossHealth.enabled = false;

        yield return new WaitForSeconds(2f);

        bossHealth.enabled = true;
        ChangeState(BossState.Idle);
    }

    private void Death()
    {
        Destroy(gameObject, 1f);
    }

    private void OnDrawGizmos()
    {
        int shootpointSegments = 15;
        if (shootpoints != null)
        {
            for (int i = 0; i < shootpoints.Length; i++)
            {
                if (shootpoints[i] == null) continue;

                Vector3 origin = shootpoints[i].position;
                Vector3 direction = shootpoints[i].forward.normalized;
                float length = RangeGun;

                Gizmos.color = Color.yellow;
                Gizmos.DrawRay(origin, direction * length);

                for (int j = 0; j < shootpointSegments; j++)
                {
                    float angle = (360f / shootpointSegments) * j;
                    Quaternion rot = Quaternion.AngleAxis(angle, direction);
                    Vector3 offset = rot * Vector3.up * rayThickness;
                    Gizmos.DrawRay(origin + offset, direction * length);
                }
            }
        }

        for (int i = 0; i < firePoints.Length; i++)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(firePoints[i].position, firePoints[i].forward * RangeLazer);
        }
    }
}
