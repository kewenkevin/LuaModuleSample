using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using UnityEditor;
using UnityEngine;

public class HotUpdateService 
{
    protected enum ResultCode
        {
            /// <summary>
            /// Use to indicate that the request succeeded.
            /// </summary>
            Ok = 200,
            /// <summary>
            /// Use to indicate that the requested resource could not be found.
            /// </summary>
            NotFound = 404
        }

        internal class FileUploadOperation
        {
            HttpListenerContext m_Context;
            byte[] m_ReadByteBuffer;
            FileStream m_ReadFileStream;
            long m_TotalBytesRead;
            bool m_IsDone = false;
            public bool IsDone => m_IsDone;

            
            public FileUploadOperation(HttpListenerContext context, string filePath)
            {
                m_Context = context;
                m_Context.Response.ContentType = "application/octet-stream";

                m_ReadByteBuffer = new byte[k_FileReadBufferSize];
                try
                {
                    m_ReadFileStream = File.OpenRead(filePath);
                }
                catch (Exception e)
                {
                    m_IsDone = true;
                    Console.Write(e);
                    throw;
                }
                m_Context.Response.ContentLength64 = m_ReadFileStream.Length;
            }

            public void Update(double diff, int bytesPerSecond)
            {
                if (m_Context == null || m_ReadFileStream == null)
                    return;

                int countToRead = (int)(bytesPerSecond * diff);

                try
                {
                    while (countToRead > 0)
                    {
                        int count = countToRead > m_ReadByteBuffer.Length ? m_ReadByteBuffer.Length : countToRead;
                        int read = m_ReadFileStream.Read(m_ReadByteBuffer, 0, count);
                        m_Context.Response.OutputStream.Write(m_ReadByteBuffer, 0, read);
                        m_TotalBytesRead += read;
                        countToRead -= count;

                        if (m_TotalBytesRead == m_ReadFileStream.Length)
                        {
                            Stop();
                            break;
                        }
                    }
                }
                catch (Exception e)
                {
                    Stop();
                    Console.Write(e);
                    throw;
                }
            }

            public void Stop()
            {
                if (m_IsDone)
                {
                    Console.Write("FileUploadOperation has already completed.");
                    return;
                }

                m_IsDone = true;
                m_ReadFileStream.Dispose();
                m_ReadFileStream = null;
                m_Context.Response.OutputStream.Close();
                m_Context = null;
            }
        }

        const int k_FileReadBufferSize = 64 * 1024;
        private const int k_OneGBPS = 1024 * 1024 * 1024;
        List<FileUploadOperation> m_ActiveUploads = new List<FileUploadOperation>();

        public int ServicePort = 0;
        public string RootDir = "";
        static readonly IPEndPoint k_DefaultLoopbackEndpoint = new IPEndPoint(IPAddress.Loopback, 0);
        private int m_UploadSpeed = 0;

        public bool IsHostingServiceRunning = false;

        protected HttpListener MyHttpListener { get; set; }

        double m_LastFrameTime;




        public HotUpdateService()
        {
            MyHttpListener = new HttpListener();
        }

        ~HotUpdateService()
        {
            StopHostingService();
        }

        public void StopHostingService()
        {
            if (!IsHostingServiceRunning) return;
            Debug.LogFormat("Stopping");
            MyHttpListener.Stop();
            // Abort() is the method we want instead of Close(), because the former frees up resources without
            // disposing the object.
            MyHttpListener.Abort();
            foreach (FileUploadOperation operation in m_ActiveUploads)
                operation.Stop();
            m_ActiveUploads.Clear();
            EditorApplication.update -= Update;
        }

        public void StartServer()
        {
            LoadCfg();
            if (ServicePort <= 0)
            {
                ServicePort = GetAvailablePort();
                Debug.LogFormat("使用端口 {0}", ServicePort);
            }
            else if (!IsPortAvailable(ServicePort))
            {
                Debug.LogFormat("Port {0} is in use, cannot start service!", ServicePort);
                return;
            }
            Debug.Log("StartServer");
            ConfigureHttpListener();
            MyHttpListener.Start();
            MyHttpListener.BeginGetContext(HandleRequest, null);

            EditorApplication.update += Update;
        }

