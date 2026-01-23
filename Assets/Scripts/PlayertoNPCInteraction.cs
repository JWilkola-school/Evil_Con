using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Tooltip("The range the player can be from the NPC to interact.")]
    public float interactionDistance = 2f;

    // We'll use this to keep track of the nearby NPC we can talk to
    private NPCInteraction currentTargetNPC = null;
    private bool isDialogueActive = false;

    void Update()
    {
        // Only check for interaction when dialogue is not currently running
        if (!isDialogueActive)
        {
            // Find the closest NPC in range
            FindNearbyNPC();

            // Check for the interaction key press (e.g., 'E')
            if (currentTargetNPC != null && Input.GetKeyDown(KeyCode.E))
            {
                StartInteraction(currentTargetNPC);
            }
        }
        else // Dialogue is active, check for the key to end it (if applicable)
        {
            // For simple testing, press E again to end the interaction
            if (Input.GetKeyDown(KeyCode.E))
            {
                EndInteraction();
            }
        }
    }

    private void FindNearbyNPC()
    {
        // SphereCast/SphereOverlap is more reliable than checking the NPC's trigger
        Collider[] hits = Physics.OverlapSphere(transform.position, interactionDistance);

        currentTargetNPC = null;

        foreach (Collider hit in hits)
        {
            // Try to get the NPCInteraction script
            NPCInteraction npc = hit.GetComponent<NPCInteraction>();

            if (npc != null && npc.isPlayerInRange) // Check if the NPC is in its own trigger zone
            {
                currentTargetNPC = npc;
                // For simplicity, we just take the first one found
                return;
            }
        }
    }

    private void StartInteraction(NPCInteraction npc)
    {
        isDialogueActive = true;

        // Hide/lock player movement input here
        // Example: GetComponent<BasicPlayerController>().enabled = false;

        npc.StartTalkingAnimation();
        Debug.Log("Dialogue STARTED with " + npc.gameObject.name);
    }

    private void EndInteraction()
    {
        isDialogueActive = false;

        // Resume player movement input here
        // Example: GetComponent<BasicPlayerController>().enabled = true;

        if (currentTargetNPC != null)
        {
            currentTargetNPC.StopTalkingAnimation();
            Debug.Log("Dialogue ENDED.");
        }
        currentTargetNPC = null;
    }
}