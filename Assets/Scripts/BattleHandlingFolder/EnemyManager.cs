using UnityEngine;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }

    // Dictionary to track enemies by their unique ID
    private Dictionary<string, GameObject> enemyRegistry = new Dictionary<string, GameObject>();

    void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // Persist across scenes
    }

    void Start()
    {
        Debug.Log("EnemyManager initialized");
    }

    // Register an enemy when Scene 1 loads
    public void RegisterEnemy(GameObject enemy)
    {
        string enemyID = GenerateEnemyID(enemy);

        if (!enemyRegistry.ContainsKey(enemyID))
        {
            enemyRegistry[enemyID] = enemy;
            Debug.Log($"Registered enemy: {enemy.name} with ID: {enemyID}");
        }
    }

    // Remove an enemy when defeated in battle
    public void RemoveDefeatedEnemy(string enemySceneID)
    {
        if (enemyRegistry.ContainsKey(enemySceneID))
        {
            GameObject enemy = enemyRegistry[enemySceneID];
            if (enemy != null)
            {
                Destroy(enemy); // Permanently remove the enemy
                Debug.Log($"Permanently removed enemy: {enemy.name}");
            }
            enemyRegistry.Remove(enemySceneID);
        }
        else
        {
            Debug.LogWarning($"Enemy with ID {enemySceneID} not found in registry");
        }
    }

    // Generate a unique ID for an enemy
    private string GenerateEnemyID(GameObject enemy)
    {
        // Use scene name + enemy name + position for uniqueness
        return $"{UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}_{enemy.name}_{enemy.transform.position.x:F2}_{enemy.transform.position.z:F2}";
    }

    // Clean up on scene change
    public void ClearRegistry()
    {
        enemyRegistry.Clear();
    }

    // Debug: List all registered enemies
    public void ListAllEnemies()
    {
        Debug.Log("=== Registered Enemies ===");
        foreach (var entry in enemyRegistry)
        {
            Debug.Log($"ID: {entry.Key}, Object: {entry.Value?.name ?? "NULL"}");
        }
    }
}