        protected virtual void HandleRequest(IAsyncResult ar)
        {
            var c = MyHttpListener.EndGetContext(ar);
            MyHttpListener.BeginGetContext(HandleRequest, null);

            var relativePath = c.Request.Url.LocalPath.Substring(1);
            Debug.Log(relativePath);
            var fullPath = FindFileInContentRoots(relativePath);
            var result = fullPath != null ? ResultCode.Ok : ResultCode.NotFound;
            var info = fullPath != null ? new FileInfo(fullPath) : null;
            var size = info != null ? info.Length.ToString() : "-";
            var remoteAddress = c.Request.RemoteEndPoint != null ? c.Request.RemoteEndPoint.Address : null;
            var timestamp = DateTime.Now.ToString("o");
            Debug.LogFormat("{0} - - [{1}] \"{2}\" {3} {4}", remoteAddress, timestamp, fullPath, (int)result, size);
            switch (result)
            {
                case ResultCode.Ok:
                    ReturnFile(c, fullPath);
                    break;
                case ResultCode.NotFound:
                    Return404(c);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected virtual void ReturnFile(HttpListenerContext context, string filePath, int readBufferSize = k_FileReadBufferSize)
        {
            if (m_UploadSpeed > 0)
            {
                m_ActiveUploads.Add(new FileUploadOperation(context, filePath));
            }
            else
            {
                context.Response.ContentType = "application/octet-stream";

                var buffer = new byte[readBufferSize];
                using (var fs = File.OpenRead(filePath))
                {
                    context.Response.ContentLength64 = fs.Length;
                    int read;
                    while ((read = fs.Read(buffer, 0, buffer.Length)) > 0)
                        context.Response.OutputStream.Write(buffer, 0, read);
                }

                context.Response.OutputStream.Close();
            }
        }

        protected virtual string FindFileInContentRoots(string relativePath)
        {
            relativePath = relativePath.TrimStart('/');
            relativePath = relativePath.TrimStart('\\');
            var fullPath = Path.Combine(RootDir, relativePath).Replace('\\', '/');
            Debug.Log(fullPath);
            if (File.Exists(fullPath))
                return fullPath;

            return null;
        }


        public void LoadCfg()
        {
            // if (File.Exists("config.json"))
            // {
            //     var cfgStr = File.ReadAllText("config.json");
            //     var cfg = JsonMapper.ToObject<ServiceInfo>(cfgStr);
            //     ServicePort = cfg.ServicePort;
            //     RootDir = cfg.RootDir;
            //     m_UploadSpeed = cfg.UploadSpeed;
            // }
        }

        protected virtual void ConfigureHttpListener()
        {
            try
            {
                MyHttpListener.Prefixes.Clear();
                MyHttpListener.Prefixes.Add("http://+:" + ServicePort + "/");
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }


        protected static int GetAvailablePort()
        {
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Bind(k_DefaultLoopbackEndpoint);

                var endPoint = socket.LocalEndPoint as IPEndPoint;
                return endPoint != null ? endPoint.Port : 0;
            }
        }

        protected static bool IsPortAvailable(int port)
        {
            try
            {
                if (port <= 0)
                    return false;

                using (var client = new TcpClient())
                {
                    var result = client.BeginConnect(IPAddress.Loopback, port, null, null);
                    var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds(500));
                    if (!success)
                        return true;

                    client.EndConnect(result);
                }
            }
            catch
            {
                return true;
            }

            return false;
        }


        protected virtual void Return404(HttpListenerContext context)
        {
            context.Response.StatusCode = 404;
            context.Response.Close();
        }


        public void Update()
        {
            if (m_LastFrameTime == 0)
                m_LastFrameTime = EditorApplication.timeSinceStartup - Time.unscaledDeltaTime;
            double diff = EditorApplication.timeSinceStartup - m_LastFrameTime;
            int speed = m_UploadSpeed * 1024;
            int bps = speed > 0 ? speed : k_OneGBPS;
            for (int i = m_ActiveUploads.Count - 1; i >= 0; --i)
            {
                m_ActiveUploads[i].Update(diff, bps);
                if (m_ActiveUploads[i].IsDone)
                    m_ActiveUploads.RemoveAt(i);
            }
        }

}
