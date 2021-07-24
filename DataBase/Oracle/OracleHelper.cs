/*
 * 2019/10/11 武文飞
 */

using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;

namespace WebCommon.DataBase.Oracle
{
    /// <summary>
    /// Defines the <see cref="OracleHelper" />
    /// </summary>
    public class OracleHelper : IDataBaseHelper
    {
        /// <summary>
        /// Defines the _conn
        /// </summary>
        private OracleConnection _conn = null;

        /// <summary>
        /// Defines the _connStr
        /// </summary>
        private string _connStr = null;

        /// <summary>
        /// Defines the _cmd
        /// </summary>
        private OracleCommand _cmd = null;

        /// <summary>
        /// Defines the _ada
        /// </summary>
        private OracleDataAdapter _ada = null;

        /// <summary>
        /// Defines the _trans
        /// </summary>
        private OracleTransaction _trans = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="OracleHelper"/> class.
        /// </summary>
        /// <param name="connStr">The connStr<see cref="string"/></param>
        public OracleHelper(string connStr = null)
        {
            if (connStr != null)
            {
                this._connStr = connStr;
                _conn = new OracleConnection(_connStr);
            }
            else
            {
                _conn = new OracleConnection();
            }
            _cmd = new OracleCommand();
        }

        /// <summary>
        /// Gets or sets the ConnectionString
        /// </summary>
        public string ConnectionString
        {
            get
            {
                return this._connStr;
            }
            set
            {
                this._connStr = value;
                this._conn.ConnectionString = this._connStr;
            }
        }

        /// <summary>
        /// The Open
        /// </summary>
        public void Open()
        {
            if (_conn.State == ConnectionState.Closed)
            {
                _conn.Open();
            }
            else if (_conn.State == ConnectionState.Broken)
            {
                _conn.Close();
                _conn.Open();
            }
        }

        /// <summary>
        /// The Close
        /// </summary>
        public void Close()
        {
            _conn.Close();
        }

        /// <summary>
        /// The BeginTransaction
        /// </summary>
        public void BeginTransaction()
        {
            _trans = _conn.BeginTransaction();
            _cmd.Transaction = _trans;
        }

        /// <summary>
        /// The Commit
        /// </summary>
        public void Commit()
        {
            if (_trans != null)
            {
                _trans.Commit();
            }
        }

        /// <summary>
        /// The RollBack
        /// </summary>
        public void RollBack()
        {
            if (_trans != null)
            {
                _trans.Rollback();
            }
        }

        /// <summary>
        /// 执行SQL,返回实体类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The sql<see cref="string"/></param>
        /// <returns>The <see cref="List{T}"/></returns>
        public List<T> GetModel<T>(string sql) where T : new()
        {
            List<T> result = new List<T>();
            DataTable dt = ExecuteDataTable(sql);
            result = DataConvert<T>.ConvertDataTableToModel(dt);
            return result;
        }

        /// <summary>
        /// 执行SQL,返回实体类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The sql<see cref="string"/></param>
        /// <param name="parms">The parms<see cref="Dictionary{string, object}"/></param>
        /// <returns>The <see cref="List{T}"/></returns>
        public List<T> GetModel<T>(string sql, Dictionary<string, object> parms) where T : new()
        {
            List<T> result = new List<T>();
            DataTable dt = ExecuteDataTable(sql, parms);
            result = DataConvert<T>.ConvertDataTableToModel(dt);
            return result;
        }

        /// <summary>
        /// 执行SQL,返回DataReader
        /// </summary>
        /// <param name="sql">The sql<see cref="string"/></param>
        /// <returns>The <see cref="IDataReader"/></returns>
        public IDataReader ExecuteDataReader(string sql)
        {
            _cmd.CommandText = sql;
            IDataReader result = _cmd.ExecuteReader();
            _cmd.Dispose();
            return result;
        }

        /// <summary>
        /// 执行SQL,返回DataReader
        /// </summary>
        /// <param name="sql">The sql<see cref="string"/></param>
        /// <param name="parms">The parms<see cref="Dictionary{string, object}"/></param>
        /// <returns>The <see cref="IDataReader"/></returns>
        public IDataReader ExecuteDataReader(string sql, Dictionary<string, object> parms)
        {
            SqlFormatCommand(sql, parms);
            IDataReader result = _cmd.ExecuteReader();
            _cmd.Dispose();
            return result;
        }

