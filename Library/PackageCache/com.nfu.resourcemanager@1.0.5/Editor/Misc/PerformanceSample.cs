using System;
using System.Collections.Generic;
using System.Linq;
using ND.Managers.ResourceMgr.Editor.ResourceTools;
using UnityEngine;

namespace ND.Managers.ResourceMgr.Editor
{

    /// <summary>
    /// 统计性能（耗时，内存)工具，查找性能问题
    /// 已过时，使用新的性能分析工具(https://gitlab.uuzu.com/yoozooopensource/toolchain/profiler)
    /// </summary>    
    public class PerformanceSample : IDisposable
    {
        private string m_Name;
        private DateTime m_StartTime;
        private long m_StartTotalMemory;
        private float m_UsedSeconds;
        private long m_UsedMemory;
        private bool m_IsDisposed;
        private static List<PerformanceSample> samples = new List<PerformanceSample>();
        private List<PerformanceSample> children = new List<PerformanceSample>();
        private PerformanceSample parent;
        static bool EnabledLog = true;

        public PerformanceSample(string name)
        {
            this.m_Name = name;

            if (EnabledLog)
                Log($"{new string(' ', samples.Count * 2)}#{samples.Count}<color=#00ff00>Performance begin: </color>{name}");

            //开始时间
            this.m_StartTime = DateTime.Now;
            //开始内存
            m_StartTotalMemory = GC.GetTotalMemory(false);
            if (samples.Count > 0)
            {
                parent = samples[samples.Count - 1];
                parent.children.Add(this);
            }
            samples.Add(this);
        }

        public float ElapsedSeconds
        {
            get => (float)(DateTime.Now - m_StartTime).TotalSeconds;
        }

        public void Dispose()
        {
            if (m_IsDisposed)
                return;

            int index = samples.LastIndexOf(this);
            if (index >= 0)
            {
                try
                {
                    //主动设置所有子节点完成
                    for (int i = samples.Count - 1; i > index; i--)
                    {
                        samples[i].Dispose();
                    }

                    samples.RemoveAt(index);
                }
                catch(Exception ex)
                {
                    Debug.LogWarning(ex);
                }
            }

            m_IsDisposed = true;

            //使用的内存
            m_UsedMemory = GC.GetTotalMemory(false) - m_StartTotalMemory;
            //使用的时间
            m_UsedSeconds = ElapsedSeconds;

            float totalChildrenTime = children.Sum(o => o.m_UsedSeconds);

            if (EnabledLog)
                Log($"{new string(' ', samples.Count * 2)}#{samples.Count} <color=#00ff00>Performance end: </color>{m_Name} ({m_UsedSeconds:0.###}s, {(m_UsedMemory / 1024):0.#}k, self({m_UsedSeconds - totalChildrenTime:0.###}s))");

        }

        private void Log(string msg)
        {
            if (!Application.isBatchMode && !EditorResourceSettings.PerformanceLogEnabled)
                return;
            //输出堆栈
            var current = parent;
            while (current != null)
            {
                msg += "\n" + current.m_Name;
                current = current.parent;
            }
            Debug.Log(msg);
        }

    }
}
