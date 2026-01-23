using UnityEngine;
using UnityEngine.SceneManagement;

public static class BattleTransitioner
{
    private const string BATTLE_SCENE_NAME = "BattleScene";

    // Static properties to store enemy info
    public static string EncounteredEnemyName { get; private set; }
    public static string EnemySceneID { get; private set; } // NEW: Unique identifier
    public static Vector3 EnemyPosition { get; private set; } // NEW: Position in Scene 1

    /// <summary>
    /// Initiates the combat sequence by pausing the overworld and loading the battle scene.
    /// </summary>
    /// <param name="enemyGameObject">The enemy GameObject that triggered the encounter.</param>
    public static void InitiateForcedCombat(GameObject enemyGameObject)
    {
        // 1. Safety check
        if (SceneManager.GetActiveScene().name == BATTLE_SCENE_NAME)
        {
            Debug.LogWarning("Attempted to start combat while already in the Battle Scene.");
            return;
        }

        // 2. Store enemy info
        EncounteredEnemyName = enemyGameObject.name;

        // NEW: Create a unique ID for this enemy
        EnemySceneID = $"{SceneManager.GetActiveScene().name}_{enemyGameObject.name}_{enemyGameObject.transform.position.x:F2}_{enemyGameObject.transform.position.z:F2}";

        // NEW: Store position
        EnemyPosition = enemyGameObject.transform.position;

        Debug.Log($"Combat initiated by {EncounteredEnemyName} at position {EnemyPosition}. ID: {EnemySceneID}");

        // 3. Disable the enemy in Scene 1 (so it doesn't trigger again)
        enemyGameObject.SetActive(false);

        // 4. Load the Battle Scene
        SceneManager.LoadScene(BATTLE_SCENE_NAME);
    }

    // NEW: Clear data after battle
    public static void ClearBattleData()
    {
        EncounteredEnemyName = null;
        EnemySceneID = null;
        EnemyPosition = Vector3.zero;
    }
}