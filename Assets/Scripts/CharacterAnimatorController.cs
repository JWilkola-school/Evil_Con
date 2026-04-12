using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
public class CharacterAnimatorController : MonoBehaviour
{
    private readonly int speedHash = Animator.StringToHash("Speed");
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void SetMovementSpeed(float targetSpeed)
    {
        if (animator == null) return;

        animator.SetFloat(speedHash, targetSpeed);
    }
}