using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
using ND.Managers.ResourceMgr.Framework.Resource;

namespace ND.Managers.ResourceMgr.Runtime
{
    public static class Extensions
    {
        public static T LoadAssetImmediate<T>(this IResourceManager mgr, string assetName)
            where T : UnityEngine.Object
        {
            return (T)mgr.LoadAssetImmediate(assetName, typeof(T));
        }
        
        public static IEnumerable<Assembly> Referenced(this IEnumerable<Assembly> assemblies, Assembly referenced)
        {
            string fullName = referenced.FullName;

            foreach (var ass in assemblies)
            {
                if (referenced == ass)
                {
                    yield return ass;
                }
                else
                {
                    foreach (var refAss in ass.GetReferencedAssemblies())
                    {
                        if (fullName == refAss.FullName)
                        {
                            yield return ass;
                            break;
                        }
                    }
                }
            }
        }
    }
}
