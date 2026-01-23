using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    void Start()
    {
        // Ensure this persists across scenes
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Scene 1")
        {
            Debug.Log("Scene 1 loaded - resetting player state...");

            // Re-enable player controller if it exists
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                BasicPlayerController controller = player.GetComponent<BasicPlayerController>();
                if (controller != null)
                {
                    controller.enabled = true;
                    Debug.Log("Player controller re-enabled");
                }
            }

            // Optional: Reset player HP here if needed
            // You might want to create a PlayerStats manager for this
        }
        else if (scene.name == "BattleScene")
        {
            Debug.Log("BattleScene loaded - preparing for battle...");
        }
    }
}