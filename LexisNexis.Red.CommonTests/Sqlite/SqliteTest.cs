using NUnit.Framework;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.CommonTests.Sqlite.Tests
{

    [TestFixture()]
    class SqliteTest
    {
        private readonly string dbFilePath = "test.db";

        public class Stock
        {
            [PrimaryKey, AutoIncrement]
            public int Id { get; set; }

            [MaxLength(8)]
            public string Symbol { get; set; }
        }

        [SetUp]
        public void Setup()
        {
            File.Delete(dbFilePath);
        }

        [Test()]
        public void GeneralDBTest()
        {
            // create/connect db
            using (var db = new SQLiteConnection(dbFilePath))
            {
            

                // int d = db.Execute("SELECT  top 1 * FROM Stock ");
                // create table
                var ff = db.CreateTable<Stock>();

                var d = db.Query<Stock>("select * from Stock");
                Assert.IsTrue(File.Exists(dbFilePath));

                // add
                Stock s1 = new Stock()
                {
                    Symbol = @"zgsy",
                };
                s1.Id = db.Insert(s1);

                // select
                var result = db.Query<Stock>("select * from Stock where Id = ?", s1.Id);
                Assert.AreEqual(1, result.Count);
                Assert.AreEqual(s1.Symbol, result[0].Symbol);

                // update
                s1.Symbol = @"shqc";
                db.Update(s1);
                result = db.Query<Stock>("select * from Stock where Id = ?", s1.Id);
                Assert.AreEqual(1, result.Count);
                Assert.AreEqual(s1.Symbol, result[0].Symbol);

                // delete
                db.Delete<Stock>(s1.Id);
                result = db.Query<Stock>("select * from Stock where Id = ?", s1.Id);
                Assert.AreEqual(0, result.Count);
            }
        }

        [TearDown]
        public void TearDown()
        {
            File.Delete(dbFilePath);
        }
    }
}
