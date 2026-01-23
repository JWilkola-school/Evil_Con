using UnityEngine;

public class EnemyIdentifier : MonoBehaviour
{
    void Start()
    {
        // Register this enemy with the EnemyManager when Scene 1 loads
        if (EnemyManager.Instance != null)
        {
            EnemyManager.Instance.RegisterEnemy(gameObject);
        }
        else
        {
            Debug.LogWarning("EnemyManager not found when trying to register: " + gameObject.name);
        }
    }

    void OnDestroy()
    {
        Debug.Log($"Enemy {name} is being destroyed");
    }
}