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

    [SerializeField] protected float FollowDistance = 15f;
    [SerializeField] protected float AttackDistance = 2.5f;
    [SerializeField] protected float AttackRate = 1f;
    [SerializeField] private float waitTimeAtDestination = 2f;
    [SerializeField] private GameObject[] Pickups;

    protected float lastAttackTime;
    protected NavMeshAgent agent;
    protected Transform targetPlayer;

    private List<Transform> players = new List<Transform>();
    private HashSet<int> knownPlayerIds = new HashSet<int>();

    public BoxCollider patrolZone;
    private float waitTimer;
    private float patrolTimer;
    private bool destinationSet;
    private bool isDead;

    protected virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        ChangeState(EnemyState.Idle);

        if (patrolZone == null)
            patrolZone = FindNearestPatrolZone();
    }

    protected virtual void Update()
    {
        UpdatePlayers();
        SetTargetPlayer();
        HandleState();
        SetState();
    }

    private void UpdatePlayers()
    {
        GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject obj in playerObjects)
        {
            Transform tf = obj.transform;
            if (tf != null && !knownPlayerIds.Contains(tf.GetInstanceID()))
            {
                players.Add(tf);
                knownPlayerIds.Add(tf.GetInstanceID());
            }
        }

        players.RemoveAll(p => p == null); // Clean up destroyed players
    }

    private void SetTargetPlayer()
    {
        float closestDistance = Mathf.Infinity;
        Transform closest = null;

        foreach (Transform player in players)
        {
            float dist = Vector3.Distance(transform.position, player.position);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                closest = player;
            }
        }

        targetPlayer = closest;
    }

    public void ChangeState(EnemyState newState)
    {
        currentState = newState;
    }

    protected virtual void SetState()
    {
        if (currentState == EnemyState.Dead || targetPlayer == null) return;

        float distance = Vector3.Distance(transform.position, targetPlayer.position);

        if (distance <= AttackDistance)
        {
            ChangeState(EnemyState.Attack);
        }
        else if (distance <= FollowDistance)
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
                if (!isDead) UpdateDead();
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
            agent.isStopped = false;
            agent.SetDestination(targetPlayer.position);
        }
    }

    protected virtual void UpdateAttack()
    {
        if (targetPlayer == null) return;

        agent.isStopped = true;

        Vector3 lookDirection = targetPlayer.position - transform.position;
        lookDirection.y = 0;
        if (lookDirection != Vector3.zero)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDirection), Time.deltaTime * 5f);

        if (Time.time - lastAttackTime >= AttackRate)
        {
            lastAttackTime = Time.time;
            // Perform your attack logic here
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
