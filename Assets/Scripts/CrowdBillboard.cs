using UnityEngine;

public class DistanceFadeBillboard : MonoBehaviour
{
    // --- Configurable Settings ---
    [Header("Fade Settings")]
    [Tooltip("The distance from the player where the fade BEGINS (fully visible, Alpha = 1).")]
    public float fadeStartDistance = 25f; // Recommended for distant crowd

    [Tooltip("The distance from the player where the fade ENDS (fully transparent, Alpha = 0).")]
    public float fadeEndDistance = 8f;     // Recommended for distant crowd

    // --- References ---
    private Transform mainCameraTransform;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        // 1. Get Camera Reference
        if (Camera.main != null)
        {
            mainCameraTransform = Camera.main.transform;
        }

        // 2. Get SpriteRenderer Reference (should be on a child object)
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer not found in children of the Fading Billboard object!");
            enabled = false;
        }
    }

    void LateUpdate()
    {
        if (mainCameraTransform == null || spriteRenderer == null) return;

        // --- 1. Billboarding Logic (Faces the Camera) ---

        Vector3 lookDirection = mainCameraTransform.position - transform.position;
        // Zero out the Y component to prevent tilting up/down
        lookDirection.y = 0;
        transform.rotation = Quaternion.LookRotation(lookDirection);

        // --- 2. Fading Logic (Updates Transparency) ---
        UpdateFade();
    }

    private void UpdateFade()
    {
        // Calculate the horizontal distance to the camera (ignoring height differences)
        Vector3 camPosFlat = new Vector3(mainCameraTransform.position.x, transform.position.y, mainCameraTransform.position.z);
        float distance = Vector3.Distance(transform.position, camPosFlat);

        // Map the distance to a 0-1 range (0=End, 1=Start)
        float fadeFactor = Mathf.InverseLerp(fadeEndDistance, fadeStartDistance, distance);

        // Clamp and apply the new alpha value to the sprite's color
        fadeFactor = Mathf.Clamp01(fadeFactor);
        Color currentColor = spriteRenderer.color;
        currentColor.a = fadeFactor;
        spriteRenderer.color = currentColor;
    }
}