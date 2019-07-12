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


        public SqlBuilder()
        {
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
            TableAttribute tableEntity= GetTableInfo(type);
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
            sqlBuild.Replace("{tableName}", tableEntity.TableName);
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
            sqlBuild.Replace("{columnNames}", columnNameSql.ToString());
            sqlBuild.Replace("{columnValues}",columnValueSql.ToString());
            var autoIncrementSql = DbFactory.GetIncrementSql(tableEntity.IsGetIncrementValue);
            sqlBuild.Append(autoIncrementSql);
            dbEntity = new DbEntity()
            {
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
            var tableEntity = GetTableInfo(type);
            if (tableEntity == null)
            {
                return dbEntity;
            }
            TableColumnAttribute pkColumn = GetPkColumnInfo(type);
            if (pkColumn == null)
            {
                return dbEntity;
            }
            pkColumn.ColumnValue = id;
            string dbOperator = DbFactory.GetDbParamOperator();
            var dbParams = new List<IDbDataParameter>();
            StringBuilder sqlBuild = new StringBuilder("delete from {tableName} where {columnName}={dbOperator}{columnName}");
            sqlBuild.Replace("{tableName}", tableEntity.TableName);
            sqlBuild.Replace("{columnName}", pkColumn.ColumnName);
            sqlBuild.Replace("{dbOperator}", dbOperator);
            var dbParam = DbFactory.GetDbParam(pkColumn);
            dbParams.Add(dbParam);
            dbEntity = new DbEntity()
            {
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
            var tableEntity = GetTableInfo(type);
            if (tableEntity == null)
            {
                return dbEntity;
            }
            var dbOperatore = DbFactory.GetDbParamOperator();
            var dbParams = new List<IDbDataParameter>();
            var whereColumns = new AttributeBuilder().GetColumnInfos(whereParam);
            StringBuilder sqlBuild = new StringBuilder("delete from {tableName} {whereCriteria}");
            sqlBuild.Replace("{tableName}", tableEntity.TableName);
            HandleWhereParam(whereSql,whereColumns, ref sqlBuild, ref dbParams);
            dbEntity = new DbEntity()
            {
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
            var tableEntity = GetTableInfo(type);
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
            var pkColumn = GetPkColumnInfo(type);
            if (pkColumn == null)
            {
                return dbEntity;
            }
            pkColumn.ColumnValue = id;
            var dbOperator = DbFactory.GetDbParamOperator();
            var dbParams = new List<IDbDataParameter>();
            var dbParam = DbFactory.GetDbParam(pkColumn);
            dbParams.Add(dbParam);
            StringBuilder sqlBuild = new StringBuilder("update {tableName} set {updateCriteria}  where {columnName}={dbOperator}{columnName}");
            sqlBuild.Replace("{tableName}", tableEntity.TableName);
            sqlBuild.Replace("columnName", pkColumn.ColumnName);
            sqlBuild.Replace("{dbOperator}", dbOperator);
            HandleUpdateParam(updateColumns, ref sqlBuild, ref dbParams);
            dbEntity = new DbEntity()
            {
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
            var tableEntity = GetTableInfo(type);
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
            List<TableColumnAttribute> whereColumns = attributeBuilder.GetColumnInfos(whereParam);
            StringBuilder sqlBuild = new StringBuilder("update {tableName} set {updateCriteria} {whereCriteria}");
            sqlBuild.Replace("{tableName}", tableEntity.TableName);
            HandleUpdateParam(updateColumns, ref sqlBuild, ref dbParams);
            HandleWhereParam(whereSql, whereColumns, ref sqlBuild, ref dbParams);
            dbEntity = new DbEntity()
            {
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
            var tableEntity = GetTableInfo(type);
            if (tableEntity == null)
            {
                return dbEntity;
            }
            TableColumnAttribute pkColumn = GetPkColumnInfo(type);
            if (pkColumn == null)
            {
                return dbEntity;
            }
            pkColumn.ColumnValue = id;
            string dbOperator = DbFactory.GetDbParamOperator();
            List<IDbDataParameter> dbParams = new List<IDbDataParameter>();
            StringBuilder sqlBuild = new StringBuilder("select * from {tableName} where {columnName}={dbOperator}{columnName}");
            sqlBuild.Replace("{tableName}", tableEntity.TableName);
            sqlBuild.Replace("{columnName}", pkColumn.ColumnName);
            sqlBuild.Replace("{dbOperator}", dbOperator);
            var dbParam = DbFactory.GetDbParam(pkColumn);
            dbParams.Add(dbParam);
            dbEntity = new DbEntity()
            {
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
            var tableEntity = GetTableInfo(type);
            if (tableEntity == null)
            {
                return dbEntity;
            }
            string dbOperator = DbFactory.GetDbParamOperator();
            var dbParams = new List<IDbDataParameter>();
            List<TableColumnAttribute> whereColumns = new AttributeBuilder().GetColumnInfos(whereObjParams);
            StringBuilder sqlBuild = new StringBuilder("select {queryColumns} from {tableName} {whereCriteria}");
            sqlBuild.Replace("{tableName}", tableEntity.TableName);
            HandleQueryParam(queryColumns,"",ref sqlBuild);
            HandleWhereParam(whereSql,whereColumns,ref sqlBuild,ref dbParams);
            dbEntity = new DbEntity()
            {
                TableEntity = tableEntity,
                CommandText = sqlBuild.ToString(),
                DbParams = dbParams,
            };
            return dbEntity;
        }

        /// <summary>
        /// 单表分页数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryColumns"></param>
        /// <param name="whereSql"></param>
        /// <param name="sortCriteria"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="whereObjParams"></param>
        /// <returns></returns>
        public virtual DbEntity QueryPageList<T>(string queryColumns, string whereSql, string sortColumn, string sortType, long pageSize, long pageIndex, object whereObjParams)
        {
            DbEntity dbEntity = null;
            if (string.IsNullOrEmpty(sortColumn) || string.IsNullOrEmpty(sortType))
            {
                return dbEntity;
            }
            if (pageSize <= 0 || pageIndex <= 0)
            {
                return dbEntity;
            }
            dbEntity = new DbEntity();
            return dbEntity;
        }

        /// <summary>
        /// 根据查询条件、where参数查询实体对应数据总数量
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
            sqlBuild.Replace("{tableName}", tableEntity.TableName);
            HandleWhereParam(whereSql,whereColumns, ref sqlBuild, ref dbParams);
            dbEntity = new DbEntity()
            {
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
                TableEntity = tableEntity,
                CommandText = sql,
                DbParams = dbParams,
            };
            return dbEntity;
        }

        /// <summary>
        /// 处理查询字段
        /// </summary>
        /// <param name="queryColumns"></param>
        /// <param name="alias"></param>
        /// <param name="sqlBuild"></param>
        public virtual void HandleQueryParam(string queryColumns, string alias, ref StringBuilder sqlBuild)
        {
            StringBuilder querySqlBuilder = new StringBuilder();
            if (string.IsNullOrEmpty(queryColumns))
            {
                queryColumns = "*";
            }
            if (queryColumns.IndexOf(',') > 0)
            {
                IList<string> columnItmes = columnItmes = (from w in queryColumns.Split(',') where !w.Equals("") select string.IsNullOrEmpty(alias)?w: alias + "." + w).ToList();
                querySqlBuilder.Append(string.Join(",", columnItmes));
            }
            else
            {
                querySqlBuilder.Append(string.IsNullOrEmpty(alias)?queryColumns:alias+"."+queryColumns);
            }
            sqlBuild.Replace("{queryColumns}", querySqlBuilder.ToString());
        }

        /// <summary>
        /// 处理实体处理updateParam参数
        /// </summary>
        /// <param name="updateColumns"></param>
        /// <param name="sqlBuild"></param>
        /// <param name="dbParams"></param>
        public virtual void HandleUpdateParam(List<TableColumnAttribute> updateColumns, ref StringBuilder sqlBuild, ref List<IDbDataParameter> dbParams)
        {
            StringBuilder updateSqlBuild = new StringBuilder();
            if (updateColumns != null)
            {
                var dbOperator = DbFactory.GetDbParamOperator();
                if (updateColumns != null && updateColumns.Count > 0)
                {
                    int i = 0;
                    foreach (var columnItem in updateColumns)
                    {
                        updateSqlBuild.AppendFormat("{0}={1}{0}", columnItem.ColumnName, dbOperator);
                        if (i != updateColumns.Count - 1)
                        {
                            updateSqlBuild.Append(",");
                        }
                        var dbParam = DbFactory.GetDbParam(columnItem);
                        dbParams.Add(dbParam);
                        i++;
                    }
                }
            }
            sqlBuild.Replace("{updateCriteria}", updateSqlBuild.ToString());
        }

        /// <summary>
        /// 处理实体whereParam参数
        /// </summary>
        /// <param name="whereSql"></param>
        /// <param name="whereColumns"></param>
        /// <param name="sqlBuild"></param>
        /// <param name="dbParams"></param>
        public virtual void HandleWhereParam(string whereSql, List<TableColumnAttribute> whereColumns, ref StringBuilder sqlBuild, ref List<IDbDataParameter> dbParams)
        {
            StringBuilder wherSqlBuild = new StringBuilder();
            if (!string.IsNullOrEmpty(whereSql))
            {
                if (whereColumns != null)
                {
                    foreach (var columnItem in whereColumns)
                    {
                        var dbParam = DbFactory.GetDbParam(columnItem);
                        dbParams.Add(dbParam);
                    }
                }
                wherSqlBuild.Append(" where " + whereSql);
            }
            sqlBuild.Replace("{whereCriteria}", wherSqlBuild.ToString());
        }

        /// <summary>
        /// 获取实体表信息
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
        /// 获取实体表主键字段信息
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

    }
}
