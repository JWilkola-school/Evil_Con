using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerSpawnController : MonoBehaviour
{
    void Start()
    {
        if (SceneManager.GetActiveScene().name == "BattleScene")
        {
            return;
        }

        Debug.Log("<color=cyan>DUDEBRO WOKE UP! Returning Flag is: " + BattleTransitioner.returningFromBattle + "</color>");
        // Check if we are returning from battle        
        if (BattleTransitioner.returningFromBattle)
        {
            // 2. Start the delay instead of teleporting immediately!
            StartCoroutine(TeleportPlayerSafely());
            return; // Stop here
        }

        // --- Original Spawn Logic ---
        if (string.IsNullOrEmpty(SpawnPointManager.targetSpawnPoint))
            return;

        SpawnPoint[] points = Object.FindObjectsByType<SpawnPoint>(FindObjectsSortMode.None);
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

    // 3. THE BULLETPROOF TELEPORT
    private IEnumerator TeleportPlayerSafely()
    {
        // Wait for Unity to completely finish loading the scene and physics!
        yield return new WaitForEndOfFrame();

        CharacterController cc = GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        // Teleport exactly where we were standing
        transform.position = BattleTransitioner.playerReturnPosition;
        Physics.SyncTransforms();

        if (cc != null) cc.enabled = true;

        // Reset the flag
        BattleTransitioner.returningFromBattle = false;

        Debug.Log("Successfully teleported player to: " + transform.position);
    }
}
