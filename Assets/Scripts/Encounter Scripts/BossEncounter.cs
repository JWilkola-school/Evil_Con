using UnityEngine;

public class BossEncounter : MonoBehaviour
{
    private Animator animator;
    private readonly int startIntroHash = Animator.StringToHash("StartIntro");

    [Header("Encounter Settings")]
    [Tooltip("The tag of the player character.")]
    public string playerTag = "Player";

    // Flag to ensure the cinematic sequence only triggers once
    private bool hasTriggered = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Fur King is missing an Animator component.");
        }
    }

    // Called when the Player's Collider enters the trigger zone
    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering is the player and the sequence hasn't started
        if (other.CompareTag(playerTag) && !hasTriggered)
        {
            hasTriggered = true; // Locks the trigger
            StartIntroSequence();
        }
    }

    private void StartIntroSequence()
    {
        Debug.Log("Fur King Intro Sequence Triggered! Playing cinematic.");

        // Fire the intro animation (which has an Animation Event to launch the scene)
        if (animator != null)
        {
            animator.SetTrigger(startIntroHash);
        }
    }

    /// <summary>
    /// PUBLIC METHOD: This MUST be called at the end of the Intro Animation clip
    /// via an Animation Event. It launches the battle using the Transitioner.
    /// </summary>
    public void LaunchBattleScene()
    {
        Debug.Log("Fur King Intro Complete. Launching Battle.");
        // Call the static transition method, passing the Fur King's own GameObject.
        // This is where the scene change happens.
        BattleTransitioner.InitiateForcedCombat(this.gameObject);
    }
}