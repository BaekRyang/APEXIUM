using System;
using UnityEngine;

public class Lobby : MonoBehaviour
{
    private void Awake()
    {
        EventBus.Subscribe<ButtonPressedAction>(OnButtonPressed);
    }

    private void OnButtonPressed(ButtonPressedAction _obj)
    {
        switch (_obj.buttonName)
        {
            case "StartGame":
                StartGame();
                break;
            
            case "MultiPlayer":
                OpenMultiplayer();
                break;
            
            case "Settings":
                OpenSettings();
                break;
            
            case "Quit":
                QuitGame();
                break;
            
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void StartGame()
    {
        throw new NotImplementedException();
    }

    private void OpenMultiplayer()
    {
        throw new NotImplementedException();
    }

    private void OpenSettings()
    {
        throw new NotImplementedException();
    }

    private void QuitGame()
    {
        throw new NotImplementedException();
    }
}
