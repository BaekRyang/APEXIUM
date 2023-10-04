using Cysharp.Threading.Tasks;
using UnityEngine;
using MoreMountains.Tools;
using UnityEngine.UI;

namespace MoreMountains.Feedbacks
{
    [AddComponentMenu("")]
    [FeedbackPath("_CustomFeedbacks/ObjectOpacity")]
    public class MMF_ChangeSpritesOpacity : MMF_Feedback
    {
        [MMFInspectorGroup("Target Root Object", true, 79)]
        [SerializeField] private Transform _targetObject;

        [SerializeField] private float       _duration     = 1f;
        [SerializeField] private float       _opacityFrom  = 1f;
        [SerializeField] private float       _opacityto    = 1f;
        [SerializeField] public  MMTweenType _tweenOpacity = new(new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.3f, 1.5f), new Keyframe(1, 0)));

        private UniTask _animateTask;

        public override string RequiredTargetText => _targetObject != null ? _targetObject.name : "";

        // use this override to specify the duration of your feedback (don't hesitate to look at other feedbacks for reference)
        // 피드백의 지속시간을 지정하기 위한 오버라이드(참고를 위해 다른 피드백을 보는 것을 권장)
        protected override async void CustomPlayFeedback(Vector3 _position, float _feedbacksIntensity = 1.0f)
        {
            if (!Active) return;

            // your play code goes here
            if (_targetObject != null)
            {
                _animateTask = AnimateOpacity();
            }
            else
                Debug.LogError("Target Object is null");
        }

        private async UniTask AnimateOpacity()
        {
            float _from, _to;
            switch (Owner.Direction)
            {
                case MMFeedbacks.Directions.TopToBottom:
                    _from = _opacityFrom;
                    _to   = _opacityto;
                    break;
                case MMFeedbacks.Directions.BottomToTop:
                default:
                    _from = _opacityto;
                    _to   = _opacityFrom;
                    break;
            }

            var   _renderers   = _targetObject.GetComponentsInChildren<Renderer>();
            float _elapsedTime = 0f;
            while (_elapsedTime < _duration)
            {
                _elapsedTime += Time.deltaTime;
                float _t              = _elapsedTime / _duration;
                float _currentOpacity = _tweenOpacity.Evaluate(_t);
                foreach (var _renderer in _renderers)
                {
                    Color _color = _renderer.material.color;
                    _color.a                 = Mathf.Lerp(_from, _to, _currentOpacity);
                    _renderer.material.color = _color;
                }

                await UniTask.Yield();
            }
        }

        protected override void CustomStopFeedback(Vector3 _position, float _feedbacksIntensity = 1)
        {
            // your stop code goes here
        }
    }
}