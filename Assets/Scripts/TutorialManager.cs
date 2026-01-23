using UnityEngine;
using UnityEngine.UI;  // Use TMPro if using TMP
using TMPro;

public class TutorialManager : MonoBehaviour
{
    public string[] tutorialMessages;   // Your steps
    public TextMeshProUGUI tutorialText;           // If using TMP: TextMeshProUGUI
    public KeyCode nextKey = KeyCode.Space;

    private int currentIndex = 0;
    private bool tutorialActive = true;

    void Start()
    {
        // Show first message
        if (tutorialMessages.Length > 0)
        {
            tutorialText.text = tutorialMessages[currentIndex];
        }
    }

    void Update()
    {
        if (!tutorialActive) return;

        if (Input.GetKeyDown(nextKey))
        {
            currentIndex++;

            // Finished tutorial?
            if (currentIndex >= tutorialMessages.Length)
            {
                tutorialActive = false;
                tutorialText.gameObject.SetActive(false);
            }
            else
            {
                tutorialText.text = tutorialMessages[currentIndex];
            }
        }
    }
}
