using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace ConsoleTestUtil
{
    public class DLBase
    {
       
        public DLBase() { }

        public DLBase(DataRow row) 
        {
            var columnMap = Util.MapColumns(row);

            foreach (PropertyInfo pi in this.GetType().GetProperties())
            {
                if (columnMap.ContainsKey(pi.Name))
                {
                    object value = row.ItemArray[columnMap[pi.Name]];
                    ReflectionSetValue(pi, value);
                }
            }
        }

        private void ReflectionSetValue(PropertyInfo pi, object value) 
        {
            if (pi.Name.ToLower().Contains("date"))
            {
                DateTime dateTime;
                if (DateTime.TryParse(value.ToString(), out dateTime))
                    value = dateTime;
            }

            if (pi.CanWrite && !string.IsNullOrEmpty(value.ToString()))
            {
                try
                {
                    Int32 result = 0;
                    if(Int32.TryParse(value.ToString(), out result))
                        value = result;

                    pi.SetValue(this, value);
                }
                catch (Exception ex)
                {
                }
            }
        }
    }
}
