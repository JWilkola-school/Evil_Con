using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class BasicPlayerController : MonoBehaviour
{
    public float walkSpeed = 10f;
    public float sprintSpeed = 15f;
    public float rotationSpeed = 10f;

    public Transform cameraTransform;

    private CharacterController controller;
    private Animator anim;
    private float turnVelocity;

    private readonly int speedHash = Animator.StringToHash("Speed");

    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();

        if (anim == null)
        {
            anim = GetComponentInChildren<Animator>();
        }

        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 inputDir = new Vector3(horizontal, 0f, vertical).normalized;
        bool isInputActive = inputDir.magnitude >= 0.1f;
        bool isSprinting = Input.GetKey(KeyCode.LeftShift);

        float moveSpeed = 0f;
        float animValue = 0f;

        if (isInputActive)
        {
            float camY = (cameraTransform != null) ? cameraTransform.eulerAngles.y : 0f;
            float targetAngle = Mathf.Atan2(inputDir.x, inputDir.z) * Mathf.Rad2Deg + camY;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnVelocity, rotationSpeed * Time.deltaTime);

            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            moveSpeed = isSprinting ? sprintSpeed : walkSpeed;
            animValue = isSprinting ? 1f : 0.5f;

            controller.Move(moveDir * moveSpeed * Time.deltaTime);
        }
        else
        {
            animValue = 0f;
        }

        if (anim != null)
        {
            anim.SetFloat(speedHash, animValue);
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // Make sure your enemy prefab has the tag "Enemy" assigned to it in the Inspector!
        if (hit.gameObject.CompareTag("Enemy"))
        {
            // Pass the enemy we hit, and "this.gameObject" (the player)
            BattleTransitioner.InitiateForcedCombat(hit.gameObject, this.gameObject);
        }
    }
}