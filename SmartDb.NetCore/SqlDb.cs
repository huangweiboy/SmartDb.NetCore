using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartDb.Mapper.NetCore;

namespace SmartDb.NetCore
{
    public abstract class SqlDb
    {

        #region private protected public  variables

        /// <summary>
        /// 数据库库连接串
        /// </summary>
        protected string _connectionString = string.Empty;

        /// <summary>
        /// 是否显示SQL相关到控制台
        /// </summary>
        protected bool _isShowSqlToConsole = false;

        /// <summary>
        /// DbHelper
        /// </summary>
        protected DbUtility DbHelper { get; set; }

        /// <summary>
        /// 基础SqlBuilder
        /// </summary>
        public SqlBuilder DbBuilder { get; set; }

        /// <summary>
        /// 数据库库连接串
        /// </summary>
        public string ConnectionString
        {
            get
            {
                return _connectionString;
            }
            set
            {
                if (!DbHelper.IsStartTrans)
                {
                    _connectionString = value;
                    DbHelper.ConnectionString = _connectionString;
                }
            }
        }

        /// <summary>
        /// 是否显示SQL相关到控制台
        /// </summary>
        public bool IsShowSqlToConsole
        {
            get
            {
                return _isShowSqlToConsole;
            }
            set
            {
                _isShowSqlToConsole = value;
                DbHelper.IsShowSqlToConsole = _isShowSqlToConsole;
            }
        }

        #endregion

        #region public  Methods

        public SqlDb()
        {
            DbHelper = new DbUtility();
        }

        /// <summary>
        /// 执行增、删、改操作,返回影响行数
        /// </summary>
        /// <param name="cmdText">SQL语句/存储过程/参数化SQL语句</param>
        /// <param name="dbParams">IDbDataParameter参数数组</param>
        /// <param name="cmdType">命令类型:SQL语句/存储过程</param>
        /// <returns>int</returns>
        public virtual int ExecuteNoneQuery(string cmdText, List<IDbDataParameter> dbParams = null, CommandType cmdType = CommandType.Text)
        {
            int result = 0;
            result = DbHelper.ExecuteNonQuery(cmdText, dbParams, cmdType);
            return result;
        }

        /// <summary>
        /// 执行增、删、改操作,返回影响行数
        /// </summary>
        /// <param name="cmdText">SQL语句/存储过程/参数化SQL语句</param>
        /// <param name="objParams">object参数对象</param>
        /// <param name="cmdType">命令类型:SQL语句/存储过程</param>
        /// <returns>int</returns>
        public virtual int ExecuteNoneQueryWithObjParam(string cmdText, object objParams = null, CommandType cmdType = CommandType.Text)
        {
            int result = 0;
            List<IDbDataParameter> dbParams = DbHelper.DbFactory.GetDbParamList(objParams);
            result = DbHelper.ExecuteNonQuery(cmdText, dbParams, cmdType);
            return result;
        }

        /// <summary>
        /// 执行查询操作,返回DataSet
        /// </summary>
        /// <param name="cmdText">SQL语句/存储过程/参数化SQL语句</param>
        /// <param name="dbParams">IDbDataParameter参数数组</param>
        /// <param name="cmdType">命令类型:SQL语句/存储过程</param>
        /// <returns>DataSet</returns>
        public virtual DataSet ExecuteQuery(string cmdText, List<IDbDataParameter> dbParams = null, CommandType cmdType = CommandType.Text)
        {
            DataSet result = null;
            result = DbHelper.ExecuteQuery(cmdText, dbParams, cmdType);
            return result;
        }

        /// <summary>
        /// 执行查询操作,返回DataSet
        /// </summary>
        /// <param name="cmdText">SQL语句/存储过程/参数化SQL语句</param>
        /// <param name="objParams">object参数对象</param>
        /// <param name="cmdType">命令类型:SQL语句/存储过程</param>
        /// <returns>DataSet</returns>
        public virtual DataSet ExecuteQueryWithObjParam(string cmdText, object objParams = null, CommandType cmdType = CommandType.Text)
        {
            DataSet result = null;
            List<IDbDataParameter> dbParams = DbHelper.DbFactory.GetDbParamList(objParams);
            result = DbHelper.ExecuteQuery(cmdText, dbParams, cmdType);
            return result;
        }

