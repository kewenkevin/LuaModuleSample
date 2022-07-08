// 
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
//     File Name           :        AssetIncludeFilterLabelEditor.cs
// 
//     Programmer          :        Wei Wei (Battle Mage Gandalf)
// 
//     Start Date          :        04/14/2021
// 
//     Last Update         :        04/14/2021 10:09 [Wei]
// 
//     Description         :        write here
// 
// =============================================================================================
// Contributors:
// ---------------------------------------------------------------------------------------------
// Battle Mage Gandalf                 wwei@yoozoo.com             Product technology Center
// =============================================================================================

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ND.Managers.ResourceMgr.Editor.ResourceTools;
using UnityEditor;
using UnityEngine;

namespace ND.Managers.ResourceMgr.Editor.ResourceTools
{
    public class AssetIncludeFilterLabelEditor
    {
        
        [MenuItem("Assets/标记Asset打包")]
        private static void MarkAsset()
        {
            EditorResourceSettings.EnsureLoadConfig();
            string[] label0 = EditorResourceSettings.AssetIncludeLabels;
            string[] label1 = EditorResourceSettings.AllAssetIncludeLabels;

            List<string> labels = label0.Concat(label1).ToList();

            for (int i = 0; i < Selection.objects.Length; i++)
            {
                MarkSelection(Selection.objects[i],labels);
            }

            AssetDatabase.SaveAssets();
            EditorUtility.DisplayDialog("成功","成功标记","知道了");
        }
        
        
        [MenuItem("Assets/取消标记Asset打包")]
        private static void UnMarkAsset()
        {
            EditorResourceSettings.EnsureLoadConfig();
            string[] label0 = EditorResourceSettings.AssetIncludeLabels;
            string[] label1 = EditorResourceSettings.AllAssetIncludeLabels;

            List<string> labels = label0.Concat(label1).ToList();

            for (int i = 0; i < Selection.objects.Length; i++)
            {
                MarkSelection(Selection.objects[i],labels,true);
            }

            AssetDatabase.SaveAssets();
            EditorUtility.DisplayDialog("成功","成功取消标记","知道了");
        }

        
        /// <summary>
        /// 清除所有资源标记
        /// </summary>
        public static void ClearAllAssetIncludeMark()
        {
            EditorResourceSettings.EnsureLoadConfig();
            string[] label0 = EditorResourceSettings.AssetIncludeLabels;
            string[] label1 = EditorResourceSettings.AllAssetIncludeLabels;

            List<string> labels = label0.Concat(label1).ToList();
            MarkSinglePath("Assets",labels, true);
        }

        
        /// <summary>
        /// 标记一组路径（支持文件及文件夹）
        /// </summary>
        /// <param name="paths">文件或文件夹路径列表</param>
        /// <param name="invert">是否是反（取消）标记</param>
        public static void MarkPaths(List<string> paths,bool invert = false)
        {
            EditorResourceSettings.EnsureLoadConfig();
            string[] label0 = EditorResourceSettings.AssetIncludeLabels;
            string[] label1 = EditorResourceSettings.AllAssetIncludeLabels;

            List<string> labels = label0.Concat(label1).ToList();
            
            for (int i = 0; i < paths.Count; i++)
            {
                MarkSinglePath(paths[i],labels,invert);
            }
        }


        private static void MarkSinglePath(string path, List<string> labels, bool invert = false)
        {
            if (File.Exists(path))
            {
                var asset = AssetDatabase.LoadAssetAtPath<Object>(path);
                if(!invert)MarkLabel(asset,labels);
                else UnMarkLabel(asset,labels);
            }
            else if (Directory.Exists(path))
            {
                if (EditorUtility.DisplayDialog("选择", "标记该目录的方式", "不包含子目录", "包含子目录下"))
                {
                    var files = Directory.GetFiles(path,"*",SearchOption.TopDirectoryOnly);
                    for(int i=0;i<files.Length;i++){  
                        if (files[i].EndsWith(".meta")){  
                            continue;  
                        }

                        var asset = AssetDatabase.LoadAssetAtPath<Object>(files[i]);
                        if(!invert)MarkLabel(asset,labels);
                        else UnMarkLabel(asset,labels);
                    }  
                    
                }
                else
                {
                    var assets = AssetDatabase.FindAssets("", new []{path});
                    for (int i = 0; i < assets.Length; i++)
                    {
                        var assetPath = AssetDatabase.GUIDToAssetPath(assets[i]);
                        if (Directory.Exists(assetPath))
                        {
                            continue;
                        }

                        var asset = AssetDatabase.LoadAssetAtPath<Object>(assetPath);

                        if(!invert)MarkLabel(asset,labels);
                        else UnMarkLabel(asset,labels);
                        
                    }
                }
            }
        }

        private static void MarkSelection(UnityEngine.Object target,List<string> labels,bool invert = false)
        {
            if (!labels.Any())
            {
                EditorUtility.DisplayDialog("错误","请先至少制定一个可用标签","知道了");
                return;
            }

            string path = null;
            if (target == null)
            {
                EditorUtility.DisplayDialog("错误","请先选中一个对象","知道了");
                return;
            }
            
            path = AssetDatabase.GetAssetPath(target);

            if (File.Exists(path))
            {
                if(!invert)MarkLabel(target,labels);
                else UnMarkLabel(target,labels);
            }
            else if (Directory.Exists(path))
            {
                if (EditorUtility.DisplayDialog("选择", "标记该目录的方式", "不包含子目录", "包含子目录下"))
                {
                    var files = Directory.GetFiles(path,"*",SearchOption.TopDirectoryOnly);
                    for(int i=0;i<files.Length;i++){  
                        if (files[i].EndsWith(".meta")){  
                            continue;  
                        }

                        var asset = AssetDatabase.LoadAssetAtPath<Object>(files[i]);
                        if(!invert)MarkLabel(asset,labels);
                        else UnMarkLabel(asset,labels);
                    }  
                    
                }
                else
                {
                    var assets = AssetDatabase.FindAssets("", new []{path});
                    for (int i = 0; i < assets.Length; i++)
                    {
                        var assetPath = AssetDatabase.GUIDToAssetPath(assets[i]);
                        if (Directory.Exists(assetPath))
                        {
                            continue;
                        }

                        var asset = AssetDatabase.LoadAssetAtPath<Object>(assetPath);

                        if(!invert)MarkLabel(asset,labels);
                        else UnMarkLabel(asset,labels);
                        
                    }
                }
            }
        }

        private static void UnMarkLabel(UnityEngine.Object target,List<string> labels)
        {
            var exist = AssetDatabase.GetLabels(target);

            var intersect = labels.Intersect(exist);
            if (intersect.Any())
            {
                var newlabels = exist.Except(intersect).ToArray();
                AssetDatabase.SetLabels(target,newlabels);
            }
        }
        private static void MarkLabel(UnityEngine.Object target,List<string> labels)
        {
            if (target == null) return;
            
            var exist = AssetDatabase.GetLabels(target);

            var intersect = labels.Intersect(exist);
            if (!intersect.Any())
            {
                var newlabels = exist.Append(labels.First()).ToArray();
                AssetDatabase.SetLabels(target,newlabels);
            }
        }
    }
}