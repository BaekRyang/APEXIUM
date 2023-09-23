using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonListener : MonoBehaviour, IPointerEnterHandler
{
    private void Start()
    {
        Button _button = GetComponent<Button>();
        if (_button != null)
        {
            _button.onClick.AddListener(ButtonAction);
        }
        
    }

    public void ButtonAction()
    { 
        EventBus.Publish(new ButtonPressedAction(gameObject.name));
    }

    public void OnPointerEnter(PointerEventData _eventData)
    {
        EventSystem.current.SetSelectedGameObject(gameObject);
    }
}

public class ButtonPressedAction
{
    public readonly string buttonName;

    public ButtonPressedAction(string _buttonName)
    {
        buttonName = _buttonName;
    }
}
