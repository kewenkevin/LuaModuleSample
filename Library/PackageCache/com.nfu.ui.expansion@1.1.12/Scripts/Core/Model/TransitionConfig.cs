using System;
using System.Collections;
using System.Collections.Generic;
using ND.UI.Core.Model;
using UnityEngine;

namespace ND.UI.Core.Model
{
    [Serializable]
    public class TransitionConfig
    {
        [SerializeField]
        public string name;

        [SerializeField]
        public bool autoPlay;

        [SerializeField]
        public int autoPlayTimes;

        [SerializeField]
        public float autoPlayDelay;

        [SerializeField]
        public KeyFrameConfig[] keyFrameConfigs;
    }
}