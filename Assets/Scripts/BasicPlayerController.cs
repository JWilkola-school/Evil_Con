using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class BasicPlayerController : MonoBehaviour
{
    public float walkSpeed = 3.5f;
    public float sprintSpeed = 6.5f;
    public float rotationSpeed = 10f;

    public Transform cameraTransform;
    private CharacterController controller;
    private Animator anim;
    private float turnVelocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 inputDir = new Vector3(horizontal, 0f, vertical);
        float inputMagnitude = inputDir.magnitude;

        bool isInputActive = inputMagnitude >= 0.1f;
        bool isSprinting = Input.GetKey(KeyCode.LeftShift);

        if (isInputActive)
        {
            float camY = (cameraTransform != null) ? cameraTransform.eulerAngles.y : 0f;
            float targetAngle = Mathf.Atan2(inputDir.x, inputDir.z) * Mathf.Rad2Deg + camY;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnVelocity, Mathf.Max(0.001f, rotationSpeed * Time.deltaTime));

            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            float speed = isSprinting ? sprintSpeed : walkSpeed;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }

        if (anim != null)
        {
            anim.SetBool("isWalking", isInputActive);
            anim.SetBool("IsRunning", isInputActive && isSprinting);
        }
    }
}