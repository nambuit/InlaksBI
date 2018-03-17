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
    public class SqlServerWarehouse:WarehouseInterface
    {
         public  List<ValuePair> getFilterColumns(string tablename)
        {
            DBInterface db;
            db = new SQLServerDBInterfac(new Settings().warehousedb);
            var pairs = db.getValuePair("name", "name", "select c.name from sys.columns c where c.object_id = OBJECT_ID('"+tablename+"')");



            return pairs;
        }

        public void DeleteDataSet(string datasetname)
        {
            var db = new SQLServerDBInterfac(new Settings().warehousedb);
            db.Execute(" drop view " + datasetname );
        }


        public List<ValuePair> getViewColumns(string tablename)
        {
            DBInterface db;
            db = new SQLServerDBInterfac(new Settings().warehousedb);
            var pairs = db.getValuePair("name", "name", "select c.name from sys.columns c where c.object_id = OBJECT_ID('" + tablename + "')");



            return pairs;
        }

        public List<ValuePair> getDataSets(string moduleid)
        {

            var db = new InlaksBIContext();

            var pairs = new List<ValuePair>();

            foreach (DataSetDetail set in db.DataSets.Where(d => d.Module.value == moduleid))
            {
                var pair = new ValuePair();
                pair.ID = set.DataSetName;
                pair.Value = set.DataSetName;
                pairs.Add(pair);
            }

            return pairs;
        }

        public List<ValuePair> getTables()
        {
            DBInterface db;
            db = new SQLServerDBInterfac(new Settings().warehousedb);
            db = new SQLServerDBInterfac(new Settings().warehousedb);
            var pairs = db.getValuePair("table_name", "table_name", "select c.name as table_name from sys.objects c where c.type_desc in('USER_TABLE','VIEW')");

            return pairs;
        }



        public DataTable FilteredData(string dataset, List<DataSetFilter> filters)
        {
            var querybuild = new StringBuilder();

            querybuild.Append("select ");

            var included = filters.Where(f => f.IsIncluded).ToList();

            bool isIncluded = included.Count > 0;

            included = isIncluded ? included : filters;

            //if ()
            //{
                var includecolums = new string[included.Count];

                for (int i = 0; i < included.Count; i++)
                {
                    includecolums[i] = included[i].DisplayName.isNull()?included[i].ColumnName: (included[i].ColumnName+" as "+ included[i].DisplayName);
                }

                querybuild.Append(string.Join(",", includecolums));
            //}
            //else
            //{
            //    querybuild.Append("*");
            //}

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

            DBInterface db = new SQLServerDBInterfac(new Settings().warehousedb);

            return db.getData(sql);

        }


        public string getDatasetBuilder(List<DatasetObject> datasetobject)
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder sb2 = new StringBuilder();

            int tcounter = 0;

            int ccounter = 0;

            string[] tags = { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };

            foreach (DatasetObject tbitem in datasetobject)
            {

                if (tcounter == 0)    //handling the first table
                {
                    sb2.Append(" from " + tbitem.TableName + " " + tags[0]);
                }
                else
                {
                    var pretag = tags[tcounter - 1];       //setting previous table values
                    var pretable = datasetobject[tcounter - 1];
                    sb2.Append(" inner join " + tbitem.TableName + " " + tags[tcounter] + " on (" + pretag + "." + pretable.NxtTable + "=" + tags[tcounter] + "." + tbitem.PreTable + ") ");
                }

                ccounter = 0;

                if (tbitem.Columns.isNull())
                {
                    sb.Append(tags[tcounter] + ".*");
                }
                else
                {
                    foreach (string column in tbitem.Columns)
                    {
                        ccounter++;

                        sb.Append(tags[tcounter] + "." + column + "");

                        if (ccounter < tbitem.Columns.Length)
                        {
                            sb.Append(",");
                        }
                    }

                }
                tcounter++;

                if (tcounter < datasetobject.Count)
                {
                    sb.Append(",");
                }



            }

            string cols = sb.ToString();


            string tabs = sb2.ToString();



            var script = "CREATE VIEW " + datasetobject[0].DataSetName + " AS   Select ";



            script = script + cols + tabs;

            return script;
        }


        public DataTable testobject(string objectname)
        {
            var sql = "select TOP 1 * from " + objectname + "";

            var dt = new SQLServerDBInterfac(new Settings().warehousedb).getData(sql);

            return dt;
        }


    }

 
}