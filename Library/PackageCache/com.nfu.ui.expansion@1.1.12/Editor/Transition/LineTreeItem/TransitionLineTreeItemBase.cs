using System.Collections.Generic;
using ND.UI.Core;
using ND.UI.Core.Model;
using UnityEngine;
using UnityEditor;


namespace ND.UI
{
    public abstract class TransitionLineTreeItemBase :TransitionTreeItemBase
    {
        protected bool _canUseTween;

        protected LineTypeState _lineType;

        protected int _valueSize;

        protected List<TimelineKeyFrameItem> _frameList = new List<TimelineKeyFrameItem>();

        protected List<TimelineTweenItem> _tweenList = new List<TimelineTweenItem>();

        protected EaseType _recordEaseType = EaseType.None;

        public LineTypeState LineType { get => _lineType; set => _lineType = value; }
        public bool CanUseTween { get => _canUseTween; set => _canUseTween = value; }

        public GameObject Target
        {
            get
            {
                if (_parent == null)
                {
                    return null;
                }
                return (_parent as TransitionGameObjectTreeItem).Target;
            }
        }

        public List<TimelineKeyFrameItem> FrameList { get => _frameList; set => _frameList = value; }
        public List<TimelineTweenItem> TweenList { get => _tweenList; set => _tweenList = value; }

        public TransitionLineTreeItemBase(LineTypeState lineType)
        {
            _type = TransitionTreeItemType.Line;
            _lineType = lineType;
            AddValueTreeItems();
        }

