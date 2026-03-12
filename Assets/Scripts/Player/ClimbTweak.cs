using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ClimbTweak : MonoBehaviour
{
    public Rigidbody playerRigidbody;
    public UnityEngine.XR.Interaction.Toolkit.Interactors.XRDirectInteractor leftHandInteractor;
    public UnityEngine.XR.Interaction.Toolkit.Interactors.XRDirectInteractor rightHandInteractor;

    [Header("Settings")]
    public float throwStrengthScalar = 1.5f;
    public float climbStrength = 1.0f; // Multiplier for pull speed

    private bool leftHandClimbing = false;
    private bool rightHandClimbing = false;

    private Vector3 previousLeftHandPos;
    private Vector3 previousRightHandPos;

    private Vector3 leftHandVelocity;
    private Vector3 rightHandVelocity;

    public void OnSelectEnter(SelectEnterEventArgs args)
    {
        var interactable = args.interactableObject as UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable;
        if (interactable != null && interactable.CompareTag("Climable"))
        {
            if (args.interactorObject == leftHandInteractor)
            {
                leftHandClimbing = true;
                previousLeftHandPos = leftHandInteractor.transform.position;
            }
            else if (args.interactorObject == rightHandInteractor)
            {
                rightHandClimbing = true;
                previousRightHandPos = rightHandInteractor.transform.position;
            }

            // Disable gravity while climbing to prevent sliding down
            playerRigidbody.useGravity = false;
        }
    }

    public void OnSelectExit(SelectExitEventArgs args)
    {
        if (args.interactorObject == leftHandInteractor)
            leftHandClimbing = false;
        else if (args.interactorObject == rightHandInteractor)
            rightHandClimbing = false;

        if (!leftHandClimbing && !rightHandClimbing)
        {
            playerRigidbody.useGravity = true;

            // Calculate "Throw" velocity
            Vector3 releaseVelocity = (leftHandVelocity + rightHandVelocity) * -throwStrengthScalar;

            // Limit the throw so you don't fly into orbit
            playerRigidbody.linearVelocity = releaseVelocity;
        }
    }

    void FixedUpdate() // Using FixedUpdate for Rigidbody physics
    {
        if (leftHandClimbing || rightHandClimbing)
        {
            Vector3 totalDelta = Vector3.zero;

            if (leftHandClimbing)
            {
                Vector3 delta = leftHandInteractor.transform.position - previousLeftHandPos;
                leftHandVelocity = delta / Time.fixedDeltaTime;
                totalDelta -= delta;
                previousLeftHandPos = leftHandInteractor.transform.position;
            }

            if (rightHandClimbing)
            {
                Vector3 delta = rightHandInteractor.transform.position - previousRightHandPos;
                rightHandVelocity = delta / Time.fixedDeltaTime;
                totalDelta -= delta;
                previousRightHandPos = rightHandInteractor.transform.position;
            }

            // Move the player using velocity so Colliders actually block movement
            playerRigidbody.linearVelocity = (totalDelta * climbStrength) / Time.fixedDeltaTime;
        }
    }

    void OnEnable()
    {
        leftHandInteractor.selectEntered.AddListener(OnSelectEnter);
        leftHandInteractor.selectExited.AddListener(OnSelectExit);
        rightHandInteractor.selectEntered.AddListener(OnSelectEnter);
        rightHandInteractor.selectExited.AddListener(OnSelectExit);
    }

    void OnDisable()
    {
        leftHandInteractor.selectEntered.RemoveListener(OnSelectEnter);
        leftHandInteractor.selectExited.RemoveListener(OnSelectExit);
        rightHandInteractor.selectEntered.RemoveListener(OnSelectEnter);
        rightHandInteractor.selectExited.RemoveListener(OnSelectExit);
    }
}