using Cysharp.Threading.Tasks;
using UnityEngine;
using MoreMountains.Tools;
using UnityEngine.UI;

namespace MoreMountains.Feedbacks
{
    [AddComponentMenu("")]
    [FeedbackPath("_CustomFeedbacks/UI/Focusable")]
    public class MMF_SelectObject : MMF_Feedback
    {
        [MMFInspectorGroup("Target Selectable Object", true, 79)]
        [SerializeField] private Selectable _targetObject;
        
        public override string RequiredTargetText => _targetObject != null ? _targetObject.name : "";

        // use this override to specify the duration of your feedback (don't hesitate to look at other feedbacks for reference)
        // 피드백의 지속시간을 지정하기 위한 오버라이드(참고를 위해 다른 피드백을 보는 것을 권장)
        protected override async void CustomPlayFeedback(Vector3 _position, float _feedbacksIntensity = 1.0f)
        {
            if (!Active) return;

            // your play code goes here
            if (_targetObject != null)
            {
                if (Owner.Direction == MMFeedbacks.Directions.TopToBottom)
                {
                    //반대로 재생중일때는 충돌을 막기위해 기능 제한
                    await UniTask.Yield(); //이렇게 해야지 마우스로 클릭한 뒤에도 Select가 작동함
                    _targetObject.Select();
                }
            }
            else
                Debug.LogError("Target Object is null");
        }

        protected override void CustomStopFeedback(Vector3 _position, float _feedbacksIntensity = 1)
        {
            // your stop code goes here
        }
    }
}