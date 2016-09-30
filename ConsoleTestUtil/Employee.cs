using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;

namespace ConsoleTestUtil
{
    public class Employee : DLBase
    {
        public Int32 ID { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Gender { get; set; }
        public string HomePhone { get; set; }
        public string CellPhone { get; set; }
        public string Email { get; set; }
        public Int32 StoreID { get; set; }
        public Int32 JobID { get; set; }
        public DateTime HireDate { get; set; }
        public bool IsHispanic { get; set; }

        public Employee() 
        {
        }

        public Employee(string testValues)
        {
            LastName = "Oberst";
            FirstName = "Bob";
            Gender = "M";
            CellPhone = "3147071202";
            Email = "bob.oberst@prodigy.net";
            StoreID = 1;
            JobID = 1;
            HireDate = DateTime.Today;
        }

        public Employee(Int32 id) 
        {
            ID = id;
        }

        public Employee(DataRow dr) : base(dr)
        {
        }

        //public override string ToString()
        //{
            
        //}
    }

}
