using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

// This is a Scene Transitioner that creates a fade panel to transition scenes

public class SceneTransitioner : MonoBehaviour
{
    public static SceneTransitioner Instance { get; private set; }

    private CanvasGroup fadeGroup;
    private bool isTransitioning = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SetupCanvas();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void SetupCanvas()
    {
        // 1. Create Canvas
        GameObject canvasObj = new GameObject("TransitionCanvas");
        canvasObj.transform.SetParent(this.transform);

        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999; // keeping this at the top.

        canvasObj.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasObj.AddComponent<GraphicRaycaster>();

        // 2. Create Background Panel
        GameObject panelObj = new GameObject("FadePanel");
        panelObj.transform.SetParent(canvasObj.transform, false);

        Image image = panelObj.AddComponent<Image>();
        image.color = Color.black;

        // 3. Make Panel fill entire screen
        RectTransform rect = panelObj.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;

        // 4. Add CanvasGroup for transparency
        fadeGroup = canvasObj.AddComponent<CanvasGroup>();
        fadeGroup.alpha = 0;
        fadeGroup.blocksRaycasts = false;
    }

    // Bridge to BattleTransitioner
    public void StartTransition(string sceneName)
    {
        if (isTransitioning) return;
        StartCoroutine(TransitionRoutine(sceneName));
    }

    private IEnumerator TransitionRoutine(string sceneName)
    {
        isTransitioning = true;

        // 1. Fade to black
        yield return StartCoroutine(Fade(1, 0.5f));

        // 2. Load scene asynchronously
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        while (!op.isDone)
        {
            yield return null;
        }

        // 3. Fade to new scene
        yield return StartCoroutine(Fade(0, 0.5f));
        isTransitioning = false;
    }

    private IEnumerator Fade(float targetAlpha, float duration)
    {
        // Block clicks during fade
        fadeGroup.blocksRaycasts = true;

        float startAlpha = fadeGroup.alpha;
        float elapsed = 0;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            fadeGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);
            yield return null; // wait for next frame
        }

        fadeGroup.alpha = targetAlpha;

        // Only stop blocking raycasts if fully transparent
        if (targetAlpha <= 0)
        {
            fadeGroup.blocksRaycasts = false;
        }
    }
}
