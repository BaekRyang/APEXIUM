using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using MoreMountains.Feedbacks;
using UnityEditor.Localization.Plugins.XLIFF.V20;
using UnityEngine;
using UnityEngine.InputSystem;

public class ESCScreen : MonoBehaviour
{
    [SerializeField] private float       fadeTime = .2f;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private CanvasGroup screen;
    [SerializeField] private Settings    settings;
    [SerializeField] private MMF_Player  contentsPlayer, settingsPlayer, confirmPlayer;

    private bool IsOpened => screen.interactable;
    Coroutine    _coroutine;

    private void Start()
    {
        EventBus.Subscribe<ButtonPressedAction>(OnButtonPressed);
        playerInput.actions["Cancel"].performed += _ => OnESC();
    }

    private void OnESC()
    {
        if (_coroutine != null)
            return;

        if (IsOpened)
            settings.gameObject.SetActive(false);

        _coroutine     = StartCoroutine(TransitUI(!IsOpened));
        Time.timeScale = IsOpened ? 0 : 1;

    }

    private void OnButtonPressed(ButtonPressedAction _obj)
    {
        switch (_obj.ButtonName)
        {
            case "Resume":
                StartCoroutine(TransitUI(false));
                break;
            case "Setting":
                LerpMMFPlayer(contentsPlayer, settingsPlayer);
                break;
            case "BackToMain":
                confirmPlayer.PlayFeedbacks();
                break;

            case "Back":
                LerpMMFPlayer(settingsPlayer, contentsPlayer);
                break;
        }
    }

    private async void LerpMMFPlayer(MMF_Player _current, MMF_Player _next)
    {
        _current.Direction = MMFeedbacks.Directions.BottomToTop;
        _next.Direction    = MMFeedbacks.Directions.TopToBottom;

        var _position = transform.position;

        UniTask _currentTask = _current.PlayFeedbacksUniTask(_position);

        _next.gameObject.SetActive(true);
        UniTask _nextTask = _next.PlayFeedbacksUniTask(_position);

        if (_currentTask.Status != UniTaskStatus.Succeeded || _nextTask.Status != UniTaskStatus.Succeeded)
            await UniTask.WhenAll(_currentTask, _nextTask);

        _current.gameObject.SetActive(false);
    }

    private IEnumerator TransitUI(bool _isOpen)
    {
        float _start = _isOpen ? 0 : 1;
        float _end   = _isOpen ? 1 : 0;

        float _elapsedTime                          = 0f;
        screen.blocksRaycasts = screen.interactable = _isOpen;

        while (_elapsedTime < fadeTime)
        {
            _elapsedTime += Time.unscaledDeltaTime;
            screen.alpha =  Mathf.Lerp(_start, _end, _elapsedTime / fadeTime);
            yield return null;
        }

        screen.alpha = _end;

        _coroutine = null;
    }
}