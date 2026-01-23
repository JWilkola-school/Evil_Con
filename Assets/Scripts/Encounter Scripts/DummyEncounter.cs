using UnityEngine;

public class DummyEncounter : MonoBehaviour
{
    [Header("Encounter Settings")]
    [Tooltip("The tag of the player character.")]
    public string playerTag = "Player";

    // Flag to ensure the transition only happens once
    private bool hasTriggered = false;

    // Called when the Player's Collider enters the trigger zone
    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering is the player and the transition hasn't started
        if (other.CompareTag(playerTag) && !hasTriggered)
        {
            hasTriggered = true; // Locks the trigger
            LaunchBattleScene();
        }
    }

    private void LaunchBattleScene()
    {
        Debug.Log("Dummy Encounter! Launching Battle.");

        // Call the static transition method, passing the Dummy's own GameObject.
        // The BattleStateController will use the Dummy's name to set up the fight.
        BattleTransitioner.InitiateForcedCombat(this.gameObject);
    }
}
