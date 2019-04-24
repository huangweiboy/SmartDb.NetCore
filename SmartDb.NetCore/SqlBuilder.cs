using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDb.NetCore
{
   public abstract class SqlBuilder
    {
        /// <summary>
        /// 数据库工厂
        /// </summary>
        public virtual SqlDbFactory DbFactory { get; set; }

        /// <summary>
        /// 是否启用缓存
        /// </summary>
        public virtual bool IsStartCache { get; set; }

        public SqlBuilder()
        {
            IsStartCache = false;
        }

        /// <summary>
        /// 获取表信息
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected TableAttribute GetTableInfo(Type type)
        {
            TableAttribute tableEntity = null;
            var attributeBuilder = new AttributeBuilder();
            tableEntity = attributeBuilder.GetTableInfo(type);
            return tableEntity;
        }

        /// <summary>
        /// 获取表主键字段信息
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected TableColumnAttribute GetPkColumnInfo(Type type)
        {
            TableColumnAttribute pkColumn = null;
            var attributeBuilder = new AttributeBuilder();
            pkColumn = attributeBuilder.GetPkColumnInfo(type);
            return pkColumn;
        }

        /// <summary>
        /// 获取表缓存信息
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual TableAttribute GetCacheTableInfo(Type type)
        {
            TableAttribute tableEntity = null;
            if (!IsStartCache)
            {
                tableEntity = GetTableInfo(type);
                return tableEntity;
            }
            var cacheInstance = CacheFactroy.GetInstance();
            var cacheKey = string.Format(SmartDbConstants.cacheTableKey, type.FullName.ToLower());
            var cacheValue = cacheInstance.GetCache(cacheKey);
            if (cacheValue == null)
            {
                tableEntity = GetTableInfo(type);
                if (tableEntity != null)
                {
                    cacheInstance.SetCache(cacheKey, tableEntity);
                }
                return tableEntity;
            }
            tableEntity = cacheValue as TableAttribute;
            return tableEntity;
        }

        /// <summary>
        /// 获取表主键字段缓存信息
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual TableColumnAttribute GetCachePkColumnInfo(Type type)
        {
            TableColumnAttribute pkColumn = null;
            if (!IsStartCache)
            {
                pkColumn = GetPkColumnInfo(type);
                return pkColumn;
            }
            var cacheInstance = CacheFactroy.GetInstance();
            var cacheKey = string.Format(SmartDbConstants.cachePKCloumnKey, type.FullName.ToLower());
            var cacheValue = cacheInstance.GetCache(cacheKey);
            if (cacheValue == null)
            {
                pkColumn = GetPkColumnInfo(type);
                if (pkColumn != null)
                {
                    cacheInstance.SetCache(cacheKey, pkColumn);
                }
                return pkColumn;
            }
            pkColumn = cacheValue as TableColumnAttribute;
            return pkColumn;
        }

        /// <summary>
        /// 添加当前实体对应数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public virtual DbEntity Insert<T>(T entity)
        {
            DbEntity dbEntity = null;
            if (entity == null)
            {
                return dbEntity;
            }
            Type type = typeof(T);
            TableAttribute tableEntity= GetCacheTableInfo(type);
            if (tableEntity == null)
            {
                return dbEntity;
            }
            List<TableColumnAttribute> columns=null;
            var attributeBuilder = new AttributeBuilder();
            columns = attributeBuilder.GetColumnInfos(type, entity);
            if (columns == null|| columns.Count <= 0)
            {
                return dbEntity;
            }
            columns = columns.Where(a => !a.IsAutoIncrement).ToList();
            if (columns == null|| columns.Count<= 0)
            {
                return dbEntity;
            }
            var dbOperatore = DbFactory.GetDbParamOperator();
            var dbParams = new List<IDbDataParameter>();
            var sqlBuild = new StringBuilder("insert into {tableName}({columnNames}) values({columnValues})");
            sqlBuild.Replace(SmartDbConstants.template_tableName, tableEntity.TableName);
            var columnNameSql = new StringBuilder();
            var columnValueSql = new StringBuilder();
            int i = 0;
            foreach (var column in columns)
            {
                columnNameSql.Append(column.ColumnName);
                columnValueSql.Append(dbOperatore + column.ColumnName);
                if (i != columns.Count - 1)
                {
                    columnNameSql.Append(",");
                    columnValueSql.Append(",");
                }
                var dbParam = DbFactory.GetDbParam(column);
                dbParams.Add(dbParam);
                i++;
            }
            sqlBuild.Replace(SmartDbConstants.template_columnNames, columnNameSql.ToString());
            sqlBuild.Replace(SmartDbConstants.template_columnValues, columnValueSql.ToString());
            var autoIncrementSql = DbFactory.GetIncrementSql(tableEntity.IsGetIncrementValue);
            sqlBuild.Append(autoIncrementSql);
            dbEntity = new DbEntity()
            {
                DbType = DbFactory.DbType,
                TableEntity = tableEntity,
                CommandText = sqlBuild.ToString(),
                DbParams = dbParams
            };
            return dbEntity;
        }

        /// <summary>
        /// 根据主键删除实体对应数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual DbEntity DeleteById<T>(object id)
        {
            DbEntity dbEntity = null;
            if (id == null)
            {
                return dbEntity;
            }
            Type type = typeof(T);
            var tableEntity = GetCacheTableInfo(type);
            if (tableEntity == null)
            {
                return dbEntity;
            }
            TableColumnAttribute pkColumn = GetCachePkColumnInfo(type);
            if (pkColumn == null)
            {
                return dbEntity;
            }
            pkColumn.ColumnValue = id;
            string dbOperator = DbFactory.GetDbParamOperator();
            var dbParams = new List<IDbDataParameter>();
            StringBuilder sqlBuild = new StringBuilder("delete from {tableName} where {columnName}={dbOperator}{columnName}");
            sqlBuild.Replace(SmartDbConstants.template_tableName, tableEntity.TableName);
            sqlBuild.Replace(SmartDbConstants.template_columnName, pkColumn.ColumnName);
            sqlBuild.Replace(SmartDbConstants.template_dbOperator, dbOperator);
            var dbParam = DbFactory.GetDbParam(pkColumn);
            dbParams.Add(dbParam);
            dbEntity = new DbEntity()
            {
                DbType = DbFactory.DbType,
                TableEntity = tableEntity,
                CommandText = sqlBuild.ToString(),
                DbParams = dbParams
            };
            return dbEntity;
        }

        /// <summary>
        /// 根据where条件Sql、参数删除实体对应数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="whereSql"></typeparam>
        /// <param name="whereParam"></param>
        /// <returns></returns>
        public virtual DbEntity DeleteByParam<T>(string whereSql,object whereParam)
        {
            DbEntity dbEntity = null;
            Type type = typeof(T);
            var tableEntity = GetCacheTableInfo(type);
            if (tableEntity == null)
            {
                return dbEntity;
            }
            var dbOperatore = DbFactory.GetDbParamOperator();
            var dbParams = new List<IDbDataParameter>();
            StringBuilder sqlBuild = new StringBuilder("delete from {tableName} {whereCriteria}");
            sqlBuild.Replace(SmartDbConstants.template_tableName, tableEntity.TableName);
            var attributeBuilder = new AttributeBuilder();
            var whereColumns = attributeBuilder.GetColumnInfos(whereParam);
            HandleWhereParam(whereSql,whereColumns, ref sqlBuild, ref dbParams);
            dbEntity = new DbEntity()
            {
                DbType = DbFactory.DbType,
                TableEntity = tableEntity,
                CommandText = sqlBuild.ToString(),
                DbParams = dbParams
            };
            return dbEntity;
        }

        /// <summary>
        ///  根据修改参数、主键修改实体对应数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="updateParam">参数,例:new {Uname="joyet"}</param>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual DbEntity UpdateById<T>(object updateParam, object id)
        {
            DbEntity dbEntity = null;
            if (updateParam == null|| id == null)
            {
                return dbEntity;
            }
            Type type = typeof(T);
            var tableEntity = GetCacheTableInfo(type);
            if (tableEntity == null)
            {
                return dbEntity;
            }
            var attributeBuilder = new AttributeBuilder();
            var updateColumns = attributeBuilder.GetColumnInfos(updateParam);
            if (updateColumns == null)
            {
                return dbEntity;
            }
            var pkColumn = GetCachePkColumnInfo(type);
            if (pkColumn == null)
            {
                return dbEntity;
            }
            pkColumn.ColumnValue = id;
            var dbOperator = DbFactory.GetDbParamOperator();
            var dbParams = new List<IDbDataParameter>();
            StringBuilder sqlBuild = new StringBuilder("update {tableName} set {updateCriteria}  where {columnName}={dbOperator}{columnName}");
            sqlBuild.Replace(SmartDbConstants.template_tableName, tableEntity.TableName);
            sqlBuild.Replace(SmartDbConstants.template_columnName, pkColumn.ColumnName);
            sqlBuild.Replace(SmartDbConstants.template_dbOperator, dbOperator);
            HandleUpdateParam(updateColumns, ref sqlBuild, ref dbParams);
            var dbParam = DbFactory.GetDbParam(pkColumn);
            dbParams.Add(dbParam);
            dbEntity = new DbEntity()
            {
                DbType = DbFactory.DbType,
                TableEntity = tableEntity,
                CommandText = sqlBuild.ToString(),
                DbParams = dbParams,
            };
            return dbEntity;
        }

        /// <summary>
        ///  根据修改参数、where条件Sql、where参数修改实体对应数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="updateParam">参数,例:new {Uname="joyet"}</param>
        /// <param name="whereSql"></param>
        /// <param name="whereParam">参数,例:new {Uid=1}</param>
        /// <returns></returns>
        public virtual DbEntity UpdateByParam<T>(object updateParam, string whereSql, object whereParam)
        {
            DbEntity dbEntity = null;
            Type type = typeof(T);
            if (updateParam == null)
            {
                return dbEntity;
            }
            var tableEntity = GetCacheTableInfo(type);
            if (tableEntity == null)
            {
                return dbEntity;
            }
            var attributeBuilder = new AttributeBuilder();
            List<TableColumnAttribute> updateColumns= attributeBuilder.GetColumnInfos(updateParam);
            if (updateColumns == null)
            {
                return dbEntity;
            }
            string dbOperator = DbFactory.GetDbParamOperator();
            var dbParams = new List<IDbDataParameter>();
            StringBuilder sqlBuild = new StringBuilder("update {tableName} set {updateCriteria} {whereCriteria}");
            sqlBuild.Replace(SmartDbConstants.template_tableName, tableEntity.TableName);
            List<TableColumnAttribute> whereColumns = attributeBuilder.GetColumnInfos(whereParam);
            HandleUpdateParam(updateColumns, ref sqlBuild, ref dbParams);
            HandleWhereParam(whereSql, whereColumns, ref sqlBuild, ref dbParams);
            dbEntity = new DbEntity()
            {
                DbType = DbFactory.DbType,
                TableEntity = tableEntity,
                CommandText = sqlBuild.ToString(),
                DbParams = dbParams,
            };
            return dbEntity;
        }

        /// <summary>
        ///根据主键查询实体对应数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual DbEntity QueryById<T>(object id)
        {
            DbEntity dbEntity = null;
            Type type = typeof(T);
            var tableEntity = GetCacheTableInfo(type);
            if (tableEntity == null)
            {
                return dbEntity;
            }
            TableColumnAttribute pkColumn = GetCachePkColumnInfo(type);
            if (pkColumn == null)
            {
                return dbEntity;
            }
            pkColumn.ColumnValue = id;
            string dbOperator = DbFactory.GetDbParamOperator();
            List<IDbDataParameter> dbParams = new List<IDbDataParameter>();
            StringBuilder sqlBuild = new StringBuilder("select * from {tableName} where {columnName}={dbOperator}{columnName}");
            sqlBuild.Replace(SmartDbConstants.template_tableName, tableEntity.TableName);
            sqlBuild.Replace(SmartDbConstants.template_columnName, pkColumn.ColumnName);
            sqlBuild.Replace(SmartDbConstants.template_dbOperator, dbOperator);
            var dbParam = DbFactory.GetDbParam(pkColumn);
            dbParams.Add(dbParam);
            dbEntity = new DbEntity()
            {
                DbType = DbFactory.DbType,
                TableEntity = tableEntity,
                CommandText = sqlBuild.ToString(),
                DbParams = dbParams,
            };
            return dbEntity;
        }

        /// <summary>
        /// 根据查询字段、where条件、where参数查询实体对应数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryColumns"></param>
        /// <param name="whereSql"></param>
        /// <param name="whereObjParams"></param>
        /// <returns></returns>
        public virtual DbEntity QueryByParam<T>(string queryColumns, string whereSql, object whereObjParams)
        {
            DbEntity dbEntity = null;
            if (string.IsNullOrEmpty(queryColumns))
            {
                return dbEntity;
            }
            Type type = typeof(T);
            var tableEntity = GetCacheTableInfo(type);
            if (tableEntity == null)
            {
                return dbEntity;
            }
            string dbOperator = DbFactory.GetDbParamOperator();
            var dbParams = new List<IDbDataParameter>();
            StringBuilder sqlBuild = new StringBuilder("select {queryColumns} from {tableName} {whereCriteria}");
            sqlBuild.Replace(SmartDbConstants.template_tableName, tableEntity.TableName);
            var attributeBuilder = new AttributeBuilder();
            List<TableColumnAttribute> whereColumns = attributeBuilder.GetColumnInfos(whereObjParams);
            HandleQuerColumns(queryColumns,"",ref sqlBuild,ref dbParams);
            HandleWhereParam(whereSql,whereColumns,ref sqlBuild,ref dbParams);
            dbEntity = new DbEntity()
            {
                DbType = DbFactory.DbType,
                TableEntity = tableEntity,
                CommandText = sqlBuild.ToString(),
                DbParams = dbParams,
            };
            return dbEntity;
        }

        /// <summary>
        /// 根据查询字段、where参数、排序条件、每页数量、总页数查询实体对应分页数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryColumns"></param>
        /// <param name="whereSql"></param>
        /// <param name="whereObjParams"></param>
        /// <param name="sortCriteria"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public abstract DbEntity QueryPageList<T>(string queryColumns,string whereSql, object whereObjParams, string sortCriteria, int pageSize, int pageIndex);

        /// <summary>
        /// 根据查询字段、where参数、排序条件查询实体对应数据总数量
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="whereSql"></param>
        /// <param name="whereObjParams"></param>
        /// <returns></returns>
        public virtual DbEntity QueryTotalPageCount<T>(string whereSql,object whereObjParams)
        {
            DbEntity dbEntity = null;
            Type type = typeof(T);
            var attributeBuilder = new AttributeBuilder();
            var tableEntity = attributeBuilder.GetTableInfo(type);
            if (tableEntity == null)
            {
                return dbEntity;
            }
            var dbParams = new List<IDbDataParameter>();
            string dbOperator = DbFactory.GetDbParamOperator();
            List<TableColumnAttribute> whereColumns = attributeBuilder.GetColumnInfos(whereObjParams);
            StringBuilder sqlBuild = new StringBuilder("select count(*) from  {tableName} {whereCriteria}");
            sqlBuild.Replace(SmartDbConstants.template_tableName, tableEntity.TableName);
            HandleWhereParam(whereSql,whereColumns, ref sqlBuild, ref dbParams);
            dbEntity = new DbEntity()
            {
                DbType = DbFactory.DbType,
                TableEntity = tableEntity,
                CommandText = sqlBuild.ToString(),
                DbParams = dbParams,
            };
            return dbEntity;
        }

        /// <summary>
        /// 根据sql语句、参数执行查询实体对应数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">sql语句或存储过程</param>
        /// <param name="objParam">参数,例:new {Uname="joyet"}</param>
        /// <returns></returns>
        public virtual DbEntity Query<T>(string sql, object objParam)
        {
            DbEntity dbEntity = null;
            if (string.IsNullOrEmpty(sql))
            {
                return dbEntity;
            }
            Type type = typeof(T);
            var attributeBuilder = new AttributeBuilder();
            var tableEntity = attributeBuilder.GetTableInfo(type);
            if (tableEntity == null)
            {
                return dbEntity;
            }
            var dbParams = new List<IDbDataParameter>();
            List<TableColumnAttribute> columns = attributeBuilder.GetColumnInfos(objParam);
            if (columns != null)
            {
                foreach (var column in columns)
                {
                    var dbParam = DbFactory.GetDbParam(column);
                    dbParams.Add(dbParam);
                }
            }
            dbEntity = new DbEntity()
            {
                DbType = DbFactory.DbType,
                TableEntity = tableEntity,
                CommandText = sql,
                DbParams = dbParams,
            };
            return dbEntity;
        }

        /// <summary>
        ///  处理查询字段
        /// </summary>
        /// <param name="queryColumns"></param>
        /// <param name="tableAlias"></param>
        /// <param name="sqlBuild"></param>
        /// <param name="dbParams"></param>
        public void HandleQuerColumns(string queryColumns,string tableAlias, ref StringBuilder sqlBuild, ref List<IDbDataParameter> dbParams)
        {
            if (!string.IsNullOrEmpty(queryColumns))
            {
                var queryColumnBuilder = new StringBuilder();
                if (queryColumns.IndexOf(',') > 0)
                {
                    var columnNames = queryColumns.Split(',');
                    var columnNameList = (from a in columnNames where a != "" select a).ToList();
                    int i = 0;
                    foreach (var columnName in columnNameList)
                    {
                        var tmpColumnName = string.IsNullOrEmpty(tableAlias) ? columnName : tableAlias + "." + columnName;
                        queryColumnBuilder.Append(tmpColumnName);
                        if (i != columnNameList.Count-1)
                        {
                            queryColumnBuilder.Append(",");
                        }
                        i++;
                    }
                }
                else
                {
                    var tmpColumnName = string.IsNullOrEmpty(tableAlias) ? queryColumns : tableAlias + "." + queryColumns;
                    queryColumnBuilder.Append(tmpColumnName);
                }
                sqlBuild.Replace("{queryColumns}", queryColumnBuilder.ToString());
            }
            else
            {
                sqlBuild.Replace("{queryColumns}", "*");
            }
           
        }

        /// <summary>
        /// 处理updateParam参数
        /// </summary>
        /// <param name="updateColumns"></param>
        /// <param name="sqlBuild"></param>
        /// <param name="dbParams"></param>
        public void HandleUpdateParam(List<TableColumnAttribute> updateColumns, ref StringBuilder sqlBuild, ref List<IDbDataParameter> dbParams)
        {
            if (updateColumns == null)
            {
                sqlBuild.Replace("{updateCriteria}", "");
            }
            else
            {
                int i = 0;
                var dbOperator = DbFactory.GetDbParamOperator();
                StringBuilder updateSqlBuild = new StringBuilder();
                foreach (var updateColumn in updateColumns)
                {
                    updateSqlBuild.AppendFormat("{0}={1}{0}", updateColumn.ColumnName, dbOperator);
                    if (i != updateColumns.Count - 1)
                    {
                        updateSqlBuild.Append(",");
                    }
                    var dbParam = DbFactory.GetDbParam(updateColumn);
                    dbParams.Add(dbParam);
                    i++;
                }
                sqlBuild.Replace("{updateCriteria}", updateSqlBuild.ToString());
            }
        }

        /// <summary>
        /// 处理whereParam参数
        /// </summary>
        /// <param name="whereSql"></param>
        /// <param name="whereColumns"></param>
        /// <param name="sqlBuild"></param>
        /// <param name="dbParams"></param>
        public void HandleWhereParam(string whereSql,List<TableColumnAttribute> whereColumns, ref StringBuilder sqlBuild, ref List<IDbDataParameter> dbParams)
        {
            if (!string.IsNullOrEmpty(whereSql))
            {
                if (whereColumns != null)
                {
                    foreach (var whereColumn in whereColumns)
                    {
                        var dbParam = DbFactory.GetDbParam(whereColumn);
                        dbParams.Add(dbParam);
                    }
                }
                sqlBuild.Replace("{whereCriteria}", "where " + whereSql);
            }
            else
            {
                sqlBuild.Replace("{whereCriteria}", "");
            }
        }

    }
}
