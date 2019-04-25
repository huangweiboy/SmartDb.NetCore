using System;

namespace TestSmartDbMyqlConsole
{
    class Program
    {
        static string dbName = "f:\\testdb.sqlite";
        static string connectString = "Data Source=f:\\testdb.sqlite;Pooling=true;FailIfMissing=false;";
        static bool isShowSqlToConsole = true;

        static void Main(string[] args)
        {
            Test();
            Console.ReadLine();
        }

        public static void Test()
        {
            var dbTest = new DbTest(connectString, isShowSqlToConsole);
            dbTest.CreateTable(dbName);
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
