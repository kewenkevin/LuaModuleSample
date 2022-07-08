//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using System.IO;
using System.Text;
using ND.Managers.ResourceMgr.Framework.Resource;

namespace ND.Managers.ResourceMgr.Runtime
{
    /// <summary>
    /// 内置版本资源列表序列化器。
    /// </summary>
    public static partial class BuiltinVersionListSerializer
    {
        /// <summary>
        /// 尝试从可更新模式版本资源列表（版本 0）获取指定键的值回调函数。
        /// </summary>
        /// <param name="stream">指定流。</param>
        /// <param name="key">指定键。</param>
        /// <param name="value">指定键的值。</param>
        /// <returns></returns>
        public static bool UpdatableVersionListTryGetHeaderCallback_V0(Stream stream, out UpdatableVersionList header)
        {
            using (BinaryReader binaryReader = new BinaryReader(stream, Encoding.UTF8))
            {
                byte[] encryptBytes = binaryReader.ReadBytes(CachedHashBytesLength);
                string applicableGameVersion = binaryReader.ReadEncryptedString(encryptBytes);
                string variantPrefixUrl = binaryReader.ReadEncryptedString(encryptBytes);
                int internalResourceVersion = binaryReader.ReadInt32();

                header = new UpdatableVersionList(applicableGameVersion, internalResourceVersion, null, null, null, null, variantPrefixUrl);
            }

            return true;
        }

        /// <summary>
        /// 尝试从可更新模式版本资源列表（版本 1 或版本 2）获取指定键的值回调函数。
        /// </summary>
        /// <param name="stream">指定流。</param>
        /// <param name="key">指定键。</param>
        /// <param name="value">指定键的值。</param>
        /// <returns></returns>
        public static bool UpdatableVersionListTryGetHeaderCallback_V1_V2(Stream stream, out UpdatableVersionList header)
        {
            using (BinaryReader binaryReader = new BinaryReader(stream, Encoding.UTF8))
            {
                byte[] encryptBytes = binaryReader.ReadBytes(CachedHashBytesLength);
                string applicableGameVersion = binaryReader.ReadEncryptedString(encryptBytes);
                string variantPrefixUrl = binaryReader.ReadEncryptedString(encryptBytes);
                int internalResourceVersion = binaryReader.Read7BitEncodedInt32();

                header = new UpdatableVersionList(applicableGameVersion, internalResourceVersion, null, null, null, null, variantPrefixUrl);
            }

            return true;
        }
    }
}
