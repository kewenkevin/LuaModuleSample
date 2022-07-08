using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using ND.Managers.ResourceMgr.Framework;
using ND.Managers.ResourceMgr.Framework.FileSystem;
using Debug = UnityEngine.Debug;

namespace ND.Managers.ResourceMgr.Runtime
{

    /// <summary>
    /// obb 的 InputStream Seek 有问题，所以读取所有字节
    /// </summary>
    public class AndroidInputStream : FileSystemStream
    {
        private static readonly string SplitFlag = "!/assets/";
        private static readonly int SplitFlagLength = SplitFlag.Length;
        private byte[] m_Buffer;

        public AndroidInputStream(string fullPath)
        {
            int position = fullPath.LastIndexOf(SplitFlag, StringComparison.Ordinal);
            if (position < 0)
            {
                throw new GameFrameworkException("Can not find split flag in full path.");
            }

            //android obb文件
            m_Buffer = ResourcesAndroidUtility.ReadFileAllBytes(fullPath);
            if (m_Buffer == null)
            {
                throw new GameFrameworkException(
                    Utility.Text.Format("Open file '{0}' from Android asset manager failure.", fullPath));
            }

        }

        public override long Position
        {
            get;
            set;
        }

        public override long Length => m_Buffer.Length;

        public override void Close()
        {

        }

        public override void Flush()
        {

        }

        public override int Read(byte[] buffer, int startIndex, int length)
        {
            int count;
            if (Position + length > Length)
            {
                count = (int)(Length - Position);
            }
            else
            {
                count = length;
            }
            Buffer.BlockCopy(m_Buffer, (int)Position, buffer, startIndex, count);
            Position += count;
            return count;
        }

        public override void SetLength(long length)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int startIndex, int length)
        {
            throw new NotImplementedException();
        }

        protected override int ReadByte()
        {
            byte b;
            if (Position < Length)
            {
                b = m_Buffer[Position];
                Position++;
            }
            else
            {
                b = 0;
            }
            return b;
        }

        protected override void Seek(long offset, SeekOrigin origin)
        {
            var originPos = Position;
            switch (origin)
            {
                case SeekOrigin.Begin:
                    Position = offset;
                    break;
                case SeekOrigin.Current:
                    Position += offset;
                    break;
                case SeekOrigin.End:
                    //offset 为负数
                    Position = Length - 1 + offset;
                    break;
            }
        }

        protected override void WriteByte(byte value)
        {
            throw new NotImplementedException();
        }
    }
}
