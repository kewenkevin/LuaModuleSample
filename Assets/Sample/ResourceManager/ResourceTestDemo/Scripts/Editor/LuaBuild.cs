using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace ND.Gameplay.Managers.ResourceManagerV2.Editor
{
    public class LuaBuild
    {
        private static readonly string LUA_OUTPUT_PATH = Application.dataPath + "/Lua";
        
        
        public static void DeleteBuildLua()
        {
            if (Directory.Exists(LUA_OUTPUT_PATH))
            {
                Directory.Delete(LUA_OUTPUT_PATH, true);
            }
        }
        
        #if ToLua
        
#if UNITY_IPHONE && UNITY_EDITOR_OSX
        private  static readonly string _luajit = Application.dataPath + "/../Lua/Dist/mac/ios64";
#elif UNITY_EDITOR_OSX && UNITY_ANDROID
        private  static readonly string _luajit = Application.dataPath + "/../Lua/Dist/mac/android32";
#else
        private static readonly string _luajit = Application.dataPath + "/../Lua/Dist/windows/android32";
#endif

        private static bool IS_OPEN_LUAJIT = false;

        [MenuItem("Tools/Build/LuaBuild")]
        public static void GenerateBuildLua()
        {
            DeleteBuildLua();
            if (!Directory.Exists(LUA_OUTPUT_PATH))
            {
                Directory.CreateDirectory(LUA_OUTPUT_PATH);
            }

            StringBuilder strErr = new StringBuilder();
            bool hasErr = false;
            CopyLuaBytesFilesJit(ref hasErr, Path.GetFullPath("Packages/com.nfu.luamodules/Runtime"), LUA_OUTPUT_PATH,
                IS_OPEN_LUAJIT, strErr);
            CopyLuaBytesFilesJit(ref hasErr, Path.GetFullPath("Packages/com.nfu.tolua/Runtime/Lua"), LUA_OUTPUT_PATH,
                IS_OPEN_LUAJIT, strErr);
            CopyLuaBytesFilesJit(ref hasErr, Application.dataPath + "/../Lua/", LUA_OUTPUT_PATH, IS_OPEN_LUAJIT,
                strErr);

            if (hasErr)
            {
                Exception ex = new Exception(strErr.ToString());
                throw (ex);
            }

            AssetDatabase.Refresh();
        }


        private static void CopyLuaBytesFilesJit(ref bool hasErr, string sourceDir, string destDir,
            bool luajitcode = false, StringBuilder strErr = null)
        {
            if (!Directory.Exists(sourceDir))
            {
                return;
            }

            int len = destDir.Length;
            if (destDir[len - 1] != '/' && destDir[len - 1] != '\\')
            {
                destDir += "/";
            }

            len = sourceDir.Length;
            if (sourceDir[len - 1] != '/' && sourceDir[len - 1] != '\\')
            {
                destDir += "/";
            }

            len = sourceDir.Length;

            string[] files = Directory.GetFiles(sourceDir, "*.*", SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++)
            {
                string srcPath = files[i];
                if (srcPath.Contains(".svn")) continue;
                if (srcPath.Contains(".git")) continue;
                if (srcPath.Contains(".idea")) continue;
                if (srcPath.Contains(".meta")) continue;
                if (srcPath.Contains(".vs")) continue;
                if (srcPath.Contains(".md")) continue;
                string str = files[i].Remove(0, len);
                str = str.Replace(".bytes", "");
                string dest = destDir + str + ".bytes";

                string dir = Path.GetDirectoryName(dest);
                Directory.CreateDirectory(dir);

                if (luajitcode)
                {
                    LuaJit(srcPath, dest, ref strErr);
                }
                else
                {
                    File.Copy(srcPath, dest);
                }
            }
        }

        private static bool LuaJit(string sourceDir, string destPath, ref StringBuilder strErr)
        {
            using (System.Diagnostics.Process proc = new System.Diagnostics.Process())
            {
                proc.StartInfo.FileName = string.Format("{0}/luajit", _luajit);
                proc.StartInfo.WorkingDirectory = _luajit;
                proc.StartInfo.Arguments = string.Format("-b {0} {1}", sourceDir, destPath);
                proc.StartInfo.CreateNoWindow = true;
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.RedirectStandardError = true;
                proc.StartInfo.RedirectStandardInput = true;
                proc.EnableRaisingEvents = true;

                StringBuilder errorSb = new StringBuilder();
                proc.ErrorDataReceived += (sender, args) => errorSb.Append(args.Data);

                proc.Start();
                proc.BeginErrorReadLine();
                proc.WaitForExit();

                if (errorSb.Length > 0)
                {
                    if (null != strErr)
                    {
                        strErr.Append(errorSb.ToString() + "\n");
                    }

                    Debug.LogError(errorSb.ToString());
                    return false;
                }
            }

            return true;
        }

#endif
        
#if XLua
        private static bool IS_OPEN_LUAC_BYTECODE = true;
        
        
#if UNITY_IPHONE && UNITY_EDITOR_OSX
        private  static readonly string _luac = Path.GetFullPath("Packages/com.nfu.xlua/Luac") + "/iOS";
#elif UNITY_EDITOR_OSX && UNITY_ANDROID
        private  static readonly string _luac = Path.GetFullPath("Packages/com.nfu.xlua/Luac") + "/iOS";
#else
        private static readonly string _luac = Path.GetFullPath("Packages/com.nfu.xlua/Luac") + "/x86";
#endif
        
        [MenuItem("Tools/Build/LuaBuild")]
        public static void GenerateBuildLua()
        {
            DeleteBuildLua();
            if (!Directory.Exists(LUA_OUTPUT_PATH))
            {
                Directory.CreateDirectory(LUA_OUTPUT_PATH);
            }
            
#if UNITY_EDITOR_OSX
             using (System.Diagnostics.Process proc = new System.Diagnostics.Process()){
                 proc.StartInfo.FileName = "sh";
                 proc.StartInfo.UseShellExecute = false;
                 proc.StartInfo.RedirectStandardInput = true;
                 proc.StartInfo.RedirectStandardOutput = true;
                 proc.StartInfo.RedirectStandardError = true;
                 proc.StartInfo.CreateNoWindow = true;
                 proc.Start();
                 proc.StandardInput.WriteLine($"chmod 777 {_luac}/luac");
                 proc.StandardInput.WriteLine("exit");
                 string strResult = proc.StandardOutput.ReadToEnd();
                 proc.Close();
             }
#endif

            StringBuilder strErr = new StringBuilder();
            bool hasErr = false;
            CopyLuaBytesFilesByteCode(ref hasErr, Path.GetFullPath("Packages/com.nfu.luamodules/Runtime"), LUA_OUTPUT_PATH,
                IS_OPEN_LUAC_BYTECODE, strErr);
            CopyLuaBytesFilesByteCode(ref hasErr, Path.GetFullPath("Packages/com.nfu.xlua/Lua"), LUA_OUTPUT_PATH,
                IS_OPEN_LUAC_BYTECODE, strErr);
            CopyLuaBytesFilesByteCode(ref hasErr, Application.dataPath + "/../Lua/", LUA_OUTPUT_PATH, IS_OPEN_LUAC_BYTECODE,
                strErr);

            if (hasErr)
            {
                Exception ex = new Exception(strErr.ToString());
                throw (ex);
            }

            AssetDatabase.Refresh();
        }
        
        
        private static void CopyLuaBytesFilesByteCode(ref bool hasErr, string sourceDir, string destDir,
            bool luacByteCode = false, StringBuilder strErr = null)
        {
            if (!Directory.Exists(sourceDir))
            {
                return;
            }

            // int len = destDir.Length;
            // if (destDir[len - 1] != '/' && destDir[len - 1] != '\\')
            // {
            //     destDir += "/";
            // }
            //
            if (!destDir.EndsWith("/") && !destDir.EndsWith("\\"))
                destDir += "/";

            // len = sourceDir.Length;
            // if (sourceDir[len - 1] != '/' && sourceDir[len - 1] != '\\')
            // {
            //     sourceDir += "/";
            // }

            if (!sourceDir.EndsWith("/") && !sourceDir.EndsWith("\\"))
                sourceDir += "/";

            int len = sourceDir.Length;

            string[] files = Directory.GetFiles(sourceDir, "*.*", SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++)
            {
                string srcPath = files[i];
                if (srcPath.Contains(".svn")) continue;
                if (srcPath.Contains(".git")) continue;
                if (srcPath.Contains(".idea")) continue;
                if (srcPath.Contains(".meta")) continue;
                if (srcPath.Contains(".vs")) continue;
                if (srcPath.Contains(".md")) continue;
                string str = files[i].Remove(0, len);
                str = str.Replace(".bytes", "");
                string dest = destDir + str + ".bytes";

                string dir = Path.GetDirectoryName(dest);
                Directory.CreateDirectory(dir);

                if (luacByteCode && dest.EndsWith(".lua.bytes"))
                {
                    LuacByteCode(srcPath, dest, ref strErr);
                }
                else
                {
                    File.Copy(srcPath, dest);
                }
            }
        }
        
        private static bool LuacByteCode(string sourceDir, string destPath, ref StringBuilder strErr)
        {
            using (System.Diagnostics.Process proc = new System.Diagnostics.Process())
            {
                proc.StartInfo.FileName = string.Format("{0}/luac", _luac);
                proc.StartInfo.WorkingDirectory = _luac;
                proc.StartInfo.Arguments = string.Format("-o {0} {1}", destPath, sourceDir);
                proc.StartInfo.CreateNoWindow = true;
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.RedirectStandardError = true;
                proc.StartInfo.RedirectStandardInput = true;
                proc.EnableRaisingEvents = true;

                StringBuilder errorSb = new StringBuilder();
                proc.ErrorDataReceived += (sender, args) => errorSb.Append(args.Data);

                proc.Start();
                proc.BeginErrorReadLine();
                proc.WaitForExit();

                if (errorSb.Length > 0)
                {
                    if (null != strErr)
                    {
                        strErr.Append(errorSb.ToString() + "\n");
                    }

                    Debug.LogError(errorSb.ToString());
                    return false;
                }
            }

            return true;
        }
        
#endif
    }
}