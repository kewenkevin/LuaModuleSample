using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ND.UI
{
    public class TransitionValueTreeItemBase : TransitionTreeItemBase
    {
        protected string _tag;

        protected TransitionValueTypeState _valueType;

        protected int _valueIndex;

        public TransitionValueTypeState ValueType { get => _valueType; set => _valueType = value; }
        public string Tag { get => _tag; set => _tag = value; }
        public int ValueIndex { get => _valueIndex; set => _valueIndex = value; }

        public TransitionLineTreeItemBase Line
        {
            get { return _parent as TransitionLineTreeItemBase; }
        }

        public TransitionValueTreeItemBase(TransitionValueTypeState valueType, string tag, int valueIndex)
        {
            _tag = tag;
            _valueType = valueType;
            _valueIndex = valueIndex;
            _state = TransitionTreeItemState.Show;
        }
    }
}