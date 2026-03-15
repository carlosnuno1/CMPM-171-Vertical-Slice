using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Turning;
using UnityEngine.UI;

public class LocomotionSettings : MonoBehaviour
{
    public ContinuousTurnProvider turnProvider;
    public Slider turnSpeedSlider;

    void Start()
    {
        if (turnProvider != null && turnSpeedSlider != null)
        {
            turnSpeedSlider.value = turnProvider.turnSpeed;
        }
    }

    public void SetTurnSpeed(float value)
    {
        if (turnProvider != null)
        {
            turnProvider.turnSpeed = value;
        }
    }
}