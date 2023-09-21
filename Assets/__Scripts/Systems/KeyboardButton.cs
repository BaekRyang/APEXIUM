using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class KeyboardButton : MonoBehaviour
{
    [SerializeField] private bool    useAnyKey   = true;
    [SerializeField] private KeyCode keyCode     = KeyCode.None;
    [SerializeField] private float   repeatDelay = 1f;
    
    private float _lastPressTime = 0f;

    [Space(10)]
    
    [SerializeField] private UnityEvent onPress = new();
    private void Update()
    {
        if (Time.time < 1f) return;
        
        if (Time.time - _lastPressTime < repeatDelay) return;

        if (useAnyKey)
        {
            if (Input.anyKeyDown)
                InvokeMethod();
        }
        else
        {
            if (Input.GetKeyDown(keyCode))
                InvokeMethod();
        }
    }

    private void InvokeMethod()
    {
        _lastPressTime = Time.time;
        onPress.Invoke();
    }

    
}