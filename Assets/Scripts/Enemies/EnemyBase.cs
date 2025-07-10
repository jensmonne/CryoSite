using System.Collections.Generic;
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
    [SerializeField] protected int FollowDistance;
    [SerializeField] protected float AttackDistance;
    [SerializeField] protected float AttackRate;

    public BoxCollider patrolZone;
    [SerializeField] private float waitTimeAtDestination;
    [SerializeField] private GameObject[] Pickups;

    protected float lastAttackTime;
    protected Transform targetPlayer;
    protected List<Transform> players = new List<Transform>();

    public NavMeshAgent agent;

    private float waitTimer;
    private bool destinationSet;
    private float patrolTimer = 0f;
    private bool isDead = false;

    protected virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        ChangeState(EnemyState.Idle);

        // Collect all players with the "Player" tag
        GameObject[] playerObjs = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject playerObj in playerObjs)
        {
            players.Add(playerObj.transform);
        }

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
    }

    protected virtual void SetState()
    {
        if (currentState == EnemyState.Dead) return;

        float closestDistance = Mathf.Infinity;
        Transform closestPlayer = null;

        foreach (Transform player in players)
        {
            if (player == null) continue;

            float distance = Vector3.Distance(transform.position, player.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPlayer = player;
            }
        }

        targetPlayer = closestPlayer;

        if (targetPlayer == null)
        {
            ChangeState(EnemyState.Wandering);
            return;
        }

        if (closestDistance < AttackDistance)
        {
            ChangeState(EnemyState.Attack);
        }
        else if (closestDistance < FollowDistance)
        {
            ChangeState(EnemyState.Chase);
        }
        else
        {
            ChangeState(EnemyState.Wandering);
        }
    }

    protected void HandleState()
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
                    patrolTimer = 0f;
                }

                waitTimer = 0f;
            }
        }
        else
        {
            patrolTimer += Time.deltaTime;

            if (patrolTimer >= 10f)
            {
                agent.ResetPath();
                destinationSet = false;
                patrolTimer = 0f;
            }
        }
    }

    protected virtual void UpdateChase()
    {
        if (targetPlayer != null)
        {
            agent.SetDestination(targetPlayer.position);
        }
    }

    protected virtual void UpdateAttack()
    {
        if (targetPlayer != null)
        {
            // Add your actual attack logic here (damage, animation, etc.)
            lastAttackTime = Time.time;
        }
    }

    protected virtual void UpdateDead()
    {
        isDead = true;

        if (Pickups.Length > 0 && Random.value <= 0.25f)
        {
            int index = Random.Range(0, Pickups.Length);
            Instantiate(Pickups[index], transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
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
}
