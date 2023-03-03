/*
 * 2019/11/20 武文飞
 */

using System;
using System.Diagnostics;

namespace WebCommon.Log
{
    /// <summary>
    /// 错误日志工具类 <see cref="Error" />
    /// </summary>
    public static class Error
    {
        /// <summary>
        /// 写一条日志
        /// </summary>
        /// <param name="isQueue">日志队列<see cref="bool"/></param>
        /// <param name="message">日志内容<see cref="string"/></param>
        public static void Write(bool isQueue, string message)
        {
            StackFrame sf = new StackTrace(true).GetFrame(1);
            LogMessage log = new LogMessage
            {
                level = LogMessage.LogLevel.Error,
                message = message,
                exception = null,
                stackFrame = sf
            };

            if (isQueue)
            {
                Log_QueueUsed.Write(log);
            }
            else
            {
                Log_QueueNotUsed.Write(log);
            }
        }

        /// <summary>
        /// 写一条日志
        /// </summary>
        /// <param name="isQueue">日志队列<see cref="bool"/></param>
        /// <param name="message">日志内容<see cref="string"/></param>
        /// <param name="ex">Exception对象<see cref="Exception"/></param>
        public static void Write(bool isQueue, string message, Exception ex)
        {
            StackFrame sf = null;
            if (ex != null)
            {
                StackFrame[] frame = new StackTrace(ex, true).GetFrames();
                if (frame != null)
                {
                    sf = frame[frame.Length - 1];
                }
            }
            else
            {
                sf = new StackTrace(true).GetFrame(1);
            }
            LogMessage log = new LogMessage
            {
                level = LogMessage.LogLevel.Error,
                message = message,
                exception = ex,
                stackFrame = sf
            };

            if (isQueue)
            {
                Log_QueueUsed.Write(log);
            }
            else
            {
                Log_QueueNotUsed.Write(log);
            }
        }
    }
}
