using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using MoreMountains.Feedbacks;
using UnityEngine;

public class Lobby : MonoBehaviour
{
    [SerializeField] private RectTransform entranceUI;
    [SerializeField] private RectTransform mainUI;
    [SerializeField] private RectTransform characterSelectUI;
    [SerializeField] private RectTransform multiplayerUI;

    private void Awake()
    {
        EventBus.Subscribe<ButtonPressedAction>(OnButtonPressed);
    }

    private void OnButtonPressed(ButtonPressedAction _obj)
    {
        switch (_obj.buttonName)
        {
            case "Entrance":
                Entrance();
                break;
            
            case "StartGame":
                SingleCharacterSelect();
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
        }
    }

    private void Entrance()
    {
        LerpToScene(entranceUI, mainUI);
    }

    private void SingleCharacterSelect()
    {
        LerpToScene(mainUI, characterSelectUI);
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

    private async UniTaskVoid LerpToScene(RectTransform _previous, RectTransform _next)
    {
        Vector3  _position = transform.position;
        
        Task _task     = _previous.GetComponent<MMF_Player>().PlayFeedbacksTask(_position);
        await _task;
        _previous.gameObject.SetActive(false);
        
        _next.gameObject.SetActive(true);
        _task = _next.GetComponent<MMF_Player>().PlayFeedbacksTask(_position);
        await _task;
    }
}