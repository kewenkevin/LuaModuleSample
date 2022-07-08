using System.Collections.Generic;
using ND.UI.Core;
using ND.UI.Core.Model;
using UnityEngine;
using UnityEditor;


namespace ND.UI
{
    public class TimelineKeyFrameItem : TransitionTimelineItemBase
    {
        private int _frameIndex;

        private int _showIndex;

        private int _valueSize;

        private List<float> _values = new List<float>();

        private Sprite _spriteValue;

        private string _strValue;

        private List<bool> _actives = new List<bool>();

        private TimelineTweenItem _leftTween;

        private TimelineTweenItem _rightTween;

        private float _height;

        private float _startY;

        private Tweener _tweener;

        private AnimationCurve _customerCurve;

        public int FrameIndex { get => _frameIndex; set => _frameIndex = value; }
        public int ShowIndex { get => _showIndex; set => _showIndex = value; }
        public int ValueSize { get => _valueSize; set => _valueSize = value; }
        public TimelineTweenItem LeftTween { get => _leftTween; set => _leftTween = value; }
        public TimelineTweenItem RightTween { get => _rightTween; set => _rightTween = value; }
        public float Height { get => _height; set => _height = value; }
        public float StartY { get => _startY; set => _startY = value; }
        public List<bool> Actives { get => _actives; set => _actives = value; }
        public List<float> Values { get => _values; set => _values = value; }
        public Sprite SpriteValue { get => _spriteValue; set => _spriteValue = value; }
        public string StrValue { get => _strValue; set => _strValue = value; }

        public AnimationCurve CustomerCurve { get => _customerCurve; set => _customerCurve = value; }
        public TimelineKeyFrameItem(TransitionLineTreeItemBase parent, int valueSize, KeyFrameConfig config) : base(parent, TransitionTimelineItemTypeState.Frame)
        {
            _frameIndex = _showIndex = UIExpansionManager.Instance.CurUIExpansion.StoredInts[config.dataList[1]];
            switch (config.lineType)
            {
                case LineTypeState.ImageSprite:
                    _spriteValue = UIExpansionManager.Instance.CurUIExpansion.StoredSprites[config.dataList[2]];
                    break;
                case LineTypeState.Event:
                    _strValue = UIExpansionManager.Instance.CurUIExpansion.StoredStrings[config.dataList[2]];
                    break;
                default:
                    _valueSize = valueSize;
                    int checkNum = 2;
                    for (int i = 0; i < valueSize; i++)
                    {
                        _values.Add(UIExpansionManager.Instance.CurUIExpansion.StoredFloats[config.dataList[checkNum++]]);
                    }
                    for (int i = 0; i < valueSize; i++)
                    {
                        _actives.Add(config.dataList[checkNum++] == UIExpansionUtility.TrueValue);
                    }

                    if (checkNum < config.dataList.Length)
                    {
                        // if (RightTween!=null)
                        // {
                        //     if (RightTween.RightFrame!=null)
                        //     {
                        //         RightTween.RightFrame.CustomerCurve = UIExpansionManager.Instance.CurUIExpansion.StoredAnimationCurves[config.dataList[config.dataList.Length - 1]];
                        //         CustomerCurve = RightTween.RightFrame.CustomerCurve;
                        //     }
                        // }
                        
                        //适配1.0.17(含)之前的动效 老版本动效没有动画曲线 导致解析错误
                        if (UIExpansionManager.Instance.CurUIExpansion.StoredAnimationCurves == null || UIExpansionManager.Instance.CurUIExpansion.StoredAnimationCurves.Length<=0)
                        {
                            CustomerCurve = AnimationCurve.Linear(0, 0, 1, 1);
                        }
                        else
                        {
                            CustomerCurve = UIExpansionManager.Instance.CurUIExpansion.StoredAnimationCurves[config.dataList[config.dataList.Length - 1]];
                        }
                        
                    }
                    break;
            }
        }

        public TimelineKeyFrameItem(TransitionLineTreeItemBase parent, int frameIndex, Vector4 value) : base(parent, TransitionTimelineItemTypeState.Frame)
        {
            _frameIndex = _showIndex = frameIndex;
            _valueSize = 4;
            _values = new List<float> { value.x, value.y, value.z,value.w };
            _actives = new List<bool> { true, true, true ,true};
        }

        public Vector4 GetValue(Vector4 originVec4)
        {
            return new Vector4(
                _actives[0] ? _values[0] : originVec4.x,
                _actives[1] ? _values[1] : originVec4.y,
                _actives[2] ? _values[2] : originVec4.z,
                _actives[3] ? _values[3] : originVec4.w
                );
        }

