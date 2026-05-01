using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }

    // THE GRAVEYARD: We only store Strings now! Strings survive scene changes perfectly.
    public List<string> defeatedEnemies = new List<string>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Called when an enemy dies in Battle!
    public void RemoveDefeatedEnemy(string enemySceneID)
    {
        if (!defeatedEnemies.Contains(enemySceneID))
        {
            defeatedEnemies.Add(enemySceneID);
            Debug.Log($"Enemy {enemySceneID} added to the Graveyard!");
        }
    }

    // Your brilliant auto-generator!
    public string GenerateEnemyID(GameObject enemy)
    {
        return $"{SceneManager.GetActiveScene().name}_{enemy.name}_{enemy.transform.position.x:F2}_{enemy.transform.position.z:F2}";
    }

    // Quick check for the Overworld enemies to use
    public bool IsEnemyDefeated(string enemyID)
    {
        return defeatedEnemies.Contains(enemyID);
    }
}