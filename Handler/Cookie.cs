/*
 * 2019/10/31 武文飞
 */

using System;
using System.Web;

namespace WebCommon.AspHandleBase
{
    /// <summary>
    /// WebCookie操作 <see cref="Cookie" />
    /// </summary>
    internal class Cookie
    {
        /// <summary>
        /// 添加一个Cookie
        /// </summary>
        /// <param name="cookieName">Cookie名<see cref="string"/></param>
        /// <param name="cookieValue">Cookie值<see cref="string"/></param>
        /// <param name="date">有效期(小时)<see cref="double"/></param>
        /// <param name="domain">有效域名<see cref="string"/></param>
        /// <param name="path">有效路径<see cref="string"/></param>
        public static void Add(string cookieName, string cookieValue, double date, string domain, string path)
        {
            HttpCookie cookie = new HttpCookie(cookieName)
            {
                Value = cookieValue
            };
            DateTime validate = new DateTime();
            validate = validate.AddHours(date);
            cookie.Expires = validate;
            cookie.Domain = domain;
            cookie.Path = path;
            cookie.HttpOnly = true;
            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        /// <summary>
        /// 添加一个Cookie
        /// </summary>
        /// <param name="cookieName">Cookie名<see cref="string"/></param>
        /// <param name="cookieValue">Cookie值<see cref="string"/></param>
        public static void Add(string cookieName, string cookieValue)
        {
            Add(cookieName, cookieValue, 0, "", "");
        }

        /// <summary>
        /// 获取Cookie值
        /// </summary>
        /// <param name="cookieName">Cookie名<see cref="string"/></param>
        /// <returns>The <see cref="string"/></returns>
        public static string GetValue(string cookieName)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[cookieName];
            if (cookie == null)
            {
                return "";
            }
            else
            {
                return cookie.Value;
            }
        }

        /// <summary>
        /// 删除一个Cookie
        /// </summary>
        /// <param name="cookieName">Cookie名<see cref="string"/></param>
        /// <param name="domain">有效域名<see cref="string"/></param>
        /// <param name="path">有效路径<see cref="string"/></param>
        public static void Delete(string cookieName, string domain, string path)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[cookieName];
            if (cookie != null)
            {
                cookie.Domain = domain;
                cookie.Path = path;
                cookie.Values.Clear();
                cookie.Expires = DateTime.Now.AddDays(-1);
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
        }

        /// <summary>
        /// 删除一个Cookie
        /// </summary>
        /// <param name="cookieName">Cookie名<see cref="string"/></param>
        public static void Delete(string cookieName)
        {
            Delete(cookieName, "", "");
        }
    }
}