#### SmartDb.NetCore是一套基于ADO.Net和DoNetCore对MSSql、MySql、Oracle数据库支持的快速开发和轻量级ORM框架.

SmartDb.NetCore框架特点如下：
   * 支持.NF和DoNetCore框架。
   * 轻量级半ORM框架，封装基于单表CRUD等操作，同时支持事务、SQL语句、存储过程操作。
   * 提供基于Emit将IDataReader、DataTable转化为实体对象。
   * 支持非参数化SQL语句、原生参数化SQL语句及类似Dapper参数化功能，提供原生ADO.Net对CRUD操作功能。

本源码提供SmartDb.NetCore对MSSql、MySql调用一些测试代码，大家根据测试项目配置自己创建测试数据库和测试数据表。

SmartDb.MySql.NetCore是SmartDb.NetCore对MySql支持的驱动包，Nuget包地址如下：[SmartDb.MySql.NetCore](https://www.nuget.org/packages/SmartDb.MySql.NetCore/)<br>
SmartDb.SqlServer.NetCore是SmartDb.NetCore对SqlServer支持的驱动包，Nuget包地址如下：[SmartDb.SqlServer.NetCore](https://www.nuget.org/packages/SmartDb.SqlServer.NetCore/)

实体类：
``` 
 [Table(TableName="userinfo")]
 public class UserInfo
 {
      [TableColumn(IsPrimaryKey = true)]
      public int UserId { get; set; }

      public string UserName { get; set; }

      public int Age { get; set; }

      public string Email { get; set; }
 }
```

封装调用SmartDb.NetCore的封装类
```
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
            var pageDataList1 = db.QueryPageList<UserInfo>(10, 1, "*", "UserId>2", "UserId asc", null);
            var pageDataList2 = db.QueryPageList<UserInfo>(10, 1, "UserId,UserName", string.Format("UserId>{0}UserId", dbOperator), "UserId asc", new { UserId = 2 });
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
```
控制台调用示例代码：
```
 class Program
    {
       static string connectString = "server=localhost;user id=root;password=123456;database=testdb;SslMode=None";
        static bool isShowSqlToConsole = true;

        static void Main(string[] args)
        {
            Test();
            Console.ReadLine();
        }

        public static void Test()
        {
            var dbTest = new DbTest(connectString, isShowSqlToConsole);
            dbTest.DeleteAll();
            dbTest.Insert();
            dbTest.Delete();
            dbTest.Update();
            dbTest.Query();
            dbTest.OrtherQuery();
            dbTest.OrtherNoneQuery();
        }
    }
```
