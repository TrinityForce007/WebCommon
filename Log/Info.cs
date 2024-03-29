﻿/*
 * 2019/11/20 武文飞
 */

using System.Diagnostics;

namespace WebCommon.Log
{
    /// <summary>
    /// 消息日志工具类 <see cref="Info" />
    /// </summary>
    public static class Info
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
                level = LogMessage.LogLevel.Info,
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
    }
}
