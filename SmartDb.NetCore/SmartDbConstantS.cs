using System;
using System.Collections.Generic;
using System.Text;

namespace SmartDb.NetCore
{
    public class SmartDbConstants
    {
        public const string cacheTableKey= "smartdb:table:{0}";
        public const string cachePKCloumnKey = "smartdb:pkcolumn:{0}";

        public const string template_tableName = "{tableName}";
        public const string template_dbOperator = "{dbOperator}";
        public const string template_columnName = "{columnName}";
        public const string template_columnNames = "{columnNames}";
        public const string template_columnValues = "{columnValues}";


    }
}
