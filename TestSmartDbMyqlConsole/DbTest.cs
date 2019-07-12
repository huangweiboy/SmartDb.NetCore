using SmartDb;
using SmartDb.MySql;
using SmartDb.MySql.NetCore;
using SmartDb.NetCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestSmartDbMyqlConsole
{
    public class DbTest
    {
        private string _connString;
        private bool _isShowSqlToConsole;

        public DbTest(string connString, bool isShowSqlToConsole)
        {
            _connString = connString;
            _isShowSqlToConsole = isShowSqlToConsole;
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        public void Insert()
        {
            SqlDb db = new MySqlDb(_connString);
            db.IsShowSqlToConsole = _isShowSqlToConsole;
            var dbFactory = db.DbBuilder.DbFactory;
            var dbOperator = dbFactory.GetDbParamOperator();
            db.BeginTransaction();
            db.IsShowSqlToConsole = _isShowSqlToConsole;
            for (int i = 1; i <= 20; i++)
            {
                db.Insert<UserInfo>(new UserInfo() { UserId = i, UserName = "joyet" + i.ToString(), Age = 110, Email = "joyet" + i.ToString() + "@qq.com" });
            }
            db.CommitTransaction();
        }

        /// <summary>
        /// 删除所有数据
        /// </summary>
        public void DeleteAll()
        {
            SqlDb db = new MySqlDb(_connString);
            db.IsShowSqlToConsole = _isShowSqlToConsole;
            var dbFactory = db.DbBuilder.DbFactory;
            var dbOperator = dbFactory.GetDbParamOperator();
            var result = db.Delete<UserInfo>("", null);
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        public void Delete()
        {
            SqlDb db = new MySqlDb(_connString);
            db.IsShowSqlToConsole = _isShowSqlToConsole;
            var dbFactory = db.DbBuilder.DbFactory;
            var dbOperator = dbFactory.GetDbParamOperator();

            //根据实体主键参数值查询数据
            var result = db.Delete<UserInfo>(1);

            //根据过滤SQL、过滤参数查询数据列表1
            result = db.Delete<UserInfo>("UserId=2", null);
            result = db.Delete<UserInfo>(string.Format("UserId={0}UserId", dbOperator), new { UserId = 3 });
        }

        /// <summary>
        /// 修改数据
        /// </summary>
        public void Update()
        {
            SqlDb db = new MySqlDb(_connString);
            db.IsShowSqlToConsole = _isShowSqlToConsole;
            var dbFactory = db.DbBuilder.DbFactory;
            var dbOperator = dbFactory.GetDbParamOperator();

            //根据修改字段参数及值、过滤SQL、过滤参数修改数据
            var result = db.Update<UserInfo>(new { UserName = "joyet44" }, "UserId=4", null);
            result = db.Update<UserInfo>(new { UserName = "joyet444" }, string.Format("UserId={0}UserId", dbOperator), new { UserId = 4 });

        }

        /// <summary>
        /// 查询数据
        /// </summary>
        public void Query()
        {
            SqlDb db = new MySqlDb(_connString);
            db.IsShowSqlToConsole = _isShowSqlToConsole;
            var dbFactory = db.DbBuilder.DbFactory;
            var dbOperator = dbFactory.GetDbParamOperator();

            //根据实体主键参数值查询数据
            var data = db.QueryById<UserInfo>(5);

            //根据查询字段、过滤SQL、过滤参数查询数据列表
            var dataList1 = db.Query<UserInfo>("UserId,UserName", "UserId=4", null);
            var dataList2 = db.Query<UserInfo>("UserId,UserName", string.Format("UserId={0}UserId", dbOperator), new { UserId = 4 });

            //根据查询参数化SQL、参数列表查询数据列表
            var dataList3 = db.Query<UserInfo>("select * from UserInfo where UserId=4", null);
            var dataList4 = db.Query<UserInfo>(string.Format("select * from UserInfo where UserId={0}UserId", dbOperator), new { UserId = 4 });

            //分页查询列表
            var pageDataList1 = db.QueryPageList<UserInfo>("*", "UserId>2", "UserId", "asc",10,1,null);
            var pageDataList2 = db.QueryPageList<UserInfo>("UserId,UserName", string.Format("UserId>{0}UserId", dbOperator), "UserId", "asc", 10, 1, new { UserId = 2 });
        }

        /// <summary>
        /// 执行非查询操作
        /// </summary>
        public void OrtherNoneQuery()
        {
            SqlDb db = new MySqlDb(_connString);
            db.IsShowSqlToConsole = _isShowSqlToConsole;
            var dbFactory = db.DbBuilder.DbFactory;
            var dbOperator = dbFactory.GetDbParamOperator();
            var sql = "";
            var paramSql = string.Format("delete from UserInfo where UserId={0}UserId", dbOperator);

            //根据SQL语句、参数列表删除数据用法1
            sql = "delete from UserInfo where UserId=16";
            var result=db.ExecuteNoneQuery(sql,null);
            var dbParams = new List<IDbDataParameter>();
            dbParams.Add(dbFactory.GetDbParam("UserId",17));
            result=db.ExecuteNoneQuery(paramSql, dbParams);

            //根据SQL语句、object参数列表删除数据用法2
            sql = "delete from UserInfo where UserId=18";
            result=db.ExecuteNoneQueryWithObjParam(sql);
            result=db.ExecuteNoneQueryWithObjParam(paramSql, new { UserId = 19 });

        }

        /// <summary>
        /// 执行查询操作
        /// </summary>
        public void OrtherQuery()
        {
            SqlDb db = new MySqlDb(_connString);
            db.IsShowSqlToConsole = _isShowSqlToConsole;
            var dbFactory = db.DbBuilder.DbFactory;
            var dbOperator = dbFactory.GetDbParamOperator();
            var dbParams = new List<IDbDataParameter>();
            var sql = "select * from UserInfo where UserId=4";
            var paramSql = string.Format("select * from UserInfo where UserId={0}UserId", dbOperator);

            //自定义查询,返回单条数据(无参)
            UserInfo data;
            using (var dataReader = db.ExecuteReaderWithObjParam(sql, null))
            {
                data = db.DataReaderToEntity<UserInfo>(dataReader);
            }
            using (var dataReader = db.ExecuteReader(sql, null))
            {
                data = db.DataReaderToEntity<UserInfo>(dataReader);
            }

            //自定义查询,返回单条数据(有参)
            using (var dataReader = db.ExecuteReaderWithObjParam(paramSql, new { UserId = 4 }))
            {
                data = db.DataReaderToEntity<UserInfo>(dataReader);
            }
            dbParams = new List<IDbDataParameter>();
            dbParams.Add(dbFactory.GetDbParam("UserId", 4));
            using (var dataReader = db.ExecuteReader(paramSql, dbParams))
            {
                data = db.DataReaderToEntity<UserInfo>(dataReader);
            }

            //自定义查询,返回多条数据(无参)
            sql = "select * from UserInfo where UserId>2";
            List<UserInfo> dataList;
            using (var dataReader = db.ExecuteReaderWithObjParam(sql, null))
            {
                dataList = db.DataReaderToEntityList<UserInfo>(dataReader);
            }
            using (var dataReader = db.ExecuteReader(sql, null))
            {
                dataList = db.DataReaderToEntityList<UserInfo>(dataReader);
            }

            //自定义查询,返回多条数据(有参)
            paramSql = string.Format("select * from UserInfo where UserId>{0}UserId", dbOperator);
            using (var dataReader = db.ExecuteReaderWithObjParam(paramSql, new { UserId = 2 }))
            {
                dataList = db.DataReaderToEntityList<UserInfo>(dataReader);
            }
            dbParams = new List<IDbDataParameter>();
            dbParams.Add(dbFactory.GetDbParam("UserId", 2));
            using (var dataReader = db.ExecuteReader(paramSql, dbParams))
            {
                dataList = db.DataReaderToEntityList<UserInfo>(dataReader);
            }
        }
    }
}
