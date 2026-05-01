using UnityEngine;

public class PlayerSpawnController : MonoBehaviour
{
    void Start()
    {
        // --- NEW: Check if we are returning from battle first! ---
        if (BattleTransitioner.returningFromBattle)
        {
            // Teleport exactly where we were standing
            transform.position = BattleTransitioner.playerReturnPosition;

            // Reset the flag so we don't accidentally teleport next time
            BattleTransitioner.returningFromBattle = false;

            return; // Stop here, we don't need to check SpawnPoints!
        }

        if (string.IsNullOrEmpty(SpawnPointManager.targetSpawnPoint))
            return; // No specific spawn requested → use default

        // New recommended API
        SpawnPoint[] points = Object.FindObjectsByType<SpawnPoint>(FindObjectsSortMode.None);
        /* Whenever a battle initiates, drop a spawn point where the player was
         * and set it to be the new target spawn point. -FM*/
        foreach (var point in points)
        {
            if (point.spawnPointID == SpawnPointManager.targetSpawnPoint)
            {
                transform.position = point.transform.position;
                transform.rotation = point.transform.rotation;
                return;
            }
        }
    }
}
