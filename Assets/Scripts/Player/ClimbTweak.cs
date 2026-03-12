using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ClimbTweak : MonoBehaviour
{
    public Rigidbody playerRigidbody;                  // Reference to the player's Rigidbody
    public UnityEngine.XR.Interaction.Toolkit.Interactors.XRDirectInteractor leftHandInteractor;      // Reference to the left hand interactor
    public UnityEngine.XR.Interaction.Toolkit.Interactors.XRDirectInteractor rightHandInteractor;     // Reference to the right hand interactor
    public float throwStrengthScalar = 2f;             // Scalar factor for throw strength
    public float smoothClimbSpeed = 2f;                // Speed for smooth climbing

    private bool isClimbing = false;                   // Flag to track if the player is climbing
    private bool leftHandClimbing = false;
    private bool rightHandClimbing = false;

    private Vector3 previousLeftHandPos;               // Previous position of the left hand
    private Vector3 previousRightHandPos;              // Previous position of the right hand

    private Vector3 leftHandVelocity;                  // Left hand velocity for scaling throw strength
    private Vector3 rightHandVelocity;                 // Right hand velocity for scaling throw strength

    // Called when the interactor selects an object (hand grabs an object)
    public void OnSelectEnter(SelectEnterEventArgs args)
    {
        UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable interactable = args.interactableObject as UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable; // Explicit cast to XRBaseInteractable
        if (interactable != null && interactable.CompareTag("Climable")) // Ensure climbable surfaces are tagged correctly
        {
            isClimbing = true;

            if (args.interactorObject == leftHandInteractor)
            {
                leftHandClimbing = true;
                previousLeftHandPos = leftHandInteractor.transform.position;
            }

            if (args.interactorObject == rightHandInteractor)
            {
                rightHandClimbing = true;
                previousRightHandPos = rightHandInteractor.transform.position;
            }
        }
    }

    // Called when the interactor exits an object (hand releases the object)
    public void OnSelectExit(SelectExitEventArgs args)
    {
        UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable interactable = args.interactableObject as UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable; // Explicit cast to XRBaseInteractable
        if (interactable != null && interactable.CompareTag("Climable"))
        {
            if (args.interactorObject == leftHandInteractor)
                leftHandClimbing = false;

            if (args.interactorObject == rightHandInteractor)
                rightHandClimbing = false;

            if (!leftHandClimbing && !rightHandClimbing)
            {
                // Apply an upward force based on the velocity of the hand when releasing the object
                float leftHandThrowStrength = leftHandVelocity.y * throwStrengthScalar; // Scaling by hand velocity
                float rightHandThrowStrength = rightHandVelocity.y * throwStrengthScalar; // Scaling by hand velocity

                // Average the throw strengths from both hands to decide the final throw strength
                float throwStrength = (leftHandThrowStrength + rightHandThrowStrength) / 2;

                // Apply upward force when releasing the climbable object
                playerRigidbody.linearVelocity = new Vector3(playerRigidbody.linearVelocity.x, throwStrength, playerRigidbody.linearVelocity.z);
                isClimbing = false;
            }
        }
    }

    void FixedUpdate()
    {
        if (leftHandClimbing || rightHandClimbing)
        {
            Vector3 movement = Vector3.zero;

            if (leftHandClimbing)
            {
                Vector3 delta = leftHandInteractor.transform.position - previousLeftHandPos;
                leftHandVelocity = delta / Time.deltaTime;
                movement -= delta;
                previousLeftHandPos = leftHandInteractor.transform.position;
            }

            if (rightHandClimbing)
            {
                Vector3 delta = rightHandInteractor.transform.position - previousRightHandPos;
                rightHandVelocity = delta / Time.deltaTime;
                movement -= delta;
                previousRightHandPos = rightHandInteractor.transform.position;
            }

            Vector3 climbVelocity = movement / Time.deltaTime;

            playerRigidbody.linearVelocity = new Vector3(
                climbVelocity.x,
                climbVelocity.y,
                climbVelocity.z
            );
        }
    }

    // Attach the events to the interactors in the inspector or programmatically
    void OnEnable()
    {
        leftHandInteractor.selectEntered.AddListener(OnSelectEnter); // Using the new event system
        leftHandInteractor.selectExited.AddListener(OnSelectExit);  // Using the new event system

        rightHandInteractor.selectEntered.AddListener(OnSelectEnter); // Using the new event system
        rightHandInteractor.selectExited.AddListener(OnSelectExit);  // Using the new event system
    }

    void OnDisable()
    {
        leftHandInteractor.selectEntered.RemoveListener(OnSelectEnter); // Using the new event system
        leftHandInteractor.selectExited.RemoveListener(OnSelectExit);  // Using the new event system

        rightHandInteractor.selectEntered.RemoveListener(OnSelectEnter); // Using the new event system
        rightHandInteractor.selectExited.RemoveListener(OnSelectExit);  // Using the new event system
    }
}