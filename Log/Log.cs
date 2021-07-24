using System;
using System.Collections.Concurrent;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Common
{
    /// <summary>
    /// 日志工具类
    /// 使用方式 Log.Info()、Log.Warn、Log.Error
    /// </summary>
    public class Log
    {
        /// <summary>
        /// 线程安全队列
        /// </summary>
        private static readonly ConcurrentQueue<LogMessage> logsQue;
        /// <summary>
        /// 信号
        /// </summary>
        private static readonly ManualResetEvent resetEvent;
        /// <summary>
        /// 日志文件锁
        /// </summary>
        private static readonly ReaderWriterLockSlim lockSlim;


        static Log()
        {
            logsQue = new ConcurrentQueue<LogMessage>();
            resetEvent = new ManualResetEvent(false);
            lockSlim = new ReaderWriterLockSlim();
            Task.Run(() => Initialize());
        }

        /// <summary>
        /// 消息日期
        /// </summary>
        /// <param name="message">日志内容</param>
        /// <param name="args">格式化参数</param>
        public static void Info(string message, params string[] args)
        {
            StackFrame sf = new StackTrace(true).GetFrame(1);
            LogMessage logMessage = new LogMessage
            {
                level = LogLevel.Info,
                message = string.Format(Regex.Replace(message?.Replace("{", "{{").Replace("}", "}}") ?? "", @"{{(\d+)}}", "{$1}"), args),
                stackFrame = sf
            };
            logsQue.Enqueue(logMessage);
            resetEvent.Set();
        }

        /// <summary>
        /// 警告日志
        /// </summary>
        /// <param name="messge">日志内容</param>
        public static void Warn(string messge)
        {
            StackFrame sf = new StackTrace(true).GetFrame(1);
            LogMessage logMessage = new LogMessage
            {
                level = LogLevel.Warn,
                message = messge,
                stackFrame = sf
            };
            logsQue.Enqueue(logMessage);
            resetEvent.Set();
        }

        /// <summary>
        /// 错误日志
        /// </summary>
        /// <param name="message">日志内容</param>
        /// <param name="args">格式化参数</param>
        public static void Error(string message, params string[] args)
        {
            StackFrame sf = new StackTrace(true).GetFrame(1);
            LogMessage logMessage = new LogMessage
            {
                level = LogLevel.Error,
                message = string.Format(Regex.Replace(message?.Replace("{", "{{").Replace("}", "}}") ?? "", @"{{(\d+)}}", "{$0}"), args),
                stackFrame = sf
            };
            logsQue.Enqueue(logMessage);
            resetEvent.Set();
        }
        /// <summary>
        /// 错误日志
        /// </summary>
        /// <param name="ex">错误Exception对象</param>
        /// <param name="message">日志内容</param>
        /// <param name="args">格式化参数</param>
        public static void Error(Exception ex, string message, params string[] args)
        {
            StackFrame sf = null;
            if (ex != null)
            {
                StackFrame[] frame = new StackTrace(ex, true).GetFrames();
                sf = frame?[frame.Length - 1];
            }
            else
            {
                new StackTrace(true).GetFrame(1);

            }
            LogMessage logMessage = new LogMessage
            {
                level = LogLevel.Error,
                message = string.Format(Regex.Replace(message?.Replace("{", "{{").Replace("}", "}}") ?? "", @"{{(\d+)}}", "{$1}"), args),
                exception = ex,
                stackFrame = sf
            };
            logsQue.Enqueue(logMessage);
            resetEvent.Set();

        }

        /// <summary>
        /// 初始化
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
        /// 写日志
        /// </summary>
        private static void Write()
        {
            //获取日志路径
            string logDir = GetPhysicalPath(ConfigurationManager.AppSettings["logPath"] ?? @"Logs\Error");
            //按照当天日期生成日志文件名
            string fielName = DateTime.Now.ToShortDateString().ToString() + ".log";
            fielName = fielName.Replace('/', '-');
            string logPath = logDir + fielName;

            try
            {
                //日志锁，不允许别人写入
                lockSlim.EnterWriteLock();

                if (Directory.Exists(logDir))
                {
                    Directory.CreateDirectory(logDir);
                }

                StreamWriter writer = new StreamWriter(logPath, true, Encoding.UTF8);

                while (logsQue.Count > 0)
                {
                    LogMessage log = null;

                    //从队列中取出一个LogMessage对象
                    logsQue.TryDequeue(out log);

                    if (writer != null)
                    {
                        writer.WriteLine("[级别：" + log.level + "]");
                        writer.WriteLine("[时间：" + DateTime.Now.ToString() + "]");
                        writer.WriteLine("[类名：" + log.stackFrame.GetMethod().DeclaringType.FullName + "]");
                        writer.WriteLine("[方法：" + log.stackFrame.GetMethod().Name + "]");
                        writer.WriteLine("[行号：" + log.stackFrame.GetFileLineNumber() + "]");

                        if (!string.IsNullOrEmpty(log.message))
                        {
                            writer.WriteLine("[内容：" + log.message + "]");
                        }

                        if (log.exception != null)
                        {
                            writer.WriteLine("[异常：" + log.exception.StackTrace.ToString() + "]");
                        }
                        writer.WriteLine("--------------------------------------------------------------------------------------------------------------------");
                    }
                }
                writer.Close();
                writer.Dispose();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                //退出死锁
                lockSlim.ExitWriteLock();
            }
        }

        /// <summary>
        /// 获取物理路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static string GetPhysicalPath(string path)
        {
            string physicalPath = AppDomain.CurrentDomain.BaseDirectory;

            if (!string.IsNullOrEmpty(path))
            {
                path = path.Replace("~", "").Replace("/", @"\").TrimStart('\\').TrimEnd('\\');
                int start = path.LastIndexOf('\\') + 1;
                int length = path.Length - start;
                physicalPath = Path.Combine(physicalPath, path.Substring(start, length).Contains(".") ? path : path + @"\");
            }
            return physicalPath;
        }

        /// <summary>
        /// 日志级别
        /// </summary>
        private enum LogLevel
        {
            Info,
            Warn,
            Error
        }

        /// <summary>
        /// 日志Model
        /// </summary>
        private class LogMessage
        {
            /// <summary>
            /// 异常级别
            /// </summary>
            public LogLevel level { get; set; }
            /// <summary>
            /// 异常信息
            /// </summary>
            public string message { get; set; }
            /// <summary>
            /// 异常
            /// </summary>
            public Exception exception { get; set; }
            /// <summary>
            /// 栈帧
            /// </summary>
            public StackFrame stackFrame { get; set; }

        }
    }
}
