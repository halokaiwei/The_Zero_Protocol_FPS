using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RegularFoeAI : MonoBehaviour
{
    public Transform player;
    public Transform attackPoint;
    public FoeAttributes foeAttributes;
    public Animator animator;

    [Header("Melee Settings")]
    public float attackDamage = 10f;
    public float damageDelay = 0.6f; 
    public float attackCooldown = 1.5f; 
    public float maxDistanceAllowed = 30f; 

    [Header("Sound Effect")]
    public AudioClip WalkingSound;
    public AudioClip AttackSound;

    [SerializeField] private NavMeshAgent navMeshAgent;
    private Vector3 initialPosition;
    private Vector3 targetPosition;
    private float chasingStartTime;
    private float chaseCooldownTimer = 0f;
    public float chaseCooldownDuration = 3f;

    private bool canAttack = true;
    private FoeAttributes.FoeState currentState = FoeAttributes.FoeState.Idle;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.updateRotation = false;
        navMeshAgent.stoppingDistance = foeAttributes.attackRange * 0.8f;
        navMeshAgent.speed = foeAttributes.patrolSpeed;
        initialPosition = transform.position;
        targetPosition = GenerateRandomPatrolPosition();
    }

    void Update()
    {
        switch (currentState)
        {
            case FoeAttributes.FoeState.Idle: UpdateIdleState(); break;
            case FoeAttributes.FoeState.Patrolling: UpdatePatrolState(); break;
            case FoeAttributes.FoeState.Chasing: UpdateChaseState(); break;
            case FoeAttributes.FoeState.Attacking: UpdateAttackState(); break;
        }
    }

    private void FaceTarget(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        direction.y = 0; 

        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
        }
    }

    private void UpdateIdleState()
    {
        animator.SetBool("Idling", true);
        animator.SetBool("Walking", false);
        if (Vector3.Distance(transform.position, player.position) <= foeAttributes.activateRange)
            currentState = FoeAttributes.FoeState.Patrolling;
    }

    private void UpdatePatrolState()
    {
        animator.SetBool("Idling", false);
        animator.SetBool("Walking", true);

        navMeshAgent.destination = targetPosition;
        FaceTarget(navMeshAgent.steeringTarget);
        if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            targetPosition = GenerateRandomPatrolPosition();

        float currentDist = Vector3.Distance(transform.position, player.position);

        if (currentDist <= foeAttributes.chaseRange)
        {
            if (Time.time >= chaseCooldownTimer)
            {
                if (CanSeePlayer())
                {
                    Debug.Log("Start CHasing");
                    StartChasing();
                }
                else
                {
                    Debug.Log("CanSeePlayer False");
                }
            }
            else
            {
                Debug.Log("CD havent");
            }
        }
    }

    private void UpdateChaseState()
    {
        animator.SetBool("Walking", true);
        navMeshAgent.SetDestination(player.position);
        FaceTarget(player.position);
        SoundManager.Instance.PlaySoundByInterval(WalkingSound, 0.4f);

        float distToPlayer = Vector3.Distance(attackPoint.position, player.position);
        float distFromStart = Vector3.Distance(transform.position, initialPosition);
        Debug.DrawLine(attackPoint.position, player.position,
    Vector3.Distance(attackPoint.position, player.position) <= foeAttributes.attackRange ? Color.green : Color.red);
        float startAttackBuffer = 0.2f;

        if (distToPlayer <= (foeAttributes.attackRange + startAttackBuffer) && CanSeePlayer())
        {
            StartAttacking();
        }

    }


    private void UpdateAttackState()
    {
        Vector3 targetDir = player.position - transform.position;
        targetDir.y = 0;

        if (targetDir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetDir) * Quaternion.Euler(0, 180, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }

        if (canAttack) StartCoroutine(MeleeAttackSequence());
    }


    private IEnumerator MeleeAttackSequence()
    {
        canAttack = false;

        int attackType = Random.Range(0, 3);
        animator.SetInteger("AttackType", attackType);
        animator.SetTrigger("Attacking");

        yield return new WaitForSeconds(damageDelay);

        float currentDist = Vector3.Distance(attackPoint.position, player.position);

        if (currentDist <= foeAttributes.attackRange + 0.5f)
        {
            player.GetComponent<HealthStaminaSystem>().TakeDamage(attackDamage);
        }

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;

        if (Vector3.Distance(attackPoint.position, player.position) > foeAttributes.attackRange + 1.0f)
            currentState = FoeAttributes.FoeState.Chasing;
    }

    private bool CanSeePlayer()
    {
        RaycastHit hit;
        Vector3 origin = transform.position + Vector3.up * 1.2f;

        Vector3 targetPoint = player.position + Vector3.up * 0.5f;

        Vector3 dir = (targetPoint - origin).normalized;

        Debug.DrawRay(origin, dir * foeAttributes.chaseRange, Color.red);

        if (Physics.Raycast(origin, dir, out hit, foeAttributes.chaseRange))
        {
            if (hit.transform == player || hit.transform.IsChildOf(player))
            {
                return true;
            }
        }
        return false;

    }

    private void StartChasing()
    {
        currentState = FoeAttributes.FoeState.Chasing;
        navMeshAgent.speed = foeAttributes.chaseSpeed;
        chasingStartTime = Time.time;
        animator.SetBool("Walking", true);
    }

    private void StopChasing()
    {
        currentState = FoeAttributes.FoeState.Patrolling;
        navMeshAgent.speed = foeAttributes.patrolSpeed;

        targetPosition = initialPosition;
        navMeshAgent.SetDestination(targetPosition);
        chaseCooldownTimer = Time.time + chaseCooldownDuration;
    }

    private void StartAttacking()
    {
        currentState = FoeAttributes.FoeState.Attacking;
        animator.SetBool("Walking", false);
        animator.SetBool("Idling", false);
        Debug.Log("Start Attacking");
        navMeshAgent.ResetPath();
    }

    private Vector3 GenerateRandomPatrolPosition()
    {
        float randomX = Random.Range(initialPosition.x - foeAttributes.patrolRange, initialPosition.x + foeAttributes.patrolRange);
        float randomZ = Random.Range(initialPosition.z - foeAttributes.patrolRange, initialPosition.z + foeAttributes.patrolRange);
        return new Vector3(randomX, transform.position.y, randomZ);
    }
}