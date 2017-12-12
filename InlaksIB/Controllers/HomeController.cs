using BackBone;
using InlaksIB.Properties;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InlaksIB.Controllers
{
    public class HomeController : Controller
    {

        override
            protected void OnActionExecuting(ActionExecutingContext context)
        {
            var user = (User)context.HttpContext.Session["User"];

            if(user ==null||((string)context.HttpContext.Session["LoggedIn"]) == "False"){

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



            switch (reportid.Trim().ToLower())
            {
                default:
                    return RedirectToAction("ProcessStandard", "Report", new { id = id});
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

                    return View("RoleSetup", new InlaksBIContext().Roles.Where(r=>r.RoleID>1));

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
            try { 
            var dbcontext = new InlaksBIContext();
            module.Industry = dbcontext.Industries.FirstOrDefault(i => i.IndustryID == module.IndustryID);
                module.IconClass = "glyphicon glyphicon-list";
            switch (mode)
            {
                case "create":
                        module.ModuleID = id;
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
            catch(Exception e)
            {
                ViewBag.errorclass = "red";
                ViewBag.message = "Operation failed. Please seek technical assistance";
            }

            return RedirectToAction("ModuleSetup", new { id=0,mode="create" });
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
                        resource.Module = dbcontext.Modules.FirstOrDefault(m => m.ModuleID == resource.ModuleID);
                        dbcontext.Resources.Add(resource);
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
            catch(Exception d)
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

                ViewBag.message = auth.AuthType+" Authentication Mode Activated Successfully";
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

            return View("ChangePassword",new PasswordChange());

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
           var  rolesresources= new InlaksBIContext().RolesResources.ToList();

           var  selected = rolesresources.Where(r => r.Role.RoleID == id).ToList();

            var others = new InlaksBIContext().Resources.ToList();

            var resourcelist = new List<RoleResourceList>();

            others.ForEach(re => {
                resourcelist.Add(new RoleResourceList() { ResourceID=re.ResourceID,RoleID=id,ResourceName=re.ResourceName, selected="" });
            });

            selected.ForEach(r =>
            {
                var re = resourcelist.FirstOrDefault(t => t.ResourceID == r.Resource.ResourceID);
                if (re.ResourceName!=null)
                {
                    resourcelist.Remove(re);
                    re.selected = "selected";
                    resourcelist.Add(re);
                }
        
                
            });



            return JsonConvert.SerializeObject(resourcelist);
        }


        public ActionResult ProcessRoleResources()
        {
            
            var currole = Request.Form["role"];

            if(currole == null)
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
            catch(Exception e)
            {
                ViewBag.Message = "Failed to Assign Resource(s); Please seek technical assistance";

                ViewBag.errorclass = "red";
            }



            return  View("RolesResources");
        }

        public ActionResult ProcessPassword(PasswordChange change)
        {
           
            var message = "";
            var errorclass = "green";
            var context = new InlaksBIContext();
            try
            {
                if(change.Password == change.NewPassword)
                {
                    goto End;
                }
              
                var user = (User)Session["User"];
                var hasher = new BasicHash();
                var olduser = context.Users.FirstOrDefault(u => u.UserID == user.UserID);
                if (!hasher.VerifyHashedPassword(olduser.Password,change.Password))
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
        public  string getValuePair(string id, string param)
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
                  
                    foreach(var re in res)
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

                    pairs = warehouse.getFilterColumns(param);
                    return JsonConvert.SerializeObject(pairs);


                default:
                    return "";
                    
                    
            }
        }

        [HttpGet]
        public string getFilterColumns(string id)
        {
            WarehouseInterface warehouse = new InlaksBIContext().getWarehouse(new Settings().warehousedbtype);
            var pairs = warehouse.getFilterColumns(id);
            var filters = new List<DataSetFilter>();

            foreach(var pair in pairs)
            {
                var filter = new DataSetFilter();
                filter.ColumnName = pair.ID;
                filter.ColumnValue = pair.Value;
                filter.IsIncluded = false;
                filters.Add(filter);
            }


            return JsonConvert.SerializeObject(filters);

            }
        }


    }


