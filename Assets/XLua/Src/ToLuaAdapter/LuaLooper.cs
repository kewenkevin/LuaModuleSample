/*
Copyright (c) 2015-2017 topameng(topameng@qq.com)

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
using System;
using UnityEngine;

namespace XLua
{
    public class LuaLooper : MonoBehaviour
    {
        public LuaBeatEvent UpdateEvent { get; private set; }

        public LuaBeatEvent LateUpdateEvent { get; private set; }

        public LuaBeatEvent FixedUpdateEvent { get; private set; }

        public LuaEnv luaState = null;

        void Start()
        {
            try
            {
                UpdateEvent = GetEvent("UpdateBeat");
                LateUpdateEvent = GetEvent("LateUpdateBeat");
                FixedUpdateEvent = GetEvent("FixedUpdateBeat");
            }
            catch (Exception e)
            {
                Destroy(this);
                throw e;
            }
        }

        LuaBeatEvent GetEvent(string name)
        {
            LuaTable table = luaState.Global.Get<LuaTable>(name);

            if (table == null)
            {
                throw new LuaException(string.Format("Lua table {0} not exists", name));
            }

            LuaBeatEvent e = new LuaBeatEvent(table);
            table.Dispose();
            table = null;
            return e;
        }

        

        void Update()
        {
            luaState.Global.Get<LuaFunction>("Update")?.Action(Time.deltaTime, Time.unscaledDeltaTime);
        }

        void LateUpdate()
        {
            luaState.Global.Get<LuaFunction>("LateUpdate")?.Call();
        }

        void FixedUpdate()
        {
            luaState.Global.Get<LuaFunction>("FixedUpdate")?.Action(Time.fixedDeltaTime);
        }

        public void Destroy()
        {
            if (luaState != null)
            {
                if (UpdateEvent != null)
                {
                    // UpdateEvent.Dispose();
                    UpdateEvent = null;
                }

                if (LateUpdateEvent != null)
                {
                    // LateUpdateEvent.Dispose();
                    LateUpdateEvent = null;
                }

                if (FixedUpdateEvent != null)
                {
                    // FixedUpdateEvent.Dispose();
                    FixedUpdateEvent = null;
                }

                luaState = null;
            }
        }

        void OnDestroy()
        {
            if (luaState != null)
            {
                Destroy();
            }
        }
    }
}