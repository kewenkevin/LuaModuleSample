using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ND.UI
{
    public class TweenManager
    {
        private Tweener[] _activeTweeners = new Tweener[30];

        private List<Tweener> _tweenerPool = new List<Tweener>(30);

        private int _totalActiveTweenerNum;

        private static TweenManager _instance;

        private float _deltaTime;

        public static TweenManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TweenManager();
                }
                return _instance;
            }
        }

        public static void Clear()
        {
            _instance = null;
        }

        public float DeltaTime { get => _deltaTime; set => _deltaTime = value; }

        public void ApplicationUpdate()
        {
            if (!Application.isPlaying)
            {
                return;
            }
            Update();
        }

        public void EditorUpdate(float deltaTime)
        {
            if (Application.isPlaying)
            {
                return;
            }
            Update(deltaTime);
        }

        public Tweener CreateTweener()
        {
            Tweener tweener;
            int count = _tweenerPool.Count;
            if (count > 0)
            {
                tweener = _tweenerPool[count - 1];
                _tweenerPool.RemoveAt(count - 1);
            }
            else
            {
                tweener = new Tweener();
            }
            tweener.Init();
            _activeTweeners[_totalActiveTweenerNum++] = tweener;
            if (_totalActiveTweenerNum == _activeTweeners.Length)
            {
                Tweener[] newArray = new Tweener[_activeTweeners.Length + Mathf.CeilToInt(_activeTweeners.Length * 0.5f)];
                _activeTweeners.CopyTo(newArray, 0);
                _activeTweeners = newArray;
            }
            return tweener;
        }

        private void Update(float deltaTime = 0)
        {
            this._deltaTime = deltaTime;
            int cnt = _totalActiveTweenerNum;
            int freePosStart = -1;
            for (int i = 0; i < cnt; i++)
            {
                Tweener tweener = _activeTweeners[i];
                if (tweener == null)
                {
                    if (freePosStart == -1)
                    {
                        freePosStart = i;
                    }
                }
                else if (tweener.Killed)
                {
                    tweener.Reset();
                    _tweenerPool.Add(tweener);
                    _activeTweeners[i] = null;
                    if (freePosStart == -1)
                    {
                        freePosStart = i;
                    }
                }
                else
                {
                    // 可以派生子类来做判断
                    if ((tweener.Target is GameObject) && ((GameObject)tweener.Target) == null)
                    {
                        tweener.Killed = true;
                    }
                    else if (!tweener.Paused)
                    {
                        tweener.Update();
                    }

                    if (freePosStart != -1)
                    {
                        _activeTweeners[freePosStart] = tweener;
                        _activeTweeners[i] = null;
                        freePosStart++;
                    }
                }
            }

            if (freePosStart >= 0)
            {
                if (_totalActiveTweenerNum != cnt) //new tweens added
                {
                    int j = cnt;
                    cnt = _totalActiveTweenerNum - cnt;
                    for (int i = 0; i < cnt; i++)
                    {
                        _activeTweeners[freePosStart++] = _activeTweeners[j];
                        _activeTweeners[j] = null;
                        j++;
                    }
                }
                _totalActiveTweenerNum = freePosStart;
            }
        }

        public TweenManager()
        {
            if (Application.isPlaying)
            {
                GameObject go = new GameObject("TweenEngine");
                go.SetActive(true);
                Object.DontDestroyOnLoad(go);
                go.AddComponent<TweenEngine>();
            }
        }
    }
}