/*
 * 2019/11/20 武文飞
 */

using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace WebCommon.Log
{
    /// <summary>
    /// 非阻塞的日志基类 <see cref="Log_QueueUsed" />
    /// </summary>
    internal class Log_QueueUsed
    {
        /// <summary>
        /// 日志队列
        /// </summary>
        private static readonly ConcurrentQueue<LogMessage> logsQue;

        /// <summary>
        /// 线程通信对象
        /// </summary>
        private static readonly ManualResetEvent resetEvent;

        /// <summary>
        /// Initializes static members of the <see cref="Log_QueueUsed"/> class.
        /// </summary>
        static Log_QueueUsed()
        {
            logsQue = new ConcurrentQueue<LogMessage>();
            resetEvent = new ManualResetEvent(false);
            //Task,Run()和Task.Delay()在.net framework4.0 中不可用
            Task.Run(() => Initialize());
            //ThreadEx.TaskEx.Run(() => Initialize());
        }

        /// <summary>
        /// The Initialize
        /// </summary>
        private static void Initialize()
        {
            while (true)
            {
                resetEvent.WaitOne();
                Write();
                resetEvent.Reset();
                Thread.Sleep(1);
            }
        }

        /// <summary>
        /// 将一个LogMessage对象写到日志队列中
        /// </summary>
        /// <param name="logMessage">The logMessage<see cref="LogMessage"/></param>
        public static void Write(LogMessage logMessage)
        {
            logsQue.Enqueue(logMessage);
            resetEvent.Set();
        }

        /// <summary>
        /// 从日志队列中取出一个LogMessage对象,调用Log_QueueNotUsed.Write()方法写日志
        /// </summary>
        private static void Write()
        {
            while (logsQue.Count > 0)
            {
                logsQue.TryDequeue(out LogMessage log);
                Log_QueueNotUsed.Write(log);
            }
        }
    }
}
