﻿using SmartDb;
using SmartDb.NetCore;
using System;
using System.Data;
using System.Data.SqlClient;

namespace SmartDb.SqlServer.NetCore
{
    public class SqlServerDb:SqlDb
    {
       
        public SqlServerDb(string connectionString="")
        {
            ConnectionString = connectionString;
            var dbFactory = new SqlServerFactory();
            DbHelper.DbFactory = dbFactory;
            DbBuilder = new SqlServerBuilder();
            DbBuilder.DbFactory = dbFactory;
        }

    }
}
