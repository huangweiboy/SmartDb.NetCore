using System;

namespace TestSmartDbPostgreSQLConsole
{
    class Program
    {
        static string connectString = "server=192.168.58.131;port=5432;user id=xiaozhang1;password=123456;database=testdb;";
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
}
