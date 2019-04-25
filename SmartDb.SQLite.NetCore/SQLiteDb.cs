using SmartDb;
using SmartDb.NetCore;
using System;
using System.Data;

namespace SmartDb.SQLite.NetCore
{
    public class SQLiteDb:SqlDb
    {
       
        public SQLiteDb(string connectionString="")
        {
            ConnectionString = connectionString;
            var dbFactory = new SQLiteDbFactory();
            DbHelper.DbFactory = dbFactory;
            DbBuilder = new SQLiteBuilder();
            DbBuilder.DbFactory = dbFactory;
        }

    }
}
