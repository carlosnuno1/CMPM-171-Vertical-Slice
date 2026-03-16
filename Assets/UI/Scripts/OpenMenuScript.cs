using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OpenMenuScript : MonoBehaviour
{
    [Header("UI Settings")]
    [SerializeField] public GameObject menuCanvas;
    [SerializeField] public GameObject watchEmpty;
    [SerializeField] public GameObject generalEmpty;
    [SerializeField] public GameObject controlsEmpty;
    [SerializeField] public GameObject accessiblityEmpty;
    [SerializeField] public GameObject mainMenuEmpty;
    [SerializeField] public GameObject settingsEmpty;
    [SerializeField] public GameObject creditsEmpty;
    [SerializeField] public GameObject deathPanelEmpty;
    [SerializeField] public GameObject levelOneEmpty;

    [Header("Input Actions")]
    [SerializeField] private InputActionReference openMenuAction;

    [Header("Controls To Turn OFF")]
    [SerializeField] public GameObject leftGrapple;
    [SerializeField] public GameObject rightGrapple;
    [SerializeField] public GameObject leftUIInteractor;
    [SerializeField] public GameObject rightUIInteractor;

    public Transform head;
    private bool isMenuCanvas = false;

    void start()
    {
        ToggleMenu();
    }

    private void OnEnable()
    {
        if (openMenuAction != null)
        {
            openMenuAction.action.performed += openMenuActionPerformed;
            openMenuAction.action.Enable();
        }
    }

    private void OnDisable()
    {
        if (openMenuAction != null)
        {
            openMenuAction.action.performed -= openMenuActionPerformed;
            openMenuAction.action.Disable();
        }
    }

    private void openMenuActionPerformed(InputAction.CallbackContext context)
    {
        ToggleMenu();
    }

    public void ToggleMenu()
    {
        isMenuCanvas = !isMenuCanvas;
        menuCanvas.SetActive(isMenuCanvas);

        if (isMenuCanvas)
        {
            PanelsOff();

            float yaw = head.eulerAngles.y;

            Vector3 forwardDirection = Quaternion.Euler(0, yaw, 0) * Vector3.forward;
            forwardDirection.Normalize();

            Vector3 targetPosition = head.position + (forwardDirection * 1); //adjust closeness
            targetPosition.y = head.position.y + 1f; // adjust height

            menuCanvas.transform.position = targetPosition;
            menuCanvas.transform.rotation = Quaternion.Euler(0, yaw + 180, 0);
            menuCanvas.transform.forward *= -1; // flip forawrd

            Debug.Log("canvas on controlls off");
            rightGrapple.SetActive(false);
            leftGrapple.SetActive(false);
            rightUIInteractor.SetActive(true);
            leftUIInteractor.SetActive(true);
        } else 
        {
            Debug.Log("canvas off controlls on");
            rightGrapple.SetActive(true);
            leftGrapple.SetActive(true);
            rightUIInteractor.SetActive(false);
            leftUIInteractor.SetActive(false);
        }
    }

    private void PanelsOff()
    {
        controlsEmpty.SetActive(false);
        watchEmpty.SetActive(true);
        generalEmpty.SetActive(false);
        settingsEmpty.SetActive(false);
        accessiblityEmpty.SetActive(false);
        mainMenuEmpty.SetActive(false);
        creditsEmpty.SetActive(false);
        deathPanelEmpty.SetActive(false);
        levelOneEmpty.SetActive(false);

    }

}