        /// <summary>
        /// 执行查询操作,返回IDataReader
        /// </summary>
        /// <param name="cmdText">SQL语句/存储过程/参数化SQL语句</param>
        /// <param name="dbParams">IDbDataParameter参数数组</param>
        /// <param name="cmdType">命令类型:SQL语句/存储过程</param>
        /// <returns>IDataReader</returns>
        public virtual IDataReader ExecuteReader(string cmdText, List<IDbDataParameter> dbParams = null, CommandType cmdType = CommandType.Text)
        {
            IDataReader result = null;
            result = DbHelper.ExecuteReader(cmdText, dbParams, cmdType);
            return result;
        }

        /// <summary>
        /// 执行查询操作,返回IDataReader
        /// </summary>
        /// <param name="cmdText">SQL语句/存储过程/参数化SQL语句</param>
        /// <param name="objParams">object参数对象</param>
        /// <param name="cmdType">命令类型:SQL语句/存储过程</param>
        /// <returns>IDataReader</returns>
        public virtual IDataReader ExecuteReaderWithObjParam(string cmdText, object objParams = null, CommandType cmdType = CommandType.Text)
        {
            IDataReader result = null;
            List<IDbDataParameter> dbParams = DbHelper.DbFactory.GetDbParamList(objParams);
            result = DbHelper.ExecuteReader(cmdText, dbParams, cmdType);
            return result;
        }

        /// <summary>
        /// 执行查询操作,返回object
        /// </summary>
        /// <param name="cmdText">SQL语句/存储过程/参数化SQL语句</param>
        /// <param name="dbParams">IDbDataParameter参数数组</param>
        /// <param name="cmdType">命令类型:SQL语句/存储过程</param>
        /// <returns>object</returns>
        public virtual object ExecuteScalar(string cmdText, List<IDbDataParameter> dbParams = null, CommandType cmdType = CommandType.Text)
        {
            object result = null;
            result = DbHelper.ExecuteScalar(cmdText, dbParams, cmdType);
            return result;
        }

        /// <summary>
        /// 执行查询操作,返回object
        /// </summary>
        /// <param name="cmdText">SQL语句/存储过程/参数化SQL语句</param>
        /// <param name="objParams">object参数对象</param>
        /// <param name="cmdType">命令类型:SQL语句/存储过程</param>
        /// <returns>object</returns>
        public virtual object ExecuteScalarWithObjParam(string cmdText, object objParams = null, CommandType cmdType = CommandType.Text)
        {
            object result = null;
            List<IDbDataParameter> dbParams = DbHelper.DbFactory.GetDbParamList(objParams);
            result = DbHelper.ExecuteScalar(cmdText, dbParams, cmdType);
            return result;
        }

        /// <summary>
        /// 添加单条实体对象数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual object Insert<T>(T entity)
        {
            object result = default(object);
            var dbEntity = DbBuilder.Insert<T>(entity);
            if (dbEntity == null)
            {
                return result;
            }
            result = dbEntity.TableEntity.IsGetIncrementValue ?
                DbHelper.ExecuteScalar(dbEntity.CommandText, dbEntity.DbParams) :
                DbHelper.ExecuteNonQuery(dbEntity.CommandText, dbEntity.DbParams);
            return result;
        }

        /// <summary>
        /// 根据主键删除实体对应数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual int Delete<T>(object id)
        {
            int result = 0;
            var dbEntity = DbBuilder.DeleteById<T>(id);
            if (dbEntity == null)
            {
                return result;
            }
            result = DbHelper.ExecuteNonQuery(dbEntity.CommandText, dbEntity.DbParams);
            return result;
        }

        /// <summary>
        /// 根据删除条件、删除参数删除数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="whereSql"></param>
        /// <param name="whereObjParams"></param>
        /// <returns></returns>
        public virtual int Delete<T>(string whereSql, object whereObjParams)
        {
            int result = 0;
            var dbEntity = DbBuilder.DeleteByParam<T>(whereSql, whereObjParams);
            if (dbEntity == null)
            {
                return result;
            }
            result = DbHelper.ExecuteNonQuery(dbEntity.CommandText, dbEntity.DbParams);
            return result;
        }

