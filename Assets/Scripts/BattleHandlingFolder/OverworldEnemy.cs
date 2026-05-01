using UnityEngine;
using UnityEngine.SceneManagement;

public class OverworldEnemy : MonoBehaviour
{
    private string myUniqueID;

    void Start()
    {
        // 1. Generate my unique ID based on where I am standing
        if (EnemyManager.Instance != null)
        {
            myUniqueID = EnemyManager.Instance.GenerateEnemyID(gameObject);

            // 2. Check the Graveyard. Am I supposed to be dead?
            if (EnemyManager.Instance.IsEnemyDefeated(myUniqueID))
            {
                gameObject.SetActive(false); // Disappear immediately!
            }
        }
    }

    // When the player touches me to start a battle:
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // We just hand the enemy (gameObject) and the player (other.gameObject) 
            // directly to the Stage Director and let it handle everything!
            BattleTransitioner.InitiateForcedCombat(gameObject, other.gameObject);
        }
    }
}
