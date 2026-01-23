using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // This method will be called when the button is clicked
    public void StartGame()
    {
        SceneManager.LoadScene("Scene 1"); // Change "GameScene" to your target scene name
    }

    public void QuitGame()
    {
        Debug.Log("Quit!");
        Application.Quit();
    }

    public void Settings()
    {

    }
}
