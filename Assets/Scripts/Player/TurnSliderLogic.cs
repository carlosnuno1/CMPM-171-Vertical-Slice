using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TurnSliderLogic : MonoBehaviour
{
    public Slider slider;
    public TextMeshProUGUI turnText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        slider.value = VRPhysicsLocomotion.turnSpeedDegrees;
    }

    public void ChangeTurnSpeed()
    {
        VRPhysicsLocomotion.turnSpeedDegrees = slider.value;
        turnText.text = slider.value.ToString();
    }
}
