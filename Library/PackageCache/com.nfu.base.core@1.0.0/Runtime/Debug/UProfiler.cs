// Copyright 2020 Yoozoo Net Inc.
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
//     File Name           :        UProfiler.cs
// 
//     Programmer          :        Wei Wei (Battle Mage Gandalf)
// 
//     Start Date          :        04/12/2021
// 
//     Last Update         :        04/12/2021 15:55 [Wei]
// 
//     Description         :        write here
// 
// =============================================================================================
// Contributors:
// ---------------------------------------------------------------------------------------------
// Battle Mage Gandalf                 wwei@yoozoo.com             Product technology Center
// =============================================================================================
using System.Collections.Generic;
using System.Diagnostics;

public class UProfiler 
{
    struct ProfilerItem
    {
        public string m_name;
        public string m_moduleName;
        public Stopwatch m_watcher;

        public ProfilerItem(string name,string moduleName)
        {
            this.m_name = name;
            this.m_moduleName = moduleName;
            this.m_watcher = new Stopwatch();
        }

        public void Start()
        {
            m_watcher.Start();
        }

        public void Stop()
        {
            m_watcher.Stop();
        }

        public void Print()
        {
            if(string.IsNullOrEmpty(m_moduleName))
                UnityEngine.Debug.LogFormat("<color=#FFFF00>[UProfiler]</color>,({0}): {1}ms",this.m_name,m_watcher.Elapsed.TotalMilliseconds);
            else
                UnityEngine.Debug.LogFormat("<color=#FFFF00>[UProfiler]</color><color=#FF00FF>[{2}]</color>,({0}): {1}ms",this.m_name,m_watcher.Elapsed.TotalMilliseconds,m_moduleName);
        }
    }

    private static Stack<ProfilerItem> _stackWatch=new Stack<ProfilerItem>();
    
    public static void BeginSample(string name,string moduleName = null)
    {
        var item = new ProfilerItem(name, moduleName);
        item.Start();
        _stackWatch.Push(item);
    }
    
    public static void EndSample(){
        var watcher = _stackWatch.Pop();
        watcher.Stop();
        watcher.Print();
    }
}
