using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.InputSystem.UI;

public class Lobby : MonoBehaviour
{
    [SerializeField] private RectTransform            entranceUI;
    [SerializeField] private RectTransform            mainUI;
    [SerializeField] private RectTransform            characterSelectUI;
    [SerializeField] private RectTransform            multiplayerUI;
    [SerializeField] private RectTransform            settingsUI;
    [SerializeField] private RectTransform            blackScreen;
    [SerializeField] private InputSystemUIInputModule inputSystemUiInputModule;

    [SerializeField] private MMF_Player _currentMMF;
    [SerializeField] private MMF_Player _previousMMF;

    private void Start()
    {
        EventBus.Subscribe<ButtonPressedAction>(OnButtonPressed);

        entranceUI.gameObject.SetActive(true);
        mainUI.gameObject.SetActive(false);
        characterSelectUI.gameObject.SetActive(false);
        settingsUI.gameObject.SetActive(false);

        // multiplayerUI.gameObject.SetActive(false);
    }

    private void OnButtonPressed(ButtonPressedAction _obj)
    {
        Debug.Log(_obj.buttonName);
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

            case "Back":
                BackToPrevious();
                break;
        }
    }

    
    private void BackToPrevious()
    {
        LerpToScene(_currentMMF, _previousMMF);
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
        LerpToScene(mainUI, settingsUI);
    }

    private async void QuitGame()
    {
        await blackScreen.GetComponent<MMF_Player>().PlayFeedbacksTask(transform.position);
        Application.Quit();
    }

    private void LerpToScene(RectTransform _current, RectTransform _next)
    {
        MMF_Player _previousPlayer = _current.GetComponent<MMF_Player>();
        MMF_Player _nextPlayer     = _next.GetComponent<MMF_Player>();

        LerpToScene(_previousPlayer, _nextPlayer);
    }

    private async void LerpToScene(MMF_Player _current, MMF_Player _previous)
    {
        inputSystemUiInputModule.enabled = false;

        _currentMMF  = _previous;
        _previousMMF = _current;

        Vector3 _position = transform.position;

        _current.Direction = MMFeedbacks.Directions.BottomToTop;
        _current.PlayFeedbacks(_position);


        _previous.gameObject.SetActive(true);
        _previous.Direction = MMFeedbacks.Directions.TopToBottom;
        Task _task = _previous.PlayFeedbacksTask(_position);
        await _task;
        _current.gameObject.SetActive(false);

        inputSystemUiInputModule.enabled = true;
    }
}