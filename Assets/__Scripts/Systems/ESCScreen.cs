using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ESCScreen : MonoBehaviour
{
    [SerializeField] private float       fadeTime = .2f;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private CanvasGroup screen;
    [SerializeField] private Settings    settings;
    [SerializeField] private MMF_Player  contentsPlayer, settingsPlayer, confirmPlayer, exitPlayer;

    private bool IsOpened => screen.interactable;
    Coroutine    _coroutine;

    private void OnEnable()
    {
        EventBus.Subscribe<ButtonPressedAction>(OnButtonPressed);
        playerInput.actions["Cancel"].performed += OnESC;
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<ButtonPressedAction>(OnButtonPressed);
        playerInput.actions["Cancel"].performed -= OnESC;
    }

    private void OnESC(InputAction.CallbackContext _)
    {
        if (_coroutine != null)
            return;

        if (IsOpened)
        {
            settings.gameObject.SetActive(false);
            confirmPlayer.gameObject.SetActive(false);
        }
        else
        {
            contentsPlayer.gameObject.SetActive(true);
            contentsPlayer.Direction = MMFeedbacks.Directions.TopToBottom;
            contentsPlayer.PlayFeedbacks();
        }

        Time.timeScale = IsOpened ? 1 : 0;
        _coroutine = StartCoroutine(TransitUI(!IsOpened));
    }

    private async void OnButtonPressed(ButtonPressedAction _obj)
    {
        switch (_obj.ButtonName)
        {
            case "Resume":
                StartCoroutine(TransitUI(false));
                break;
            case "Settings":
                LerpMMFPlayer(contentsPlayer, settingsPlayer);
                EventBus.Publish(new ButtonPressedAction("General"));
                break;
            case "BackToMain":
                contentsPlayer.GetComponent<CanvasGroup>().interactable = false;
                confirmPlayer.gameObject.SetActive(true);
                LerpMMFPlayer(contentsPlayer, confirmPlayer);
                break;

            case "Back":
                LerpMMFPlayer(settingsPlayer, contentsPlayer);
                break;

            case "Exit_Yes":
                exitPlayer.GetComponent<Image>().raycastTarget = true;
                await exitPlayer.PlayFeedbacksUniTask(exitPlayer.transform.position);
                await UniTask.Delay(TimeSpan.FromSeconds(1f), DelayType.UnscaledDeltaTime);
                SceneManager.LoadScene("Lobby");
                break;

            case "Exit_No":
                contentsPlayer.GetComponent<CanvasGroup>().interactable = true;
                LerpMMFPlayer(confirmPlayer, contentsPlayer);
                // confirmPlayer.Direction = MMFeedbacks.Directions.BottomToTop;
                // await confirmPlayer.PlayFeedbacksUniTask(confirmPlayer.transform.position);
                confirmPlayer.gameObject.SetActive(false);

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

        _coroutine     = null;
        Time.timeScale = _isOpen ? 0 : 1;
    }
}