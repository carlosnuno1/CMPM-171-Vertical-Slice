using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit;

public class OpenMenuScript : MonoBehaviour
{
    [Header("UI Settings")]
    [SerializeField] public GameObject menuCanvas;
    [SerializeField] public float menuHeightAdjust;

    [Header("Input Actions")]
    [SerializeField] private InputActionAsset actions;

    [Header("Controls")]
    [SerializeField] public GameObject leftGrapple;
    [SerializeField] public GameObject rightGrapple;
    [SerializeField] public GameObject locomotion;
    [SerializeField] public GameObject leftUIInteractor;
    [SerializeField] public GameObject rightUIInteractor;

    public Transform head;
    private bool isMenuCanvas = false;
    private InputAction openMenuAction;

    void Awake()
    {
        openMenuAction = actions.FindAction("Player/Open Menu", true);
        menuCanvas.SetActive(false);
        leftUIInteractor.SetActive(false);
        rightUIInteractor.SetActive(false);
    }
    private void OnEnable()
    {
        if (openMenuAction != null)
        {
            openMenuAction.performed += openMenuActionPerformed;
            openMenuAction.Enable();
        }
    }

    private void OnDisable()
    {
        if (openMenuAction != null)
        {
            openMenuAction.performed -= openMenuActionPerformed;
            openMenuAction.Disable();
        }
    }

    private void openMenuActionPerformed(InputAction.CallbackContext context)
    {
        ToggleMenu();
    }

    private void ToggleMenu()
    {
        isMenuCanvas = !isMenuCanvas;
        menuCanvas.SetActive(isMenuCanvas);

        if (isMenuCanvas)
        {
            float yaw = head.eulerAngles.y;

            Vector3 forwardDirection = Quaternion.Euler(0, yaw, 0) * Vector3.forward;
            forwardDirection.Normalize();

            Vector3 targetPosition = head.position + (forwardDirection * 1); //adjust closeness
            targetPosition.y = head.position.y + menuHeightAdjust; // adjust height

            menuCanvas.transform.position = targetPosition;
            menuCanvas.transform.rotation = Quaternion.Euler(0, yaw + 180, 0);
            menuCanvas.transform.forward *= -1; // flip forawrd

            locomotion.SetActive(false);
            leftGrapple.SetActive(false);
            rightGrapple.SetActive(false);
            leftUIInteractor.SetActive(true);
            rightUIInteractor.SetActive(true);
        } 
        else 
        {
            locomotion.SetActive(true);
            rightGrapple.SetActive(true);
            leftGrapple.SetActive(true);
            leftUIInteractor.SetActive(false);
            rightUIInteractor.SetActive(false);
        }
    }
}