        /// <summary>
        ///  根据修改参数、主键值修改实体对应数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="updateParams"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual int Update<T>(object updateParams, object id)
        {
            int result = 0;
            var dbEntity = DbBuilder.UpdateById<T>(updateParams, id);
            if (dbEntity == null)
            {
                return result;
            }
            result = DbHelper.ExecuteNonQuery(dbEntity.CommandText, dbEntity.DbParams);
            return result;
        }

        /// <summary>
        ///  根据修改参数、where参数修改实体对应数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="updateParams"></param>
        /// <param name="whereSql"></param>
        /// <param name="whereObjParams"></param>
        /// <returns></returns>
        public virtual int Update<T>(object updateParams, string whereSql, object whereObjParams)
        {
            int result = 0;
            var dbEntity = DbBuilder.UpdateByParam<T>(updateParams, whereSql, whereObjParams);
            if (dbEntity == null)
            {
                return result;
            }
            result = DbHelper.ExecuteNonQuery(dbEntity.CommandText, dbEntity.DbParams);
            return result;
        }

        /// <summary>
        /// 根据主键查询实体对应数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual T QueryById<T>(object id)
        {
            T result = default(T);
            var dbEntity = DbBuilder.QueryById<T>(id);
            if (dbEntity == null)
            {
                return result;
            }
            var reader = DbHelper.ExecuteReader(dbEntity.CommandText, dbEntity.DbParams);
            result = DataReaderToEntity<T>(reader);
            return result;
        }

        /// <summary>
        /// 根根据查询字段、查询条件、查询参数查询实体对应数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryColumns"></param>
        /// <param name="whereSql"></param>
        /// <param name="whereObjParams"></param>
        /// <returns></returns>
        public virtual List<T> Query<T>(string queryColumns, string whereSql, object whereObjParams)
        {
            List<T> result = null;
            var dbEntity = DbBuilder.QueryByParam<T>(queryColumns, whereSql, whereObjParams);
            if (dbEntity == null)
            {
                return result;
            }
            var reader = DbHelper.ExecuteReader(dbEntity.CommandText, dbEntity.DbParams);
            result = DataReaderToEntityList<T>(reader);
            return result;
        }

        /// <summary>
        ///  根据sql语句、参数执行查询实体对应数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">sql语句或存储过程</param>
        /// <param name="objParams">参数,例:new {Uname="joyet"}</param>
        /// <returns></returns>
        public virtual List<T> Query<T>(string sql, object objParams)
        {
            List<T> result = null;
            var dbEntity = DbBuilder.Query<T>(sql, objParams);
            if (dbEntity == null)
            {
                return result;
            }
            var reader = DbHelper.ExecuteReader(dbEntity.CommandText, dbEntity.DbParams);
            result = DataReaderToEntityList<T>(reader);
            return result;
        }

        /// <summary>
        /// 根据查询字段、where参数、排序条件、每页数量、总页数查询实体对应分页数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="queryColumns"></param>
        /// <param name="whereSql"></param>
        /// <param name="sortCriteria"></param>
        /// <param name="whereObjParams"></param>
        /// <returns></returns>
        public virtual PageResultEntity QueryPageList<T>(int pageSize, int pageIndex, string queryColumns, string whereSql,string sortCriteria,object whereObjParams)
        {
            PageResultEntity pageResult = new PageResultEntity();
            pageResult.PageSize = pageSize;
            pageResult.CurrentPageIndex = pageIndex;
            var totalPageDbEntity = DbBuilder.QueryTotalPageCount<T>(whereSql, whereObjParams);
            if (totalPageDbEntity == null)
            {
                return pageResult;
            }
            var objTotalCount = DbHelper.ExecuteScalar(totalPageDbEntity.CommandText, totalPageDbEntity.DbParams);
            if (objTotalCount == null)
            {
                return pageResult;
            }
            pageResult.TotalCount = Convert.ToInt32(objTotalCount);
            if (pageResult.TotalCount <= 0)
            {
                return pageResult;
            }
            var dbEntity = DbBuilder.QueryPageList<T>(queryColumns, whereSql, whereObjParams, sortCriteria, pageSize, pageIndex);
            if (dbEntity == null)
            {
                return pageResult;
            }
            var reader = DbHelper.ExecuteReader(dbEntity.CommandText, dbEntity.DbParams);
            var listData = DataReaderToEntityList<T>(reader);
            pageResult.Data = listData;
            pageResult = SetPageListResult<T>(pageResult);
            return pageResult;
        }

