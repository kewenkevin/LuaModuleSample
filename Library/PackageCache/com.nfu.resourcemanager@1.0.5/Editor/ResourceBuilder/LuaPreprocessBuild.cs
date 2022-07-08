using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace ND.Managers.ResourceMgr.Editor.ResourceTools
{
    public class LuaPreprocessBuild : IResourcePreprocessBuild
    {
        [Tooltip("源代码位置")]
        public string sourcePath = "Lua";
        [Tooltip("源代码扩展名")]
        public string sourceExtension = ".lua|.lua.txt|.bytes";
        [Tooltip("输出路径")]
        public string outputPath = "Assets/Lua";
        [Tooltip("输出扩展名")]
        public string outputExtension = ".lua.bytes";
        [Tooltip("开启编译")]
        public bool compileEnabled = true;
        [Tooltip("编译并发数")]
        public int compileMaxParallel = 10;


        public virtual void PreprocessBuildInitialize()
        {

        }

        public virtual void PreprocessBuild()
        {
            if (string.IsNullOrEmpty(outputPath))
            {
                Debug.LogWarning("lua output path empty");
                return;
            }

            DeleteBuildLua();
            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }
#if XLua
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
            CopyLuaBytesFilesByteCode(ref hasErr, Path.GetFullPath("Packages/com.nfu.luamodules/Runtime"), outputPath,
                compileEnabled, strErr);
            CopyLuaBytesFilesByteCode(ref hasErr, Path.GetFullPath("Packages/com.nfu.managers.xlua/Lua"), outputPath,
                compileEnabled, strErr);

            if (!string.IsNullOrEmpty(sourcePath))
            {
                CopyLuaBytesFilesByteCode(ref hasErr, sourcePath, outputPath, compileEnabled,
                    strErr);
            }

            if (hasErr)
            {
                Exception ex = new Exception(strErr.ToString());
                throw (ex);
            }

            //删除所有空目录
            MetaFile.DeleteEmptyChildrenDirectory(outputPath, true);

            AssetDatabase.Refresh();
#endif
        }





        public void DeleteBuildLua()
        {
            //清理lua输出文件，保留meta文件
            MetaFile.DeleteFiles(outputPath, true);
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

        
        public void GenerateBuildLua()
        {
            DeleteBuildLua();
            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }

            StringBuilder strErr = new StringBuilder();
            bool hasErr = false;
            CopyLuaBytesFilesJit(ref hasErr, Path.GetFullPath("Packages/com.nfu.luamodules/Runtime"), outputPath,
                IS_OPEN_LUAJIT, strErr);
            CopyLuaBytesFilesJit(ref hasErr, Path.GetFullPath("Packages/com.nfu.managers.tolua/Runtime/Lua"), outputPath,
                IS_OPEN_LUAJIT, strErr);
            if (!string.IsNullOrEmpty(sourcePath))
            {
                CopyLuaBytesFilesJit(ref hasErr, sourcePath, outputPath, IS_OPEN_LUAJIT,
                    strErr);
            }

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

#if UNITY_IPHONE && UNITY_EDITOR_OSX
        private  static readonly string _luac = Path.GetFullPath("Packages/com.nfu.xlua/Luac") + "/iOS";
#elif UNITY_EDITOR_OSX && UNITY_ANDROID
        private  static readonly string _luac = Path.GetFullPath("Packages/com.nfu.xlua/Luac") + "/iOS";
#else
        private static readonly string _luac = Path.GetFullPath("Packages/com.nfu.xlua/Luac") + "/x86";
#endif



        private void CopyLuaBytesFilesByteCode(ref bool hasErr, string sourceDir, string destDir,
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
            string[] extensions = this.sourceExtension.Split('|').Where(o => !string.IsNullOrEmpty(o)).ToArray();

            string[] files = Directory.GetFiles(sourceDir, "*.*", SearchOption.AllDirectories);
            Dictionary<string, string> srcFiles = new Dictionary<string, string>();
            UTF8Encoding encoding = new UTF8Encoding(false);
            for (int i = 0; i < files.Length; i++)
            {
                string srcPath = files[i];
                string extension = Path.GetExtension(srcPath).ToLower();
                if (!extensions.Contains(extension))
                    continue;

                string newFileName = files[i].Remove(0, len);
                string dest;
                bool isCopy = false;
                if (extension != ".bytes" && !string.IsNullOrEmpty(outputExtension))
                {
                    //如果存在 outputExtension 则修改扩展名
                    int index = newFileName.IndexOf('.');
                    if (index >= 0)
                    {
                        newFileName = newFileName.Substring(0, index);
                    }
                    dest = destDir + newFileName + outputExtension;
                }
                else
                {
                    //保持扩展名不变
                    dest = destDir + newFileName;
                    isCopy = true;
                }

                string dir = Path.GetDirectoryName(dest);
                Directory.CreateDirectory(dir);
                if (!isCopy)
                {
                    srcFiles.Add(srcPath, dest);
                }
                else
                {
                    if (extension != ".bytes")
                    {
                        File.WriteAllText(dest, File.ReadAllText(srcPath, Encoding.UTF8), encoding);
                    }
                    else
                    {
                        File.Copy(srcPath, dest);
                    }
                }
            }

            try
            {
                ParallelOptions parallelOptions = new ParallelOptions();
                parallelOptions.MaxDegreeOfParallelism = compileMaxParallel;
                Parallel.ForEach(srcFiles, parallelOptions, item =>
                {
                    string srcPath = item.Key;
                    string dest = item.Value;
                    if (luacByteCode)
                    {
                        LuacByteCode(srcPath, dest);
                    }
                    else
                    {
                        if (!srcPath.EndsWith(".bytes"))
                        {
                            File.WriteAllText(dest, File.ReadAllText(srcPath, Encoding.UTF8), encoding);
                        }
                        else
                        {
                            File.Copy(srcPath, dest);
                        }
                    }
                });
            }
            catch (AggregateException ex)
            {
                foreach (var ex2 in ex.InnerExceptions)
                {
                    Debug.LogException(ex2);
                    if (strErr != null)
                        strErr.Append(ex2.Message);
                }
                hasErr = true;
            }
        }

        private static bool LuacByteCode(string sourceDir, string destPath)
        {
            sourceDir = Path.GetFullPath(sourceDir);
            destPath = Path.GetFullPath(destPath);

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
                    throw new Exception(errorSb.ToString());
                }
            }

            return true;
        }

#endif

        /// <summary>
        /// 删除所有空目录
        /// </summary>
        static string[] DeleteAllEmptyDirectory(string dir, Func<string, bool> fileFilter = null)
        {
            if (!Directory.Exists(dir))
                return new string[0];
            List<string> list = new List<string>();
            foreach (var subDir in Directory.GetDirectories(dir, "*", SearchOption.AllDirectories))
            {
                if (Directory.Exists(subDir))
                {
                    string[] files = Directory.GetFiles(subDir, "*", SearchOption.AllDirectories);
                    int count = 0;
                    if (fileFilter != null)
                        count = files.Count(o => fileFilter(o));
                    else
                        count = files.Length;
                    if (count == 0)
                    {
                        Directory.Delete(subDir, true);
                        list.Add(subDir);
                    }
                }
            }
            return list.ToArray();
        }

        public virtual void PreprocessBuildCleanup()
        {

        }
    }
}