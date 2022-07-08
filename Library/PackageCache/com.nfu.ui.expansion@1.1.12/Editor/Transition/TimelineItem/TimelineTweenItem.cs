using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ND.UI
{
    public class TimelineTweenItem : TransitionTimelineItemBase,ITweenListener
    {
        private TimelineKeyFrameItem _leftFrame;

        private TimelineKeyFrameItem _rightFrame;

        private EaseType _easeType;

        private Tweener _tweener;

        private float _height;

        private float _startY;

        private int _moveDelta;

        private float _easeOvershootOrAmplitude = 1.70158f;

        private int _easePeriod = 0;

        public TimelineKeyFrameItem LeftFrame { get => _leftFrame; set => _leftFrame = value; }
        public TimelineKeyFrameItem RightFrame { get => _rightFrame; set => _rightFrame = value; }
        public EaseType EaseType { get => _easeType; set => _easeType = value; }

        public List<bool> Actives
        {
            get
            {
                List<bool> actives = new List<bool>();
                for (int i = 0; i < LeftFrame.Actives.Count; i++)
                {
                    actives.Add(_leftFrame.Actives[i] || _rightFrame.Actives[i]);
                }
                return actives;
            }
        }

        public TweenValue Value
        {
            get
            {
                return _tweener.Value;
            }
        }

        public TimelineTweenItem(TransitionLineTreeItemBase parent, TimelineKeyFrameItem leftFrame, TimelineKeyFrameItem rightFrame, EaseType easeType = EaseType.Linear) : base(parent, TransitionTimelineItemTypeState.Tween)
        {
            _leftFrame = leftFrame;
            _rightFrame = rightFrame;
            _easeType = easeType;
        }

        public override void OnGUI(Rect rowArea)
        {
            base.OnGUI(rowArea);
            _startY = rowArea.y;
            _height = rowArea.height;
            HandleInput();
            float leftPosX = UIExpansionEditorUtility.TRANSITION_TIMELINE_MARGIN_LEFT +
                UIExpansionManager.Instance.TransitionSettings. PanelOffsetX +
                UIExpansionManager.Instance.TransitionSettings.FrameWidth * _leftFrame.ShowIndex;
            float RightPosX = UIExpansionEditorUtility.TRANSITION_TIMELINE_MARGIN_LEFT +
                UIExpansionManager.Instance.TransitionSettings.PanelOffsetX +
                UIExpansionManager.Instance.TransitionSettings.FrameWidth * _rightFrame.ShowIndex;
            Color tempColor = GUI.color;
            GUI.color = new Color(0.5137255f, 0.7921569f, 0.3294118f, 1f);
            GUI.DrawTexture(new Rect(leftPosX + 2, _startY + 2, RightPosX - leftPosX - 4, _height - 4), Texture2D.whiteTexture);
            GUI.color = tempColor;
        }

        protected override Rect GetOperateArea()
        {
            float leftPosX = UIExpansionEditorUtility.TRANSITION_TIMELINE_MARGIN_LEFT +
                UIExpansionManager.Instance.TransitionSettings.PanelOffsetX +
                UIExpansionManager.Instance.TransitionSettings.FrameWidth * _leftFrame.ShowIndex;
            float RightPosX = UIExpansionEditorUtility.TRANSITION_TIMELINE_MARGIN_LEFT +
                UIExpansionManager.Instance.TransitionSettings.PanelOffsetX +
                UIExpansionManager.Instance.TransitionSettings.FrameWidth * _rightFrame.ShowIndex;
            return new Rect(leftPosX + 2, _startY + 2, RightPosX - leftPosX - 4, _height - 4);
        }

        protected override void OnRightMouseButtonClick()
        {
            base.OnRightMouseButtonClick();
            GenericMenu genericMenu = new GenericMenu();
            genericMenu.AddItem(new GUIContent("Delete"), false, DeleteSelf);
            genericMenu.ShowAsContext();
        }

        private void DeleteSelf()
        {
            _parent.RemoveTween(this);
        }

        public override void OnItemDragStart()
        {
            base.OnItemDragStart();
            _leftFrame.ShowIndex = _leftFrame.FrameIndex;
            _rightFrame.ShowIndex = _rightFrame.FrameIndex;
            _moveDelta = 0;
        }

        public override void OnItemDragUpdate()
        {
            base.OnItemDragUpdate();
            int moveDelta = Mathf.RoundToInt((Event.current.mousePosition.x - _mouseDownPos.x) / UIExpansionManager.Instance.TransitionSettings.FrameWidth);
            if (_moveDelta == moveDelta)
            {
                return;
            }
            _moveDelta = moveDelta;
            if (_parent.TweenCanMove(this, moveDelta))
            {
                _leftFrame.ShowIndex = _leftFrame.FrameIndex + _moveDelta;
                _rightFrame.ShowIndex = _rightFrame.FrameIndex + _moveDelta;
            }
        }

        public override void OnItemDragEnd()
        {
            base.OnItemDragEnd();
            _leftFrame.FrameIndex = _leftFrame.ShowIndex;
            _rightFrame.FrameIndex = _rightFrame.ShowIndex;
            UIExpansionManager.Instance.CurTransitionWrapper.RefreshTotalFrameNum();
        }

        public Vector3 GetValue(int frameIndex, Vector3 originVec3)
        {
            frameIndex = Mathf.Clamp(frameIndex, _leftFrame.FrameIndex, _rightFrame.FrameIndex);
            float normalizedTime = EaseManager.Evaluate(_easeType, frameIndex - _leftFrame.FrameIndex, _rightFrame.FrameIndex - _leftFrame.FrameIndex, _easeOvershootOrAmplitude, _easePeriod, _leftFrame.CustomerCurve);
            List<float> values = new List<float>();
            for (int i = 0; i < _leftFrame.ValueSize; i++)
            {
                float n1 = _leftFrame.Values[i];
                float n2 = _rightFrame.Values[i];
                float f = n1 + (n2 - n1) * normalizedTime;
                values.Add(f);
            }

            return new Vector3(
                Actives[0] ? values[0] : originVec3.x,
                Actives[1] ? values[1] : originVec3.y,
                Actives[2] ? values[2] : originVec3.z
                );
        }
        
        public Vector2 GetValue(int frameIndex, Vector2 originVec2)
        {
            frameIndex = Mathf.Clamp(frameIndex, _leftFrame.FrameIndex, _rightFrame.FrameIndex);
            float normalizedTime = EaseManager.Evaluate(_easeType, frameIndex - _leftFrame.FrameIndex, _rightFrame.FrameIndex - _leftFrame.FrameIndex, _easeOvershootOrAmplitude, _easePeriod, _leftFrame.CustomerCurve);
            List<float> values = new List<float>();
            for (int i = 0; i < _leftFrame.ValueSize; i++)
            {
                float n1 = _leftFrame.Values[i];
                float n2 = _rightFrame.Values[i];
                float f = n1 + (n2 - n1) * normalizedTime;
                values.Add(f);
            }

            return new Vector2(
                Actives[0] ? values[0] : originVec2.x,
                Actives[1] ? values[1] : originVec2.y
            );
        }

        public Color GetValue(int frameIndex, Color value)
        {
            frameIndex = Mathf.Clamp(frameIndex, _leftFrame.FrameIndex, _rightFrame.FrameIndex);
            float normalizedTime = EaseManager.Evaluate(_easeType, frameIndex - _leftFrame.FrameIndex, _rightFrame.FrameIndex - _leftFrame.FrameIndex, _easeOvershootOrAmplitude, _easePeriod, _leftFrame.CustomerCurve);
            List<float> values = new List<float>();
            for (int i = 0; i < _leftFrame.ValueSize; i++)
            {
                float n1 = _leftFrame.Values[i];
                float n2 = _rightFrame.Values[i];
                float f = n1 + (n2 - n1) * normalizedTime;
                values.Add(f);
            }

            return new Color(
                Actives[0] ? values[0] : value.r,
                Actives[1] ? values[1] : value.g,
                Actives[2] ? values[2] : value.b,
                Actives[3] ? values[3] : value.a
                );
        }

        public void Play()
        {
            float delayTime = (float)_leftFrame.FrameIndex / (float)UIExpansionManager.Instance.TransitionSettings.FPS;
            float durationTime = (float)(_rightFrame.FrameIndex - _leftFrame.FrameIndex) / (float)UIExpansionManager.Instance.TransitionSettings.FPS;
            this._tweener = TweenManager.Instance.CreateTweener().To(_leftFrame.Values, _rightFrame.Values, _leftFrame.ValueSize, durationTime);
            this._tweener.SetDelay(delayTime)
                 .SetEase(_easeType,_leftFrame.CustomerCurve)
                 .SetTimeScale(1)
                 .SetIgnoreEngineTimeScale(true)
                 .SetTarget(this)
                 .SetListener(this);
        }

        public void Stop()
        {
            if (_tweener != null)
            {
                _tweener.Kill(false);
                _tweener = null;
            }
        }

        void ITweenListener.OnTweenStart()
        {

        }

        void ITweenListener.OnTweenUpdate()
        {
            _parent.ApplyValue(this);
        }

        void ITweenListener.OnTweenComplete()
        {
            if (_tweener != null)
            {
                _tweener.Kill(false);
                _tweener = null;
            }
            UIExpansionManager.Instance.CurTransitionWrapper.OnItemTweenComplete();
        }
    }
}