using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ND.UI
{
    public interface ITweenListener 
    {
        void OnTweenStart();

        void OnTweenUpdate();

        void OnTweenComplete();
    }
}