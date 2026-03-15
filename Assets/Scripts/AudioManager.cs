using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public partial class AudioManager : MonoBehaviour
{
    public AudioMixer mainMixer;

    public void SetMasterVolume(float value)
    {
        mainMixer.SetFloat("MasterVol", Mathf.Log10(value) * 20);
    }

    public void SetBGMVolume(float value)
    {
        mainMixer.SetFloat("BGMVol", Mathf.Log10(value) * 20);
    }

    public void SetSFXVolume(float value)
    {
        mainMixer.SetFloat("SFXVol", Mathf.Log10(value) * 20);
    }
}