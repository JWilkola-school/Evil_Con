using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
public class CharacterAnimatorController : MonoBehaviour
{
    private readonly int isWalkingHash = Animator.StringToHash("isWalking");
    private readonly int isRunningHash = Animator.StringToHash("IsRunning");

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            enabled = false;
        }
    }

    public void SetMovementSpeed(float targetSpeed)
    {
        if (animator == null) return;

        bool moving = targetSpeed > 0.1f;
        bool sprinting = targetSpeed > 4.0f;

        animator.SetBool(isWalkingHash, moving);
        animator.SetBool(isRunningHash, sprinting);
    }
}