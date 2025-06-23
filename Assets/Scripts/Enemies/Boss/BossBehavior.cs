using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

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
    [SerializeField] private Transform playerTransform;

    private float attackTimer;
    private BossHealth bossHealth; 
    
    private bool isAttacking = false;
    
    [Header("Drone Spawn")]
    // Attack 1 (Drone Spawn)
    [SerializeField] private GameObject KamikazeDrones;
    [SerializeField] private GameObject SpawnpointDrone;
    [SerializeField] private float Timebetweenspawns;
    
    [Header("Gun")]
    // Attack 2 (Gun)
    [SerializeField] private Transform[] shootpoints;
    [SerializeField] private int RangeGun;
    [SerializeField] private float FireRateGun;
    [SerializeField] private LayerMask playerlayer;
    [SerializeField] private int damageamountGun;

    [Header("Lazer")]
    // Attack 3 (Lazer)
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
    
    private int nextAttackIndex = 0;

    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        ChangeState(BossState.Idle);
    }

    private void Update()
    {
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
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 3f); // 3f = rotation speed
            }
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

     void HandleState()
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
                        // Simulate drone attack
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
    
    //Attack 1 (Drone)
    private IEnumerator DroneAttackRoutine()
    {

        yield return new WaitForSeconds(2f);
    }
    
    //Attack2 (Gun)
    private IEnumerator GunAttackRoutine(float speedMultiplier)
    {
        float attackDuration = 1.5f / speedMultiplier;
        float[] firetimer = new float[shootpoints.Length];
        for (int i = 0; i < shootpoints.Length; i++)
        {
            firetimer[i] = 0f;
        }
        while (elapsed < attackDuration)
        {
            for (int i = 0; i < shootpoints.Length; i++)
            {
                Ray ray = new Ray(shootpoints[i].position, shootpoints[i].forward);
                if (Physics.Raycast(ray, out RaycastHit hit, RangeGun, playerlayer))
                {
                    if (firetimer[i] <= 0f)
                    {
                        PlayerHealth playerHealth = hit.collider.GetComponent<PlayerHealth>();
                        if (playerHealth != null)
                        {
                            playerHealth.TakeDamage(damageamountGun);
                            firetimer[i] = FireRateGun;
                        }
                    }
                }
                firetimer[i] -= Time.deltaTime;
            }
        }

        yield return new WaitForSeconds(attackDuration); 
    }
    //Attack3 (Lazer)
    private IEnumerator LazerShoot()
    {
        isFiringLazer = true;

        LineRenderer[] lasers = new LineRenderer[firePoints.Length];
        float[] tickTimers = new float[firePoints.Length];
        
        for (int i = 0; i < firePoints.Length; i++)
        {
            lasers[i] = Instantiate(lineRendererLazer, transform);
            lasers[i].enabled = true;
            tickTimers[i] = 0f;
        }
        
        while (elapsed < Duration)
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
                        PlayerHealth playerHealth = hit.collider.GetComponent<PlayerHealth>();
                        if (playerHealth != null)
                        {
                            playerHealth.TakeDamage(DamageAmountLazer);
                            tickTimers[i] = FireRateLazer;
                        }
                    }
                }
                else
                {
                    lasers[i].SetPosition(1, start + direction * RangeLazer);
                }

                tickTimers[i] -= Time.deltaTime;
            }

            LazerRotator.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
            
            yield return null;
        }
        
        for (int i = 0; i < lasers.Length; i++)
        {
            if (lasers[i] != null)
            {
                lasers[i].enabled = false;
            }
        }

        isFiringLazer = false;
    }

    // stage 2 attacks
    
    // attack 1 
    private IEnumerator Stage2ComboAttackRoutine()
    {
        yield return StartCoroutine(LazerShoot());
        yield return StartCoroutine(DroneAttackRoutine());
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
        bossHealth.enabled = false;
            
        yield return new WaitForSeconds(2f);
        
        bossHealth.enabled = true;
            
        currentStage = BossStage.Stage2;
        ChangeState(BossState.Idle);
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < shootpoints.Length; i++)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(shootpoints[i].position, shootpoints[i].forward * RangeGun);
        }
        for (int i = 0; i < firePoints.Length; i++)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(firePoints[i].position, firePoints[i].forward * RangeLazer);
        }
    }
}
