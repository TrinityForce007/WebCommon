/*
 * 2019/11/20 武文飞
 */

using System;
using System.Threading;
using System.Threading.Tasks;

namespace WebCommon.ThreadEx
{
    /// <summary>
    /// 在.Net4.0中实现类似于.Net4.5的Task.Run(),Task.Delay()方法 <see cref="TaskEx" />
    /// </summary>
    public class TaskEx
    {
        /// <summary>
        /// The Run
        /// </summary>
        /// <param name="action">The action<see cref="Action"/></param>
        /// <returns>The <see cref="Task"/></returns>
        public static Task Run(Action action)
        {
            var tcs = new TaskCompletionSource<object>();
            new Thread(() =>
            {
                try
                {
                    action();
                    tcs.SetResult(null);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            })
            { IsBackground = true }.Start();
            return tcs.Task;
        }

        /// <summary>
        /// The Run
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="function">The function<see cref="Func{TResult}"/></param>
        /// <returns>The <see cref="Task{TResult}"/></returns>
        public static Task<TResult> Run<TResult>(Func<TResult> function)
        {
            var tcs = new TaskCompletionSource<TResult>();
            new Thread(() =>
            {
                try
                {
                    tcs.SetResult(function());
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            })
            { IsBackground = true }.Start();
            return tcs.Task;
        }

        /// <summary>
        /// The Delay
        /// </summary>
        /// <param name="milliseconds">The milliseconds<see cref="int"/></param>
        /// <returns>The <see cref="Task"/></returns>
        public static Task Delay(int milliseconds)
        {
            var tcs = new TaskCompletionSource<object>();
            var timer = new System.Timers.Timer(milliseconds) { AutoReset = false };
            timer.Elapsed += delegate { timer.Dispose(); tcs.SetResult(null); };
            timer.Start();
            return tcs.Task;
        }
    }
}
