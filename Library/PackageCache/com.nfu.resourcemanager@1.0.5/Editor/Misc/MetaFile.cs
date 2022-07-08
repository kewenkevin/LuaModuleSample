using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;


namespace ND.Managers.ResourceMgr.Editor
{
    /// <summary>
    /// .meta 文件工具，忽略 .meta 文件，复制文件避免 Import 大幅提高性能
    /// </summary>
    public class MetaFile
    {

        private const string MetaExtension = ".meta";

        /// <summary>
        /// 是否为 .meta 文件
        /// </summary>
        public static bool IsMetaFile(string file)
        {
            return file.EndsWith(MetaExtension, System.StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// 是否为空目录
        /// </summary>
        public static bool IsEmptyDirectory(string dir)
        {
            if (!Directory.Exists(dir))
                return true;
            string[] files = Directory.GetFiles(dir, "*", SearchOption.AllDirectories);
            string first = files.FirstOrDefault(o => !IsMetaFile(o));
            return first == null;
        }

        /// <summary>
        /// 删除空目录
        /// </summary>
        public static void DeleteEmptyChildrenDirectory(string dir, bool recursive = false)
        {
            if (!Directory.Exists(dir))
                return;

            foreach (var subDir in Directory.GetDirectories(dir, "*", recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly))
            {
                if (Directory.Exists(subDir) && IsEmptyDirectory(subDir))
                {
                    Directory.Delete(subDir, true);
                    string metaFile = subDir + MetaExtension;
                    if (File.Exists(metaFile))
                        File.Delete(metaFile);
                }
            }
        }

        /// <summary>
        /// 获取目录下文件
        /// </summary>
        public static IEnumerable<string> EnumerateFiles(string dir, bool recursive = false)
        {
            if (Directory.Exists(dir))
            {
                SearchOption option;
                if (recursive)
                    option = SearchOption.AllDirectories;
                else
                    option = SearchOption.TopDirectoryOnly;
                foreach (var file in Directory.GetFiles(dir, "*", option))
                {
                    if (IsMetaFile(file))
                        continue;
                    yield return file;
                }
            }
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="recursive"></param>
        public static void DeleteFiles(string dir, bool recursive = false)
        {
            foreach (var file in EnumerateFiles(dir, recursive))
            {
                File.Delete(file);
            }
        }

        /// <summary>
        /// 复制文件夹排除 .meta
        /// </summary>
        /// <param name="srcDir"></param>
        /// <param name="destDir"></param>
        /// <param name="filter">文件路径为相对路径</param>
        public static int CopyDirectory(string srcDir, string destDir, Func<string, bool> filter = null)
        {
            string srcFullPath = Path.GetFullPath(srcDir);
            int startIndex = srcFullPath.Length;
            if (!(srcFullPath.EndsWith("\\") || srcFullPath.EndsWith("/")))
            {
                startIndex++;
            }
            int total = 0;
            foreach (var file in EnumerateFiles(srcFullPath, true))
            {
                string relativeFileName = file.Substring(startIndex).Replace('\\', '/');
                if (filter != null && !filter(relativeFileName))
                    continue;
                string dest = Path.Combine(destDir, relativeFileName);
                if (!Directory.Exists(Path.GetDirectoryName(dest)))
                    Directory.CreateDirectory(Path.GetDirectoryName(dest));

                File.Copy(file, dest, true);
                total++;
            }
            return total;
        }

    }
}