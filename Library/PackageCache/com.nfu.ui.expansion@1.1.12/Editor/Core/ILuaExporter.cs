using System;
using System.Collections.Generic;

namespace ND.UI
{
    public interface ILuaExporter
    {
        
        /// <summary>
        /// 生成模板代码
        /// </summary>
        /// <param name="uiExpansion"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        bool GenerateFile(UIExpansion uiExpansion,string filePath);

        
        /// <summary>
        /// 导出的组件类型
        /// </summary>
        /// <returns></returns>
        List<Type> GetCanExportTypes();

        
        /// <summary>
        /// 忽略的组件类型
        /// </summary>
        /// <returns></returns>
        List<Type> GetForceIgnoreTypes();

    }
}