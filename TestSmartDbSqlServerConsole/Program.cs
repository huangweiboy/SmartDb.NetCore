using System;

namespace TestSmartDbSqlServerConsole
{
    class Program
    {
        static string connectString = "server=localhost;user id=sa;password=123456;database=testdb;";
        static bool isShowSqlToConsole = true;

        static void Main(string[] args)
        {
            Test();
            Console.ReadLine();
        }

        public static void Test()
        {
            var dbTest = new DbTest(connectString, isShowSqlToConsole);
            //dbTest.DeleteAll();
            //dbTest.Insert();
            //dbTest.Delete();
            //dbTest.Update();
            dbTest.Query();
            dbTest.OrtherQuery();
            dbTest.OrtherNoneQuery();
        }
    }
}
