using BackBone;
using CrystalDecisions.CrystalReports.Engine;
using InlaksIB.Classes;
using InlaksIB.Properties;
using InlaksIB.Reports;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
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
        private  static string Message = "";
        private static string errorClass = "";

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


          //Request.QueryString.Get("id");
            var report = new InlaksBIContext().Resources.FirstOrDefault(r => r.ResourceID.ToString() == id);
       //  id =  report.ResourceID.ToString();

            var reportid = report.value;

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
                        //var newmodule = dbcontext.Modules.Create();
                        //newmodule.ModuleName = module.ModuleName;
                        //newmodule.Industry = module.Industry;
                        dbcontext.Modules.Add(module);
                        dbcontext.SaveChanges();
                        errorClass = "green";
                        Message = "Module Created successfully";
                        break;
                    case "edit":
                        var old = dbcontext.Modules.FirstOrDefault(t => t.ModuleID == id);
                        old.ModuleName = module.ModuleName;
                        old.value = module.value;
                        old.Industry = module.Industry;
                        dbcontext.SaveChanges();
                        errorClass = "green";
                        Message = "Module modified successfully";
                        break;

                    case "delete":
                        var item = dbcontext.Modules.FirstOrDefault(t => t.ModuleID == id);
                        dbcontext.Modules.Remove(item);

                        dbcontext.SaveChanges();
                        Message = "Module deleted successfully";
                        errorClass = "green";
                        break;
                }
            }
            catch (Exception e)
            {
                
                errorClass = "red";
                Message = "Operation failed. Please seek technical assistance";
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
                        sch.MaxDate = DateTime.ParseExact(sch.MaxDate, "dd MMM yyyy", new CultureInfo("en-US")).ToString("dd-MM-yyyy");
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
            var user = new InlaksBIContext().Users.FirstOrDefault(u => u.UserID == id);
            if (user == null)
            {
                user = new User() { errorclass = errorClass, Message = Message };
            }
            switch (mode)
            {
                case "create":
                    ViewBag.mode = "create";
                    return View("CreateUser", user);

                case "edit":

                    ViewBag.mode = "edit";
                    return View("CreateUser", user);

                default:
                    ViewBag.mode = "create";
                    return View("CreateUser", user);
            }

        }

        public ActionResult ModuleSetup(int id, string mode)
        {
            ViewBag.errorclass = errorClass; ViewBag.message = Message;
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
                        if (re.Url.isNull())
                        {
                            re.Url = "";
                        }
                        dbcontext.Resources.Add(re);
                        dbcontext.SaveChanges();
                        break;
                    case "edit":
                        var old = dbcontext.Resources.FirstOrDefault(t => t.ResourceID == id);
                        old.Module = dbcontext.Modules.FirstOrDefault(m => m.ModuleID == resource.ModuleID);
                        old.ResourceName = resource.ResourceName;
                       // old.Url = resource.Url;
                        old.value = resource.value;
                       
                        //dbcontext.Resources.Remove(old);
                        //dbcontext.Resources.Add(resource);
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
            
            errorClass = "green";
            var context = new InlaksBIContext();
            switch (mode)
            {
                case "create":
                    try
                    {


                        var role = context.Roles.FirstOrDefault(r => r.RoleID == user.RoleID);

                        IPasswordHasher hash = new BasicHash();
                        var newuser = context.Users.Create();
                        newuser.Password = hash.HashPassword(user.Password);
                        newuser.RePassword = (user.Password);
                        newuser.UserRole = role;
                        newuser.Email = user.Email;
                        newuser.Branch = user.Branch;
                        newuser.LeadCompany = user.LeadCompany;
                        newuser.UserID = user.UserID;
                        newuser.Name = user.Name;
                        newuser.LastLogin = DateTime.Now;
                        newuser.RePassword = newuser.Password;
                        newuser.RoleID = user.RoleID;
                        newuser.DefaultPassword = true;
                        context.Users.Add(newuser);

                        context.SaveChanges();

                        Message = "User Created Successfully";
                        errorClass = "green";

                    }
                    catch (DbEntityValidationException e)
                    {
                        foreach (var eve in e.EntityValidationErrors)
                        {
                           Utils.Log(string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                                eve.Entry.Entity.GetType().Name, eve.Entry.State));
                            foreach (var ve in eve.ValidationErrors)
                            {
                                 Utils.Log(string.Format("- Property: \"{0}\", Error: \"{1}\"",
                                    ve.PropertyName, ve.ErrorMessage));
                            }
                        }
                      
                    
                    Message = "Failed to create user. Please seek technical assistance";
                        errorClass = "red";
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
                        olduser.Branch = user.Branch;
                        olduser.LeadCompany = user.LeadCompany;
                        olduser.UserRole = role;


                        context.SaveChanges();

                        Message = "User Modified Successfully";

                    }
                    catch (Exception e)
                    {
                        Message = "Failed to Modify user. Please seek technical assistance";
                        errorClass = "red";
                    }
                    break;
            }

         


            return RedirectToAction ("UserSetup",new { id=user.UserID, mode="create"});
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

                case "branchlist":
                    var branches = new InlaksBIContext().Comapanies.Where(c => c.leadcompcode == param.Trim()).ToList();

                    foreach (var br in branches)
                    {
                        ValuePair pair = new ValuePair();
                        pair.ID = br.CompanyCode;
                        pair.Value = br.CompanyName;
                        pairs.Add(pair);
                    }

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

                var dbcontext = new InlaksBIContext();

                var dataset = dbcontext.DataSets.Create();

                WarehouseInterface warehouse = dbcontext.getWarehouse(new Settings().warehousedbtype);


                DBInterface db = dbcontext.getDBInterface(new Settings().warehousedbtype, new Settings().warehousedb);


                dataset.DataSetName = datasetobject[0].DataSetName;

                dataset.Script = warehouse.getDatasetBuilder(datasetobject);
             

                int resp = db.Execute(dataset.Script);

                var dt = warehouse.testobject(dataset.DataSetName);

                dataset.Module = dbcontext.Modules.Where(m => m.ModuleID == moduleid).FirstOrDefault();

                dbcontext.DataSets.Add(dataset);

                dbcontext.SaveChanges();

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

                        

                        LoadCBNReturnsReport(dt, report,id, "Form MMFBR 762");
                        result = "success";
                        break;

                    case "due_bal":
                        report = new due_bal221();

                         sql = "select distinct '' as \"Code\", \"ACCOUNT_TITLE_1\" as \"NAME_OF_BANK\", CAST(COALESCE(NULLIF(\"WORKING_BALANCE\",''),'0') as decimal) as \"Amount\"  from \"ACCOUNT\" where \"CATEGORY\" >= '5000' and \"CATEGORY\" <='5999'";

                         db = new PostgreSQLDBInterface(new Settings().sourcedb);

                        dt = db.getData(sql);

                        LoadCBNReturnsReport(dt, report,id,"");
                        result = "success";
                        break;

                    case "sch_inshd_dep":
                        report = new SID_202();

                        dt = new DataTable();


                        sch_inshd_dep(dt, report);

                        LoadCBNReturnsReport(dt, report, id, "Form MMFBR 202");

                        result = "success";

                        break;



                    case "ml_lend_model":
                        report = new SLMLL_711();

                      dt = new DataTable();


                        ml_lend_model(dt, report);

                        LoadCBNReturnsReport(dt, report, id,"");

                        result = "success";
                        
                        break;

                    case "sch_int_rates":
                        report = new ITR764();

                        dt = new DataTable();

                        sch_int_rates(dt, report);

                        LoadCBNReturnsReport(dt, report, id,"");

                        result = "success";
                        break;


                    case "pnlacct_montly":
                        report = new pnl1000();

                        dt = new DataTable();

                        pnlacct_montly(dt, report);

                        LoadCBNReturnsReport(dt, report, id,"");

                        result = "success";
                        break;

                    case "mem_items":
                        report = new mem001();

                        dt = new DataTable();

                        mem_items(dt, report);

                        LoadCBNReturnsReport(dt, report, id,"");

                        result = "success";
                        break;

                    case "sch_oth_liab":
                        report = new SOL501_();

                        dt = new DataTable();

                        sch_oth_liab(dt, report);

                        LoadCBNReturnsReport(dt, report, id, "Form MMFBR 501");

                        result = "success";
                        break;

                    case "sch_othr_res":
                        report = new SOL501_();

                        dt = new DataTable();

                        sch_othr_res(dt, report);

                        LoadCBNReturnsReport(dt, report, id, "Form MMFBR 951");

                        result = "success";
                        break;

                        

                    case "sch_borr_agnc":
                        report = new SBOA_651();

                        dt = new DataTable();

                        sch_borr_agnc(dt, report);

                        LoadCBNReturnsReport(dt, report, id, "Form MMFBR 651");

                        result = "success";
                        break;

                    case "sch_bal_other_banks":
                        report = new SBDBN_221();

                        dt = new DataTable();

                        sch_bal_other_banks(dt, report);

                        LoadCBNReturnsReport(dt, report, id, "Form MMFBR 221");

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


        private void sch_bal_other_banks(DataTable dt, ReportDocument report)
        {

            dt.AddTableColumns(new string[] { "Bank_Code", "Name_Of_Bank", "Amount_N'000" });




            var sql = " select a.*,b.FirstName from DimAccount a inner join DimCustomer b on (a.Customer=b.CustomerId) where Category in (select CategoryId from factCategory where CategoryName like '%nostro%')";

            var db = new InlaksBIContext().getDBInterface(new Settings().warehousedbtype, new Settings().warehousedb);


            var data = db.getData(sql);

            var dgroup = data.AsEnumerable().GroupBy(d => d["AccountTitle"]).Select(grp => grp.ToList());

            //int count = 1;

            foreach (var grp in dgroup)
            {
                var row = dt.NewRow();

                row["Name_Of_Bank"] = grp[0]["FirstName"];
                row["Amount_N'000"] = grp.Sum(g => g["BookValue"].toDecimal());

                dt.Rows.Add(row);
            }


        }


        private void sch_inshd_dep(DataTable dt, ReportDocument report)
        {

            dt.AddTableColumns(new string[] { "S/N", "Type_Of_Deposits", "N1_-_N100,000", "N100,001_&_Above", "Total_N'000" });




            var sql = " select a.*,b.FirstName from DimAccount a inner join DimCustomer b on (a.Customer=b.CustomerId) where Category in (select CategoryId from factCategory where CategoryName like '%nostro%')";

            var db = new InlaksBIContext().getDBInterface(new Settings().warehousedbtype, new Settings().warehousedb);


            var data = db.getData(sql);

            var dgroup = data.AsEnumerable().GroupBy(d => d["AccountTitle"]).Select(grp => grp.ToList());

            //int count = 1;

            foreach (var grp in dgroup)
            {
                var row = dt.NewRow();

                row["Name_Of_Bank"] = grp[0]["FirstName"];
                row["Amount_N'000"] = grp.Sum(g => g["BookValue"].toDecimal());

                dt.Rows.Add(row);
            }


        }


        


               private void sch_othr_res(DataTable dt, ReportDocument report)
        {

            dt.AddTableColumns(new string[] { "S/N", "Item_Description", "Amount_N'000" });




            var sql = "select* from DimAccount where Category in ('18009')";

            var db = new InlaksBIContext().getDBInterface(new Settings().warehousedbtype, new Settings().warehousedb);


            var data = db.getData(sql);

            var dgroup = data.AsEnumerable().GroupBy(d => d["AccountTitle"]).Select(grp => grp.ToList());

            int count = 1;

            foreach (var grp in dgroup)
            {
                var row = dt.NewRow();
                row["S/N"] = count++;
                row["Item_Description"] = grp[0]["AccountTitle"];
                row["Amount_N'000"] = grp.Sum(g => g["BookValue"].toDecimal());

                dt.Rows.Add(row);
            }


        }


        private void sch_borr_agnc(DataTable dt, ReportDocument report)
        {

            dt.AddTableColumns(new string[] { "S/N", "Name_Of_Lending_Institution", "Country", "Date_Facility_Granted","Tenor", "Amount_Granted_N'000" });

  
    

            var sql = "select a.LINE_DESCRIPTION, b.GLAmount as Amount from LINE a inner join FactGL b on (a.LINE_ID=b.GLId) where LINE_DESCRIPTION  like '%CONCESSIONARY LOAN%'";

            var db = new InlaksBIContext().getDBInterface(new Settings().warehousedbtype, new Settings().warehousedb);


            var data = db.getData(sql);

            var dgroup = data.AsEnumerable().GroupBy(d => d["LINE_DESCRIPTION"]).Select(grp => grp.ToList());

            int count = 1;

            foreach(var grp in dgroup)
            {
                var row = dt.NewRow();

                row["Name_Of_Lending_Institution"] = grp[0]["LINE_DESCRIPTION"];
                row["Country"] = "Nigeria";
                row["S/N"] = count++;
                row["Amount_Granted_N'000"] = grp.Sum(g => g["Amount"].toDecimal());

                dt.Rows.Add(row);
            }
          

        }






        private void sch_oth_liab(DataTable dt, ReportDocument report)
        {

            dt.AddTableColumns(new string[] { "S/N", "Item_Description", "Amount_N'000" });

            var lines = new string[] {"Accounts Payable",
            "Unearned Income",
            "Interest Accrued not Paid",
            "Uncleared Effects / Transit items",
            "Un - audited Profit to Date",
            "Provision for Dimunition in the value of Investment",
            "Provision for Losses on Off Balance Sheet Items",
            "Interest -in-Suspense",
            "Provision for Taxation",
            "Provision for Other Loan Losses",
            "Dividend Payable",
            "Suspense Account",
            "Deposits for Shares(Provide Breakdown)",
            "Miscellaneous(Provide Breakdown)"
};

            int count = 1;

            foreach(var line in lines){
                var row = dt.NewRow();
                row["S/N"] = count++;
                row["Item_Description"] = line;

                dt.Rows.Add(row);
               
            }

            var sql = "select sum(glamount)from FactGL where GLId in  (select LINE_ID FROM[inlaksbiwarehouse].[dbo].[LINE] where LINE_ID like '%NAMBTBGL%' and LINE_DESCRIPTION like '%payable%' and type = 'detail' and APPID = 'AC')";

            var db = new InlaksBIContext().getDBInterface(new Settings().warehousedbtype,new Settings().warehousedb);



            dt.Rows[Array.IndexOf(lines,"Accounts Payable")][2] = db.ExecuteScalar(sql).toDecimal();

            sql = "(select sum(GLAmount) as Total from FactGL where GLId in  (select LINE_ID FROM [inlaksbiwarehouse].[dbo].[LINE] where (LINE_DESCRIPTION like '%deposits%' or LINE_DESCRIPTION like '%deposit%')  and (type = 'detail' and LINE_ID like '%NAMBTBGL%' and APPID = 'AC') ))";


            dt.Rows[Array.IndexOf(lines, "Miscellaneous(Provide Breakdown)")][2] = db.ExecuteScalar(sql).toDecimal();

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

            var sql = "select a.CustomerID,b.CustomerSector,c.SectorName as SECTOR, d.BookValue as Amount from factArrangement a inner join DimCustomer b on (a.CustomerID=b.CustomerId) inner join factSector c on (b.CustomerSector= c.SectorId) inner join DimAccount d on (a.LinkedApplId = d.AccountId)   where a.ProductLine = 'lending'";

            var db = new InlaksBIContext().getDBInterface(new Settings().warehousedbtype,new Settings().warehousedb);

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

                row["SECTOR"] = grp[0]["SECTOR"].ToString();
                row["NUMBER_OF_LOANS"] = grp.Count;
                var amount = grp.Sum(t => t["CONVERTED_AMOUNT"].toDecimal());
                row["AMOUNT"] = amount;
                row["PERCENTAGE"] = (amount / total)*100;

                dt.Rows.Add(row);
            }
        }


       


        private void LoadCBNReturnsReport(DataTable dt, ReportDocument report, string id, string returncode)
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
            p.ID = "returnCode"; p.Value = returncode;
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
                var excelreports = new ExcelReports();
                var startdate = Request.Form["startdate"];
                var enddate = Request.Form["endate"];
                var reportname = Request.Form["reportname"];
                var downpath = HostingEnvironment.MapPath("~/exceldownloads/");
                switch (reportname.Trim())
                {
                    case "Anti Money Laundering Report":

                       Session["file"] = excelreports.generateNPFAntiMoneyLaunderingReport(startdate, enddate,downpath,reportname);

                        break;

                    case "Savings Accounts Balance Report":

                        Session["file"] = excelreports.generateNPFSavingsAccountsBalanceReport(downpath, reportname);
                        break;



                }

                return "Success";
            }
            catch (Exception d)
            {
                return d.Message;
            }

        }






        [HttpGet]
        public void DownloadCurrentReportFile()
        {
            string path = (string)Session["file"];
            var fileInfo = new FileInfo(path);
            try
            {
                //string path = (string)Session["file"];
                //var fileInfo = new FileInfo(path);


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
            finally
            {
                try
                {
                    fileInfo.Delete();
                }
                catch
                {

                }
            }
        }


    }



}


    
