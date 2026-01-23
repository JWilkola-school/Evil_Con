using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class DeathSceneManager : MonoBehaviour
{
    [Header("UI Components")]
    public TextMeshProUGUI deathMessageText;  // The text that shows "You Died!" or similar
    public Button restartButton;               // The restart button (goes to Scene 1)

    [Header("Death Messages")]
    public string[] deathMessages = {
        "You have been defeated...",
        "Your journey ends here...",
        "The battle was lost...",
        "You fell in combat...",
        "Game Over",
        "Defeat...",
        "You have perished...",
        "The enemy was victorious..."
    };

    [Header("Settings")]
    public string restartSceneName = "Scene 1"; // Scene to load when restarting
    public float messageDisplayTime = 2f;       // Time before button appears (if delayed)
    public bool showButtonImmediately = true;   // Show restart button right away

    void Start()
    {
        Debug.Log("=== DEATH SCENE LOADED ===");

        // Show cursor so player can click the button
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Setup the death message
        SetupDeathMessage();

        // Setup the restart button
        SetupRestartButton();
    }

    void SetupDeathMessage()
    {
        if (deathMessageText != null)
        {
            // Pick a random death message
            if (deathMessages.Length > 0)
            {
                int randomIndex = Random.Range(0, deathMessages.Length);
                string message = deathMessages[randomIndex];
                deathMessageText.text = message;
                Debug.Log($"Displaying death message: {message}");
            }
            else
            {
                deathMessageText.text = "Game Over";
            }

            // Make sure text is visible
            deathMessageText.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError("Death message text not assigned!");
        }
    }

    void SetupRestartButton()
    {
        if (restartButton != null)
        {
            // Add listener to the restart button
            restartButton.onClick.AddListener(RestartGame);

            // Set button text
            TextMeshProUGUI buttonText = restartButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = "RESTART";
            }

            // Show/hide based on setting
            if (showButtonImmediately)
            {
                restartButton.gameObject.SetActive(true);
            }
            else
            {
                // Hide button initially, show after delay
                restartButton.gameObject.SetActive(false);
                Invoke("ShowRestartButton", messageDisplayTime);
            }

            Debug.Log("Restart button is ready");
        }
        else
        {
            Debug.LogError("Restart button not assigned!");
        }
    }

    void ShowRestartButton()
    {
        if (restartButton != null)
        {
            restartButton.gameObject.SetActive(true);
            Debug.Log("Restart button now visible");
        }
    }

    // Called when restart button is clicked
    public void RestartGame()
    {
        Debug.Log($"Restarting game... Loading {restartSceneName}");

        // Optional: Add a fade effect here before loading

        // Load Scene 1
        SceneManager.LoadScene(restartSceneName);
    }

    void Update()
    {
        // Allow restarting with keyboard
        if (Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            RestartGame();
        }

        // Escape can still quit if you want
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Optional: Add actual quit functionality if you want
            // QuitGame();
        }
    }

    // Optional: Still include quit function if needed
    void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}