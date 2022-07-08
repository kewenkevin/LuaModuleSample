
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using CSObjectWrapEditor;
using UnityEditor;
using XLua;

public class GenEmmyLuaApi 
{
    [MenuItem("XLua/Gen EmmyLuaApi", false, 103)]
    static void GenEmmyLuaApiMethod()
    {
        Generator.GetGenConfig(XLua.Utils.GetAllTypes());
        
        string path = "./EmmyLuaApi/";
        if (Directory.Exists(path))
        {
            Directory.Delete(path, true);
        }

        Directory.CreateDirectory(path);
        GenCustom(path);
    }
    
    [MenuItem("XLua/Open EmmyLuaApi", false, 103)]
    static void OpenEmmyLuaApi()
    {

        string path = "./EmmyLuaApi/";
        if (Directory.Exists(path))
        {
            EditorUtility.RevealInFinder(path);
        }

        Directory.CreateDirectory(path);
        GenCustom(path);
    }

    static void GenCustom(string path)
    {
        foreach (var item in Generator.LuaCallCSharp)
        {
            GenType(item, true, path);
        }

        foreach (var item in Generator.GCOptimizeList)
        {
            GenType(item, true, path);
        }
    }

    static void GenType(Type t, bool custom, string path)
    {
        if (!CheckType(t, custom))
            return;
        //TODO System.MulticastDelegate
        var sb = new StringBuilder();
        string className = t.Name;
        if (!CheckType(t.BaseType, custom))
            sb.AppendFormat("---@class {0}\n", t.Name);
        else
            sb.AppendFormat("---@class {0} : {1}\n", t.Name, t.BaseType.Name);
        GenTypeField(t, sb);
        sb.AppendFormat("local {0}={{ }}\n", t.Name);

        GenTypeMehod(t, sb);

        sb.AppendFormat("{0}.{1} = {2}", t.Namespace, t.Name, t.Name);

        if (className != t.Name)
        {
            sb.AppendFormat("\n{0} = {1}", className, t.Name);
        }

        File.WriteAllText(path + t.Name + ".lua", sb.ToString(), Encoding.UTF8);
    }

    static bool CheckType(Type t, bool custom)
    {
        if (t == null)
        {
            return false;
        }

        return !isObsolete(t);
    }

    static void GenTypeField(Type t, StringBuilder sb)
    {
        FieldInfo[] fields = t.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance |
                                         BindingFlags.DeclaredOnly);
        foreach (var field in fields)
        {
            if (field.IsDefined(typeof(DoNotGenAttribute), false))
                continue;
            sb.AppendFormat("---@field public {0} {1}\n", field.Name, GetLuaType(field.FieldType));
        }

        PropertyInfo[] properties = t.GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance |
                                                    BindingFlags.DeclaredOnly);
        foreach (var pro in properties)
        {
            if (pro.IsDefined(typeof(DoNotGenAttribute), false))
                continue;
            sb.AppendFormat("---@field public {0} {1}\n", pro.Name, GetLuaType(pro.PropertyType));
        }
    }

    static void GenTypeMehod(Type t, StringBuilder sb)
    {
        MethodInfo[] methods = t.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance |
                                            BindingFlags.DeclaredOnly);
        foreach (var method in methods)
        {
            if (method.IsGenericMethod)
                continue;
            if (method.IsDefined(typeof(DoNotGenAttribute), false))
                continue;
            if (method.Name.StartsWith("get_") || method.Name.StartsWith("set_"))
                continue;
            sb.AppendLine("---@public");
            var paramstr = new StringBuilder();
            foreach (var param in method.GetParameters())
            {
                sb.AppendFormat("---@param {0} {1}\n", param.Name, GetLuaType(param.ParameterType));
                if (paramstr.Length != 0)
                {
                    paramstr.Append(", ");
                }

                paramstr.Append(param.Name);
            }

            sb.AppendFormat("---@return {0}\n", method.ReturnType == null ? "void" : GetLuaType(method.ReturnType));
            if (method.IsStatic)
            {
                sb.AppendFormat("function {0}.{1}({2}) end\n", t.Name, method.Name, paramstr);
            }
            else
            {
                sb.AppendFormat("function {0}:{1}({2}) end\n", t.Name, method.Name, paramstr);
            }
        }
    }

    static string GetLuaType(Type t)
    {
        if (t.IsEnum
            || t == typeof(ulong)
            || t == typeof(long)
            || t == typeof(int)
            || t == typeof(uint)
            || t == typeof(float)
            || t == typeof(double)
            || t == typeof(byte)
            || t == typeof(ushort)
            || t == typeof(short)
        )
            return "number";
        if (t == typeof(bool))
            return "bool";
        if (t == typeof(string))
            return "string";
        if (t == typeof(void))
            return "void";

        return t.Name;
    }
    
    
    static bool isObsolete(MemberInfo mb)
    {
        if (mb == null) return false;
        ObsoleteAttribute oa = GetCustomAttribute(mb, typeof(ObsoleteAttribute)) as ObsoleteAttribute;
#if XLUA_GENERAL && !XLUA_ALL_OBSOLETE || XLUA_JUST_EXCLUDE_ERROR
            return oa != null && oa.IsError;
#else
        return oa != null;
#endif
    }
    
    static object GetCustomAttribute(MemberInfo test, Type type)
    {
#if XLUA_GENERAL
            return test.GetCustomAttributes(false).FirstOrDefault(ca => ca.GetType().ToString() == type.ToString());
#else
        return test.GetCustomAttributes(type, false).FirstOrDefault();
#endif
    }
}