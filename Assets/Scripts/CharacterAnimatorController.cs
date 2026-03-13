using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CharacterAnimatorController : MonoBehaviour
{
    private readonly int speedHash = Animator.StringToHash("Speed");
    private readonly int attackHash = Animator.StringToHash("AttackTrig");
    private readonly int itemHash = Animator.StringToHash("ItemTrigger");
    private readonly int blockHash = Animator.StringToHash("BlockTrig");
    private readonly int deathHash = Animator.StringToHash("DeathTrig");

    [Header("Blending")]
    public float dampTime = 0.1f;

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void SetMovementSpeed(float speed)
    {
        animator.SetFloat(speedHash, speed, dampTime, Time.deltaTime);
    }

    public void TriggerAttack()
    {
        animator.SetTrigger(attackHash);
    }

    public void TriggerItem()
    {
        animator.SetTrigger(itemHash);
    }

    public void SetBlocking(bool isBlocking)
    {
        animator.SetBool(blockHash, isBlocking);
    }

    public void TriggerDeath()
    {
        animator.SetTrigger(deathHash);

        if (TryGetComponent<BasicPlayerController>(out var move))
        {
            move.enabled = false;
        }

        if (TryGetComponent<CharacterController>(out var cc))
        {
            cc.enabled = false;
        }
    }
}