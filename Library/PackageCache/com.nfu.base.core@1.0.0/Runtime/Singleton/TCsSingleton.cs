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
//     Project Name        :        UMT Framework Core Liberary
// 
//     File Name           :        TCsSingleton.cs
// 
//     Programmer          :        Wei Wei (Battle Mage Gandalf)
// 
//     Start Date          :        04/07/2021
// 
//     Last Update         :        04/07/2021 16:30 [Wei]
// 
//     Description         :        write here
// 
// =============================================================================================
// Contributors:
// ---------------------------------------------------------------------------------------------
// Battle Mage Gandalf                 wwei@yoozoo.com             Product technology Center
// =============================================================================================

namespace ND.Core
{
    public class TCsSingleton<T> : ISingleton where T : TCsSingleton<T>, new()
    {
        protected static T		Instance;
        protected static readonly object Lock = new object();

        public static T S
        {
            get
            {
                if(Instance == null)
                {
                    lock (Lock)
                    {
                        if (Instance == null)
                        {
                            Instance = new T();
                            Instance.OnSingletonInit();
                        }
                    }
                }
                return Instance;
            }
        }

        public static T ResetInstance()
        {
            Instance = new T();
            Instance.OnSingletonInit();
            return Instance;
        }
		
        public virtual void Init()
        {
        }

        public virtual void OnSingletonInit()
        {
        }
    }
}