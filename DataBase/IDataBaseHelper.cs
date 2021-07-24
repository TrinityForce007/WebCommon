/*
 * 2019/10/11 武文飞
 */

using System.Collections.Generic;
using System.Data;

namespace WebCommon.DataBase
{
    /// <summary>
    /// Defines the <see cref="IDataBaseHelper" />
    /// </summary>
    internal interface IDataBaseHelper
    {
        /// <summary>
        /// 开启连接
        /// </summary>
        void Open();

        /// <summary>
        /// 关闭连接
        /// </summary>
        void Close();

        /// <summary>
        /// 开启事务
        /// </summary>
        void BeginTransaction();

        /// <summary>
        /// 提交事务
        /// </summary>
        void Commit();

        /// <summary>
        /// 回滚事务
        /// </summary>
        void RollBack();

        /// <summary>
        /// 执行Insert/Update/delete
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <returns>返回受影响的行数</returns>
        int ExecuteNonQuery(string sql);

        /// <summary>
        /// 执行Insert/Update/delete
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parms"></param>
        /// <returns></returns>
        int ExecuteNonQuery(string sql, Dictionary<string, object> parms);

        /// <summary>
        /// 查询单个值
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <returns></returns>
        object ExecuteScalar(string sql);

        /// <summary>
        /// 查询单个值
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parms"></param>
        /// <returns></returns>
        object ExecuteScalar(string sql, Dictionary<string, object> parms);

        /// <summary>
        /// 返回DataTable
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <returns></returns>
        DataTable ExecuteDataTable(string sql);

        /// <summary>
        /// 返回DataTable
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parms"></param>
        /// <returns></returns>
        DataTable ExecuteDataTable(string sql, Dictionary<string, object> parms);

        /// <summary>
        /// 返回DataDataSet
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <returns></returns>
        DataSet ExecuteDataSet(string sql);

        /// <summary>
        /// 返回DataDataSet
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parms"></param>
        /// <returns></returns>
        DataSet ExecuteDataSet(string sql, Dictionary<string, object> parms);

        /// <summary>
        /// 返回DataReader
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <returns></returns>
        IDataReader ExecuteDataReader(string sql);

        /// <summary>
        /// 返回DataReader
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parms"></param>
        /// <returns></returns>
        IDataReader ExecuteDataReader(string sql, Dictionary<string, object> parms);

        /// <summary>
        /// 查询一个表的行数
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        int GetRowsCount(string tableName);

        /// <summary>
        /// 查询得到实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">SQL语句</param>
        /// <returns></returns>
        List<T> ExecuteData<T>(string sql) where T : new();

        /// <summary>
        /// 查询得到实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">SQL语句</param>
        /// <param name="parms"></param>
        /// <returns></returns>
        List<T> ExecuteData<T>(string sql, Dictionary<string, object> parms) where T : new();
    }
}