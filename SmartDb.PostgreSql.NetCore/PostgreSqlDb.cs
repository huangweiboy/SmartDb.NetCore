using SmartDb;
using SmartDb.NetCore;
using System;
using System.Data;

namespace SmartDb.PostgreSql.NetCore
{
    public class PostgreSqlDb:SqlDb
    {
       
        public PostgreSqlDb(string connectionString="")
        {
            ConnectionString = connectionString;
            var dbFactory = new PostgreSqlFactory();
            DbHelper.DbFactory = dbFactory;
            DbBuilder = new PostgreSqlBuilder();
            DbBuilder.DbFactory = dbFactory;
        }

    }
}
