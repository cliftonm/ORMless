using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Dynamic;

namespace ORMless
{
    public static class ExtensionMethods
    {
        public static dynamic ToDynamic(this object obj)
        {
            // Null-check

            IDictionary<string, object> expando = new ExpandoObject();

            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(obj.GetType()))
            {
                expando.Add(property.Name, property.GetValue(obj));
            }

            return expando;
        }

        public static List<dynamic> ToDynamicList(this DataTable dt)
        {
            List<dynamic> objects = new List<dynamic>();

            foreach (DataRow row in dt.Rows)
            {
                ExpandoObject obj = new ExpandoObject();
                objects.Add(obj);

                foreach (DataColumn col in dt.Columns)
                {
                    ((IDictionary<string, object>)obj)[col.ColumnName] = row[col];
                }
            }

            return objects;
        }

        public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (var t in items) action(t);
        }
    }

    class Program
    {
        const string CONNSTR = @"Data Source=MCLIFTON-5P5KZN\SQLEXPRESS;Initial Catalog=bmbfa;Integrated Security=True;";

        static void Main(string[] args)
        {
            var records = Query("Center", new { Id = 0, CenterName = "" });

            foreach (var rec in records)
            {
                Console.WriteLine($"{rec.Id} : {rec.CenterName}");
            }

            var newRec = Insert("Center", new
            {
                CenterName = "Test",
                Address = "123 Polka St",
                City = "Chicago",
                State = "IL",
                Zip = "60007",
                Phone = "123-456-6789",
                Email = "a@b.com",
                AdminUserId = 15,
            });

            Console.WriteLine("Inserted ID: " + newRec.Id);

            Update("Center", new { newRec.Id, CenterName = "Test2" });
            Delete("Center", new { newRec.Id });
        }

        static dynamic Insert(string tableName, object data)
        {
            return FluentOrmless.ConnectWith(CONNSTR).ForTable(tableName).Insert(data).Execute();
        }

        static List<dynamic> Query(string tableName, object columnTemplate)
        {
            return FluentOrmless.ConnectWith(CONNSTR).ForTable(tableName).Select(columnTemplate).Fill().ToDynamicList();
        }

        static void Update(string tableName, object data)
        {
            var cmd = FluentOrmless.ConnectWith(CONNSTR).ForTable(tableName).Update(data);
            Console.WriteLine(cmd.Sql);
            cmd.Execute();
        }

        static void Delete(string tableName, object data)
        {
            var cmd = FluentOrmless.ConnectWith(CONNSTR).ForTable(tableName).Delete(data);
            Console.WriteLine(cmd.Sql);
            cmd.Execute();
        }
    }
}
