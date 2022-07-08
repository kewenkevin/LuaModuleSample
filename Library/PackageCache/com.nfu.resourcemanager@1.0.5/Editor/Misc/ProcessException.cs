using System;
using System.Collections;
using System.Collections.Generic;

namespace ND.Managers.ResourceMgr.Editor
{
    /// <summary>
    /// 进程运行异常
    /// </summary>
    class ProcessException : Exception
    {

        public ProcessException(string message, string filePath, string arguments, string output, Exception innerException)
            : base(message, innerException)
        {
            FilePath = filePath;
            Arguments = arguments;
            Output = output;
        }

        public ProcessException(string message, string filePath, string arguments, string output)
            : this(message, filePath, arguments, output, null)
        {
        }

        public ProcessException(string message, Exception innerException = null)
            : this(message, null, null, null, innerException)
        {
        }

        /// <summary>
        /// 执行文件路径
        /// </summary>
        public string FilePath { get; private set; }

        /// <summary>
        /// 执行文件参数
        /// </summary>
        public string Arguments { get; private set; }

        /// <summary>
        /// 输出
        /// </summary>
        public string Output { get; private set; }

        public override string ToString()
        {
            return $"{GetType().FullName}: {Message} (FilePath '{FilePath}' Arguments '{Arguments}' Output '{Output}')";
        }
    }


}