using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.InputSystem.UI;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Lobby : MonoBehaviour
{
    [SerializeField] private RectTransform            entranceUI;
    [SerializeField] private RectTransform            mainUI;
    [SerializeField] private RectTransform            characterSelectUI;
    [SerializeField] private RectTransform            multiplayerUI;
    [SerializeField] private RectTransform            settingsUI;
    [SerializeField] private RectTransform            foregroundUI;
    [SerializeField] private InputSystemUIInputModule inputSystemUiInputModule;

    [SerializeField] private MMF_Player _currentMMF;
    [SerializeField] private MMF_Player _nextMMF;

    private void Start()
    {
        EventBus.Subscribe<ButtonPressedAction>(OnButtonPressed);

        entranceUI.gameObject.SetActive(true);
        mainUI.gameObject.SetActive(false);
        characterSelectUI.gameObject.SetActive(false);
        settingsUI.gameObject.SetActive(false);
        foregroundUI.gameObject.SetActive(false);

        Time.timeScale = 1;
        // multiplayerUI.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<ButtonPressedAction>(OnButtonPressed);
    }

    private void OnButtonPressed(ButtonPressedAction _obj)
    {
        switch (_obj.ButtonName)
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

            case "LoadGame":
                LoadGame();
                break;
        }
    }

    private async void LoadGame()
    {
        foregroundUI.gameObject.SetActive(true);
        await foregroundUI.GetComponent<MMF_Player>().PlayFeedbacksUniTask(transform.position);
        SceneManager.LoadScene("GameScene");
    }

    private void BackToPrevious()
    {
        LerpToScene(_currentMMF, _nextMMF);
    }

    private void Entrance()
    {
        LerpToScene(entranceUI, mainUI);
    }

    private void SingleCharacterSelect()
    {
        //첫번째 요소로 이동
        characterSelectUI.GetComponentInChildren<ScrollRect>().content.localPosition += new Vector3(1000, 0, 0);
        LerpToScene(mainUI, characterSelectUI);
    }

    private void OpenMultiplayer()
    {
        throw new NotImplementedException();
    }

    private async void OpenSettings()
    {
        LerpToScene(mainUI, settingsUI);
        await UniTask.Delay(TimeSpan.FromSeconds(.1f));
        EventBus.Publish(new ButtonPressedAction("General"));
    }

    private async void QuitGame()
    {
        foregroundUI.gameObject.SetActive(true);
        await foregroundUI.GetComponent<MMF_Player>().PlayFeedbacksUniTask(transform.position);
        Application.Quit();
    }

    private void LerpToScene(RectTransform _current, RectTransform _next)
    {
        MMF_Player _previousPlayer = _current.GetComponent<MMF_Player>();
        MMF_Player _nextPlayer     = _next.GetComponent<MMF_Player>();

        LerpToScene(_previousPlayer, _nextPlayer);
    }

    private async void LerpToScene(MMF_Player _current, MMF_Player _next)
    {
        inputSystemUiInputModule.enabled = false;

        _currentMMF  = _next;
        _nextMMF = _current;

        Vector3 _position = transform.position;

        _current.Direction = MMFeedbacks.Directions.BottomToTop;
        UniTask _previousTask = _current.PlayFeedbacksUniTask(_position);

        _next.gameObject.SetActive(true);
        _next.Direction = MMFeedbacks.Directions.TopToBottom;
        UniTask _nextTask = _next.PlayFeedbacksUniTask(_position);

        //두 피드백이 모두 끝날때까지 기다림
        if (_previousTask.Status != UniTaskStatus.Succeeded || _nextTask.Status != UniTaskStatus.Succeeded)
            await UniTask.WhenAll(_previousTask, _nextTask);

        _current.gameObject.SetActive(false);

        inputSystemUiInputModule.enabled = true;
    }
}