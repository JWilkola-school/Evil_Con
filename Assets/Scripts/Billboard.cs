using UnityEngine;

public class Billboard : MonoBehaviour
{
    // A private reference to the main camera's Transform component
    private Transform mainCameraTransform;

    void Start()
    {
        // Find the main camera in the scene and store its Transform
        if (Camera.main != null)
        {
            mainCameraTransform = Camera.main.transform;
        }
        else
        {
            Debug.LogError("The scene is missing a Main Camera tagged 'MainCamera'. Billboarding will not work.");
            enabled = false; // Disable the script if no camera is found
        }
    }

    // LateUpdate is used to ensure billboarding happens after all camera movement
    void LateUpdate()
    {
        if (mainCameraTransform == null) return;

        // 1. Calculate the direction vector from the object to the camera.
        Vector3 lookDirection = mainCameraTransform.position - transform.position;

        // 2. IMPORTANT: Zero out the Y component.
        // This ensures the object only rotates around the vertical axis, 
        // preventing the sprite from tilting up or down to face the camera's height.
        lookDirection.y = 0;

        // 3. Create the rotation (Quaternion) needed to look along that direction vector.
        Quaternion targetRotation = Quaternion.LookRotation(lookDirection);

        // 4. Apply the rotation to the object.
        transform.rotation = targetRotation;
    }
}