        public TimelineKeyFrameItem(TransitionLineTreeItemBase parent, int frameIndex, Vector3 value) : base(parent, TransitionTimelineItemTypeState.Frame)
        {
            _frameIndex =_showIndex = frameIndex;
            _valueSize = 3;
            _values = new List<float> { value.x, value.y, value.z };
            _actives = new List<bool> { true, true, true };
        }

        public Vector3 GetValue(Vector3 originVec3)
        {
            return new Vector3(
                _actives[0] ? _values[0] : originVec3.x,
                _actives[1] ? _values[1] : originVec3.y,
                _actives[2] ? _values[2] : originVec3.z
                );
        }

        public Vector3 Vector3Value
        {
            get
            {
                return new Vector3(_values[0], _values[1], _values[2]);
            }
            set
            {
                _values[0] = value.x;
                _values[1] = value.y;
                _values[2] = value.z;
            }
        }

        public TimelineKeyFrameItem(TransitionLineTreeItemBase parent, int frameIndex, Vector2 value) : base(parent, TransitionTimelineItemTypeState.Frame)
        {
            _frameIndex = _showIndex = frameIndex;
            _valueSize = 2;
            _values = new List<float> { value.x, value.y };
            _actives = new List<bool> { true, true };
        }

        public Vector2 GetValue(Vector2 originVec2)
        {
            return new Vector2(
                _actives[0] ? _values[0] : originVec2.x,
                _actives[1] ? _values[1] : originVec2.y
                );
        }

        public Vector2 Vector2Value
        {
            get { return new Vector2(_values[0], _values[1]); 
            }
            set
            {
                _values[0] = value.x;
                _values[1] = value.y;
            }
        }

        public TimelineKeyFrameItem(TransitionLineTreeItemBase parent, int frameIndex, float value) : base(parent, TransitionTimelineItemTypeState.Frame)
        {
            _frameIndex = _showIndex = frameIndex;
            _valueSize = 1;
            _values = new List<float> { value };
            _actives = new List<bool> { true };
        }

        public float GetValue(float originValue)
        {
            return _values[0];
        }

        public TimelineKeyFrameItem(TransitionLineTreeItemBase parent, int frameIndex, bool value) : base(parent, TransitionTimelineItemTypeState.Frame)
        {
            _frameIndex = _showIndex = frameIndex;
            _valueSize = 1;
            _values = new List<float> { value ? 1 : 0 };
            _actives = new List<bool> { true };
        }

        public bool GetValue(bool originValue)
        {
            return _values[0] > 0;
        }

        public TimelineKeyFrameItem(TransitionLineTreeItemBase parent, int frameIndex, Color value) : base(parent, TransitionTimelineItemTypeState.Frame)
        {
            _frameIndex = _showIndex = frameIndex;
            _valueSize = 4;
            _values = new List<float> { value.r, value.g, value.b, value.a };
            _actives = new List<bool> { true, true, true, true };
        }

        public Color GetValue(Color originValue)
        {
            return new Color(_values[0], _values[1], _values[2], _values[3]);
        }

        public Color ColorValue
        {
            get { return new Color(_values[0], _values[1], _values[2], _values[3]); }
            set
            {
                _values[0] = value.r;
                _values[1] = value.g;
                _values[2] = value.b;
                _values[3] = value.a;
            }
        }

        public Color GetColorValue()
        {
            return new Color(_values[0], _values[1], _values[2], _values[3]);
        }

        public TimelineKeyFrameItem(TransitionLineTreeItemBase parent, int frameIndex, Sprite value) : base(parent, TransitionTimelineItemTypeState.Frame)
        {
            _frameIndex = _showIndex = frameIndex;
            _valueSize = 0;
            _spriteValue = value;
        }

        public Sprite GetSpriteValue()
        {
            return _spriteValue;
        }

        public TimelineKeyFrameItem(TransitionLineTreeItemBase parent, int frameIndex, string value) : base(parent, TransitionTimelineItemTypeState.Frame)
        {
            _frameIndex = _showIndex = frameIndex;
            _valueSize = 1;
            _strValue = value;
        }

        public string GetStrValue()
        {
            return _strValue;
        }

