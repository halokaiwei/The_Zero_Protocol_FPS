using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class CatAI : MonoBehaviour
{
    public enum CatBehavior { WanderAroundOrigin, FollowPlayer } 

    private NavMeshAgent agent;
    private Animator anim;

    [Header("Behavior Mode")]
    public CatBehavior currentBehavior = CatBehavior.WanderAroundOrigin;
    public Transform player;

    [Header("Systems")]
    public DialogueManager dialogueManager;

    [Header("Settings")]
    public float wanderRadius = 10f;
    public float minActionTime = 4f;
    public float maxActionTime = 8f;
    public float meowTime = 3f;

    [Header("Creepy Look At")]
    public Transform neckBone;
    public float lookSpeed = 5f;

    [Header("Narrative Trigger")]
    public bool isCreepyLooking = false;

    private Vector3 originPosition;
    private bool hasSpokenAboutCreepyCat = false;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        originPosition = transform.position;

        if (player == null) player = GameObject.FindGameObjectWithTag("Player").transform;

        StartCoroutine(CatBehaviorRoutine());
    }

    IEnumerator CatBehaviorRoutine()
    {
        while (true)
        {
            Vector3 targetPos;
            if (currentBehavior == CatBehavior.FollowPlayer && player != null)
            {
                targetPos = GetLocationNearPlayer();
            }
            else
            {
                targetPos = GetRandomLocation();
            }

            agent.SetDestination(targetPos);
            agent.isStopped = false;

            anim.SetBool("isWalking", true);
            anim.SetBool("isSitting", false);
            anim.SetBool("isMeow", false);

            float walkDuration = Random.Range(minActionTime, maxActionTime);
            float elapsed = 0f;

            while (elapsed < walkDuration)
            {
                if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.1f)
                    break;
                elapsed += Time.deltaTime;
                yield return null;
            }

            anim.SetBool("isWalking", false);
            anim.SetBool("isSitting", true);
            agent.isStopped = true;
            agent.velocity = Vector3.zero;

            yield return new WaitForSeconds(Random.Range(minActionTime, maxActionTime));

            anim.SetBool("isSitting", false);
            anim.SetBool("isMeow", true);
            yield return new WaitForSeconds(meowTime);
            anim.SetBool("isMeow", false);
        }
    }

    void LateUpdate()
    {
        if (isCreepyLooking && neckBone != null && player != null)
        {
            Vector3 direction = player.position - neckBone.position;

            if (direction.magnitude < 0.1f) return;

            Quaternion targetRotation = Quaternion.LookRotation(direction);

            neckBone.rotation = targetRotation;

            float dist = Vector3.Distance(transform.position, player.position);
            if (!hasSpokenAboutCreepyCat && dist < 3f)
            {
                StartCoroutine(TriggerCreepyCatDialogue());
                hasSpokenAboutCreepyCat = true;
            }
        }
    }

    IEnumerator TriggerCreepyCatDialogue()
    {
        yield return new WaitForSeconds(1f);
        dialogueManager.GlobalShowMessage("Zero: Artemis, the cat... its eyes are locked on me. It looks... broken.");

        yield return new WaitForSeconds(4f);
        dialogueManager.GlobalShowMessage("Artemis: It is a legacy asset, Zero. Its skeletal solver is malfunctioning. Ignore the unit and proceed to the terminal.");
    }

    Vector3 GetRandomLocation()
    {
        Vector3 randomDir = Random.insideUnitSphere * wanderRadius;
        randomDir += originPosition;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDir, out hit, wanderRadius, 1)) return hit.position;
        return transform.position;
    }

    public void SwitchToFollowMode()
    {
        if (currentBehavior != CatBehavior.FollowPlayer)
        {
            Debug.Log("[CatAI] Trigger Door Open. Follow Player.");
            currentBehavior = CatBehavior.FollowPlayer;

            agent.isStopped = false;
            StopAllCoroutines();
            StartCoroutine(CatBehaviorRoutine());
        }
    }

    Vector3 GetLocationNearPlayer()
    {
        Vector3 randomDir = Random.insideUnitSphere * wanderRadius;
        randomDir += player.position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDir, out hit, wanderRadius, 1)) return hit.position;
        return transform.position;
    }
}