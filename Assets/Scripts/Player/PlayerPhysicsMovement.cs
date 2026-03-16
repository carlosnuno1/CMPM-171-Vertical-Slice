using UnityEngine;
using UnityEngine.InputSystem;

public class VRPhysicsLocomotion : MonoBehaviour
{
    public Rigidbody rb;
    public Transform headYawSource;

    public float moveForce = 30f;
    public float maxSpeed = 5f;
    public float airControlModifier = 0.5f;
    public float turnSpeedDegrees = 90f;
    public float turnDeadzone = 0.2f;

    public InputActionProperty movementAxis;
    public InputActionProperty turnAxis;

    private Vector2 moveInput;
    private float turnInput;
    private float modifiedMoveForce;

    void Update()
    {
        moveInput = movementAxis.action.ReadValue<Vector2>();

        float rawTurn = turnAxis.action.ReadValue<Vector2>().x;
        turnInput = Mathf.Abs(rawTurn) > turnDeadzone ? rawTurn : 0f;
    }

    void FixedUpdate()
    {
        HandleMovement();
        HandleTurning();
    }

    void HandleMovement()
    {
        Vector3 forward = headYawSource.forward;
        Vector3 right = headYawSource.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        Vector3 moveDir = forward * moveInput.y + right * moveInput.x;

        modifiedMoveForce = VRHoverController.grounded
            ? moveForce
            : moveForce * airControlModifier;

        if (moveDir.sqrMagnitude > 0.001f)
        {
            rb.AddForce(moveDir.normalized * modifiedMoveForce, ForceMode.Acceleration);
        }

        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        if (flatVel.magnitude > maxSpeed)
        {
            flatVel = flatVel.normalized * maxSpeed;
            rb.linearVelocity = new Vector3(flatVel.x, rb.linearVelocity.y, flatVel.z);
        }
    }

    void HandleTurning()
    {
        if (Mathf.Abs(turnInput) < 0.001f)
            return;

        float deltaYaw = turnInput * turnSpeedDegrees * Time.fixedDeltaTime;
        Quaternion yawRotation = Quaternion.Euler(0f, deltaYaw, 0f);

        rb.MoveRotation(rb.rotation * yawRotation);

        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        Vector3 yVel = new Vector3(0f, rb.linearVelocity.y, 0f);
        flatVel = yawRotation * flatVel;
        rb.linearVelocity = flatVel + yVel;
    }
}