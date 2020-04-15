using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityRawInput;

[RequireComponent(typeof(TextMeshProUGUI))]
public class CounterScript : MonoBehaviour
{
    public TextMeshProUGUI text;

    private AppManager manager;

    [SerializeField, HideInInspector] private int counter = 0;
    private bool workInBackground = true;

    private void Awake()
    {
        //RawKeyInput.Start(workInBackground);
        text = GetComponent<TextMeshProUGUI>();
    }

    void Start()
    {
        manager = AppManager.Instance;

        counter = PlayerPrefs.HasKey(manager.counterSaveKey) ? PlayerPrefs.GetInt(manager.counterSaveKey) : 0;
        text.text = counter.ToString();
    }

    public void IncreaseCounter()
    {
        counter++;

        if (manager.hasTarget)
        {
            text.text = string.Format($"{counter.ToString()} / {manager.targetValue.ToString()}");
        }
        else
        {
            text.text = counter.ToString();
        }
    }

    public void ResetCounter()
    {
        counter = 0;

        if (manager.hasTarget)
        {
            text.text = string.Format($"{counter.ToString()} / {manager.targetValue.ToString()}");
        }
        else
        {
            text.text = counter.ToString();
        }

        PlayerPrefs.SetInt(manager.counterSaveKey, counter);
    }

    public void ToggleHasTarget(bool hasTarget)
    {
        if (hasTarget)
        {
            text.text = string.Format($"{counter.ToString()} / {manager.targetValue.ToString()}");
        }
        else
        {
            text.text = counter.ToString();
        }
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt(manager.counterSaveKey, counter);
    }
}
