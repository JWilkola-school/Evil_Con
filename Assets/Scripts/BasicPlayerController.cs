using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class BasicPlayerController : MonoBehaviour
{
    [Header("Speeds")]
    public float walkSpeed = 3.5f;
    public float sprintSpeed = 6.5f;

    [Header("Settings")]
    public float rotationSmoothTime = 0.1f;
    public Transform cameraTransform;

    private CharacterController controller;
    private CharacterAnimatorController animCtrl;
    private float turnVelocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animCtrl = GetComponent<CharacterAnimatorController>();

        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 inputDir = new Vector3(h, 0f, v).normalized;

        float currentSpeed = 0f;

        if (inputDir.magnitude >= 0.1f)
        {
            float camY = (cameraTransform != null) ? cameraTransform.eulerAngles.y : 0f;
            float targetAngle = Mathf.Atan2(inputDir.x, inputDir.z) * Mathf.Rad2Deg + camY;

            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnVelocity, rotationSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir * currentSpeed * Time.deltaTime);
        }

        if (animCtrl != null)
        {
            animCtrl.SetMovementSpeed(currentSpeed);
        }
    }
}
