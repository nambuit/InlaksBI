using BackBone;
using InlaksIB.Properties;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace InlaksIB
{
    public class MongoWarehouse:WarehouseInterface
    {
        public List<ValuePair> getFilterColumns(string tablename)
        {
            var data = getDatasetCollection(tablename, 1,true);

            var pairs = new List<ValuePair>();

            foreach (DataColumn column in data.Columns)
            {
                var pair = new ValuePair();

                pair.ID = column.ColumnName;
                pair.Value = column.ColumnName;

                pairs.Add(pair);
            }

            return pairs;
        }
        public List<ValuePair> getDataSets(string moduleid)
        {

            var pairs = new List<ValuePair>();

            

            var datasets = new string[] { "customer_summary_cust_profit", "customersegments_custrep" };
            foreach(string dataset in datasets.Where(t => t.CleanUp().Contains(moduleid.CleanUp()))){
                var pair = new ValuePair();
                pair.ID = dataset;
                pair.Value = dataset;
                pairs.Add(pair);
            }

            return pairs;
        }


        public DataTable FilteredData(string dataset, List<DataSetFilter> filters)
        {
            
            DataTable dt = new DataTable();

            dt = getDatasetCollection(dataset, 0,false);
  
            var withfilters = filters.Where(f => !f.ColumnValue.CleanUp().isNull()).ToList();

            if (withfilters.Count > 0)
            {
                var querybuild = new StringBuilder();
               // querybuild.Append("where ");

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

                dt = dt.Select(querybuild.ToString()).CopyToDataTable();

            }


            var included = filters.Where(f => f.IsIncluded).ToList();

            if (included.Count > 0)
            {
                var includecolums = new string[included.Count];

                for (int i = 0; i < included.Count; i++)
                {
                    includecolums[i] = included[i].ColumnName;
                }

                dt = new DataView(dt).ToTable(false, includecolums); 
            }

            return dt;

        }

        private DataTable getDatasetCollection(string dataset, int size,bool headersonly)
        {
            string[] fields1, fields2, fields3, fields4,fields5,fields6,collections;

            string[][] fields; int [] fetchsizes; FilterDefinition<BsonDocument>[] filters;

            DataSet data; DataTable dt,dt1, dt2, dt3, dt4,dt5,dt6;

            var db = new MongoClient(new Settings().warehousedb).GetDatabase("inlaksbiwarehouse");
            var filter = Builders<BsonDocument>.Filter.Empty;
            switch (dataset.CleanUp())
            {
                case "customer_summary_cust_profit":

                     fields1 = new string[] { "CustomerId", "CustProfitGroup", "DepsBalGroup", "AgeGroup", "AnnualIncomeGroup", "CreditScoreGroup", "Gender", "TotalBalGroup", "CustomerIndustry" };
                     fields2 = new string[] { "EB_IND_DESCRIPTION", "ID" };
                    fields3 = new string[] { "CustMonthlySpreadIncome", "TotalBalance", "CustomerId", "BusinessDate", "DepsBalance" };
                    fields4 = new string[] { "Year_", "MonthNameYear", "YearMonthDayName", "Weekday", "Quater", "BusinessDate" };

                     fields = new string[][] {fields1,fields2,fields3,fields4};

                    if (headersonly)
                    {
                        return getTableSchema(fields);
                    }
                   

                    fetchsizes = new int[] { size, 0, 0, 0 };

                    filters = new FilterDefinition<BsonDocument> [] { filter,filter,filter,filter};

                    collections = new string[] { "customerdim", "industry", "customerfact", "datedim" };

                    data = getMongoDataSet(fields,collections,filters,db,fetchsizes);

                     dt1 = data.Tables[collections[0]];
                    dt2 = data.Tables[collections[1]];
                     dt3 = data.Tables[collections[2]];
                    dt4 = data.Tables[collections[3]];

                    dt = joinTables(new DataTable[] { dt1, dt2 }, "CustomerIndustry", "ID");

                    dt = joinTables(new DataTable[] { dt, dt3 }, "CustomerId", "CustomerId");

                    dt = joinTables(new DataTable[] { dt, dt4 }, "BusinessDate", "BusinessDate");

                    dt.Columns["EB_IND_DESCRIPTION"].ColumnName = "Industry";

                    return dt;



                case "customersegments_custrep":

                      fields1 = new string[] { "CustomerId", "FullName", "CustStartDate", "CustNewToday", "CustNewThisMonth",  "Gender", "CustomerIndustry", "CustomerSector" };
                      fields2 = new string[] { "EB_IND_DESCRIPTION", "ID" };
                      fields3 = new string[] { "EB_SEC_SHORT_NAME", "ID"};
                      fields4 = new string[] { "CustomerId", "BranchId" };
                      fields5 = new string[] { "BranchName", "Region", "BranchId" };
                      fields6 = new string[] { "Year_", "MonthNameYear", "YearMonthDayName", "Weekday", "Quater", "BusinessDate" };

                     fields = new string[][] { fields1, fields2, fields3, fields4,fields5,fields6 };

                    if (headersonly)
                    {
                        return getTableSchema(fields);
                    }


                    fetchsizes = new int[] { size, 0, 0, 0,0,0 };

                     filters = new FilterDefinition<BsonDocument>[] { filter, filter, filter, filter,filter,filter };

                     collections = new string[] { "customerdim", "industry","sector", "customerfact","branchdim", "datedim" };

                     data = getMongoDataSet(fields, collections, filters, db, fetchsizes);

                     dt1 = data.Tables[collections[0]];
                     dt2 = data.Tables[collections[1]];
                     dt3 = data.Tables[collections[2]];
                     dt4 = data.Tables[collections[3]];
                     dt5 = data.Tables[collections[4]];
                     dt6 = data.Tables[collections[5]];

                    dt = joinTables(new DataTable[] { dt1, dt2 }, "CustomerIndustry", "ID");

                    dt = joinTables(new DataTable[] { dt, dt3 }, "CustomerSector", "ID");

                    dt = joinTables(new DataTable[] { dt, dt4 }, "CustomerId", "CustomerId");

                    dt = joinTables(new DataTable[] { dt, dt5 }, "BranchId", "BranchId");

                    dt = joinTables(new DataTable[] { dt, dt5 }, "BranchId", "BranchId");

                    dt = joinTables(new DataTable[] { dt, dt6 }, "CustStartDate", "BusinessDate");

                    dt.Columns["EB_IND_DESCRIPTION"].ColumnName = "Industry";
                    dt.Columns["EB_SEC_SHORT_NAME"].ColumnName = "Sector";

                    return dt;







                default:
                    return null;
                    
            }
        }


        private DataTable getTableSchema(string[][]fieldsets)
        {
            List<string> headers = new List<string>();
            foreach(var fields in fieldsets)
            {
                foreach(var field in fields)
                {
                    if (field.Trim().Equals("EB_IND_DESCRIPTION"))
                    {
                        
                        headers.Add("Industry");
                    }
                    else if (field.Trim().Equals("EB_SEC_SHORT_NAME"))
                    {
                        headers.Add("Sector");
                    }
                    else
                    {
                        headers.Add(field);
                    }

                }
            }
            DataTable dt = new DataTable();
            foreach(var header in headers.Distinct())
            {
                dt.Columns.Add(new DataColumn() { ColumnName = header });
            }
            return dt;
        }

        private ProjectionDefinition<BsonDocument> getFieldProjection(IEnumerable<string> fields)
        {
            var projection = Builders<BsonDocument>.Projection.Exclude("_id");

            foreach (string field in fields)
            {
                projection = projection.Include(field);
            }


            return projection;
        }

        private DataTable getMongoData(string[] fields,string collection, FilterDefinition<BsonDocument> filter, IMongoDatabase db, int fetchsize)
        {
            var projection = getFieldProjection(fields);
          
           var data = db.GetCollection<BsonDocument>(collection).Find(filter).Project(projection).Limit(fetchsize).ToList();

            return data.BsonToDataTable();
        }


        private DataSet getMongoDataSet(string [][] fields, string[] collections, FilterDefinition<BsonDocument>[] filters, IMongoDatabase db, int [] fetchsizes)
        {
            var data = new DataSet();

            for(int i = 0; i < collections.Length; i++)
            {
                data.Tables.Add(getMongoData(fields[i], collections[i], filters[i], db, fetchsizes[i]));
                data.Tables[i].TableName = collections[i];
            }

            return data;

        }



        private DataTable joinTables(DataTable[] dtbs, string key, string foreignkey)
        {
            var fields = new List<string>();

            DataTable dtResult = new DataTable();



            foreach (DataTable dts in dtbs)
            {
                foreach (DataColumn column in dts.Columns)
                {
                    fields.Add(column.ColumnName);

                    try
                    {
                        dtResult.Columns.Add(new DataColumn(column.ColumnName, Type.GetType("System.String")));
                    }
                    catch
                    {
                        dtResult.Columns.Add(new DataColumn(column.ColumnName + (new Random().Next(1, 20).ToString()), Type.GetType("System.String")));
                    }
                }
            }


            var results = from dataRows1 in dtbs[0].AsEnumerable()
                          join dataRows2 in dtbs[1].AsEnumerable()
                          on dataRows1.Field<string>(key) equals dataRows2.Field<string>(foreignkey)
                          select dtResult.LoadDataRow(dataRows1.ItemArray.Concat(dataRows2.ItemArray).ToArray(), false);

            var dt = results.CopyToDataTable();

            dt = new DataView(dt).ToTable(false, fields.Distinct().ToArray());

            return dt;
        }


    }




}