        /// <summary>
        /// 执行SQL,返回DataSet
        /// </summary>
        /// <param name="sql">The sql<see cref="string"/></param>
        /// <returns>The <see cref="DataSet"/></returns>
        public DataSet ExecuteDataSet(string sql)
        {
            DataSet result = new DataSet();
            SqlFormatCommand(sql);
            _ada = new OracleDataAdapter(sql, _conn);
            _ada.Fill(result);
            _cmd.Dispose();
            _ada.Dispose();
            return result;
        }

        /// <summary>
        /// 执行SQL,返回DataSet
        /// </summary>
        /// <param name="sql">The sql<see cref="string"/></param>
        /// <param name="parms">The parms<see cref="Dictionary{string, object}"/></param>
        /// <returns>The <see cref="DataSet"/></returns>
        public DataSet ExecuteDataSet(string sql, Dictionary<string, object> parms)
        {
            DataSet result = new DataSet();
            SqlFormatCommand(sql, parms);
            _ada = new OracleDataAdapter(sql, _conn);
            _ada.SelectCommand = _cmd;
            _ada.Fill(result);
            _cmd.Dispose();
            _ada.Dispose();
            return result;
        }

        /// <summary>
        /// 执行SQL,返回DataTable
        /// </summary>
        /// <param name="sql">The sql<see cref="string"/></param>
        /// <returns>The <see cref="DataTable"/></returns>
        public DataTable ExecuteDataTable(string sql)
        {
            DataTable result = new DataTable();
            SqlFormatCommand(sql);
            _ada = new OracleDataAdapter(sql, _conn);
            _ada.Fill(result);
            _cmd.Dispose();
            _ada.Dispose();
            return result;
        }

        /// <summary>
        /// 执行SQL,返回DataTable
        /// </summary>
        /// <param name="sql">The sql<see cref="string"/></param>
        /// <param name="parms">The parms<see cref="Dictionary{string, object}"/></param>
        /// <returns>The <see cref="DataTable"/></returns>
        public DataTable ExecuteDataTable(string sql, Dictionary<string, object> parms)
        {
            DataTable result = new DataTable();
            SqlFormatCommand(sql, parms);
            _ada = new OracleDataAdapter(sql, _conn);
            _ada.SelectCommand = _cmd;
            string a = _cmd.CommandText;
            _ada.Fill(result);
            _cmd.Dispose();
            _ada.Dispose();
            return result;
        }

        /// <summary>
        /// 执行SQL,返回受影响的行数
        /// </summary>
        /// <param name="sql">The sql<see cref="string"/></param>
        /// <returns>The <see cref="int"/></returns>
        public int ExecuteNonQuery(string sql)
        {
            SqlFormatCommand(sql);
            int result = _cmd.ExecuteNonQuery();
            _cmd.Dispose();
            return result;
        }

        /// <summary>
        /// 执行SQL,返回受影响的行数
        /// </summary>
        /// <param name="sql">The sql<see cref="string"/></param>
        /// <param name="parms">The parms<see cref="Dictionary{string, object}"/></param>
        /// <returns>The <see cref="int"/></returns>
        public int ExecuteNonQuery(string sql, Dictionary<string, object> parms)
        {
            SqlFormatCommand(sql, parms);
            int result = _cmd.ExecuteNonQuery();
            _cmd.Dispose();
            return result;
        }

        /// <summary>
        /// 执行SQL,返回第一行第一列的值
        /// </summary>
        /// <param name="sql">The sql<see cref="string"/></param>
        /// <returns>The <see cref="object"/></returns>
        public object ExecuteScalar(string sql)
        {
            SqlFormatCommand(sql);
            object result = _cmd.ExecuteScalar();
            _cmd.Dispose();
            return result;
        }

        /// <summary>
        /// 执行SQL,返回第一行第一列的值
        /// </summary>
        /// <param name="sql">The sql<see cref="string"/></param>
        /// <param name="parms">The parms<see cref="Dictionary{string, object}"/></param>
        /// <returns>The <see cref="object"/></returns>
        public object ExecuteScalar(string sql, Dictionary<string, object> parms)
        {
            SqlFormatCommand(sql, parms);
            object result = _cmd.ExecuteScalar();
            _cmd.Dispose();
            return result;
        }

