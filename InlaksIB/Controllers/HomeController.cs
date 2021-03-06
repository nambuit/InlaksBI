﻿using BackBone;
using CrystalDecisions.CrystalReports.Engine;
using InlaksIB.Properties;
using InlaksIB.Reports;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace InlaksIB.Controllers
{
    public class HomeController : Controller
    {

        override
            protected void OnActionExecuting(ActionExecutingContext context)
        {
            var user = (User)context.HttpContext.Session["User"];

            if (user == null || ((string)context.HttpContext.Session["LoggedIn"]) == "False")
            {

                context.HttpContext.Response.Redirect("~/Login");
            }

        }



        // GET: Home
        public ActionResult Index()
        {



            Session["Industry"] = new Properties.Settings().ActiveIndustry;
            return View();
        }



        public ActionResult ReportLauncher(string id)
        {


            var reportid = id;//Request.QueryString.Get("id");
            var report = new InlaksBIContext().Resources.FirstOrDefault(r => r.value == id);
         id =  report.ResourceID.ToString();


            switch (reportid.Trim().ToLower())
            {
                default:
                    return RedirectToAction("ProcessStandard", "Report", new { id = id });

                case "excelwritter":
                    ViewBag.Title = report.ResourceName;

                    return View("ExcelReports");
               
            }

        }

        public ActionResult Logout()
        {
            Session["LoggedIn"] = "False";
            Session.Remove("User");

            return RedirectToAction("Index", "Login");
        }



        public ActionResult RoleSetup(int id, string mode)
        {
            switch (mode.ToLower())
            {
                case "list":

                    return View("RoleSetup", new InlaksBIContext().Roles.Where(r => r.RoleID > 1));

                case "create":
                    ViewBag.Mode = "CreateRole";
                    ViewBag.role = new Role();
                    return View("RoleSetup", new InlaksBIContext().Roles.Where(r => r.RoleID > 1));

                default:


                    if (mode.Trim() == "edit")
                    {
                        ViewBag.Mode = "EditRole";
                    }
                    else
                    {
                        ViewBag.Mode = "DeleteRole";
                    }

                    ViewBag.role = new InlaksBIContext().Roles.FirstOrDefault(r => r.RoleID == id);
                    return View("RoleSetup", new InlaksBIContext().Roles.Where(r => r.RoleID > 1));
            }

        }

        public ActionResult ProcessRole(Role role, int id, string mode)
        {
            var dbcontext = new InlaksBIContext();
            switch (mode)
            {
                case "create":
                    dbcontext.Roles.Add(role);
                    dbcontext.SaveChanges();
                    break;
                case "edit":
                    var old = dbcontext.Roles.FirstOrDefault(t => t.RoleID == id);
                    old.RoleName = role.RoleName;
                    dbcontext.SaveChanges();

                    break;

                case "delete":
                    var item = dbcontext.Roles.FirstOrDefault(t => t.RoleID == id);
                    dbcontext.Roles.Remove(item);

                    dbcontext.SaveChanges();
                    break;
            }


            return RedirectToAction("RoleSetup", new { id = 0, mode = "list" });
        }

        public ActionResult ProcessModule(Module module, int id, string mode)
        {
            try
            {
                var dbcontext = new InlaksBIContext();
                module.Industry = dbcontext.Industries.FirstOrDefault(i => i.IndustryID == module.IndustryID);
                module.IconClass = "glyphicon glyphicon-list";
                switch (mode)
                {
                    case "create":
                       // module.ModuleID = id;
                        dbcontext.Modules.Add(module);
                        dbcontext.SaveChanges();
                        ViewBag.errorclass = "green";
                        ViewBag.message = "Module Created successfully";
                        break;
                    case "edit":
                        var old = dbcontext.Modules.FirstOrDefault(t => t.ModuleID == id);
                        old.ModuleName = module.ModuleName;
                        old.value = module.value;
                        old.Industry = module.Industry;
                        dbcontext.SaveChanges();
                        ViewBag.errorclass = "green";
                        ViewBag.message = "Module modified successfully";
                        break;

                    case "delete":
                        var item = dbcontext.Modules.FirstOrDefault(t => t.ModuleID == id);
                        dbcontext.Modules.Remove(item);

                        dbcontext.SaveChanges();
                        ViewBag.message = "Module deleted successfully";
                        ViewBag.errorclass = "green";
                        break;
                }
            }
            catch (Exception e)
            {
                ViewBag.errorclass = "red";
                ViewBag.message = "Operation failed. Please seek technical assistance";
            }

            return RedirectToAction("ModuleSetup", new { id = 0, mode = "create" });
        }


        public ActionResult MigrationSetup(int id, string mode)
        {
            switch (mode.ToLower())
            {
                case "list":

                    return View("MigrationSetup", new InlaksBIContext().MigrationSchedules.AsEnumerable());

                case "create":
                    ViewBag.Mode = "CreateMigration";
                    ViewBag.schedule = new Migration_Schedule();
                    return View("MigrationSetup", new InlaksBIContext().MigrationSchedules.AsEnumerable());

                default:


                    if (mode.Trim() == "edit")
                    {
                        ViewBag.Mode = "EditMigration";
                    }
                    else
                    {
                        ViewBag.Mode = "DeleteMigration";
                    }

                    var sch = new InlaksBIContext().MigrationSchedules.FirstOrDefault(t => t.ID == id);

                    if (!string.IsNullOrEmpty(sch.MaxDate))
                    {
                        sch.MaxDate = DateTime.ParseExact(sch.MaxDate, "dd MMM yyyy", new System.Globalization.CultureInfo("en-US")).ToString("dd-MM-yyyy");
                    }

                    ViewBag.schedule = sch;
                    return View("MigrationSetup", new InlaksBIContext().MigrationSchedules.AsEnumerable());
            }

        }

        public ActionResult ReportSetup(int id, string mode)
        {
            switch (mode.ToLower())
            {
                case "list":

                    return View("ReportSetup", new InlaksBIContext().ResourceReports.ToList());

                case "create":
                    ViewBag.Mode = "CreateReport";
                    ViewBag.ResourceReport = new ResourceReport();
                    return View("ReportSetup", new InlaksBIContext().ResourceReports.ToList());

                default:


                    if (mode.Trim() == "edit")
                    {
                        ViewBag.Mode = "EditReport";
                    }
                    else
                    {
                        ViewBag.Mode = "DeleteReport";
                    }

                    ViewBag.ResourceReport = new InlaksBIContext().ResourceReports.FirstOrDefault(t => t.ReportID == id);
                    return View("ReportSetup", new InlaksBIContext().ResourceReports.ToList());
            }

        }


        public ActionResult DataSetSetup(string id, string mode)
        {
            if (mode.isNull()) mode = "create"; int ID = 0;
            if (!id.isNull())
            {
                ID = id.toInt();
            }
            switch (mode.ToLower())
            {
                case "list":

                    return View("DataSetSetup", new InlaksBIContext().DataSets.ToList());

                case "create":
                    ViewBag.Mode = "CreateDataSet";
                    ViewBag.DataSetDetail = new DataSetDetail();
                    return View("DataSetSetup", new InlaksBIContext().DataSets.ToList());

                default:


                    if (mode.Trim() == "edit")
                    {
                        ViewBag.Mode = "EditReport";
                    }
                    else
                    {
                        ViewBag.Mode = "DeleteReport";
                    }

                    ViewBag.DataSetDetail = new InlaksBIContext().DataSets.FirstOrDefault(t => t.ID == ID);
                    return View("DataSetSetup", new InlaksBIContext().DataSets.ToList());
            }

        }








        public ActionResult ResourceSetup(int id, string mode)
        {
            switch (mode.ToLower())
            {
                case "list":

                    return View("ResourceSetup", new InlaksBIContext().Resources.ToList());

                case "create":
                    ViewBag.Mode = "CreateResource";
                    ViewBag.Resource = new Resource();
                    return View("ResourceSetup", new InlaksBIContext().Resources.ToList());

                default:


                    if (mode.Trim() == "edit")
                    {
                        ViewBag.Mode = "EditResource";
                    }
                    else
                    {
                        ViewBag.Mode = "DeleteResource";
                    }

                    ViewBag.Resource = new InlaksBIContext().Resources.FirstOrDefault(t => t.ResourceID == id);
                    return View("ResourceSetup", new InlaksBIContext().Resources.ToList());
            }

        }




        public ActionResult UserSetup(string id, string mode)
        {
            switch (mode)
            {
                case "create":
                    ViewBag.mode = "create";
                    return View("CreateUser", new User() { Message = "", errorclass = "" });

                case "edit":

                    var user = new InlaksBIContext().Users.FirstOrDefault(u => u.UserID == id);

                    ViewBag.mode = "edit";
                    return View("CreateUser", user);

                default:

                    return View("CreateUser", new User() { Message = "", errorclass = "" });
            }

        }

        public ActionResult ModuleSetup(int id, string mode)
        {
            ViewBag.errorclass = ""; ViewBag.message = "";
            var module = new InlaksBIContext().Modules.FirstOrDefault(u => u.ModuleID == id);
            switch (mode)
            {
                case "create":
                    ViewBag.mode = "create";
                    return View("CreateModule", new Module());

                case "edit":



                    ViewBag.mode = "edit";
                    return View("CreateModule", module);

                default:
                    ViewBag.mode = "delete";

                    return View("CreateModule", module);
            }

        }



        public ActionResult ProcessMigration(Migration_Schedule schedule, int id, string mode)
        {
            var dbcontext = new InlaksBIContext();
            //schedule.MaxDate = Request.Form["MaxDate"];
            if (!string.IsNullOrEmpty(schedule.MaxDate))
            {
                schedule.MaxDate = DateTime.ParseExact(schedule.MaxDate, "dd-MM-yyyy", new System.Globalization.CultureInfo("en-US")).ToString("dd MMM yyyy");
            }
            
            switch (mode)
            {
                case "create":

                    dbcontext.MigrationSchedules.Add(schedule);
                    dbcontext.SaveChanges();
                    break;
                case "edit":
                    var old = dbcontext.MigrationSchedules.FirstOrDefault(t => t.ID == id);
                    old.DateFilter = schedule.DateFilter;
                    old.Application_Name = schedule.Application_Name;
                    old.FetchSize = schedule.FetchSize;
                    old.MaxDate = schedule.MaxDate;
                    old.Time = schedule.Time;
                    old.Fields = schedule.Fields;
                    old.Status = schedule.Status;
                    old.Interval = schedule.Interval;
                    old.IntervalDescription = schedule.IntervalDescription;
                    old.AllowWeekends = schedule.AllowWeekends;

                    //dbcontext.MigrationSchedules.Remove(old);
                    // dbcontext.MigrationSchedules.Add(schedule);
                    dbcontext.SaveChanges();

                    break;

                case "delete":
                    var item = dbcontext.MigrationSchedules.FirstOrDefault(t => t.ID == id);
                    dbcontext.MigrationSchedules.Remove(item);

                    dbcontext.SaveChanges();
                    break;
            }


            return RedirectToAction("MigrationSetup", new { id = 0, mode = "list" });
        }



        public ActionResult ProcessResource(Resource resource, int id, string mode)
        {
            var dbcontext = new InlaksBIContext();
            //schedule.MaxDate = Request.Form["MaxDate"];
            try
            {
                switch (mode)
                {
                    case "create":
                      var   re = dbcontext.Resources.Create();

                        re.Module = dbcontext.Modules.FirstOrDefault(m => m.ModuleID == resource.ModuleID);
                        re.ResourceName = resource.ResourceName;
                        re.SubMenus = resource.SubMenus;
                        re.value = resource.value;
                        re.Url = resource.Url;
                        dbcontext.Resources.Add(re);
                        dbcontext.SaveChanges();
                        break;
                    case "edit":
                        var old = dbcontext.Resources.FirstOrDefault(t => t.ResourceID == id);
                        resource.Module = dbcontext.Modules.FirstOrDefault(m => m.ModuleID == resource.ModuleID);
                        dbcontext.Resources.Remove(old);
                        dbcontext.Resources.Add(resource);
                        dbcontext.SaveChanges();

                        break;

                    case "delete":
                        var item = dbcontext.Resources.FirstOrDefault(t => t.ResourceID == id);
                        dbcontext.Resources.Remove(item);

                        dbcontext.SaveChanges();
                        break;
                }


                return RedirectToAction("ResourceSetup", new { id = 0, mode = "list" });
            }
            catch (Exception d)
            {
                return RedirectToAction("ResourceSetup", new { id = 0, mode = "list" });
            }

        }

        public ActionResult ProcessReport(ResourceReport resource, int id, string mode)
        {
            var dbcontext = new InlaksBIContext();
            var d = Request.Form;
            //schedule.MaxDate = Request.Form["MaxDate"];
            switch (mode)
            {
                case "create":
                    //resource.Module = dbcontext.Modules.FirstOrDefault(m => m.ModuleID == resource.ModuleID);
                    //dbcontext.Resources.Add(resource);
                    //dbcontext.SaveChanges();
                    break;
                case "edit":
                    //var old = dbcontext.Resources.FirstOrDefault(t => t.ResourceID == id);
                    //resource.Module = dbcontext.Modules.FirstOrDefault(m => m.ModuleID == resource.ModuleID);
                    //dbcontext.Resources.Remove(old);
                    //dbcontext.Resources.Add(resource);
                    //dbcontext.SaveChanges();

                    break;

                case "delete":
                    //var item = dbcontext.Resources.FirstOrDefault(t => t.ResourceID == id);
                    //dbcontext.Resources.Remove(item);

                    //dbcontext.SaveChanges();
                    break;
            }


            return RedirectToAction("ResourceSetup", new { id = 0, mode = "list" });
        }


        public ActionResult ProcessAuth(AuthSetup auth)
        {
            try
            {

                var db = new InlaksBIContext();
                var oldauth = db.AuthConfig.First();

                oldauth.AuthType = auth.AuthType;
                oldauth.Server = auth.Server;
                db.SaveChanges();

                ViewBag.message = auth.AuthType + " Authentication Mode Activated Successfully";
                ViewBag.errorclass = "green";
            }
            catch (Exception e)
            {
                ViewBag.message = "Failed to apply changes, please seek technical assistance";
                ViewBag.errorclass = "red";
            }




            return View("AuthSetup", auth);
        }



        public ActionResult ProcessUser(User user, string id, string mode)
        {
            var message = "";
            var errorclass = "green";
            var context = new InlaksBIContext();
            switch (mode)
            {
                case "create":
                    try
                    {


                        var role = context.Roles.FirstOrDefault(r => r.RoleID == user.RoleID);

                        IPasswordHasher hash = new BasicHash();

                        user.Password = hash.HashPassword(user.Password);
                        user.RePassword = (user.Password);
                        user.UserRole = role;

                        context.Users.Add(user);

                        context.SaveChanges();

                        message = "User Created Successfully";

                    }
                    catch (Exception e)
                    {
                        message = "Failed to create user. Please seek technical assistance";
                        errorclass = "red";
                    }
                    break;

                case "edit":
                    ViewBag.mode = "edit";
                    try
                    {


                        var role = context.Roles.FirstOrDefault(r => r.RoleID == user.RoleID);

                        var olduser = context.Users.FirstOrDefault(u => u.UserID == id);



                        olduser.UserRole = role;
                        olduser.Name = user.Name;
                        olduser.Email = user.Email;
                        olduser.RePassword = olduser.Password;

                        olduser.UserRole = role;


                        context.SaveChanges();

                        message = "User Modified Successfully";

                    }
                    catch (Exception e)
                    {
                        message = "Failed to Modify user. Please seek technical assistance";
                        errorclass = "red";
                    }
                    break;
            }


            return View("CreateUser", new User() { errorclass = errorclass, Message = message });
        }

        public ActionResult ChangePassword()
        {

            return View("ChangePassword", new PasswordChange());

        }

        public ActionResult AuthenticationSetup()
        {
            var db = new InlaksBIContext();
            try
            {
                return View("AuthSetup", db.AuthConfig.First());
            }
            catch
            {

                //db.SaveChanges();

                return AuthenticationSetup();


            }

        }



        public ActionResult RolesResources()
        {
            return View("RolesResources");
        }


        [HttpGet]
        public string RoleResources(int id)
        {
            var rolesresources = new InlaksBIContext().RolesResources.ToList();

            var selected = rolesresources.Where(r => r.Role.RoleID == id).ToList();

            var others = new InlaksBIContext().Resources.ToList();

            var resourcelist = new List<RoleResourceList>();

            others.ForEach(re =>
            {
                resourcelist.Add(new RoleResourceList() { ResourceID = re.ResourceID, RoleID = id, ResourceName = re.ResourceName, selected = "" });
            });

            selected.ForEach(r =>
            {
                var re = resourcelist.FirstOrDefault(t => t.ResourceID == r.Resource.ResourceID);
                if (re.ResourceName != null)
                {
                    resourcelist.Remove(re);
                    re.selected = "selected";
                    resourcelist.Add(re);
                }


            });



            return JsonConvert.SerializeObject(resourcelist);
        }


        [HttpGet]
        public string DataSetColumns(string id)
        {
            var warehouse = new InlaksBIContext().getWarehouse(new Settings().warehousedbtype);

            var pairs = warehouse.getFilterColumns(id);




            return JsonConvert.SerializeObject(pairs);
        }

        [HttpGet]
        public string TableList()
        {
            var warehouse = new InlaksBIContext().getWarehouse(new Settings().warehousedbtype);

            var pairs = warehouse.getTables();




            return JsonConvert.SerializeObject(pairs);
        }

        public ActionResult ProcessRoleResources()
        {

            var currole = Request.Form["role"];

            if (currole == null)
            {
                ViewBag.Message = "A role must be selected";

                ViewBag.errorclass = "red";

                return View("RolesResources");
            }

            try
            {
                var resources = Request.Form["resources"];

                var roleid = int.Parse(currole);

                var db = new InlaksBIContext();

                var previous = db.RolesResources.Where(r => r.Role.RoleID == roleid).ToList();

                db.RolesResources.RemoveRange(previous);

                if (resources != null)
                {

                    foreach (string rid in resources.Split(','))
                    {
                        var resourceid = int.Parse(rid);

                        var role = db.Roles.FirstOrDefault(r => r.RoleID == roleid);
                        var resource = db.Resources.FirstOrDefault(r => r.ResourceID == resourceid);

                        var rolesresource = new RoleResource() { Resource = resource, Role = role };

                        db.RolesResources.Add(rolesresource);




                    }

                }


                ViewBag.Message = "Resource(s) Assigned Successfully";

                ViewBag.errorclass = "green";

                db.SaveChanges();

            }
            catch (Exception e)
            {
                ViewBag.Message = "Failed to Assign Resource(s); Please seek technical assistance";

                ViewBag.errorclass = "red";
            }



            return View("RolesResources");
        }

        public ActionResult ProcessPassword(PasswordChange change)
        {

            var message = "";
            var errorclass = "green";
            var context = new InlaksBIContext();
            try
            {
                if (change.Password == change.NewPassword)
                {
                    goto End;
                }

                var user = (User)Session["User"];
                var hasher = new BasicHash();
                var olduser = context.Users.FirstOrDefault(u => u.UserID == user.UserID);
                if (!hasher.VerifyHashedPassword(olduser.Password, change.Password))
                {

                    message = "Current Password is Invalid";
                    errorclass = "red";
                }
                else
                {
                    olduser.Password = new BasicHash().HashPassword(change.NewPassword);
                    olduser.RePassword = olduser.Password;
                    context.SaveChanges();
                    message = "Password Changed Sucessfully";
                    errorclass = "green";
                }

                End:;
            }
            catch (Exception e)
            {
                message = "Failed to effect password change. Please seek technical assistance";
                errorclass = "red";
            }
            ViewBag.message = message;
            ViewBag.errorclass = errorclass;
            return View("ChangePassword", new PasswordChange());
        }



        [HttpGet]
        public string getValuePair(string id, string param)
        {
            if (param.isNull())
            {

            }

            var pairs = new List<ValuePair>();
            WarehouseInterface warehouse = new InlaksBIContext().getWarehouse(new Settings().warehousedbtype);
            switch (id.CleanUp())
            {
                case "resources":
                    int mid = param.toInt();

                    var res = new InlaksBIContext().Resources.Where(r => r.Module.ModuleID == mid).ToList();

                    foreach (var re in res)
                    {
                        ValuePair pair = new ValuePair();
                        pair.ID = re.ResourceID.ToString();
                        pair.Value = re.ResourceName;
                        pairs.Add(pair);
                    }

                    return JsonConvert.SerializeObject(pairs);


                case "dataset":
                    int modid = param.toInt();
                    var module = new InlaksBIContext().Modules.FirstOrDefault(m => m.ModuleID == modid);

                    pairs = warehouse.getDataSets(module.value);
                    return JsonConvert.SerializeObject(pairs);

                case "datasetcolumns":

                    pairs = warehouse.getViewColumns(param);
                    return JsonConvert.SerializeObject(pairs);


                default:
                    return "";


            }
        }

        [HttpGet]
        public string getFilterColumns(string id)
        {
            WarehouseInterface warehouse = new InlaksBIContext().getWarehouse(new Settings().warehousedbtype);
            var pairs = warehouse.getViewColumns(id);
            var filters = new List<DataSetFilter>();

            foreach (var pair in pairs)
            {
                var filter = new DataSetFilter();
                filter.ColumnName = pair.ID;
                filter.ColumnValue = pair.Value;

                filter.IsIncluded = false;
                filters.Add(filter);
            }


            return JsonConvert.SerializeObject(filters);

        }


        [HttpGet]
        public string DeleteDataSet(string id)
        {

            try
            {
                var db = new InlaksBIContext();
                var dataset = db.DataSets.FirstOrDefault(d => d.DataSetName == id.Trim());
                db.DataSets.Remove(dataset);
                db.SaveChanges();

                var warehouse = db.getWarehouse(new Settings().warehousedbtype);

                warehouse.DeleteDataSet(id);

                return "0";
            }
            catch (Exception s)
            {
                return "1";
            }

        }

        

        [HttpPost]
        public string CreateDataSet()
        {


            try
            {

                var data = Request.Form["datasetdata"];

                var moduleid = Request.Form["module"].toInt();

                var datasetobject = JsonConvert.DeserializeObject<List<DatasetObject>>(data);

                bool isduplicateTable = datasetobject.GroupBy(d => d.TableName).Count() < datasetobject.Count;

                if (isduplicateTable)
                {
                    return "1";

                }

                StringBuilder sb = new StringBuilder();
                StringBuilder sb2 = new StringBuilder();

                int tcounter = 0;

                int ccounter = 0;

                string[] tags = { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };

                foreach (DatasetObject tbitem in datasetobject)
                {

                    if (tcounter == 0)    //handling the first table
                    {
                        sb2.Append(" from \"" + tbitem.TableName + "\" " + tags[0]);
                    }
                    else
                    {
                        var pretag = tags[tcounter - 1];       //setting previous table values
                        var pretable = datasetobject[tcounter - 1];
                        sb2.Append(" inner join \"" + tbitem.TableName + "\" " + tags[tcounter] + " on (" + pretag + ".\"" + pretable.NxtTable + "\"=" + tags[tcounter] + ".\"" + tbitem.PreTable + "\") ");
                    }

                    ccounter = 0;

                    if (tbitem.Columns.isNull())
                    {
                        sb.Append(tags[tcounter]+".*");
                    }
                    else
                    {
                        foreach (string column in tbitem.Columns)
                        {
                            ccounter++;

                            sb.Append(tags[tcounter] + ".\"" + column + "\"");

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

                var db = new InlaksBIContext();

                var script = "CREATE MATERIALIZED VIEW public." + datasetobject[0].DataSetName + " AS   Select ";

                var dataset = new DataSetDetail();

                dataset.Script = script + cols + tabs;

                dataset.DataSetName = datasetobject[0].DataSetName;


                DBInterface dbinterface = new PostgreSQLDBInterface(new Settings().warehousedb);

                int resp = dbinterface.Execute(dataset.Script);

                var sql = "select * from \"" + dataset.DataSetName + "\" LIMIT 1";

                var dt = dbinterface.getData(sql);


                dataset.Module = db.Modules.Where(m => m.ModuleID == moduleid).FirstOrDefault();

                db.DataSets.Add(dataset);

                db.SaveChanges();

                if (dt.Rows.Count > 0)
                {

                    return "0";
                }

                else
                {
                    return "DataSet Created sucessfully but has no data";
                }
            }


            catch (Exception e)
            {
                return e.Message;
            }

        }





        [HttpGet]
        public string LaunchStatic(string id)
        {
            string result;
            try
            {
                ReportDocument report;
                DataTable dt; string sql = ""; DBInterface db;
                
                switch (id.ToLower())
                {

                    default:
                        result = "failed";
                        break;

                    case "sect_analysis":
                        report = new crmbase1();
                        dt = new DataTable();

                        sec_analysis(dt, report);

                        

                        LoadCBNReturnsReport(dt, report,id);
                        result = "success";
                        break;

                    case "due_bal":
                        report = new due_bal221();

                         sql = "select distinct '' as \"Code\", \"ACCOUNT_TITLE_1\" as \"NAME_OF_BANK\", CAST(COALESCE(NULLIF(\"WORKING_BALANCE\",''),'0') as decimal) as \"Amount\"  from \"ACCOUNT\" where \"CATEGORY\" >= '5000' and \"CATEGORY\" <='5999'";

                         db = new PostgreSQLDBInterface(new Settings().sourcedb);

                        dt = db.getData(sql);

                        LoadCBNReturnsReport(dt, report,id);
                        result = "success";
                        break;

                    case "ml_lend_model":
                        report = new SLMLL_711();

                      dt = new DataTable();


                        ml_lend_model(dt, report);

                        LoadCBNReturnsReport(dt, report, id);

                        result = "success";
                        
                        break;

                    case "sch_int_rates":
                        report = new ITR764();

                        dt = new DataTable();

                        sch_int_rates(dt, report);

                        LoadCBNReturnsReport(dt, report, id);

                        result = "success";
                        break;


                    case "pnlacct_montly":
                        report = new pnl1000();

                        dt = new DataTable();

                        pnlacct_montly(dt, report);

                        LoadCBNReturnsReport(dt, report, id);

                        result = "success";
                        break;

                    case "mem_items":
                        report = new mem001();

                        dt = new DataTable();

                        mem_items(dt, report);

                        LoadCBNReturnsReport(dt, report, id);

                        result = "success";
                        break;

                }

                

                return result;
            }
            catch(Exception d)
            {
                return "failed";
            }

        
        }




        private void pnlacct_montly(DataTable dt, ReportDocument report)
        {
            dt.AddTableColumns(new string[] { "int_income", "less_int_expense", "commision", "Fees/Charges", "inc_from_inv", "oth_inc_from_non_fin",
           "Staff_cost","Directors","Depreciation","Prov_For_Bad_Debts","Overheads","Bad_Debts_Written_Off","Penalties_Paid","eoi_items","tax_on_eoi","Less_Provision_For_Taxation" });
            

            var row = dt.NewRow();

            row["int_income"] = 1342332.23;
            row["less_int_expense"] = 42312.23;
            row["commision"] = 3213231.32;
            row["Fees/Charges"] = 3213231.32;
            row["inc_from_inv"] = 3213231.32;
            row["oth_inc_from_non_fin"] = 3213231.32;
            row["Staff_cost"] = 3213231.32;
            row["Directors"] = 3213231.32;
            row["Depreciation"] = 3213231.32;
            row["Prov_For_Bad_Debts"] = 3213231.32;
            row["Overheads"] = 3213231.32;
            row["Bad_Debts_Written_Off"] = 3213231.32;
            row["Penalties_Paid"] = 2433423.24;
            row["eoi_items"] = 33.23;
            row["tax_on_eoi"] = 3433.23;
            row["Less_Provision_For_Taxation"] = 3433.23;


            dt.Rows.Add(row);
        }

        private void mem_items(DataTable dt, ReportDocument report)
        {
            dt.AddTableColumns(new string[] { "int_income", "less_int_expense", "commision", "Fees/Charges", "inc_from_inv", "oth_inc_from_non_fin",
           "Staff_cost","Directors","Depreciation","Prov_For_Bad_Debts","Overheads","Bad_Debts_Written_Off","Penalties_Paid","eoi_items","tax_on_eoi","Less_Provision_For_Taxation" });


            var row = dt.NewRow();

            row["int_income"] = 1342332.23;
            row["less_int_expense"] = 42312.23;
            row["commision"] = 3213231.32;
            row["Fees/Charges"] = 3213231.32;
            row["inc_from_inv"] = 3213231.32;
            row["oth_inc_from_non_fin"] = 3213231.32;
            row["Staff_cost"] = 3213231.32;
            row["Directors"] = 3213231.32;
            row["Depreciation"] = 3213231.32;
            row["Prov_For_Bad_Debts"] = 3213231.32;
            row["Overheads"] = 3213231.32;
            row["Bad_Debts_Written_Off"] = 3213231.32;
            row["Penalties_Paid"] = 2433423.24;
            row["eoi_items"] = 33.23;
            row["tax_on_eoi"] = 3433.23;
            row["Less_Provision_For_Taxation"] = 3433.23;


            dt.Rows.Add(row);
        }

        private void sch_int_rates(DataTable dt, ReportDocument report)
        {

            dt.AddTableColumns(new string[] { "TYPE_OF_ACCOUNT","0-30", "31-60", "61-90","91-180","181-360","OVER360" });
            
          

            var sql = "select distinct a.\"CONDITION_CODE\", a.\"DESCRIPTION\" as Type_of_Account,b.\"CR_INT_RATE\" as interest_rate, b.\"GROUP_CY_DATE\", TRIM(both 'N D' from(substring(b.\"GROUP_CY_DATE\" FROM 5 FOR 9))) as effective_date from \"ACCT_GEN_CONDITION\" a join \"GROUP_CREDIT_INT\" b on \n" +
                     "(a.\"CONDITION_CODE\"=TRIM(both 'N' from substring(b.\"GROUP_CY_DATE\",'..')))";

            var db = new PostgreSQLDBInterface(new Settings().sourcedb);

            var data = db.getData(sql);

            data.AddTableColumns(new string[] {"AgeGroup"});

            foreach(DataRow row in data.Rows)
            {
                var datestr = row["effective_date"].ToString();

                var efdate = DateTime.ParseExact(datestr,"yyyyMMdd", new CultureInfo("en-US"));

                row["AgeGroup"] = efdate.getAgeGroup();

                
            }


            var grps = data.AsEnumerable().GroupBy(g => g["Type_of_Account"]).Select(grp => grp.ToList());

            foreach(var grp in grps)
            {
                var row = dt.NewRow();

                row["TYPE_OF_ACCOUNT"] = grp[0]["Type_of_Account"].ToString();

                foreach(DataRow grprow in grp)
                {
                    row[grprow["AgeGroup"].ToString()] = grprow["interest_rate"].ToString() + "%";
                }

                dt.Rows.Add(row);
            }


        }

        private void ml_lend_model(DataTable dt, ReportDocument report)
        {

            var myDataColumn = new DataColumn
            {
                DataType = Type.GetType("System.String"),
                ColumnName = "S/N"
            };

            dt.Columns.Add(myDataColumn);

            myDataColumn = new DataColumn
            {
                DataType = Type.GetType("System.String"),
                ColumnName = "Lending_Model"
            };

            dt.Columns.Add(myDataColumn);

            myDataColumn = new DataColumn
            {
                DataType = typeof(Int32),
                ColumnName = "Number"
            };

            dt.Columns.Add(myDataColumn);

            myDataColumn = new DataColumn
            {
                DataType = Type.GetType("System.Decimal"),
                ColumnName = "Amount_N'000"
            };

            dt.Columns.Add(myDataColumn);


            myDataColumn = new DataColumn
            {
                DataType = Type.GetType("System.Decimal"),
                ColumnName = "%"
            };

            dt.Columns.Add(myDataColumn);


            var sql = "SELECT  b.\"DESCRIPTION\" as Lending_Model, CAST(COALESCE(NULLIF(a.\"Amount\",''),'0') as decimal) as \"Amount\" FROM \"AA_LOANS_IL\" a inner join \"CATEGORY\" b on (a.\"Category\"=b.\"@ID\");";

            var db = new PostgreSQLDBInterface(new Settings().sourcedb);

            var data = db.getData(sql);

     
            var total = data.AsEnumerable().Sum(t => t["Amount"].toDecimal());

            var sectgroup = data.AsEnumerable().GroupBy(r => r["Lending_Model"]).Select(grp => grp.ToList());
            int i = 1;

            foreach (var grp in sectgroup)
            {
                var row = dt.NewRow();
                row["S/N"] = i++;
                row["Lending_Model"] = grp[0]["Lending_Model"].ToString();
                row["Number"] = grp.Count;
                var amount = grp.Sum(t => t["Amount"].toDecimal());
                row["Amount_N'000"] = amount;
                row["%"] = (amount / total) * 100;

                dt.Rows.Add(row);
            }
        }

        private void sec_analysis(DataTable dt, ReportDocument report)
        {
          
            var myDataColumn = new DataColumn
            {
                DataType = Type.GetType("System.String"),
                ColumnName = "SECTOR"
            };

            dt.Columns.Add(myDataColumn);

            myDataColumn = new DataColumn
            {
                DataType = typeof(Int32),
                ColumnName = "NUMBER_OF_LOANS"
            };

            dt.Columns.Add(myDataColumn);

            myDataColumn = new DataColumn
            {
                DataType = Type.GetType("System.Decimal"),
                ColumnName = "AMOUNT"
            };

            dt.Columns.Add(myDataColumn);

            myDataColumn = new DataColumn
            {
                DataType = Type.GetType("System.Decimal"),
                ColumnName = "PERCENTAGE"
            };

            dt.Columns.Add(myDataColumn);

            var sql = "select a.*,b.* from \"AA_LOANS_IL\" a inner join  \"SECTOR\" b on (a.\"Sector\"=b.\"@ID\")";

            var db = new PostgreSQLDBInterface(new Settings().sourcedb);

            var data = db.getData(sql);

            //dt.Columns.Add(myDataColumn);

            myDataColumn = new DataColumn
            {
                DataType = Type.GetType("System.Decimal"),
                ColumnName = "CONVERTED_AMOUNT"
            };

            data.Columns.Add(myDataColumn);


            data = data.AsEnumerable().Select(t =>
            {
                t["CONVERTED_AMOUNT"] = t["Amount"].ToString().isNull()?0: t["Amount"].toDecimal();

                return t;
            }).CopyToDataTable();

            var total = data.AsEnumerable().Sum(t => t["CONVERTED_AMOUNT"].toDecimal());

            var sectgroup = data.AsEnumerable().GroupBy(r => r["SECTOR"]).Select(grp => grp.ToList());

            foreach (var grp in sectgroup)
            {
                var row = dt.NewRow();

                row["SECTOR"] = grp[0]["SHORT_NAME"].ToString();
                row["NUMBER_OF_LOANS"] = grp.Count;
                var amount = grp.Sum(t => t["CONVERTED_AMOUNT"].toDecimal());
                row["AMOUNT"] = amount;
                row["PERCENTAGE"] = (amount / total)*100;

                dt.Rows.Add(row);
            }
        }





        private void LoadCBNReturnsReport(DataTable dt, ReportDocument report, string id)
        {

            report.SetDataSource(dt);

            var reportparams = new List<ValuePair>();

            var p = new ValuePair();
            p.ID = "mfbCode"; p.Value = "50629";
            reportparams.Add(p);

            p = new ValuePair();
            p.ID = "mfbName"; p.Value = "NPF MICROFINANCE BANK PLC";
            reportparams.Add(p);

            p = new ValuePair();
            p.ID = "returnCode"; p.Value = "MMFBR M00";
            reportparams.Add(p);

            p = new ValuePair();
            p.ID = "stCode"; p.Value = "MMFBR M00";
            reportparams.Add(p);

            p = new ValuePair();
            p.ID = "stName"; p.Value = "LAGOS";
            reportparams.Add(p);

            p = new ValuePair();
            p.ID = "lgCode"; p.Value = "ETI-OSA";
            reportparams.Add(p);

            p = new ValuePair();
            p.ID = "lgName"; p.Value = "LAGOS ISLAND";
            reportparams.Add(p);

            var reportobject = new InlaksBIContext().Resources.FirstOrDefault(r => r.value == id);

            var title = reportobject.isNull() ? "" : reportobject.ResourceName;

            p = new ValuePair();
            p.ID = "returnName"; p.Value = title;
            reportparams.Add(p);







            Session["reportparams"] = reportparams;
            Session["report"] = report;



           
          
        }


        [HttpPost]
        public string LaunchExcel()
        {
            try
            {

                var startdate = Request.Form["startdate"];
                var enddate = Request.Form["endate"];
                var reportname = Request.Form["reportname"];
                var path = HostingEnvironment.MapPath("~/exceldownloads/");
                switch (reportname)
                {
                    case "failed items":

                       Session["file"] = generateTestExcel(startdate, enddate,downpath,reportname);

                        break;

                  
                }

                return "Success";
            }
            catch (Exception d)
            {
                return d.Message;
            }

        }



        string generateTestExcel(string startdate, string enddate, string downpath, string reportname)
        {
            var sql = "";

           // var db = new 

            downpath = downpath + startdate + "-" + enddate + reportname+".xlsx";


            ExcelWorksheet oSheet;
            ExcelPackage xlPackage;


            FileInfo newFile = new FileInfo(downpath);

            if (newFile.Exists)
            {
                newFile.Delete();
            }
            xlPackage = new ExcelPackage(newFile);


            oSheet = xlPackage.Workbook.Worksheets.Add("Finacle Reports");

            int count = 0; int curRow = 0; int curRow2 = 0;


            oSheet.Cells[1, 1].Value = "Type of Report";
            oSheet.Cells[1, 1].Style.Font.Bold = true;
            oSheet.Cells[1, 1].Style.Font.UnderLine = true;
            //oSheet.Cells[curRow - 3, 1, curRow - 3, 13].Merge = true;

            return " ";
        }






        public void DownloadCurrentReportFile()
        {
            try
            {
                string path = (string)Session["file"];
                var fileInfo = new FileInfo(path);


                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats";
                Response.AddHeader("Content-Disposition",
                                   "attachment; filename=" + fileInfo.Name);
                Response.WriteFile(fileInfo.FullName);
                Response.Flush();
            }
            catch (Exception d)
            {
                throw (d);
            }
        }


    }



}


    
