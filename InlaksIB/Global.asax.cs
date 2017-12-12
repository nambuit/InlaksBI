
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace InlaksIB
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            using (var db = new InlaksBIContext())
            {
               
                bool newdb = !db.Database.Exists();
                db.Database.CreateIfNotExists();
               

                if (newdb)
                {
                    db.PopulateDefaultData();
                }
             

            }
        }

        public static void ISUserLoggedIn(Controller controller)
        {
            string loggedin = (string)controller.Session["LoggedIn"];

            if (string.IsNullOrEmpty(loggedin) || loggedin != "True")
            {
                throw new InvalidOperationException("User Session not Active");
            }
        }

        }


}
