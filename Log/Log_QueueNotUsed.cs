/*
 * 2019/11/20 武文飞
 */

using System;
using System.Configuration;
using System.IO;
using System.Text;
using System.Threading;

namespace WebCommon.Log
{
    /// <summary>
    /// 阻塞的日志基类 <see cref="Log_QueueNotUsed" />
    /// </summary>
    internal class Log_QueueNotUsed
    {
        /// <summary>
        /// 文件读写锁
        /// </summary>
        private static readonly ReaderWriterLockSlim lockSlim;

        /// <summary>
        /// Initializes static members of the <see cref="Log_QueueNotUsed"/> class.
        /// </summary>
        static Log_QueueNotUsed()
        {
            lockSlim = new ReaderWriterLockSlim();
        }

        /// <summary>
        /// 将日志写入到日志文件中
        /// </summary>
        /// <param name="log">LogMessage对象<see cref="LogMessage"/></param>
        public static void Write(LogMessage log)
        {
            //获取日志路径
            string logDir = GetPhysicalPath(ConfigurationManager.AppSettings["logPath"] ?? AppDomain.CurrentDomain.BaseDirectory);
            //按照当天日期生成日志文件名
            string fileName = DateTime.Now.ToShortDateString().ToString() + ".log";
            fileName = fileName.Replace('/', '-');

            try
            {
                //日志锁，不允许别人写入
                //适合文件写入次数远大于读取次数的场景
                lockSlim.EnterWriteLock();
                //路径不存在时则创建
                if (Directory.Exists(logDir))
                {
                    Directory.CreateDirectory(logDir);
                }

                string logPath = logDir + fileName;
                StreamWriter writer = new StreamWriter(logPath, true, Encoding.UTF8);

                if (writer != null)
                {
                    writer.WriteLine("[级别]：" + log.level);
                    writer.WriteLine("[时间]：" + DateTime.Now.ToString());
                    writer.WriteLine("[方法]：" + log.stackFrame.GetMethod().DeclaringType.FullName);

                    if (!string.IsNullOrEmpty(log.message))
                    {
                        writer.WriteLine("[内容]：" + log.message);
                    }

                    if (null != log.exception)
                    {
                        if (null != log.exception.StackTrace)
                        {
                            writer.WriteLine("[异常]：" + log.exception.StackTrace.ToString());
                        }
                    }
                    writer.WriteLine("---------");
                }
                writer.Close();
                writer.Dispose();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                //退出死锁
                lockSlim.ExitWriteLock();
            }
        }

        /// <summary>
        /// 获取文件地址
        /// </summary>
        /// <param name="path">The path<see cref="string"/></param>
        /// <returns>The <see cref="string"/></returns>
        public static string GetPhysicalPath(string path)
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
    }
}
