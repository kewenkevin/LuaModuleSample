// Copyright 2020 Yoozoo Net Inc.
// 
// UMT Framework and corresponding source code is free
// software: you can redistribute it and/or modify it under the terms of
// the GNU General Public License as published by the Free Software Foundation,
// either version 3 of the License, or (at your option) any later version.
// 
// UMT Framework and corresponding source code is distributed
// in the hope that it will be useful, but with permitted additional restrictions
// under Section 7 of the GPL. See the GNU General Public License in LICENSE.TXT
// distributed with this program. You should have received a copy of the
// GNU General Public License along with permitted additional restrictions
// with this program. If not, see https://gitlab.uuzu.com/yoozooopensource/client/framework/core
// 
// ***********************************************************************************************
// ***                  C O N F I D E N T I A L  ---  U M T   T E A M                          ***
// ***********************************************************************************************
// 
//     Project Name        :        UMT Framework Core Library
// 
//     File Name           :        EditorFileUtils.cs
// 
//     Programmer          :        Wei Wei (Battle Mage Gandalf)
// 
//     Start Date          :        04/12/2021
// 
//     Last Update         :        04/12/2021 17:39 [Wei]
// 
//     Description         :        write here
// 
// =============================================================================================
// Contributors:
// ---------------------------------------------------------------------------------------------
// Battle Mage Gandalf                 wwei@yoozoo.com             Product technology Center
// =============================================================================================

using System.Collections.Generic;
using System.IO;

namespace ND.Core.Editor.Utility
{
    public class EditorFileUtils
    {
        public static List<string> GetDirSubFilePathList(string dirABSPath, bool isRecursive = true, string suffix = "")
        {
            List<string> pathList = new List<string>();
            DirectoryInfo di = new DirectoryInfo(dirABSPath);

            if (!di.Exists)
            {
                return pathList;
            }

            FileInfo[] files = di.GetFiles();
            foreach (FileInfo fi in files)
            {
                if (!string.IsNullOrEmpty(suffix))
                {
                    if (!fi.FullName.EndsWith(suffix, System.StringComparison.CurrentCultureIgnoreCase))
                    {
                        continue;
                    }
                }
                pathList.Add(fi.FullName);
            }

            if (isRecursive)
            {
                DirectoryInfo[] dirs = di.GetDirectories();
                foreach (DirectoryInfo d in dirs)
                {
                    pathList.AddRange(GetDirSubFilePathList(d.FullName, isRecursive, suffix));
                }
            }

            return pathList;
        }

        public static List<string> GetDirSubDirNameList(string dirABSPath)
        {
            List<string> nameList = new List<string>();
            DirectoryInfo di = new DirectoryInfo(dirABSPath);

            DirectoryInfo[] dirs = di.GetDirectories();
            foreach (DirectoryInfo d in dirs)
            {
                nameList.Add(d.Name);
            }

            return nameList;
        }

        public static string GetFileName(string absOrAssetsPath)
        {
            string name = absOrAssetsPath.Replace("\\", "/");
            int lastIndex = name.LastIndexOf("/");

            if (lastIndex >= 0)
            {
                return name.Substring(lastIndex + 1);
            }
            else
            {
                return name;
            }
        }

        public static string GetFileNameWithoutExtend(string absOrAssetsPath)
        {
            string fileName = GetFileName(absOrAssetsPath);
            int lastIndex = fileName.LastIndexOf(".");

            if (lastIndex >= 0)
            {
                return fileName.Substring(0, lastIndex);
            }
            else
            {
                return fileName;
            }
        }

        public static string RemoveFileExtend(string fileName)
        {
            int lastIndex = fileName.LastIndexOf(".");

            if (lastIndex >= 0)
            {
                return fileName.Substring(0, lastIndex);
            }
            else
            {
                return fileName;
            }
        }

        public static string GetFileExtendName(string absOrAssetsPath)
        {
            int lastIndex = absOrAssetsPath.LastIndexOf(".");

            if (lastIndex >= 0)
            {
                return absOrAssetsPath.Substring(lastIndex);
            }

            return string.Empty;
        }

        public static string GetDirPath(string absOrAssetsPath)
        {
            string name = absOrAssetsPath.Replace("\\", "/");
            int lastIndex = name.LastIndexOf("/");
            return name.Substring(0, lastIndex + 1);
        }
    }
}