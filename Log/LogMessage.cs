/*
 * 2019/11/20 武文飞
 */

using System;
using System.Diagnostics;

namespace WebCommon.Log
{
    /// <summary>
    /// LogMessage对象 <see cref="LogMessage" />
    /// </summary>
    internal class LogMessage
    {
        /// <summary>
        /// 日志级别
        /// </summary>
        public enum LogLevel
        {
            /// <summary>
            /// 消息
            /// </summary>
            Info,
            /// <summary>
            /// 警告
            /// </summary>
            Warn,
            /// <summary>
            /// 错误
            /// </summary>
            Error
        }

        /// <summary>
        /// 日志级别
        /// </summary>
        public LogLevel level { get; set; }

        /// <summary>
        /// 日志内容
        /// </summary>
        public string message { get; set; }

        /// <summary>
        /// Exception对象
        /// </summary>
        public Exception exception { get; set; }

        /// <summary>
        /// 栈帧
        /// </summary>
        public StackFrame stackFrame { get; set; }
    }
}
