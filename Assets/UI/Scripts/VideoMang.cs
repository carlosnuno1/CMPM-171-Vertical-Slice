using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Video;

public class SimpleVideoDirect : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public VideoClip englishVid;
    public VideoClip spanishVid;
    public VideoClip japaneseVid;

    private void Start()
    {
        UpdateVideoForLanguage(LocalizationSettings.SelectedLocale);
        LocalizationSettings.SelectedLocaleChanged += UpdateVideoForLanguage;
    }

    private void UpdateVideoForLanguage(Locale locale)
    {
        string languageCode = locale.Identifier.Code;

        if (languageCode == "en" && englishVid != null)
        {
            videoPlayer.clip = englishVid;
        }
        else if (languageCode == "es" && spanishVid != null)
        {
            videoPlayer.clip = spanishVid;
        }
        else if (languageCode == "ja" && japaneseVid != null)
        {
            videoPlayer.clip = japaneseVid;
        }

        videoPlayer.Play();
    }

    private void OnDestroy()
    {
        LocalizationSettings.SelectedLocaleChanged -= UpdateVideoForLanguage;
    }
}
