using System;
using Cysharp.Threading.Tasks;
using MoreMountains.Feedbacks;
using UnityEngine;

public class ButtonEventReceiver : MonoBehaviour
{
    private CanvasGroup _canvasGroup;
    private MMF_Player  _currentMMF;

    private void OnEnable()
    {
        EventBus.Subscribe<ButtonPressedAction>(OnButtonPressed);
        _canvasGroup ??= GetComponent<CanvasGroup>();
        _currentMMF  ??= GetComponent<MMF_Player>();
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<ButtonPressedAction>(OnButtonPressed);
    }

    private async void OnButtonPressed(ButtonPressedAction _obj)
    {
        bool _targetIsMe = _obj.ButtonName == gameObject.name;

        //대상이 난데, 이미 내가 활성화 되어있으면 무시
        if (_targetIsMe && _canvasGroup.interactable) return;

        //null check
        if (_currentMMF != null)
        {
            switch (_targetIsMe)
            {
                //내가 대상이 아닌데 활성화 되어있으면 사라지게
                case false when _canvasGroup.interactable: 
                    _currentMMF.Direction = MMFeedbacks.Directions.BottomToTop;
                    _currentMMF.PlayFeedbacks();
                    break;

                //내가 대상이면 나타나게
                case true:
                    await UniTask.Delay(TimeSpan.FromSeconds(0.1f), DelayType.UnscaledDeltaTime);
                    _currentMMF.Direction = MMFeedbacks.Directions.TopToBottom;
                    _currentMMF.PlayFeedbacks(); 
                    break;
            }
        }
        
        _canvasGroup.interactable   = _targetIsMe;
        _canvasGroup.blocksRaycasts = _targetIsMe;
        
    }
}