using BackBone;
using InlaksIB.Properties;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace InlaksIB.Controllers
{
    public class ReportController : Controller
    {




        override
    protected void OnActionExecuting(ActionExecutingContext context)
        {
            var user = (InlaksIB.User)context.HttpContext.Session["User"];

            if (user == null || ((string)context.HttpContext.Session["LoggedIn"]) == "False")
            {

                context.HttpContext.Response.Redirect("~/Login");
            }

        }

        // GET: Report

        public ActionResult Index()
        {
            return View("TestReport");
        }


        public ActionResult ProcessStandard(int id)
        {
            var report = new InlaksBIContext().ResourceReports.Where(r => r.Resource.ResourceID == id).ToList();
            if (report.Count > 0)
            {
                ViewBag.Title = report[0].Resource.ResourceName;

                var user = (User)Session["User"];

                var reportmodel = new ReportModel();

                reportmodel.dataSet = report[0].dataSet;
                reportmodel.Filters = new List<DataSetFilterModel>();
                foreach (var filter in report[0].Filters.ToList())
                {

                    reportmodel.Filters.Add(new DataSetFilterModel()
                    {
                        ColumnName = filter.ColumnName,
                        ColumnValue = filter.ColumnValue,
                        ID = filter.ID,
                        IsIncluded = filter.IsIncluded,
                        ReportID = filter.Report.ReportID,
                        Operator = filter.Operator

                    });


                }

                reportmodel.InstanceID = report[0].InstanceID;

                reportmodel.ResourcID = report[0].Resource.ResourceID;

                reportmodel.pivotConfig = report[0].pivotConfig;

                var rid = report[0].ReportID;

                reportmodel.ReportID = rid;

                var userconfig = new InlaksBIContext().UserReportStates.FirstOrDefault(u => u.User.UserID == user.UserID && u.ReportID == rid );

                if (userconfig == null)
                {
                    reportmodel.UserConfig = "None";
                }
                else
                {
                    if(reportmodel.InstanceID == userconfig.InstanceID)
                    {
                        reportmodel.UserConfig = userconfig.ConfigData;
                    }
                    else
                    {
                        reportmodel.UserConfig = "BaseChanged";
                    }
                }

                return View("StandardReportViewer", reportmodel);
            }

            return View("StandardReportViewer", null);
        }






        public string getCustomerAgeDistribution()
        {
            var groups = new List<AgeDistribution>();

            var db = new DBConnector(new Settings().warehousedb, "");

            var sql = " SELECT (select count(customerId) from inlaksbiwarehouse.customerdim where AgeGroup='0-18') `0-18`," +
      "(select count(customerId) from inlaksbiwarehouse.customerdim where AgeGroup = '19-25') `19 - 25`," +
      "(select count(customerId) from inlaksbiwarehouse.customerdim where AgeGroup = '26-35') `26 - 35`," +
      "(select count(customerId) from inlaksbiwarehouse.customerdim where AgeGroup = '36-50') `36 - 50`," +
      "(select count(customerId) from inlaksbiwarehouse.customerdim where AgeGroup = '51-64') `51 - 64`," +
      "(select count(customerId) from inlaksbiwarehouse.customerdim where AgeGroup = '65+') `65 +`";

            var result = db.getDataSet(sql);

            for (int i = 0; i < result.Columns.Count; i++)
            {
                var group = new AgeDistribution();
                group.AgeGroup = result.Columns[i].ColumnName;
                group.Count = result.Rows[0][i].toInt();
            }

            return JsonConvert.SerializeObject(groups);
        }



        [HttpPost]
        public string getFilteredData()
        {

            //  var profittable = getProfitabilityTable(0);
            try
            {
                var dataset = Request.Form["dataset"];
                var filtersstring = Request.Form["filters"];
                WarehouseInterface warehouse = new InlaksBIContext().getWarehouse(new Settings().warehousedbtype);

                var filters = JsonConvert.DeserializeObject<List<DataSetFilter>>(filtersstring);

                var dt = warehouse.FilteredData(dataset, filters);

                
                return dt.DataTableToJson();
            }
            catch(Exception s)
            {
               return "Failed";
            }

        }



        [HttpPost]
        public string ProcessReport()
        {

            try
            {
                //  var profittable = getProfitabilityTable(0);
                var db = new InlaksBIContext();

                var dataset = Request.Form["dataset"];
                var filtersstring = Request.Form["filters"];

                // var IsInteractive = Request.Form["IsInteractive"].Equals("1");

                var filters = JsonConvert.DeserializeObject<List<DataSetFilter>>(filtersstring);

                var pivotconfig = Request.Form["pivotconfig"];

                var resourceid = Request.Form["resourceid"].toInt();

                var resource = db.Resources.FirstOrDefault(r => r.ResourceID == resourceid);

                var prevreport = db.ResourceReports.FirstOrDefault(r => r.Resource.ResourceID == resource.ResourceID);

                var report = prevreport.isNull() ? new ResourceReport() : prevreport;

                report.Resource = resource;
                report.pivotConfig = pivotconfig;
                report.InstanceID = Guid.NewGuid().ToString();
                report.dataSet = dataset;

                //foreach(var filter in filters)
                //{
                //    filter.Report = report;

                //}

                if (prevreport.isNull())
                {
                    report.Filters = filters;
                    db.ResourceReports.Add(report);
                }
                else
                {
                    var prefilters = db.DataSetFilters.Where(f => f.Report.ReportID == report.ReportID);
                    db.DataSetFilters.RemoveRange(prefilters);
                    report.Filters = filters;

                }

                db.SaveChanges();

                return "Report Saved and Published Successfully";
            }
            catch (Exception d)
            {
                return "Error encountered while saving and publishing report. Please contact Admin";
            }

        }



        private DataTable FilteredData(string dataset, List<DataSetFilter> filters)
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
                        filtercolums[i] = " " + withfilters[i].ColumnName + " " + withfilters[i].Operator + " '%"+ withfilters[i].ColumnValue + "%' ";
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




        [HttpPost]
        public string SaveUserReportState()
        {

            try
            {
                //  var profittable = getProfitabilityTable(0);
                var db = new InlaksBIContext();

                var pivotconfig = Request.Form["pivotconfig"];
                var reportid = Request.Form["reportid"].toInt();
                var InstanceID = Request.Form["InstanceID"];
                var user = (User)Session["User"];
             
                var prevstate = db.UserReportStates.FirstOrDefault(r => r.ReportID == reportid && r.User.UserID == user.UserID);

                var state = prevstate.isNull() ? db.UserReportStates.Create() : prevstate;

                state.InstanceID = InstanceID;
                state.ConfigData = pivotconfig;
                state.ReportID = reportid;
                state.User = db.Users.FirstOrDefault(u => u.UserID == user.UserID);

                if (prevstate == null)
                {
                  db.UserReportStates.Add(state);
                }


                db.SaveChanges();



                return "Report State Saved Successfully";
            }
            catch (Exception d)
            {
                return "Error encountered while saving state. Please contact Admin";
            }

        }
    }



    }