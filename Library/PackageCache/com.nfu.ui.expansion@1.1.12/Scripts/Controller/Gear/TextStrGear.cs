using System;
using ND.UI.Core;
using ND.UI.Core.Model;
using UnityEngine;
using UnityEngine.UI;

namespace ND.UI
{
    public class TextStrGear : GearBase
    {
        public static string SPLIT = "[localizationkey]";
		public static Func<string, string> localizationProvider;
        private Text _target;

        private string[] _values;

        public TextStrGear(Controller parent, GearConfig config) : base(parent, config)
        {
        }

        public override void Init(GearConfig config)
        {
            GameObject go = Owner.GetStoredGameObject(config.StoredGameObjectIndex);
            if (go == null)
            {
                _active = false;
                return;
            }
            _target = go.GetComponent<Text>();
            _active = _target != null;
            if (!_active)
            {
                return;
            }
            _values = new string[config.dataArray.Length - 1];
            for (int i = 1; i < config.dataArray.Length; i++)
            {
                _values[i - 1] = Owner.StoredStrings[config.dataArray[i]];
            }
        }

        public override void Apply()
        {
            base.Apply();
            GetTextAndkey(_values[_parent.SelectedIndex], out var text, out var key);
            if (string.IsNullOrEmpty(key))
            {
                _target.text = text;
            }
            else
            {
                if (localizationProvider != null)
                {
                    var strLocalization = localizationProvider(key);
                    if (!string.IsNullOrEmpty(strLocalization))
                    {
                        _target.text = strLocalization;
                    }
                    else
                    {
                        _target.text = text;
                    }
                }
            }
        }

        public static void GetTextAndkey(string value, out string text, out string key)
        {
            int splitIndex = value.LastIndexOf(SPLIT, StringComparison.Ordinal);
            if (splitIndex == -1)
            {
                text = value;
                key = "";
            }
            else
            {
                text = value.Substring(0, splitIndex);
                key = value.Substring(splitIndex + SPLIT.Length);
            }
        }

        public static string GetKey(string value)
        {
            int splitIndex = value.LastIndexOf(SPLIT, StringComparison.Ordinal);
            if (splitIndex == -1)
            {
                return "";
            }
            else
            {
                return value.Substring(splitIndex + SPLIT.Length);
            }
        }

        public static string GetText(string value)
        {
            int splitIndex = value.LastIndexOf(SPLIT, StringComparison.Ordinal);
            if (splitIndex == -1)
            {
                return value;
            }
            else
            {
                return value.Substring(0, splitIndex);
            }
        }
        
    }
}