        public override void OnHeaderGUI(int index, Rect itemArea)
        {
            base.OnHeaderGUI(index, itemArea);
            if (_childrenList.Count > 0)
            {
                Rect foldoutArea = new Rect(
                    itemArea.x - 20 + DepthValue * 20,
                    itemArea.y + 1,
                    EditorGUIUtility.singleLineHeight,
                    EditorGUIUtility.singleLineHeight);
                FoldoutValue = EditorGUI.Foldout(foldoutArea, _foldoutValue, string.Empty);
            }
            Rect tagArea = new Rect(
                itemArea.x - 6 + DepthValue * 20,
                itemArea.y + (_height - EditorGUIUtility.singleLineHeight) / 2,
                140,
                EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(tagArea, _lineType.ToString());
        }

        public override void OnDetailGUI(int index, Rect itemArea)
        {
            base.OnDetailGUI(index, itemArea);
            for (int i = 0; i < _frameList.Count; i++)
            {
                _frameList[i].OnGUI(itemArea);
            }
            for (int i = 0; i < _tweenList.Count; i++)
            {
                _tweenList[i].OnGUI(itemArea);
            }
        }

        public override void OnAddGearGUI(int index, Rect itemArea)
        {
            base.OnAddGearGUI(index, itemArea);
            Rect tagArea = new Rect(
                itemArea.x - 6 + DepthValue * 20,
                itemArea.y + (AddLineHeight - EditorGUIUtility.singleLineHeight) / 2,
                140,
                EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(tagArea, _lineType.ToString());

            Rect addBtnArea = new Rect(
                itemArea.xMax - 20,
                itemArea.y + (AddLineHeight - EditorGUIUtility.singleLineHeight) / 2,
                20,
                EditorGUIUtility.singleLineHeight);
            if (GUI.Button(addBtnArea, "+", EditorStyles.toolbarButton))
            {
                State =  TransitionTreeItemState.Show;
                UIExpansionManager.Instance.CurTransitionWrapper.RebuildShowTreeList();
                UIExpansionManager.Instance.TransitionSettings.InAddNewLineState = false;
            }
        }

        public TransitionTimelineItemBase GetOperableItem(int frameIndex)
        {
            for (int i = 0; i < _frameList.Count; i++)
            {
                if (_frameList[i].FrameIndex == frameIndex)
                {
                    return _frameList[i];
                }
            }
            for (int i = 0; i < _tweenList.Count; i++)
            {
                if (_tweenList[i].LeftFrame.FrameIndex < frameIndex && _tweenList[i].RightFrame.FrameIndex > frameIndex)
                {
                    return _tweenList[i];
                }
            }
            return null;
        }

    

        public virtual void AddFrame(int frameIndex)
        {
          UIExpansionManager.Instance.CurTransitionWrapper.RefreshTotalFrameNum();
        }

        public virtual void AddFrameByConfig(KeyFrameConfig config)
        {
            TimelineKeyFrameItem frameItem = new TimelineKeyFrameItem(this, _valueSize, config);
            _frameList.Add(frameItem);
            if (_recordEaseType != EaseType.None)
            {
                CreateTween(_frameList[_frameList.Count - 2], _frameList[_frameList.Count - 1], _recordEaseType);
            }
            
            State = TransitionTreeItemState.Show;
            // if (frameItem.CustomerCurve != null)
            // {
            //     _recordEaseType = (EaseType)UIExpansionManager.Instance.CurUIExpansion.StoredInts[config.dataList[config.dataList.Length - 2]];
            // }
            // else
            // {
            //     _recordEaseType = (EaseType)UIExpansionManager.Instance.CurUIExpansion.StoredInts[config.dataList[config.dataList.Length - 1]];
            // }
            
            //适配1.0.17(含)之前的动效 老版本动效没有动画曲线 导致解析错误
            if (UIExpansionManager.Instance.CurUIExpansion.StoredAnimationCurves == null || UIExpansionManager.Instance.CurUIExpansion.StoredAnimationCurves.Length<=0)
            {
                _recordEaseType = (EaseType)UIExpansionManager.Instance.CurUIExpansion.StoredInts[config.dataList[config.dataList.Length - 1]];
            }
            else
            {
                switch (config.lineType)
                {
                    //如果是支持自定义动画曲线的动效轴，则反序列化时easeType存放位不为最后一项
                    case LineTypeState.Position:
                    case LineTypeState.Rotation:
                    case LineTypeState.Scale:
                    case LineTypeState.SizeDelta:
                    case LineTypeState.ImageColor:
                    case LineTypeState.TextColor:
                        _recordEaseType = (EaseType)UIExpansionManager.Instance.CurUIExpansion.StoredInts[config.dataList[config.dataList.Length - 2]];
                        break;
                    //其他不支持自定义动画曲线的动效轴，，则反序列化时easeType存放位为最后一项
                    default:
                        _recordEaseType = (EaseType)UIExpansionManager.Instance.CurUIExpansion.StoredInts[config.dataList[config.dataList.Length - 1]];
                        break;
                }
            }
            
        }

        public virtual void RemoveFrame(int frameIndex)
        {
            TimelineKeyFrameItem frameItem = null;
            for (int i = 0; i < _frameList.Count; i++)
            {
                if (_frameList[i].FrameIndex == frameIndex)
                {
                    frameItem = _frameList[i];
                }
            }
            if (frameItem == null)
            {
                return;
            }
            RemoveTween(frameItem, true);
            RemoveTween(frameItem, false);
            _frameList.Remove(frameItem);
        }

        public TimelineKeyFrameItem GetFrameItem(int frameIndex)
        {
            for (int i = 0; i < _frameList.Count; i++)
            {
                if (_frameList[i].FrameIndex == frameIndex)
                {
                    return _frameList[i];
                }
            }
            return null;
        }

        public TimelineKeyFrameItem GetLeftFirstFrameItem(int frameIndex)
        {
            int checkIndex = -1;
            for (int i = 0; i < _frameList.Count; i++)
            {
                if (_frameList[i].FrameIndex < frameIndex)
                {
                    if (checkIndex == -1)
                    {
                        checkIndex = i;
                    }
                    else
                    {
                        if (_frameList[i].FrameIndex > _frameList[checkIndex].FrameIndex)
                        {
                            checkIndex = i;
                        }
                    }
                }
            }
            if (checkIndex >= 0)
            {
                return _frameList[checkIndex];
            }
            else
            {
                return null;
            }
        }

        public TimelineKeyFrameItem GetRightFirstFrameItem(int frameIndex)
        {
            int checkIndex = -1;
            for (int i = 0; i < _frameList.Count; i++)
            {
                if (_frameList[i].FrameIndex > frameIndex)
                {
                    if (checkIndex == -1)
                    {
                        checkIndex = i;
                    }
                    else
                    {
                        if (_frameList[i].FrameIndex < _frameList[checkIndex].FrameIndex)
                        {
                            checkIndex = i;
                        }
                    }
                }
            }
            if (checkIndex >= 0)
            {
                return _frameList[checkIndex];
            }
            else
            {
                return null;
            }
        }

        public bool CanCreateTween(TimelineKeyFrameItem frame)
        {
            if (!_canUseTween)
            {
                return false;
            }
            if (frame.RightTween != null)
            {
                return false;
            }
            TimelineKeyFrameItem rightFrame = this.GetRightFirstFrameItem(frame.FrameIndex);
            if (rightFrame == null)
            {
                return false;
            }
            return true;
        }

        public TimelineTweenItem CreateTween(TimelineKeyFrameItem leftFrame, TimelineKeyFrameItem rightFrame, EaseType easeType = EaseType.Linear)
        {
            TimelineTweenItem tween = new TimelineTweenItem(this, leftFrame, rightFrame, easeType);
            leftFrame.RightTween = tween;
            rightFrame.LeftTween = tween;
            _tweenList.Add(tween);
            return tween;
        }

        public bool TweenCanMove(TimelineTweenItem tweenItem, int distance)
        {
            int newLeftFrameIndex = tweenItem.LeftFrame.FrameIndex + distance;
            int newRightFrameIndex = tweenItem.RightFrame.FrameIndex + distance;
            if (newLeftFrameIndex < 0 || newRightFrameIndex > UIExpansionManager.Instance.TransitionSettings.FPS * UIExpansionManager.Instance.TransitionSettings.NeedDrawTime)
            {
                return false;
            }
            for (int i = 0; i < _frameList.Count; i++)
            {
                if (_frameList[i].FrameIndex >= newLeftFrameIndex && _frameList[i].FrameIndex <= newRightFrameIndex && _frameList[i] != tweenItem.LeftFrame && _frameList[i] != tweenItem.RightFrame)
                {
                    return false;
                }
            }
            return true;
        }

        public void RemoveTween(TimelineKeyFrameItem frame, bool rightTween = true)
        {
            TimelineTweenItem targetTween = null;
            if (rightTween)
            {
                targetTween = frame.RightTween;
            }
            else
            {
                targetTween = frame.LeftTween;
            }
            RemoveTween(targetTween);
        }

        public void RemoveTween(TimelineTweenItem tween)
        {
            if (tween == null) { return; }
            tween.LeftFrame.RightTween = null;
            tween.RightFrame.LeftTween = null;
            _tweenList.Remove(tween);
        }

        public override void RebuildKeyFrameConfigList(List<KeyFrameConfig> keyFrameConfigList, UIExpansionStoredDataBuilder dataBuilder)
        {
            if (_state == TransitionTreeItemState.Hide)
            {
                return;
            }
            List<KeyFrameConfig> keyFrameCfgs = BuildKeyFrameCfgs(dataBuilder);
            for (int i = 0; i < keyFrameCfgs.Count; i++)
            {
                keyFrameConfigList.Add(keyFrameCfgs[i]);
            }
            base.RebuildKeyFrameConfigList(keyFrameConfigList, dataBuilder);
        }

        protected virtual List<KeyFrameConfig> BuildKeyFrameCfgs(UIExpansionStoredDataBuilder dataBuilder)
        {
            List<KeyFrameConfig> keyFrameCfgList = new List<KeyFrameConfig>();
            for (int i = 0; i < _frameList.Count; i++)
            {
                var frame = _frameList[i];
                // List<ushort> dataList = dataBuilder.BuildDataList(
                //     Target,
                //     frame.FrameIndex,
                //     frame.ValueSize,
                //     frame.Values,
                //     frame.Actives,
                //     frame.RightTween != null,
                //     frame.RightTween != null ? frame.RightTween.EaseType : EaseType.None);
                List<ushort> dataList = dataBuilder.BuildDataList(
                    Target,
                    frame.FrameIndex,
                    frame.ValueSize,
                    frame.Values,
                    frame.Actives,
                    frame.RightTween != null,
                    frame.RightTween != null ? frame.RightTween.EaseType : EaseType.None,
                    frame.RightTween != null ? frame.RightTween.RightFrame.CustomerCurve : null);
                KeyFrameConfig keyFrameConfig = new KeyFrameConfig(_lineType, dataList);
                keyFrameCfgList.Add(keyFrameConfig);
            }
            return keyFrameCfgList;
        }

        public override int Play()
        {
            if(State == TransitionTreeItemState.Hide)
            {
                return 0;
            }
            int totalTasks = 0;
            for (int i = 0; i < _frameList.Count; i++)
            {
                var item = _frameList[i];
                if (item.LeftTween != null || item.RightTween != null || (_parent as TransitionGameObjectTreeItem).Target == null)
                {
                    continue;
                }
                item.Play();
                totalTasks++;
            }

            for (int i = 0; i < _tweenList.Count; i++)
            {
                var tweener = _tweenList[i];
                if (tweener.LeftFrame == null || tweener.RightFrame == null || (_parent as TransitionGameObjectTreeItem).Target == null)
                {
                    continue;
                }
                tweener.Play();
                totalTasks++;
            }
            return totalTasks;
        }

        protected override void OnSelfStateChange(TransitionTreeItemState state)
        {
            if (state == TransitionTreeItemState.Hide)
            {
                _frameList = new List<TimelineKeyFrameItem>();
                _tweenList = new List<TimelineTweenItem>();
            }
        }

        public override void Stop()
        {
            for (int i = 0; i < _frameList.Count; i++)
            {
                var item = _frameList[i];
                if (item.LeftTween != null || item.RightTween != null || (_parent as TransitionGameObjectTreeItem).Target == null)
                {
                    continue;
                }
                item.Stop();
            }

            for (int i = 0; i < _tweenList.Count; i++)
            {
                var tweener = _tweenList[i];
                if (tweener.LeftFrame == null || tweener.RightFrame == null || (_parent as TransitionGameObjectTreeItem).Target == null)
                {
                    continue;
                }
                tweener.Stop();
            }
        }

        public virtual void ApplyValue(TimelineKeyFrameItem keyFrameItem)
        {

        }

        public virtual void ApplyValue(TimelineTweenItem tweenItem)
        {

        }


        public abstract void MonitorValueChange();
     

        protected virtual void AddValueTreeItems()
        {

        }
    }
}