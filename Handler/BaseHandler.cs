/*
 * 2019/11/20 武文飞
 */

using System;
using System.Web;

namespace WebCommon.Handler
{
    /// <summary>
    /// 一般处理程序扩展 <see cref="BaseHandler" />
    /// </summary>
    public class BaseHandler : IHttpHandler
    {
        /// <summary>
        /// Defines the Response
        /// </summary>
        protected HttpResponse Response;

        /// <summary>
        /// Defines the Request
        /// </summary>
        protected HttpRequest Request;

        /// <summary>
        /// The ProcessRequest
        /// </summary>
        /// <param name="context">The context<see cref="HttpContext"/></param>
        public void ProcessRequest(HttpContext context)
        {
            this.Response = context.Response;
            this.Request = context.Request;
            try
            {
                LocalRequest();
            }
            catch (Exception ex)
            {
                Log.Error.Write(false, ex.Message, ex);
                Response.Write("{\"Data\":null,\"Status\":\"error\",\"Message\":\"" + ex.Message + "\"}");
            }
            Response.Flush();
        }

        /// <summary>
        /// 在子类中重写这个方法
        /// </summary>
        public virtual void LocalRequest()
        {
        }

        /// <summary>
        /// Gets a value indicating whether IsReusable
        /// </summary>
        public bool IsReusable
        {
            get { return true; }
        }
    }
}
