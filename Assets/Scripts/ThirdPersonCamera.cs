using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;          // Player to follow
    public float distance = 3f;
    public float height = 1.5f;
    public float sensitivity = 200f;
    public float rotationSmoothTime = 0.1f;
    public Vector2 pitchLimits = new Vector2(-30f, 60f);
    public LayerMask collisionMask;   // Walls, terrain, etc.
    public float collisionRadius = 0.3f; // Adjust to fit your camera size
    public float minDistance = 0.5f;  // Minimum camera distance when colliding
    public float moveSmooth = 10f;    // Smooth return when leaving collision

    private float yaw;
    private float pitch;
    private Vector3 currentRotation;
    private Vector3 rotationSmoothVelocity;
    private float currentDistance;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        currentDistance = distance;
    }

    void LateUpdate()
    {
        if (!target) return;

        // Get mouse input
        yaw += Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        pitch -= Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, pitchLimits.x, pitchLimits.y);

        // Smooth camera rotation
        Vector3 targetRotation = new Vector3(pitch, yaw);
        currentRotation = Vector3.SmoothDamp(currentRotation, targetRotation, ref rotationSmoothVelocity, rotationSmoothTime);
        transform.eulerAngles = currentRotation;

        // Desired camera position (no collision yet)
        Vector3 desiredPosition = target.position - transform.forward * distance + Vector3.up * height;

        // Handle camera collision
        Vector3 direction = (desiredPosition - target.position).normalized;
        float targetDist = distance;

        if (Physics.SphereCast(target.position + Vector3.up * height * 0.5f, collisionRadius, direction, out RaycastHit hit, distance, collisionMask))
        {
            // Move camera closer to avoid clipping
            targetDist = Mathf.Clamp(hit.distance, minDistance, distance);
        }

        currentDistance = Mathf.Lerp(currentDistance, targetDist, Time.deltaTime * moveSmooth);

        // Set final position
        transform.position = target.position - transform.forward * currentDistance + Vector3.up * height;
    }

    void OnDrawGizmosSelected()
    {
        // visualize camera collision sphere in editor
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, collisionRadius);
    }
}
