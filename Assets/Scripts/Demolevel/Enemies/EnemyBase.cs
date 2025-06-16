using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyBase : MonoBehaviour
{
    public enum EnemyState
    {
        Idle,
        Wandering,
        Chase,
        Attack,
        Dead
    }
    
    public EnemyState currentState;
    [SerializeField] int FollowDistance;
    
    [SerializeField] Vector3 patrolCenter;
    [SerializeField] Vector3 patrolSize;
    [SerializeField] float waitTimeAtDestination;

    [SerializeField] private float AttackDistance;
    [SerializeField] private float AttackRate;
    private float lastAttackTime;
    
    private Transform Player;
    public NavMeshAgent agent;

    private float waitTimer;
    private bool destinationSet;

    private bool isDead = false;

    protected virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        ChangeState(EnemyState.Idle);
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        Player = playerObj.transform;
        
        if (patrolCenter == Vector3.zero)
            patrolCenter = transform.position;
    }

    protected virtual void Update()
    {
        HandleState();
        SetState();
    }

    public void ChangeState(EnemyState newState)
    {
        currentState = newState;
        Debug.Log($"State changed to {newState}");
    }

    protected void SetState()
    {
        if (currentState == EnemyState.Dead) return;
        float distance = Vector3.Distance(transform.position, Player.position);
        if (distance < FollowDistance && currentState != EnemyState.Attack)
        {
            ChangeState(EnemyState.Chase);
        } else if (distance > FollowDistance)
        {
            ChangeState(EnemyState.Wandering);
        }

        if (distance < AttackDistance && Time.time - lastAttackTime >= AttackRate)
        {
            UpdateAttack();
        }
    }

    protected virtual void HandleState()
    {
        switch (currentState)
        {
            case EnemyState.Idle:
                UpdateIdle();
                break;
            case EnemyState.Wandering:
                UpdatePatrol();
                break;
            case EnemyState.Chase:
                UpdateChase();
                break;
            case EnemyState.Attack:
                UpdateAttack();
                break;
            case EnemyState.Dead:
                if (!isDead)
                {
                    UpdateDead();
                }

                break;
        }
    }

    protected virtual void UpdateIdle()
    {
        ChangeState(EnemyState.Wandering);
    }

    protected virtual void UpdatePatrol()
    {
        if (!destinationSet || agent.remainingDistance <= agent.stoppingDistance)
        {
            waitTimer += Time.deltaTime;

            if (waitTimer >= waitTimeAtDestination)
            {
                Vector3 randomPos = GetRandomPatrolPosition();
                NavMeshHit hit;

                if (NavMesh.SamplePosition(randomPos, out hit, 2f, NavMesh.AllAreas))
                {
                    agent.SetDestination(hit.position);
                    destinationSet = true;
                }

                waitTimer = 0f;
            }
        }
    }

    protected virtual void UpdateChase()
    {
        agent.SetDestination(Player.position);
    }

    protected virtual void UpdateAttack()
    {
        lastAttackTime = Time.time;
    }

    protected virtual void UpdateDead()
    {
        Destroy(gameObject, 0.5f);
        enabled = false;
    }
    
    protected Vector3 GetRandomPatrolPosition()
    {
        Vector3 randomOffset = new Vector3(
            Random.Range(-patrolSize.x / 2, patrolSize.x / 2),
            0,
            Random.Range(-patrolSize.z / 2, patrolSize.z / 2)
        );
        return patrolCenter + randomOffset;
    }
    
    public abstract void Die();
    
    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(patrolCenter == Vector3.zero ? transform.position : patrolCenter, patrolSize);
    }
}