using UnityEngine;

public class FurlingEncounter : MonoBehaviour
{
    [Header("Settings")]
    public float chaseRange = 8f;
    public float battleRange = 1.5f;
    public float moveSpeed = 5f;
    public float patrolSpeed = 2f;
    public float rotationSpeed = 5f;

    private const float Y_ROTATION_OFFSET = -90f;

    [Header("Patrol Points")]
    public Transform pointA;
    public Transform pointB;
    private Transform currentTarget;

    [Header("Components")]
    public Transform player;
    public Animator childAnimator;
    private BasicPlayerController playerController;
    private OverworldBattleHandler overworldBattleHandler;

    private readonly int isChasingHash = Animator.StringToHash("IsChasing");
    private bool isChasing = false;
    private bool hasTriggeredBattle = false;

    void Start()
    {
        if (childAnimator == null) childAnimator = GetComponentInChildren<Animator>();
        currentTarget = pointA;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            playerController = playerObj.GetComponent<BasicPlayerController>();
        }
        overworldBattleHandler = FindFirstObjectByType<OverworldBattleHandler>();
    }

    void Update()
    {
        if (hasTriggeredBattle) return;

        // Ensure animation is ON while active
        SetAnimationState(true);

        float distanceToPlayer = (player != null) ? Vector3.Distance(transform.position, player.position) : float.MaxValue;

        if (distanceToPlayer <= chaseRange)
        {
            HandleChase(distanceToPlayer);
        }
        else
        {
            PerformPatrol();
        }
    }

    private void SetAnimationState(bool state)
    {
        // Only trigger the Animator if the state is actually changing
        if (isChasing != state)
        {
            isChasing = state;
            if (childAnimator != null) childAnimator.SetBool(isChasingHash, state);
        }
    }

    private void HandleChase(float distanceToPlayer)
    {
        if (distanceToPlayer <= battleRange)
        {
            LaunchBattleScene();
            return;
        }

        RotateTowards(player.position);
        transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
    }

    private void PerformPatrol()
    {
        if (pointA == null || pointB == null) return;

        transform.position = Vector3.MoveTowards(transform.position, currentTarget.position, patrolSpeed * Time.deltaTime);
        RotateTowards(currentTarget.position);

        if (Vector3.Distance(transform.position, currentTarget.position) < 0.1f)
        {
            currentTarget = (currentTarget == pointA) ? pointB : pointA;
        }
    }

    private void RotateTowards(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            Quaternion compensatedRotation = lookRotation * Quaternion.Euler(0, Y_ROTATION_OFFSET, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, compensatedRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void LaunchBattleScene()
    {
        hasTriggeredBattle = true;
        SetAnimationState(false);

        if (playerController != null) playerController.enabled = false;
        if (overworldBattleHandler != null)
        {
            overworldBattleHandler.addEnemy(new FurlingSetup());
            overworldBattleHandler.addEnemy(new FurlingSetup());
        }
        BattleTransitioner.InitiateForcedCombat(this.gameObject);
    }
}