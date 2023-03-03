using System.Text.RegularExpressions;

namespace WebCommon
{
    /// <summary>
    /// 正则表达式帮助类
    /// </summary>
    public static class RegexHelper
    {
        /// <summary>
        /// 正则表达式，截取两个字符串中间的字符串
        /// </summary>
        /// <param name="source">目标字符串</param>
        /// <param name="startStr">起始字符串</param>
        /// <param name="endStr">结束字符串</param>
        /// <returns>截取的结果</returns>
        public static string CutOutStrBetweenStartAndEnd(string source, string startStr = "", string endStr = "")
        {
            Regex rg;
            if (endStr.Equals(""))  //提取特定字符串开头的内容
            {
                rg = new Regex("(?<=(" + startStr + "))[.\\s\\S]*");
            }
            else if (startStr.Equals(""))  //提取特定字符串结尾的内容
            {
                rg = new Regex(".*?(?=(" + endStr + "))");
            }
            else  //提取特定字符串开头与结尾之间的内容
            {
                rg = new Regex("(?<=(" + startStr + "))[.\\s\\S]*?(?=(" + endStr + "))");
            }
            return rg.Match(source).Value;
        }
    }
}