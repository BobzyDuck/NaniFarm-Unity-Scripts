using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float rotationSmoothTime = 0.08f;

    [Header("Gravity")]
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float gravityMultiplier = 3f;

    [Header("Camera")]
    [SerializeField] private Transform cameraTransform;

    private CharacterController characterController;
    private Vector3 moveDirection;
    private float verticalVelocity;
    private float currentRotationVelocity;

    public float Speed
    {
        get => speed;
        set => speed = value;
    }

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        ReadMovementInput();
        ApplyGravity();
        ApplyMovement();
        ApplyRotation();
    }

    private void ReadMovementInput()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 inputDirection = new Vector3(horizontal, 0f, vertical).normalized;

        if (inputDirection.sqrMagnitude < 0.01f)
        {
            moveDirection = Vector3.zero;
            return;
        }

        if (cameraTransform != null)
        {
            Vector3 cameraForward = cameraTransform.forward;
            Vector3 cameraRight = cameraTransform.right;

            cameraForward.y = 0f;
            cameraRight.y = 0f;

            cameraForward.Normalize();
            cameraRight.Normalize();

            moveDirection = cameraForward * inputDirection.z + cameraRight * inputDirection.x;
            moveDirection.Normalize();
        }
        else
        {
            // حالت ساده برای دوربین ایزومتریک ثابت
            moveDirection = Quaternion.Euler(0f, 45f, 0f) * inputDirection;
        }
    }

    private void ApplyGravity()
    {
        if (characterController.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -1f;
        }
        else
        {
            verticalVelocity += gravity * gravityMultiplier * Time.deltaTime;
        }
    }

    private void ApplyMovement()
    {
        Vector3 finalMovement = moveDirection * speed;
        finalMovement.y = verticalVelocity;

        characterController.Move(finalMovement * Time.deltaTime);
    }

    private void ApplyRotation()
    {
        if (moveDirection.sqrMagnitude < 0.01f)
            return;

        float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;

        float smoothAngle = Mathf.SmoothDampAngle(
            transform.eulerAngles.y,
            targetAngle,
            ref currentRotationVelocity,
            rotationSmoothTime
        );

        transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);
    }

    public void MoveToPoint(Vector3 targetPosition)
    {
        characterController.enabled = false;

        Vector3 directionToTarget = targetPosition - transform.position;
        directionToTarget.y = 0f;

        if (directionToTarget.sqrMagnitude > 0.01f)
        {
            float targetAngle = Mathf.Atan2(directionToTarget.x, directionToTarget.z) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);
        }

        transform.position = targetPosition;

        verticalVelocity = 0f;

        characterController.enabled = true;
    }
}