        /// <summary>
        /// 开启事务
        /// </summary>
        public virtual void BeginTransaction()
        {
            DbHelper.BeginTransaction();
        }

        /// <summary>
        /// 提交事务
        /// </summary>
        public virtual void CommitTransaction()
        {
            DbHelper.CommitTransaction();
        }

        /// <summary>
        /// 回滚事务
        /// </summary>
        public virtual void RollbackTransaction()
        {
            DbHelper.RollbackTransaction();
        }

        /// <summary>
        /// IDataReader转化为实体
        /// </summary>
        /// <typeparam name="Entity"></typeparam>
        /// <param name="reader"></param>
        /// <returns></returns>
        public virtual T DataReaderToEntity<T>(IDataReader reader)
        {
            T entity = default(T);
            List<T> entityList = DataReaderToEntityList<T>(reader);
            if (entityList == null)
            {
                return entity;
            }
            if (entityList.Count == 0)
            {
                return entity;
            }
            entity = entityList[0];
            return entity;
        }

        /// <summary>
        /// IDataReader转化为实体列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <returns></returns>
        public virtual List<T> DataReaderToEntityList<T>(IDataReader reader)
        {
            List<T> entityList = null;
            if (reader == null)
            {
                return entityList;
            }
            entityList = new List<T>();
            DataReaderMapper<T> readBuild = DataReaderMapper<T>.GetInstance(reader);
            while (reader.Read())
            {
                entityList.Add(readBuild.Map(reader));
            }
            return entityList;
        }

        /// <summary>
        /// DataTable转化为实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        public virtual T DataTableToEntity<T>(DataTable dataTable)
        {
            T entity = default(T);
            List<T> entityList = DataTableToEntityList<T>(dataTable);
            if (entityList == null)
            {
                return entity;
            }
            if (entityList.Count == 0)
            {
                return entity;
            }
            entity = entityList[0];
            return entity;
        }

        /// <summary>
        ///  DataTable转化为实体列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public virtual List<T> DataTableToEntityList<T>(DataTable dt)
        {
            List<T> entityList = null;
            if (dt == null)
            {
                return entityList;
            }
            entityList = new List<T>();
            foreach (DataRow dataRow in dt.Rows)
            {
                DataTableMapper<T> dataRowMapper = DataTableMapper<T>.GetInstance(dataRow);
                var entity = dataRowMapper.Map(dataRow);
                if (entity != null)
                {
                    entityList.Add(dataRowMapper.Map(dataRow));
                }
            }
            return entityList;
        }

        /// <summary>
        /// 设置分页结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pageResult"></param>
        /// <returns></returns>
        public virtual PageResultEntity SetPageListResult<T>(PageResultEntity pageResult)
        {
            if (pageResult.Data == null)
            {
                return pageResult;
            }
            var dataList = pageResult.Data as List<T>;
            int totalPageIndex = 0;
            if (dataList.Count == 0)
            {
                totalPageIndex = 0;
                return pageResult;
            }
            else if (pageResult.TotalCount <= pageResult.PageSize)
            {
                totalPageIndex = 1;
            }
            else
            {
                if (pageResult.TotalCount % pageResult.PageSize == 0)
                {
                    totalPageIndex = pageResult.TotalCount / pageResult.PageSize;
                }
                else
                {
                    totalPageIndex = pageResult.TotalCount / pageResult.PageSize + 1;
                }
            }
            pageResult.TotalPageIndex = totalPageIndex;
            return pageResult;
        }

        #endregion
    }
}
