using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

using UnityEngine.Serialization;

public class GeneralScript : MonoBehaviour
{
    [Header("General Buttons")]
    [SerializeField] private Button englishButton;
    [SerializeField] private Button spanishButton;
    [SerializeField] private Button japaneseButton;

    [Header("Input Actions")]
    [SerializeField] private InputActionReference submitAction;

    private const string PREF_LOCALE = "selected_locale_code";

    private Button selectedLanguageButton;
    private ColorBlock originalEnglishColors;
    private ColorBlock originalSpanishColors;
    private ColorBlock originalJapaneseColors;

    private void Start()
    {
        if (englishButton != null) originalEnglishColors = englishButton.colors;
        if (spanishButton != null) originalSpanishColors = spanishButton.colors;
        if (japaneseButton != null) originalJapaneseColors = japaneseButton.colors;

        if (englishButton != null) englishButton.onClick.AddListener(() => SelectLanguage(englishButton, "en"));
        if (spanishButton != null) spanishButton.onClick.AddListener(() => SelectLanguage(spanishButton, "es"));
        if (japaneseButton != null) japaneseButton.onClick.AddListener(() => SelectLanguage(japaneseButton, "ja"));

        string savedLocale = PlayerPrefs.GetString(PREF_LOCALE, "en");

        if (savedLocale == "es" && spanishButton != null)
            SelectLanguage(spanishButton, "es", applyLocale: true);
        else if (savedLocale == "ja" && japaneseButton != null)
            SelectLanguage(japaneseButton, "ja", applyLocale: true);
        else if (englishButton != null)
            SelectLanguage(englishButton, "en", applyLocale: true);

        if (englishButton != null && EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(englishButton.gameObject);
        }
    }

    private void AutoSelect()
    {
        EventSystem.current.SetSelectedGameObject(null);
        englishButton.Select();
        EventSystem.current.SetSelectedGameObject(englishButton.gameObject);
    }

    private void OnEnable()
    {
        if (submitAction != null)
        {
            submitAction.action.performed += OnSubmit;
            submitAction.action.Enable();
        }
        AutoSelect();
    }

    private void OnDisable()
    {
        if (submitAction != null)
        {
            submitAction.action.performed -= OnSubmit;
            submitAction.action.Disable();
        }
    }

    private void OnSubmit(InputAction.CallbackContext context)
    {
        if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject != null)
        {
            Button currentButton = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
            if (currentButton != null)
            {
                currentButton.onClick.Invoke();
            }
        }
    }

    private void SelectLanguage(Button pressedButton, string localeCode, bool applyLocale = true)
    {
        if (pressedButton == selectedLanguageButton && applyLocale == false)
            return;

        if (englishButton != null) ResetButtonColor(englishButton, originalEnglishColors);
        if (spanishButton != null) ResetButtonColor(spanishButton, originalSpanishColors);
        if (japaneseButton != null) ResetButtonColor(japaneseButton, originalJapaneseColors);

        ColorBlock pressedColors = pressedButton.colors;
        pressedColors.normalColor = pressedColors.pressedColor;
        pressedButton.colors = pressedColors;

        selectedLanguageButton = pressedButton;
        if (applyLocale)
        {
            PlayerPrefs.SetString(PREF_LOCALE, localeCode);
            PlayerPrefs.Save();
            StartCoroutine(SetLocale(localeCode));
        }
    }

    private void ResetButtonColor(Button button, ColorBlock originalColors)
    {
        button.colors = originalColors;
    }

    private IEnumerator SetLocale(string localeCode)
    {
        yield return LocalizationSettings.InitializationOperation;

        Locale target = null;

        foreach (var locale in LocalizationSettings.AvailableLocales.Locales)
        {
            if (locale != null && locale.Identifier.Code == localeCode)
            {
                target = locale;
                break;
            }
        }

        if (target != null)
            LocalizationSettings.SelectedLocale = target;
        else
            Debug.LogWarning("Locale not found: " + localeCode);
    }

    public void openGeneralEmpty()
    {
        // generalEmpty.SetActive(true);
        // generalEmpty.SetActive(false);
    }

}
