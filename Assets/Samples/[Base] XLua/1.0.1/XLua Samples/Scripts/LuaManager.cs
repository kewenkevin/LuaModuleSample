using System.Collections.Generic;
using System.IO;
using System.Text;
using ND.Managers.ResourceMgr.Runtime;
using UnityEngine;
using XLua;
using XLua.LuaDLL;

namespace ND.Framework.XLua.Example
{
    public class LuaManager: Singleton<LuaManager>
    {
        public bool abMode;
        private bool m_isLuaStarted;
        public static bool isLuaBundle
        {
            get { return p_instance.abMode; }
        }
        
        public string luaPath;
        //public string luaModulePath;

        LuaEnv luaenv = null;
        
        public static LuaEnv mainState { get { return p_instance.luaenv; } }
        public void Start(string main)
        {
#if UNITY_EDITOR
            startEmmyLuaDebugger(emmyLuaPort);
#endif
            luaenv.DoString($"LUA_XLua = true \n require '{main}'");
            Debug.Log("模拟开启XLua全局配置：LUA_XLua = true 开始Lua入口main执行");
            m_isLuaStarted = true;
        }
        
        public override void Initialize(Options options = null)
        {
            luaenv = new LuaEnv();
            
            //LuaEnv.logHandler = ND.Tools.Debugger.Log.Info;
            //LuaEnv.warnHandler = ND.Tools.Debugger.Log.Warning;
            LuaEnv.logErrorHandler = Error;
            
            luaenv.AddBuildin("rapidjson", Lua.LoadRapidJson);
            luaenv.AddBuildin("lpeg", Lua.LoadLpeg);
            luaenv.AddBuildin("pb", Lua.LoadLuaProfobuf);
            luaenv.AddBuildin("ffi", Lua.LoadFFI);
            
            if (string.IsNullOrEmpty(luaPath))
            {
                luaPath = Path.Combine(Application.dataPath, "./Lua");
            }
            
#if UNITY_EDITOR
            AddSearchPath(luaPath);
            AddSearchPath(Path.GetFullPath("Packages/com.nfu.xlua/Lua"));
#endif
            luaenv.AddLoader(LuaLoader);
            
            base.Initialize(options);
        }
        
        private void Error(string value, string type = "Error")
        {
            UnityEngine.Debug.LogErrorFormat("Error [{0}] [{2}] {1}",type,value,System.DateTime.Now.ToString("hh:mm:fff"));
        }
        
        public bool emmyLuaDebuggerAutoLaunch { get; set; }
        public int emmyLuaPort { get; set; }
        
        
#if UNITY_EDITOR
        private void startEmmyLuaDebugger(int port = 9966)
        {
            Debug.Log("模拟开启EmmyLua调试");
            string corePath = Path.GetFullPath("Library/PackageCache/com.nfu.xlua@1.0.1/Plugins/EmmyLuaSupport/x64/?.dll");
            corePath = corePath.Replace("\\", "/");
           // if (emmyLuaDebuggerAutoLaunch)
            //{
                luaenv.DoString(
                    $"local emmyCorePath = \"{corePath}\"\n" +
                    $"local emmyCorePort = {port}\n" +
                    "package.cpath = package.cpath .. \";\" .. emmyCorePath\n" +
                    "local emmyLuaTrackBack = function(err)\n" +
                    "    print('Emmy Lua ERROR: ' .. tostring(err))\n" +
                    "    print(debug.traceback())\n" +
                    "end\n" +
                    "local status = xpcall(function()\n" +
                    "    local dbg = require('emmy_core')\n" +
                    "    dbg.tcpConnect('localhost', emmyCorePort)\n" +
                    "end, emmyLuaTrackBack)\n" +
                    "if not status then\n" +
                    "    status = xpcall(function()\n" +
                    "        local dbg = require('emmy_core')\n" +
                    "        dbg.tcpListen('localhost', emmyCorePort)\n" +
                    "    end, emmyLuaTrackBack)\n" +
                    "end");
            //}
            //else
            //{
                /*luaenv.DoString(
                    $"local emmyCorePath = \"{corePath}\"\n" +
                    $"local emmyCorePort = {port}\n" +
                    "package.cpath = package.cpath .. \";\" .. emmyCorePath\n" +
                    "local emmyLuaTrackBack = function(err)\n" +
                    "    print('Emmy Lua ERROR: ' .. tostring(err))\n" +
                    "    print(debug.traceback())\n" +
                    "end\n" +
                    "local status = xpcall(function()\n" +
                    "    local dbg = require('emmy_core')\n" +
                    "    dbg.tcpListen('localhost', emmyCorePort)\n" +
                    "end, emmyLuaTrackBack)");
            }*/
        }
#endif

