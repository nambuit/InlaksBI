using BackBone;
using InlaksIB.Properties;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace InlaksIB
{
    public class MySqlWarehouse:WarehouseInterface
    {
         public  List<ValuePair> getFilterColumns(string tablename)
        {
            DBInterface db;
            db = new MySQLDBInterface(new Settings().warehousedb);
            var pairs = db.getValuePair("Field", "Extra", "show columns from " + tablename);



            return pairs;
        }
        public List<ValuePair> getDataSets(string moduleid)
        {
            DBInterface db;
            db = new MySQLDBInterface(new Settings().warehousedb);
            db = new MySQLDBInterface(new Settings().warehousedb);
           var pairs = db.getValuePair("table_name", "table_name", "select table_name from listviews where table_name like'%" +moduleid+ "%'");

            return pairs;
        }


        public DataTable FilteredData(string dataset, List<DataSetFilter> filters)
        {
            var querybuild = new StringBuilder();

            querybuild.Append("select ");

            var included = filters.Where(f => f.IsIncluded).ToList();

            if (included.Count > 0)
            {
                var includecolums = new string[included.Count];

                for (int i = 0; i < included.Count; i++)
                {
                    includecolums[i] = included[i].ColumnName;
                }

                querybuild.Append(string.Join(",", includecolums));
            }
            else
            {
                querybuild.Append("*");
            }

            querybuild.Append(" from ").Append(dataset);

            var withfilters = filters.Where(f => !f.ColumnValue.CleanUp().isNull()).ToList();

            if (withfilters.Count > 0)
            {
                querybuild.Append(" where ");

                var filtercolums = new string[withfilters.Count];

                for (int i = 0; i < withfilters.Count; i++)
                {
                    if (withfilters[i].Operator.CleanUp().Contains("like"))
                    {
                        filtercolums[i] = " " + withfilters[i].ColumnName + " " + withfilters[i].Operator + " '%" + withfilters[i].ColumnValue + "%' ";
                    }
                    else
                    {
                        filtercolums[i] = " " + withfilters[i].ColumnName + " " + withfilters[i].Operator + " '" + withfilters[i].ColumnValue + "' ";
                    }
                }


                querybuild.Append(string.Join("and", filtercolums));


            }
            var sql = querybuild.ToString();

            DBInterface db = new MySQLDBInterface(new Settings().warehousedb);

            return db.getData(sql);

        }




    }
}