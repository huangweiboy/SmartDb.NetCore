using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Collections;
using System.Linq;
using SmartDb;
using SmartDb.NetCore;

namespace SmartDb.SQLite.NetCore
{
   public class SQLiteBuilder : SqlBuilder
    {
        /// <summary>
        /// 根据查询字段、where参数、排序条件、每页数量、总页数查询实体对应分页数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryColumns"></param>
        /// <param name="whereSql"></param>
        /// <param name="whereParam"></param>
        /// <param name="sortCriteria"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public override DbEntity QueryPageList<T>(string queryColumns,string whereSql, object whereParam, string sortCriteria, int pageSize, int pageIndex)
        {
            DbEntity dbEntity = null;
            if (string.IsNullOrEmpty(queryColumns))
            {
                return dbEntity;
            }
            Type type = typeof(T);
            var attributeBuilder = new AttributeBuilder();
            var tableEntity = attributeBuilder.GetTableInfo(type);
            dbEntity = new DbEntity()
            {
                TableEntity = tableEntity
            };
            int startNum = pageSize * (pageIndex - 1);
            int endNum = pageSize * pageIndex;
            string dbOperator = DbFactory.GetDbParamOperator();
            var pkColumn = attributeBuilder.GetPkColumnInfo(type);
            if (pkColumn == null)
            {
                return dbEntity;
            }
            List<TableColumnAttribute> whereColumns = attributeBuilder.GetColumnInfos(whereParam);
            var dbParams = new List<IDbDataParameter>();

            //分页查询模板
            StringBuilder sqlBuild = new StringBuilder("select {queryColumns} from {tableName} a,");
            sqlBuild.Append("(");
            sqlBuild.Append("select {pkColumn} from {tableName} {whereCriteria} order by {sortCriteria}  limit {Num} offset {startNum}"); 
            sqlBuild.Append(")");
            sqlBuild.Append(" b where a.{pkColumn}=b.{pkColumn}  ");
            sqlBuild.Append("order by a.{sortCriteria};");

            sqlBuild.Replace("{tableName}", tableEntity.TableName);
            sqlBuild.Replace("{pkColumn}", pkColumn.ColumnName);
            sqlBuild.Replace("{sortCriteria}", sortCriteria);
            sqlBuild.Replace("{startNum}", startNum.ToString());
            sqlBuild.Replace("{Num}", pageSize.ToString());
            HandleQuerColumns(queryColumns, "a", ref sqlBuild, ref dbParams);
            HandleWhereParam(whereSql, whereColumns, ref sqlBuild, ref dbParams);
            dbEntity.CommandText = sqlBuild.ToString();
            dbEntity.DbParams = dbParams;
            return dbEntity;
        }

        /// <summary>
        /// 根据查询Sql、where参数查询实体对应数据总数量
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="whereSql"></param>
        /// <param name="whereObjParams"></param>
        /// <returns></returns>
        public sealed  override DbEntity QueryTotalPageCount<T>(string whereSql,object whereObjParams)
        {
           return base.QueryTotalPageCount<T>(whereSql, whereObjParams);
        }
    }
}
