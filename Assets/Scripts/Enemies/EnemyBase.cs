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
    
    public BoxCollider patrolZone;
    [SerializeField] private float waitTimeAtDestination;
    private float patrolTimer = 0f;

    [SerializeField] private float AttackDistance;
    [SerializeField] protected float AttackRate;
    protected float lastAttackTime;
    
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
        
        if (patrolZone == null)
            patrolZone = FindNearestPatrolZone();
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
        
        if (distance < AttackDistance)
        {
            ChangeState(EnemyState.Attack);
        }
        
        if (distance < FollowDistance && currentState != EnemyState.Attack)
        {
            ChangeState(EnemyState.Chase);
        } 
        else if (distance > FollowDistance)
        {
            ChangeState(EnemyState.Wandering);
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
        if (patrolZone == null) return;

        if (!destinationSet || agent.remainingDistance <= agent.stoppingDistance)
        {
            waitTimer += Time.deltaTime;

            if (waitTimer >= waitTimeAtDestination)
            {
                Vector3 randomPos = GetRandomPointInPatrolZone();
                if (NavMesh.SamplePosition(randomPos, out NavMeshHit hit, 2f, NavMesh.AllAreas))
                {
                    agent.SetDestination(hit.position);
                    destinationSet = true;
                    patrolTimer = 0f; // Reset stuck timer
                }

                waitTimer = 0f;
            }
        }
        else
        {
            // Increment patrol stuck timer
            patrolTimer += Time.deltaTime;

            if (patrolTimer >= 10f)
            {
                agent.ResetPath();
                destinationSet = false;
                patrolTimer = 0f;
                Debug.Log($"{gameObject.name} reset patrol path after 10s.");
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
    
    protected BoxCollider FindNearestPatrolZone()
    {
        GameObject[] zones = GameObject.FindGameObjectsWithTag("PetrolZone");
        BoxCollider nearest = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject zoneObj in zones)
        {
            BoxCollider zone = zoneObj.GetComponent<BoxCollider>();
            if (zone == null) continue;

            float dist = Vector3.Distance(transform.position, zone.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                nearest = zone;
            }
        }

        return nearest;
    }
    
    protected Vector3 GetRandomPointInPatrolZone()
    {
        Vector3 center = patrolZone.transform.position + patrolZone.center;
        Vector3 size = patrolZone.size;

        float x = Random.Range(-size.x / 2, size.x / 2);
        float z = Random.Range(-size.z / 2, size.z / 2);

        Vector3 randomPoint = new Vector3(x, 0, z);
        return center + patrolZone.transform.rotation * randomPoint;
    }
    
    
    public abstract void Die();
    
    protected virtual void OnDrawGizmosSelected()
    {
        if (patrolZone != null)
        {
            Gizmos.color = Color.green;
            Gizmos.matrix = patrolZone.transform.localToWorldMatrix;
            Gizmos.DrawWireCube(patrolZone.center, patrolZone.size);
        }
    }
}
