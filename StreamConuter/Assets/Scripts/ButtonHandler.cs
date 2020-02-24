using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonHandler : MonoBehaviour
{
    private AppManager manager;

    [SerializeField] private Animator settingsAnim;
    [SerializeField, HideInInspector] private int fontSize = 72;

    [Header("Color Picker")]
    [SerializeField] private Button openColorBtn;
    [SerializeField] private GameObject colorPicker;
    [SerializeField] private TMP_InputField colorHexValue;

    private void Start()
    {
        manager = AppManager.Instance;

        if (PlayerPrefs.HasKey(manager.fontSizeKey))
        {
            fontSize = PlayerPrefs.GetInt(manager.fontSizeKey);
            manager.counterText.fontSize = fontSize;
        }

        if (PlayerPrefs.HasKey(manager.colorKey))
        {
            string colorString = PlayerPrefs.GetString(manager.colorKey);

            string[] values = colorString.Split(',');

            manager.backgroundColor = new Color(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2]), float.Parse(values[3]));
            FlexibleColorPicker.startingColor = manager.backgroundColor;
            manager.background.color = manager.backgroundColor;
        }
    }

    public void OpenSettings()
    {
        StartCoroutine(ActionAfterAnim(settingsAnim, "openSettings", true));

        manager.keybindInput.text = manager.keybind;

        openColorBtn.GetComponent<Image>().color = manager.backgroundColor;

        string hexValue = string.Format("{0:X2}", (int)Mathf.Ceil(manager.backgroundColor.r * 255)) +
                        string.Format("{0:X2}", (int)Mathf.Ceil(manager.backgroundColor.g * 255)) +
                        string.Format("{0:X2}", (int)Mathf.Ceil(manager.backgroundColor.b * 255));

        hexValue = "#" + hexValue;

        colorHexValue.text = hexValue;

        colorPicker.SetActive(false);
    }

    public void CloseSettings()
    {
        CloseColorPicker();
        ApplySettings();
        StartCoroutine(ActionAfterAnim(settingsAnim, "closeSettings", false));
    }

    public void ApplySettings()
    {
        manager.background.color = manager.backgroundColor;
    }

    IEnumerator ActionAfterAnim(Animator animator, string anim, bool active)
    {
        if (active)
        {
            animator.gameObject.SetActive(active);

            animator.Play(anim);
        }
        else
        {
            animator.Play(anim);
            yield return new WaitForSeconds(0.15f);
            animator.gameObject.SetActive(active);
        }
    }

    public void StartEditKeybind()
    {
        manager.RecordKeybinding();
    }

    public void EndEditKeybind()
    {
        manager.SaveKeybinding();
    }

    #region ColorPicker
    public void OpenColorPicker()
    {
        colorPicker.SetActive(true);
    }

    public void CloseColorPicker()
    {
        string hex = colorHexValue.text;
        float r = 0f, g = 0f, b = 0f;

        hex = hex.Replace("#", "");

        r = ((float)int.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber)) / 255;
        g = ((float)int.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber)) / 255;
        b = ((float)int.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber)) / 255;

        Color color = new Color(r, g, b);

        manager.backgroundColor = color;
        openColorBtn.GetComponent<Image>().color = color;
        colorPicker.SetActive(false);
    }
    #endregion

    #region FontSize
    public void SetFontSize(string input)
    {
        if (input != null && input != "")
        {
            fontSize = int.Parse(input);
            manager.counterText.fontSize = fontSize;
        }
    }
    #endregion

    private void OnApplicationQuit()
    {
        string colorString = string.Format($"{manager.backgroundColor.r},{manager.backgroundColor.g},{manager.backgroundColor.b},{manager.backgroundColor.a}");

        PlayerPrefs.SetString(manager.colorKey, colorString);
        PlayerPrefs.SetInt(manager.fontSizeKey, fontSize);
    }
}
