using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
public class CharacterAnimatorController : MonoBehaviour
{
    // --- Parameter Hashes ---
    private readonly int speedHash = Animator.StringToHash("Speed");
    private readonly int attackTriggerHash = Animator.StringToHash("TriggerAttack");
    private readonly int isBlockingHash = Animator.StringToHash("IsBlocking");

    [Header("Dampening")]
    [Tooltip("Time in seconds to smooth the Speed parameter.")]
    public float dampTime = 0.1f;

    private Animator animator;
    // CharacterController reference is no longer needed in this script, but kept for RequireComponent
    // private CharacterController controller; 

    // Internal state for smooth damping
    private float currentAnimSpeed = 0f;
    private float speedSmoothVelocity = 0f; // Velocity reference required by Mathf.SmoothDamp

    void Start()
    {
        animator = GetComponent<Animator>();
        // controller = GetComponent<CharacterController>();

        if (animator == null)
        {
            Debug.LogError("Animator component missing from GameObject.");
            enabled = false;
        }
    }

    // Update() is now empty since the logic is driven by the controller
    void Update() { }

    /// <summary>
    /// PUBLIC METHOD: Receives the target speed from the BasicPlayerController.
    /// </summary>
    public void SetMovementSpeed(float targetSpeed)
    {
        // 1. Smoothly damp the current speed towards the target speed
        currentAnimSpeed = Mathf.SmoothDamp(
            currentAnimSpeed,
            targetSpeed,
            ref speedSmoothVelocity,
            dampTime
        );

        // 2. Drive the 'Speed' float with the dampened value
        animator.SetFloat(speedHash, currentAnimSpeed);
    }

    // --- Public Animation Control Methods ---

    public void TriggerAttack()
    {
        animator.SetTrigger(attackTriggerHash);
    }

    public void SetBlocking(bool isBlocking)
    {
        animator.SetBool(isBlockingHash, isBlocking);
    }
}