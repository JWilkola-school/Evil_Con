using UnityEngine;

[RequireComponent(typeof(Animator))]
public class NPCInteraction : MonoBehaviour
{
    private Animator animator;
    private readonly int isTalkingHash = Animator.StringToHash("IsTalking");

    [Header("Interaction State")]
    // Public flag checked by the player controller
    public bool isPlayerInRange = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("NPC Animator component missing.");
        }
    }

    // --- Interaction Zone Logic ---

    // Sets the proximity flag when the player enters/leaves the trigger zone.
    private void OnTriggerEnter(Collider other)
    {
        // Assumes your player GameObject has the tag "Player"
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            Debug.Log($"NPC: {gameObject.name} is ready to talk.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            Debug.Log($"NPC: {gameObject.name} player left the zone.");
        }
    }

    // --- Public Animation Control Methods ---

    /// <summary>
    /// Starts the looping talking animation (IsTalking = True).
    /// </summary>
    public void StartTalkingAnimation()
    {
        if (animator != null)
        {
            animator.SetBool(isTalkingHash, true);
        }
        // NOTE: Add your dialogue system start logic here.
    }

    /// <summary>
    /// Stops the looping talking animation (IsTalking = False).
    /// </summary>
    public void StopTalkingAnimation()
    {
        if (animator != null)
        {
            animator.SetBool(isTalkingHash, false);
        }
        // NOTE: Add your dialogue system end logic here.
    }
}
