using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace ND.Managers.ResourceMgr.Editor.Comparer
{
    /// <summary>
    /// 对比源
    /// </summary>
    [Serializable]
    class ComparableSource : IDisposable
    {
        private string source;
        private string localPath;
        private bool diried;
        private FileSystemWatcher fsw;
        private bool originChanged;
        private string error;

        public bool Diried { get => diried; internal set => diried = value; }
        public string Source { get => source; }
        public string LocalPath { get => localPath; }
        public bool OriginChanged { get => originChanged; set => originChanged = value; }
        public string Error { get => error; set => error = value; }

        public UnityWebRequestAsyncOperation Request;


        public void Diry()
        {
            diried = true;
        }

        public void SetSource(string source)
        {
            if (this.Source == source)
                return;

            Error = null;
            this.source = source;
            localPath = null;
            Uri uri = null;

            try
            {
                uri = new Uri(source);
            }
            catch { }
            if (!string.IsNullOrEmpty(source))
            {
                source = source.Trim();
                int index = source.IndexOf("://");

                if (index < 0)
                {
                    localPath = source;
                }
                else
                {
                    string proto = source.Substring(0, index).ToLower();
                    if (proto == "file")
                    {
                        localPath = source;
                    }
                }

                if (!string.IsNullOrEmpty(localPath))
                    localPath = Path.GetFullPath(localPath);
            }

            Initalize();
        }

        public void Initalize()
        {

            try
            {
                if (string.IsNullOrEmpty(localPath))
                {
                    if (fsw != null)
                    {
                        fsw.Dispose();
                        fsw = null;
                    }
                }
                else
                {
                    if (fsw == null)
                    {
                        fsw = new FileSystemWatcher();
                        fsw.Changed += Fsw_Changed;
                    }
                    fsw.Path = Path.GetDirectoryName(localPath);
                    fsw.Filter = Path.GetFileName(localPath);
                    fsw.EnableRaisingEvents = true;
                }
            }
            catch (Exception ex)
            {
                if (fsw != null)
                {
                    fsw.Dispose();
                    fsw = null;
                }
            }

        }

        private void Fsw_Changed(object sender, FileSystemEventArgs e)
        {
            OriginChanged = true;
        }

        public void Dispose()
        {
            if (Request != null)
            {
                Request.webRequest.Dispose();
                Request = null;
            }
            if (fsw != null)
            {
                fsw.Dispose();
                fsw = null;
            }
        }
        ~ComparableSource()
        {
            Dispose();
        }
    }
}