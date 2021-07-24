/*
 * 2019/10/12 武文飞
 */

using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace WebCommon.DataBase
{
    /// <summary>
    /// Defines the <see cref="DataConvert{T}" />
    /// </summary>
    /// <typeparam name="T">实体类</typeparam>
    public static class DataConvert<T> where T : new()
    {
        /// <summary>
        /// 将DataTable转换为实体类
        /// </summary>
        /// <param name="dt">DataTable<see cref="DataTable"/></param>
        /// <returns>实体类<see cref="List{T}"/></returns>
        public static List<T> ConvertDataTableToModel(DataTable dt)
        {
            List<T> result = new List<T>();

            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }
            //按行循环
            foreach (DataRow item in dt.Rows)
            {
                T temp = new T();
                //按列循环
                for (int i = 0; i <= item.Table.Columns.Count - 1; i++)
                {
                    PropertyInfo property = temp.GetType().GetProperty(item.Table.Columns[i].ColumnName);
                    if (property != null && item[i] != DBNull.Value)
                    {
                        property.SetValue(temp, item[i], null);
                    }
                }
                result.Add(temp);
            }
            return result;
        }

        /// <summary>
        /// 将实体类转换为DataTable
        /// </summary>
        /// <param name="ts">实体类<see cref="List{T}"/></param>
        /// <returns>DataTable<see cref="DataTable"/></returns>
        public static DataTable ConvertModelToDataTable(List<T> ts)
        {
            DataTable result = null;

            if (ts != null || ts.Count == 0)
            {
                return null;
            }
            else
            {
                result = new DataTable(typeof(T).Name);
                foreach (PropertyInfo property in typeof(T).GetProperties())
                {
                    result.Columns.Add(new DataColumn(property.Name, property.PropertyType));
                }
            }

            foreach (T item in ts)
            {
                DataRow temp = result.NewRow();
                foreach (PropertyInfo property in typeof(T).GetProperties())
                {
                    temp[property.Name] = property.GetValue(temp, null);
                }
                result.Rows.Add(temp);
            }

            return result;
        }

        /// <summary>
        /// 将参数填入到sql中,用于程序异常时,输出完整的sql,方便检查错误.
        /// </summary>
        /// <param name="sql">The sql<see cref="string"/></param>
        /// <param name="parms">The parms<see cref="Dictionary{string, object}"/></param>
        /// <returns>SQL <see cref="string"/></returns>
        public static string SqlFormat(string sql, Dictionary<string, object> parms)
        {
            string result = null;
            string sqlTemp = sql;
            if (parms == null)
            {
                return sqlTemp;
            }
            foreach (var item in parms.Keys)
            {
            }
            return result;
        }
    }
}