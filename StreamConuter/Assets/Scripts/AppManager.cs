using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityRawInput;
using System.Linq;

public class AppManager : MonoBehaviour
{
    public static AppManager Instance { get; private set; }

    [SerializeField] public string keybind = "F9";
    [SerializeField] public string keybindSaveKey = "Keybind";
    [SerializeField] public string counterSaveKey = "Counter";
    [SerializeField] public string fontSizeKey = "FontSize";
    [SerializeField] public string colorKey = "Color";
    [SerializeField] public string hasTargetKey = "hasTargetValue";
    [SerializeField] public string targetValueKey = "TargetValue";
    [SerializeField] public int increaseAmmount = 1;
    [SerializeField] public int fontSize = 72;
    [SerializeField] public Color backgroundColor = Color.green;
    [SerializeField] public bool hasTarget = false;
    [SerializeField] public int targetValue = 0;

    [Header("UI Elements")]
    [SerializeField] public Image background;
    [SerializeField] public TextMeshProUGUI counterText;
    [SerializeField] public TMP_InputField keybindInput;
    [SerializeField] public TMP_InputField targetValueField;
    [SerializeField] public Toggle hasTargetToggle;

    [SerializeField] private CounterScript activeCounter;

    private bool captureKeyInBackground = true;
    private bool capturingKeybind = false;
    private List<RawKey> keybindKeys;
    private List<RawKey> inputKeyStrokes;

    private RawKey[] functionKeys = { RawKey.RightControl, RawKey.LeftControl, RawKey.RightShift, RawKey.LeftShift,
                                        RawKey.RightMenu, RawKey.LeftMenu };

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(this);

        RawKeyInput.Start(captureKeyInBackground);

        RawKeyInput.OnKeyDown += HandleKeyDown;
        RawKeyInput.OnKeyUp += HandleKeyUp;

        keybindKeys = new List<RawKey>();
        inputKeyStrokes = new List<RawKey>();

        if (PlayerPrefs.HasKey(keybindSaveKey))
        {
            keybind = PlayerPrefs.GetString(keybindSaveKey);
        }
    }

    private void HandleKeyDown(RawKey key)
    {
        if (capturingKeybind)
        {
            RawKey[] nonFuctionKeys = keybindKeys.FindAll(x => !functionKeys.Contains(x)).ToArray();

            if (nonFuctionKeys.Length > 0) //functionKeys.Contains(key) == false
            {
                if (functionKeys.Contains(key) == false && keybindKeys.Contains(key) == false) //nonFuctionKeys.Length <= 0
                    keybindKeys.Add(key);
            }
            else if (keybindKeys.Contains(key) == false)
            {
                keybindKeys.Add(key);
            }

            keybindInput.text = KeystrokesToString(keybindKeys);
        }
        else
        {
            inputKeyStrokes.Add(key);

            //Call Increase counter func
            string pressedKeys = KeystrokesToString(inputKeyStrokes);
            if (pressedKeys.Equals(keybind))
            {
                Debug.Log("Key Match");
                activeCounter.IncreaseCounter();
            }
        }
    }

    private void HandleKeyUp(RawKey key)
    {
        if (capturingKeybind)
        {
            RawKey[] nonFuctionKeys = keybindKeys.FindAll(x => !functionKeys.Contains(x)).ToArray();

            if (keybindKeys.Contains(key) && nonFuctionKeys.Length > 0)
            {
                SaveKeybinding();
            }
            else
            {
                keybindKeys.Clear();
                keybindInput.text = KeystrokesToString(keybindKeys);
            }
        }
        else
        {
            if (inputKeyStrokes.Contains(key))
            {
                inputKeyStrokes.Remove(key);
            }
        }
    }

    public void RecordKeybinding()
    {
        capturingKeybind = true;
        keybindKeys.Clear();
    }

    public void SaveKeybinding()
    {
        capturingKeybind = false;
        keybind = KeystrokesToString(keybindKeys);
    }

    private string KeystrokesToString(List<RawKey> KeyStrokes)
    {
        string keyStrokes = "";

        for (int i = 0; i < KeyStrokes.Count; i++)
        {
            keyStrokes += KeyStrokes[i].ToString();

            if (i != KeyStrokes.Count - 1)
            {
                keyStrokes += "+";
            }
        }

        return keyStrokes;
    }

    public void ApplyHasTarget()
    {
        activeCounter.ToggleHasTarget(hasTarget);
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetString(keybindSaveKey, keybind);
    }

    private void OnApplicationQuit()
    {
        SaveSettings();

        RawKeyInput.OnKeyDown -= HandleKeyDown;
        RawKeyInput.OnKeyUp -= HandleKeyUp;

        RawKeyInput.Stop();
    }
}
