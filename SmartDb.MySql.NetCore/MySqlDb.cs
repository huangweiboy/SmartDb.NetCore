using SmartDb.NetCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartDb.MySql.NetCore
{
   public class MySqlDb : SqlDb
    {

        public MySqlDb(string connectionString = "")
        {
            ConnectionString = connectionString;
            var dbFactory = new MySqlFactory();
            DbHelper.DbFactory = dbFactory;
            DbBuilder = new MySqlBuilder();
            DbBuilder.DbFactory = dbFactory;
        }

    }
}
