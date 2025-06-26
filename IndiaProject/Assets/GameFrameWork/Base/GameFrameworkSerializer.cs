using System;
using System.Collections.Generic;
using System.IO;

namespace GameFramework
{
    /// <summary>
    /// 游戏框架序列化器基类
    /// </summary>
    public abstract class GameFrameworkSerializer<T>
    {
        private readonly Dictionary<byte, SerializeCallback> m_SerializeCallbacks;
        private readonly Dictionary<byte, DeserializeCallback> m_DeserializeCallbacks;
        private readonly Dictionary<byte, TryGetValueCallback> m_TryGetValueCallbacks;
        private byte m_LatestSerializeCallbackVersion;

        /// <summary>
        /// 初始化游戏框架序列化器基类的新实例
        /// </summary>
        public GameFrameworkSerializer()
        {
            m_SerializeCallbacks = new();
            m_DeserializeCallbacks = new();
            m_TryGetValueCallbacks = new();
            m_LatestSerializeCallbackVersion = 0;
        }

        /// <summary>
        /// 序列化回调函数
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public delegate bool SerializeCallback(Stream stream, T data);

        /// <summary>
        /// 反序列化回调函数
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public delegate T DeserializeCallback(Stream stream);

        /// <summary>
        /// 尝试从指定流获取指定键的值的回调函数
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public delegate bool TryGetValueCallback(Stream stream, string key, out object value);

        /// <summary>
        /// 注册序列化回调函数
        /// </summary>
        /// <param name="version"></param>
        /// <param name="callback"></param>
        public void RegisterSerializedCallback(byte version, SerializeCallback callback)
        {
            if (callback == null)
                throw new GameFrameworkException("Serialize callback is invalid");

            m_SerializeCallbacks[version] = callback;

            if (version > m_LatestSerializeCallbackVersion)
                m_LatestSerializeCallbackVersion = version;
        }

        /// <summary>
        /// 注册反序列化回调函数
        /// </summary>
        /// <param name="version"></param>
        /// <param name="callback"></param>
        public void RegisterDeserializedCallback(byte version, DeserializeCallback callback)
        {
            if (callback == null)
                throw new GameFrameworkException("Deserialize callback is invalid");
            m_DeserializeCallbacks[version] = callback;
        }

        /// <summary>
        /// 注册尝试从指定流获取指定键的值的回调函数
        /// </summary>
        /// <param name="version"></param>
        /// <param name="callback"></param>
        public void RegisterTryGetValueCallback(byte version, TryGetValueCallback callback)
        {
            if (callback == null)
                throw new GameFrameworkException("try Get value callback is invalid");
            m_TryGetValueCallbacks[version] = callback;
        }

        /// <summary>
        /// 序列化数据到目标流中
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool Serialize(Stream stream, T data)
        {
            if (m_SerializeCallbacks.Count <= 0)
                throw new GameFrameworkException("No Serialize callback registered");
            return Serialize(stream, data, m_LatestSerializeCallbackVersion);
        }

        /// <summary>
        /// 序列化数据到目标流中。
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="data"></param>
        /// <param name="m_LatestSerializeCallbackVersion"></param>
        /// <returns></returns>
        private bool Serialize(Stream stream, T data, byte version)
        {
            byte[] header = GetHeader();
            stream.WriteByte(header[0]);
            stream.WriteByte(header[1]);
            stream.WriteByte(header[2]);
            stream.WriteByte(version);
            SerializeCallback callback = null;
            if (!m_SerializeCallbacks.TryGetValue(version, out callback))
            {
                //TODO:throw new GameFrameworkException(Utility.Text.Format("Deserialize callback '{0}' is not exist.", version));
            }

            return callback(stream, data);
        }

        public T Deserialize(Stream stream)
        {
            byte[] header = GetHeader();
            byte header0 = (byte)stream.ReadByte();
            byte header1 = (byte)stream.ReadByte();
            byte header2 = (byte)stream.ReadByte();
            if (header0 != header[0] || header1 != header[1] || header2 != header[2])
            {
                //TODO:throw new GameFrameworkException(Utility.Text.Format("Header is invalid, need '{0}{1}{2}', current '{3}{4}{5}'.", (char)header[0], (char)header[1], (char)header[2], (char)header0, (char)header1, (char)header2));
            }
            byte version = (byte)stream.ReadByte();
            DeserializeCallback callback = null;

            if (!m_DeserializeCallbacks.TryGetValue(version, out callback))
            {
                //TODO: throw new GameFrameworkException(Utility.Text.Format("Deserialize callback '{0}' is not exist.", version));
            }

            return callback(stream);
        }

        /// <summary>
        /// 获取数据投标识
        /// </summary>
        /// <returns></returns>
        protected abstract byte[] GetHeader();
    }
}