        public override void OnGUI(Rect rowArea)
        {
            _startY = rowArea.y;
            _height = rowArea.height;
            HandleInput();
            float framePosX = UIExpansionEditorUtility.TRANSITION_TIMELINE_MARGIN_LEFT +
                UIExpansionManager.Instance.TransitionSettings.PanelOffsetX +
                UIExpansionManager.Instance.TransitionSettings.FrameWidth * _showIndex;
            Color tempColor = GUI.color;
            GUI.color = new Color(0.627451f, 0.7098039f, 0.8156863f, 1f);
            GUI.DrawTexture(new Rect(framePosX - 2, _startY, 4, _height), Texture2D.whiteTexture);
            GUI.color = tempColor;
        }

        protected override Rect GetOperateArea()
        {
            float framePosX = UIExpansionEditorUtility.TRANSITION_TIMELINE_MARGIN_LEFT +
                UIExpansionManager.Instance.TransitionSettings.PanelOffsetX +
                UIExpansionManager.Instance.TransitionSettings.FrameWidth * _showIndex;
            return new Rect(
                framePosX - 2,
                _startY,
                4,
                _height);
        }

        public override void OnItemDragStart()
        {
            _showIndex = _frameIndex;
        }

        public override void OnItemDragUpdate()
        {
            // todo...
            int tempFrameIndex = Mathf.RoundToInt((Event.current.mousePosition.x - UIExpansionManager.Instance.TransitionSettings.PanelOffsetX - UIExpansionEditorUtility.TRANSITION_TIMELINE_MARGIN_LEFT) / UIExpansionManager.Instance.TransitionSettings.FrameWidth);

            if (_leftTween != null || _rightTween != null)
            {
                var leftFrame = _parent.GetLeftFirstFrameItem(_frameIndex);
                int leftIndex = leftFrame != null ? leftFrame.FrameIndex + 1 : 0;
                var rightFrame = _parent.GetRightFirstFrameItem(_frameIndex);
                int rightIndex = rightFrame != null ? rightFrame.FrameIndex - 1 : UIExpansionManager.Instance.TransitionSettings.NeedDrawTime * UIExpansionManager.Instance.TransitionSettings.FPS;
                tempFrameIndex = Mathf.Clamp(tempFrameIndex, leftIndex, rightIndex);
            }
            else
            {
                tempFrameIndex = Mathf.Clamp(tempFrameIndex, 0, UIExpansionManager.Instance.TransitionSettings.NeedDrawTime * UIExpansionManager.Instance.TransitionSettings.FPS);
            }
            if (tempFrameIndex == _showIndex)
            {
                return;
            }
            var operableItem = _parent.GetOperableItem(tempFrameIndex);
            if (operableItem != null)
            {
                if (operableItem.ItemType ==  TransitionTimelineItemTypeState.Frame && operableItem != this)
                {
                    return;
                }
                if (operableItem.ItemType == TransitionTimelineItemTypeState.Tween && operableItem != _rightTween && operableItem != _leftTween)
                {
                    return;
                }
            }

            _showIndex = tempFrameIndex;
        }

        public override void OnItemDragEnd()
        {
            base.OnItemDragEnd();
            _frameIndex = _showIndex;
            UIExpansionManager.Instance.CurTransitionWrapper.RefreshTotalFrameNum();
        }

        public void AddTween()
        {
            if (_parent.CanCreateTween(this))
            {
                TimelineKeyFrameItem rightFrameItem = _parent.GetRightFirstFrameItem(_frameIndex);
                _parent.CreateTween(this, rightFrameItem);
            }
        }

        protected override void OnRightMouseButtonClick()
        {
            base.OnRightMouseButtonClick();
            GenericMenu genericMenu = new GenericMenu();
            if (_parent.CanCreateTween(this))
            {
                genericMenu.AddItem(new GUIContent("Add Tween"), false, AddTween);
            }
            genericMenu.AddItem(new GUIContent("Delete"), false, DeleteSelf);
            genericMenu.ShowAsContext();
        }

        private void DeleteSelf()
        {
            _parent.RemoveFrame(_frameIndex);
        }

        public void Play()
        {
            float delayTime = (float)_frameIndex / (float)UIExpansionManager.Instance.TransitionSettings.FPS;
            this._tweener = TweenManager.Instance.CreateTweener()
                .SetDelay(delayTime)
                .SetTimeScale(1)
                .SetIgnoreEngineTimeScale(true)
                .SetTarget(this)
                .OnComplete(OnDelayPlayComplete);
        }

        public void Stop()
        {
            if (_tweener != null)
            {
                _tweener.Kill(false);
                _tweener = null;
            }
        }

        public void OnDelayPlayComplete()
        {
            _parent.ApplyValue(this);
            _tweener = null;
            UIExpansionManager.Instance.CurTransitionWrapper.OnItemTweenComplete();
        }
    }
}