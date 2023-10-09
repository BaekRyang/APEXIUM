using System;
using Cinemachine;
using Cysharp.Threading.Tasks;
using UnityEngine;
using MoreMountains.Tools;
using Unity.VisualScripting;
using UnityEngine.UI;

namespace MoreMountains.Feedbacks
{
    [Serializable]
    [AddComponentMenu("")]
    [FeedbackPath("_CustomFeedbacks/VirtualCamera/OrthoSize")]
    public class MMF_VirtualCameraOrthoSize : MMF_Feedback
    {
        [Inject] private CameraManager _cameraManager;

        [MMFInspectorGroup("Tween", true, 79)]
        [SerializeField] private CinemachineVirtualCamera _targetObject;

        [SerializeField] private float       _duration     = 1f;
        [SerializeField] private float       _from         = 10f;
        [SerializeField] private float       _to           = 12f;
        [SerializeField] private MMTweenType _tweenOpacity = new(new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.3f, 1.5f), new Keyframe(1, 0)));

        private UniTask _animateTask;

        public override float FeedbackDuration
        {
            get => _duration;
            set => _duration = value;
        }

        protected override void CustomInitialization(MMF_Player owner)
        {
            DIContainer.Inject(this);
            _targetObject = _cameraManager.mainVirtualCamera;
        }

        // use this override to specify the duration of your feedback (don't hesitate to look at other feedbacks for reference)
        // 피드백의 지속시간을 지정하기 위한 오버라이드(참고를 위해 다른 피드백을 보는 것을 권장)
        protected override void CustomPlayFeedback(Vector3 _position, float _feedbacksIntensity = 1.0f)
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
                    _from = this._from;
                    _to   = this._to;
                    break;
                case MMFeedbacks.Directions.BottomToTop:
                default:
                    _from = this._to;
                    _to   = this._from;
                    break;
            }

            float _elapsedTime = 0f;
            while (_elapsedTime < _duration)
            {
                _elapsedTime += Time.deltaTime;
                float _t       = _elapsedTime / _duration;
                float _current = _tweenOpacity.Evaluate(_t);

                _targetObject.m_Lens.OrthographicSize = Mathf.LerpUnclamped(_from, _to, _current);

                await UniTask.Yield();
            }
        }

        protected override void CustomStopFeedback(Vector3 _position, float _feedbacksIntensity = 1)
        {
            // your stop code goes here
        }
    }
}