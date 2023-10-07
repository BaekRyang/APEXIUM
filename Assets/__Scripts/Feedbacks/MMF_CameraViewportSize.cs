using Cysharp.Threading.Tasks;
using MoreMountains.Tools;
using UnityEngine;

namespace MoreMountains.Feedbacks
{
    [AddComponentMenu("")]
    [FeedbackPath("_CustomFeedbacks/Camera/ChangeViewportSize")]
    public class MMF_CameraViewportSize : MMF_Feedback
    {
        [MMFInspectorGroup("Tween", true, 79)]
        [Inject] [SerializeField]
        private CameraManager _camManager;

        private Camera[] _cameras;

        [SerializeField] private float       _duration       = 1f;
        [SerializeField] private bool        _targetCamIndex = false;
        [SerializeField] private float       _origin         = 10f;
        [SerializeField] private float       _range          = 2f;
        [SerializeField] private MMTweenType _tweenOpacity   = new(new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.3f, 1.5f), new Keyframe(1, 0)));

        private UniTask _animateTask;

        public override float FeedbackDuration
        {
            get => _duration;
            set => _duration = value;
        }

        protected override void CustomInitialization(MMF_Player owner)
        {
            DIContainer.Inject(this);
            _cameras = _camManager.transitionCameras;
        }

        // use this override to specify the duration of your feedback (don't hesitate to look at other feedbacks for reference)
        // 피드백의 지속시간을 지정하기 위한 오버라이드(참고를 위해 다른 피드백을 보는 것을 권장)
        protected override async void CustomPlayFeedback(Vector3 _position, float _feedbacksIntensity = 1.0f)
        {
            if (!Active) return;
            _animateTask = Animate();
        }

        private async UniTask Animate()
        {
            float _from, _to;
            switch (Owner.Direction)
            {
                case MMFeedbacks.Directions.TopToBottom:
                    _from = _origin;
                    _to   = _origin - _range;
                    break;
                case MMFeedbacks.Directions.BottomToTop:
                default:
                    _from = _origin + _range;
                    _to   = _origin;
                    break;
            }

            float   _elapsedTime = 0f;
            int     _index       = _targetCamIndex ? 1 : 0;
            Vector3 _originPos   = _cameras[_index].transform.position;
            while (_elapsedTime < _duration)
            {
                _elapsedTime += Time.deltaTime;
                float _t       = _elapsedTime / _duration;
                float _current = _tweenOpacity.Evaluate(_t);

                _cameras[_index].orthographicSize = Mathf.Lerp(_from, _to, _current);
                if (Owner.Direction == MMFeedbacks.Directions.TopToBottom)
                    _cameras[_index].transform.position = _originPos + Vector3.up * Mathf.Lerp(0, 1.5f, _current);

                await UniTask.Yield();
            }
        }

        protected override void CustomStopFeedback(Vector3 _position, float _feedbacksIntensity = 1)
        {
            // your stop code goes here
        }
    }
}