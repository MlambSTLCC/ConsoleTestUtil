using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTestUtil
{
    class Program
    {
        static void Main(string[] args)
        {
            //Create a dummy employee
            var employee = new Employee("");

            //Build a SQL Insert statement using Reflection on an employee object
            var insertSQL = Util.GetInsertSQL(employee);

            //Set SQL parameters using Reflection on an employee object
            var parmList = Util.GetSQLParameters(employee, insertSQL);

            //Pass parametized SQL statement along with parameter list into method to update data,
            //  possibly returning one or more "out" parameters holding foreign keys to this table.
            //  E.g. insert Order row, then get the ID generated to be passed to the LineItem table insert statements.

            //Needs to be updated, see App.config
            var connectionString = Util.GetConnectionString();

            //var parmReturnList = Util.UpdateData(connectionString, insertSQL, parmList);

            Console.WriteLine(insertSQL);
            Console.ReadLine();
        }
    }
}
