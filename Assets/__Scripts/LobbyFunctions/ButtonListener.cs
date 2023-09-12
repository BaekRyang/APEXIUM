using UnityEngine;

public class ButtonListener : MonoBehaviour
{
    public void ButtonAction()
    { 
        EventBus.Publish(new ButtonPressedAction(gameObject.name));
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
