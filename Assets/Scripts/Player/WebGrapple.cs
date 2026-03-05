using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using System;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class WebShooterGrapple : MonoBehaviour
{
    [Header("Core Variables")]
    public Transform shooterTip;
    public Rigidbody player;
    public GameObject webEnd;
    public LineRenderer lineRenderer;
    [SerializeField] private InputActionAsset actions;
    public XRBaseInteractor interactor;
    public bool leftHand;

    private InputAction trigger;

    [Header("Grapple Settings")]
    public float pullForce = 20f;
    public float maxDistance = 1000f;
    public LayerMask grappleLayers;

    [Header("Prediction")]
    public RaycastHit predictionHit;
    public float predictionSphereCastRadius = 0.5f;
    public Transform predictionPoint;

    private Vector3 grapplePoint;
    private bool grappling;
    private bool isHolding;
    private Vector3 realHitPoint;

    void Awake()
    {
        if (leftHand)
            trigger = actions.FindAction("Player/Left Trigger", true);
        else
            trigger = actions.FindAction("Player/Right Trigger", true);

        trigger.Enable();

        lineRenderer = GetComponent<LineRenderer>();
        webEnd.transform.parent = null;
    }

    void HandleInput()
    {
        float triggerValue = trigger.ReadValue<float>();
        bool isHolding = (interactor as IXRSelectInteractor)?.hasSelection ?? false;

        if (triggerValue > 0 && !grappling && !isHolding)
        {
            ShootGrapple();
        }
        else if ((triggerValue == 0 && grappling) || isHolding)
        {
            StopGrapple();
        }
    }

    void CheckForSwingPoints()
    {
        bool isHolding = (interactor as IXRSelectInteractor)?.hasSelection ?? false;

        RaycastHit sphereCastHit;
        Physics.SphereCast(shooterTip.position, predictionSphereCastRadius, shooterTip.forward, out sphereCastHit, maxDistance, grappleLayers);

        RaycastHit raycastHit;
        Physics.Raycast(shooterTip.position, shooterTip.forward, out raycastHit, maxDistance, grappleLayers);

        if (raycastHit.point != Vector3.zero)
            realHitPoint = raycastHit.point;
        else if (sphereCastHit.point != Vector3.zero)
            realHitPoint = sphereCastHit.point;
        else
            realHitPoint = Vector3.zero;

        if (realHitPoint != Vector3.zero)
        {
            predictionPoint.gameObject.SetActive(true);
            predictionPoint.position = realHitPoint;
        }
        else
        {
            predictionPoint.gameObject.SetActive(false);
        }

        if (isHolding)
            predictionPoint.gameObject.SetActive(false);

        predictionHit = raycastHit.point == Vector3.zero ? sphereCastHit : raycastHit;
    }

    void ShootGrapple()
    {
        if (realHitPoint == Vector3.zero) return;

        grappling = true;
        grapplePoint = predictionHit.point;

        webEnd.transform.position = grapplePoint;
        predictionPoint.gameObject.SetActive(false);
    }

    void StopGrapple()
    {
        grappling = false;
        lineRenderer.positionCount = 0;
    }

    void FixedUpdate()
    {
        if (!grappling) return;

        Vector3 direction = (grapplePoint - player.position).normalized;
        player.AddForce(direction * pullForce, ForceMode.Acceleration);
    }

    void Update()
    {
        HandleInput();
        CheckForSwingPoints();

        if (grappling)
        {
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, shooterTip.position);
            lineRenderer.SetPosition(1, webEnd.transform.position);
        }
    }
}