        private byte[] LuaLoader(ref string filepath)
        {
            return ReadByteFilesInternal(ref filepath);
        }

        public override void Update() 
        {
            if (luaenv != null)
            {
                luaenv.Tick();
            }
        }
        
        public override void Dispose()
        {
            // if (luaenv != null)
            // {
            //     luaenv.Dispose();
            // }
            base.Dispose();
        }

        public static void DoString(string str)
        {
            p_instance.luaenv.DoString(str);
        }
        public static bool IsLuaStarted()
        {
            if (p_instance == null) return false;
            return p_instance.m_isLuaStarted;
        }
        
        public static byte[] ReadByteFiles(ref string filePath)
        {
            byte[] bytes = null;
            if (isLuaBundle)
            {
                filePath = "Assets/Lua/" + filePath;
                Debug.Log("模拟Lua Ab文件加载：" + filePath);
                bytes = NFUResource.LoadBinaryFromFileSystem(filePath);
            }
            else
            {
                Debug.Log("模拟Lua文件加载：" + filePath);
                filePath = Path.Combine(Application.dataPath, "../Lua/" + filePath);
                bytes = File.ReadAllBytes(filePath);
            }

            if (bytes != null)
            {
                return bytes;
            }
            else
            {
                UnityEngine.Debug.LogError("dont has this bytes file:" + filePath);
                return null;
            }
        }
        
        private byte[] ReadByteFilesInternal(ref string filePath)
        {
            
            if (filePath.Equals("emmy_core"))
                return null;
            filePath = filePath.Replace(".","/");
            byte[] bytes = null;
            if (isLuaBundle)
            {
                filePath = "Assets/Lua/" + filePath+".lua.bytes";
                Debug.Log("模拟Lua AB文件加载：" + filePath);
                bytes = NFUResource.LoadBinaryFromFileSystem(filePath);
            }
            else
            {
                Debug.Log("模拟Lua文件加载：" + filePath);
                var newfilePath = FindFile(filePath);
                if (newfilePath == null) 
                    Debug.Log(newfilePath+"---"+filePath);

                filePath = Path.GetFullPath(newfilePath);
                bytes = System.Text.Encoding.UTF8.GetBytes(File.ReadAllText(newfilePath));
            }

            if (bytes != null)
            {
                return bytes;
            }
            else
            {
                UnityEngine.Debug.LogError("dont has this bytes file:" + filePath);
                return null;
            }
        }
        public static LuaTable GetTable(string fullPath)
        {
            return p_instance.luaenv.Global.Get<LuaTable>(fullPath);
        }
        public static void AttachProfiler()
        {
            //todo
            // p_instance.luaenv.AttachProfiler();
        }
        public static void DetachProfiler()
        {
            //todo
            // p_instance.m_luaClient.DetachProfiler();
        }
        
        protected List<string> searchPaths = new List<string>();
        
        string ToPackagePath(string path)
        {
            StringBuilder sb = new StringBuilder(256);
            sb.Append(path);
            sb.Replace('\\', '/');

            if (sb.Length > 0 && sb[sb.Length - 1] != '/')
            {
                sb.Append('/');
            }

            sb.Append("?.lua");
            return sb.ToString();
            
            /*using (CString.Block())
            {
                CString sb = CString.Alloc(256);
                sb.Append(path);
                sb.Replace('\\', '/');

                if (sb.Length > 0 && sb[sb.Length - 1] != '/')
                {
                    sb.Append('/');
                }

                sb.Append("?.lua");
                return sb.ToString();
            }*/
        }
        public bool AddSearchPath(string path, bool front = false)
        {
            path = ToPackagePath(path);
            int index = searchPaths.IndexOf(path);

            if (index >= 0)
            {
                return false;
            }

            if (front)
            {
                searchPaths.Insert(0, path);
            }
            else
            {
                searchPaths.Add(path);
            }

            return true;
        }

        public bool RemoveSearchPath(string path)
        {
            int index = searchPaths.IndexOf(path);

            if (index >= 0)
            {
                searchPaths.RemoveAt(index);
                return true;
            }

            return false;
        }

        public string FindFile(string fileName)
        {
            if (fileName == string.Empty)
            {
                return string.Empty;
            }

            if (Path.IsPathRooted(fileName))
            {
                if (!fileName.EndsWith(".lua"))
                {
                    fileName += ".lua";
                }

                return fileName;
            }

            if (fileName.EndsWith(".lua"))
            {
                fileName = fileName.Substring(0, fileName.Length - 4);
            }

            string fullPath = null;

            for (int i = 0; i < searchPaths.Count; i++)
            {
                fullPath = searchPaths[i].Replace("?", fileName);

                if (File.Exists(fullPath))
                {
                    return fullPath;
                }
            }
            return null;
        }
    }
}