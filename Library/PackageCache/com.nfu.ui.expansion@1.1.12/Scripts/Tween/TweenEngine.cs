using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ND.UI
{
    public class TweenEngine : MonoBehaviour
    {
        void Update()
        {
            TweenManager.Instance.ApplicationUpdate();
        }
    }
}