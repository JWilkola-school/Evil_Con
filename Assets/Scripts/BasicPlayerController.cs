using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class BasicPlayerController : MonoBehaviour
{
    public float walkSpeed = 3.5f;
    public float sprintSpeed = 6.5f;
    public float rotationSpeed = 10f;

    public Transform cameraTransform;  // MainCamera transform (assign in inspector)
    private CharacterController controller;

    // Reference to the Animator Controller script (optional)
    private CharacterAnimatorController animatorController;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animatorController = GetComponent<CharacterAnimatorController>();

        Cursor.lockState = CursorLockMode.Locked;

        if (animatorController == null)
            Debug.LogWarning("CharacterAnimatorController script is missing from GameObject (optional).");

        if (controller == null)
            Debug.LogError("CharacterController component missing - RequireComponent should ensure this exists.");

        if (cameraTransform == null)
            Debug.LogWarning("Camera Transform not assigned on BasicPlayerController. Movement will be world-relative instead of camera-relative.");
    }

    void Update()
    {
        // Read input using the legacy Input system (keep this if you're not using the new Input System)
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 inputDir = new Vector3(horizontal, 0f, vertical);
        float inputMagnitude = inputDir.magnitude;

        float currentSpeed = 0f; // Speed to be passed to the animator

        // If small/no input, do nothing movement-wise but still update animator.
        if (inputMagnitude >= 0.1f)
        {
            // Compute targetAngle relative to camera if camera assigned, otherwise world-forward
            float camY = (cameraTransform != null) ? cameraTransform.eulerAngles.y : 0f;
            float targetAngle = Mathf.Atan2(inputDir.x, inputDir.z) * Mathf.Rad2Deg + camY;

            // Rotation smoothing:
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnVelocity, Mathf.Max(0.001f, rotationSpeed * Time.deltaTime));
            if (!float.IsFinite(angle))
            {
                Debug.LogError($"!!CRASH-DETECT!! Smoothed angle is invalid: {angle}. Aborting movement this frame.");
                return;
            }
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            // Compute move direction from targetAngle
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            // Protect against zero/invalid vector
            if (moveDir == Vector3.zero || !float.IsFinite(moveDir.x) || !float.IsFinite(moveDir.y) || !float.IsFinite(moveDir.z))
            {
                Debug.LogError($"!!CRASH-DETECT!! moveDir invalid: {moveDir}. Aborting movement this frame.");
                return;
            }

            // Calculate final movement speed
            float targetMoveSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;

            // Sanity clamp on speed
            if (!float.IsFinite(targetMoveSpeed) || targetMoveSpeed < 0f || targetMoveSpeed > 1000f)
            {
                Debug.LogError($"!!CRASH-DETECT!! targetMoveSpeed invalid: {targetMoveSpeed}. Clamping to safe range.");
                targetMoveSpeed = Mathf.Clamp(targetMoveSpeed, 0f, 100f);
            }

            // Compose movement vector and verify it's finite
            Vector3 movement = moveDir.normalized * targetMoveSpeed * Time.deltaTime;
            if (!IsVectorFinite(movement))
            {
                Debug.LogError($"!!CRASH-DETECT!! Movement contained NaN/Inf: {movement}. Aborting movement this frame.");
                return;
            }

            // Move the character
            controller.Move(movement);

            // Set speed for animation
            currentSpeed = targetMoveSpeed;
        }
        else
        {
            // no input -> optionally stop rotation smoothing velocity to avoid drift
            // turnVelocity = 0f; // uncomment if needed
        }

        // Always pass a validated speed to animator (if present) - multiplied by input magnitude
        if (animatorController != null)
        {
            float animSpeed = currentSpeed * inputMagnitude;
            if (!float.IsFinite(animSpeed))
            {
                Debug.LogError($"!!CRASH-DETECT!! animSpeed invalid: {animSpeed}. Setting to 0.");
                animSpeed = 0f;
            }
            animatorController.SetMovementSpeed(animSpeed);
        }
    }

    private float turnVelocity;

    private static bool IsVectorFinite(Vector3 v)
    {
        return float.IsFinite(v.x) && float.IsFinite(v.y) && float.IsFinite(v.z);
    }
}
