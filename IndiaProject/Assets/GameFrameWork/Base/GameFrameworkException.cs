using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace GameFramework
{
    [Serializable]
    public class GameFrameworkException : Exception
    {
        /// <summary>
        /// 初始化游戏框架异常类的新实例
        /// </summary>
        /// <returns></returns>
        public GameFrameworkException() : base()
        {
        }

        /// <summary>
        /// 使用指定错误信息初始化游戏框架异常类的新实例
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public GameFrameworkException(string message) : base(message)
        {

        }

        /// <summary>
        /// 使用指定错误消息和对作为此异常原因的内部异常的引用来初始化游戏框架异常类的新实例
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        /// <returns></returns>
        public GameFrameworkException(string message, Exception innerException) : base(message, innerException)
        {

        }

        /// <summary>
        /// 用序列化数据初始化游戏框架异常类的新实例
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        protected GameFrameworkException(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }
    }
}