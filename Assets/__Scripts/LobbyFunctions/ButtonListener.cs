using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonListener : MonoBehaviour, IPointerEnterHandler
{
    private void Awake()
    {
        Button _button = GetComponent<Button>();
        if (_button != null)
        {
            _button.onClick.AddListener(ButtonAction);
        }
    }

    public void ButtonAction()
    {
        EventBus.Publish(new ButtonPressedAction(GetComponent<Button>()));
    }

    public void OnPointerEnter(PointerEventData _eventData)
    {
        EventSystem.current.SetSelectedGameObject(gameObject);
    }
}

public class ButtonPressedAction
{
    public readonly  Button button;
    private readonly string _buttonText; //버튼이 아니고 이름만 주어질 때 사용함

    public string ButtonName => button != null ? button.name : _buttonText;

    public ButtonPressedAction(Button _button)
    {
        button = _button;
    }

    public ButtonPressedAction(string _buttonText)
    {
        this._buttonText = _buttonText;
    }
}