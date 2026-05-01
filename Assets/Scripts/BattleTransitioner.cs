using UnityEngine;
using UnityEngine.SceneManagement;

public static class BattleTransitioner
{
    private const string BATTLE_SCENE_NAME = "BattleScene";

    // Static properties to store enemy info
    public static string EncounteredEnemyName { get; private set; }
    public static string EnemySceneID { get; private set; }
    public static Vector3 EnemyPosition { get; private set; }

    // NEW: Coordinate tracking for the player!
    public static Vector3 playerReturnPosition;
    public static bool returningFromBattle = false;
    public static string overworldSceneName;

    /// <summary>
    /// Initiates the combat sequence by pausing the overworld and loading the battle scene.
    /// </summary>
    /// <param name="enemyGameObject">The enemy GameObject that triggered the encounter.</param>
    /// <param name="playerGameObject">The player GameObject so we can save their position.</param>
    public static void InitiateForcedCombat(GameObject enemyGameObject, GameObject playerGameObject)
    {
        // 1. Safety check
        if (SceneManager.GetActiveScene().name == BATTLE_SCENE_NAME)
        {
            Debug.LogWarning("Attempted to start combat while already in the Battle Scene.");
            return;
        }

        // 2. Store enemy info
        EncounteredEnemyName = enemyGameObject.name;

        OverworldEnemy enemyMemory = enemyGameObject.GetComponent<OverworldEnemy>();

        if (enemyMemory != null)
        {
            EnemySceneID = enemyMemory.myUniqueID; // Use the memorized ID!
        }
        // 3. Sync with EnemyManager to ensure the ID is exactly the same!
        else if (EnemyManager.Instance != null)
        {
            EnemySceneID = EnemyManager.Instance.GenerateEnemyID(enemyGameObject);
        }
        else
        {
            // Fallback just in case EnemyManager isn't in the scene yet
            EnemySceneID = $"{SceneManager.GetActiveScene().name}_{enemyGameObject.name}_{enemyGameObject.transform.position.x:F2}_{enemyGameObject.transform.position.z:F2}";
        }

        EnemyPosition = enemyGameObject.transform.position;

        // 4. NEW: Save the player's position and set the return flag!
        if (playerGameObject != null)
        {
            playerReturnPosition = playerGameObject.transform.position;
            returningFromBattle = true;
        }

        overworldSceneName = SceneManager.GetActiveScene().name;

        Debug.Log($"Combat initiated by {EncounteredEnemyName} at position {EnemyPosition}. ID: {EnemySceneID}");

        // 5. Load the Battle Scene
        SceneTransitioner.Instance.StartTransition(BATTLE_SCENE_NAME);
    }

    public static void ClearBattleData()
    {
        EncounteredEnemyName = null;
        EnemySceneID = null;
        EnemyPosition = Vector3.zero;
    }
}