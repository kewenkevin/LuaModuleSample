using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.StringFormats
{

    /// <summary>
    /// 格式：#Name，如: {0:#FileName}
    /// </summary>
    public interface INameFormatter
    {
        int Priority { get; }

        string Name { get; }

        string Format(object arg, string formatArg);
    }

}
