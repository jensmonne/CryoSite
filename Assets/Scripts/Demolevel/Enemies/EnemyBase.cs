using UnityEngine;

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
    
    protected EnemyState currentState;

    protected virtual void Start()
    {
        ChangeState(EnemyState.Idle);
    }

    protected virtual void Update()
    {
        HandleState();
    }

    protected void ChangeState(EnemyState newState)
    {
        currentState = newState;
        Debug.Log($"State changed to {newState}");
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
                UpdateDead();
                break;
        }
    }
    
    protected virtual void UpdateIdle() { }
    protected virtual void UpdatePatrol() { }
    protected virtual void UpdateChase() { }
    protected virtual void UpdateAttack() { }
    protected virtual void UpdateDead() { }
    
    public abstract void TakeDamage(int amount);
    public abstract void Die();
}