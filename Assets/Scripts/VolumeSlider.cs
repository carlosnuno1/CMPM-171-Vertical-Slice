using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VolumeSlider : MonoBehaviour
{
    public Slider volumeSlider;
    public TextMeshProUGUI volumeText;

    void Awake()
    {
        volumeSlider.value = AudioListener.volume;
    }

    public void SetMasterVolume()
    {
        AudioListener.volume = volumeSlider.value;
        volumeText.text = volumeSlider.value.ToString();
    }
}