        /// <summary>
        /// 执行SQL,返回表的行数
        /// </summary>
        /// <param name="tableName">The tableName<see cref="string"/></param>
        /// <returns>The <see cref="int"/></returns>
        public int GetRowsCount(string tableName)
        {
            string sql = "select count(*) from " + tableName;
            SqlFormatCommand(sql);
            object result = _cmd.ExecuteScalar();
            _cmd.Dispose();
            return Convert.ToInt32(result);
        }

        /// <summary>
        /// The SqlFormatCommand
        /// </summary>
        /// <param name="sql">The sql<see cref="string"/></param>
        private void SqlFormatCommand(string sql)
        {
            _cmd = new OracleCommand(sql, _conn);
        }

        /// <summary>
        /// 初始化OracleCommand对象
        /// </summary>
        /// <param name="sql">The sql<see cref="string"/></param>
        /// <param name="parms">The parms<see cref="Dictionary{string, object}"/></param>
        private void SqlFormatCommand(string sql, Dictionary<string, object> parms)
        {
            OracleParameter[] parameters = ConvertDictionaryToOracleParameter(parms);
            _cmd = new OracleCommand(sql, _conn);
            _cmd.CommandText = sql;
            if (null != parameters)
            {
                foreach (var item in parameters)
                {
                    _cmd.Parameters.Add(item);
                }
            }
        }

        /// <summary>
        /// 将Dictionary<string, object>转换为OracleParameter[]
        /// </summary>
        /// <param name="parms">The parms<see cref="Dictionary{string, object}"/></param>
        /// <returns>The <see cref="OracleParameter[]"/></returns>
        private OracleParameter[] ConvertDictionaryToOracleParameter(Dictionary<string, object> parms)
        {
            int count = 0;
            if (null != parms)
            {
                count = parms.Count;
            }
            OracleParameter[] parameters = new OracleParameter[count];
            int i = 0;
            foreach (var item in parms)
            {
                parameters[i] = new OracleParameter(item.Key, item.Value);
                i++;
            }
            return parameters;
        }

        /// <summary>
        /// 调用存储过程
        /// </summary>
        /// <param name="procedureName">The procedureName<see cref="string"/></param>
        /// <param name="inputParms">The inputParms<see cref="Dictionary{string, object}"/></param>
        /// <param name="outputParms">The outputParms<see cref="string[]"/></param>
        /// <returns>The <see cref="Dictionary{string, object}"/></returns>
        public Dictionary<string, object> CallProduce(string procedureName, Dictionary<string, object> inputParms, string[] outputParms)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            _cmd.CommandType = CommandType.StoredProcedure;
            //存储过程名不能为空
            if (procedureName == null || procedureName.Trim() == "")
            {
                return null;
            }

            //存储过程输入参数设置
            if (inputParms != null && inputParms.Count != 0)
            {
                SqlFormatCommand(procedureName, inputParms);
            }

            //存储过程输出参数设置
            if (outputParms != null && outputParms.Length != 0)
            {
                for (int i = 0; i < outputParms.Length; i++)
                {
                    _cmd.Parameters.Add(new OracleParameter(outputParms[i], ParameterDirection.Output));
                }
            }
            _cmd.ExecuteNonQuery();

            //获取输出参数的值
            if (outputParms != null && outputParms.Length != 0)
            {
                for (int i = 0; i < outputParms.Length; i++)
                {
                    object value = _cmd.Parameters[outputParms[i]].Value;
                    result.Add(outputParms[i], value);
                }
            }
            return result;
        }

        /// <summary>
        /// The ExecuteData
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The sql<see cref="string"/></param>
        /// <returns>The <see cref="List{T}"/></returns>
        public List<T> ExecuteData<T>(string sql) where T : new()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The ExecuteData
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The sql<see cref="string"/></param>
        /// <param name="parms">The parms<see cref="Dictionary{string, object}"/></param>
        /// <returns>The <see cref="List{T}"/></returns>
        public List<T> ExecuteData<T>(string sql, Dictionary<string, object> parms) where T : new()
        {
            throw new NotImplementedException();
        }
    }
}
