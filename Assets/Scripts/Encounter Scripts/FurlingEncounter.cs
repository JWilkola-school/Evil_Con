using UnityEngine;

public class FurlingEncounter : MonoBehaviour
{
    // --- Configurable Settings ---
    [Header("Behavior Settings")]
    public float chaseRange = 8f;
    public float battleRange = 1.5f;
    public float moveSpeed = 5f;
    public float rotationSpeed = 5f;

    // NEW: Offset to correct the model's 90-degree sideways alignment
    private const float Y_ROTATION_OFFSET = -90f;

    [Header("Component References")]
    public Transform player;
    public Animator childAnimator;
    private BasicPlayerController playerController;

    // --- Animator Hash ---
    private readonly int isChasingHash = Animator.StringToHash("IsChasing");

    private bool isChasing = false;
    private bool hasTriggeredBattle = false;

    void Start()
    {
        if (childAnimator == null)
        {
            childAnimator = GetComponentInChildren<Animator>();
        }

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
                playerController = playerObj.GetComponent<BasicPlayerController>();
            }
            else
            {
                Debug.LogError("Player GameObject not found! Tag the player as 'Player'.");
            }
        }
    }

    void Update()
    {
        if (player == null || hasTriggeredBattle) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= chaseRange && distanceToPlayer > battleRange)
        {
            StartChase();

            // --- ROTATION LOGIC WITH OFFSET ---
            Vector3 direction = (player.position - transform.position).normalized;

            // 1. Calculate the rotation to face the target.
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

            // 2. Apply the compensation offset (90 degrees around Y)
            Quaternion compensatedRotation = lookRotation * Quaternion.Euler(0, Y_ROTATION_OFFSET, 0);

            // 3. Smoothly apply the final rotation to the Furling's root
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                compensatedRotation,
                Time.deltaTime * rotationSpeed
            );

            // --- MOVEMENT LOGIC ---
            transform.position = Vector3.MoveTowards(
                transform.position,
                player.position,
                moveSpeed * Time.deltaTime
            );
        }
        else if (distanceToPlayer <= battleRange && isChasing)
        {
            LaunchBattleScene();
        }
        else if (distanceToPlayer > chaseRange && isChasing)
        {
            StopChase();
        }
    }

    private void StartChase()
    {
        if (isChasing) return;
        isChasing = true;

        if (childAnimator != null)
        {
            childAnimator.SetBool(isChasingHash, true);
        }
    }

    private void StopChase()
    {
        isChasing = false;

        if (childAnimator != null)
        {
            childAnimator.SetBool(isChasingHash, false);
        }
    }

    private void LaunchBattleScene()
    {
        hasTriggeredBattle = true;
        StopChase();

        Debug.Log("Furling Encounter! Launching Battle.");

        if (playerController != null)
        {
            playerController.enabled = false;
        }

        BattleTransitioner.InitiateForcedCombat(this.gameObject);
    }
}