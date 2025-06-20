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

    private float attackTimer;
    private BossHealth bossHealth; 
    
    // Attack 1 (Drone Spawn)
    [SerializeField] private GameObject KamikazeDrones;
    [SerializeField] private GameObject Spawnpoint;
    [SerializeField] private int Timebetweenspawns;
    
    // Attack 2 (Gun)
    [SerializeField] private Transform[] shootpoints;
    [SerializeField] private int RangeGun;
    [SerializeField] private LayerMask playerlayer;
    [SerializeField] private int damageamount;
    
    // Attack 3 (Lazer)
    [SerializeField] private int RangeLazer;
    
    private int nextAttackIndex = 0;

    private void Start()
    {
        ChangeState(BossState.Idle);
    }

    private void Update()
    {
        if (bossHealth == null)
        {
            bossHealth = GetComponent<BossHealth>();
        }

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
                PerformAttack();
                ChangeState(BossState.Recharging);
                break;

            case BossState.Recharging: 
                Invoke(nameof(ResetToIdle), 1f);
                break;

            case BossState.StageSwap:
                Debug.Log("Boss is evolving to Stage 2!");
                currentStage = BossStage.Stage2;
                Stageswap();
                break;
        }
    }
    
    void PerformAttack()
    {
        if (currentStage == BossStage.Stage1)
        {
            switch (nextAttackIndex)
            {
                case 0:
                    Debug.Log("Attack 1: DroneSpawn");
                    break;
                case 1:
                    Debug.Log("Attack 2: Gun");
                    break;
                case 2:
                    Debug.Log("Attack 3: Lazer");
                    break;
            }
            nextAttackIndex = (nextAttackIndex + 1) % 3;
        }

        if (currentStage == BossStage.Stage2)
        {
            Debug.Log("Stage 2 bonus: More aggressive attack!");
            switch (nextAttackIndex)
            {
                case 0:
                    Debug.Log("Attack 1: Fireball barrage!");
                    break;
                case 1:
                    Debug.Log("Attack 2: Ground Slam!");
                    break;
            }
            nextAttackIndex = (nextAttackIndex + 1) % 2;
        }
        
        
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
}
