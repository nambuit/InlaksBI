
using BackBone;
using InlaksIB.Properties;
using System;
using System.Collections.Generic;
using System.Data;
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

            try
            {
                Context.Session["warehousedbconstr"] = new Settings().warehousedb;
                Context.Session["dbtype"] = new Settings().warehousedbtype;
                using (var db = new InlaksBIContext())
                {

                    bool newdb = !db.Database.Exists();
                    db.Database.CreateIfNotExists();


                    if (newdb)
                    {
                        db.PopulateDefaultData();
                    }

                    var dbinterface = new SQLServerDBInterfac(new Settings().warehousedb);

                    var dt = dbinterface.getData("Select * from DimBranch");

                    dbinterface = new SQLServerDBInterfac(new Settings().DBConstr);

                    var destdt = dbinterface.getData("Select TOP 1 * from Companies");

                    destdt = destdt.Clone();

                    foreach (DataRow row in dt.Rows)
                    {
                        var destrow = destdt.NewRow();

                        destrow["CompanyCode"] = row["SourceBranchId"];
                        destrow["branchcode"] = row["BranchNum"];
                        destrow["leadcompcode"] = row["LeadCompany"];
                        destrow["CompanyName"] = row["BranchName"];

                        destdt.Rows.Add(destrow);


                    }

                    dbinterface.CopyDataTableToDB(destdt, "TempCompanies");

                    MergeDetails details = new MergeDetails();

                    details.destTb = "Companies";
                    details.sourceTb = "TempCompanies";
                    details.sourcereference = "CompanyCode";
                    details.destreference = "CompanyCode";
                    details.destDB = "inlaksbi";
                    details.sourceDB = "inlaksbi";
                    dbinterface.MergeData(details);

                    dbinterface.Execute("truncate table TempCompanies");

                }
            }
            catch (Exception d)
            {
                Utils.Log("Error in Application Start:" + d.Message);

            }

            
        }

        public static void ISUserLoggedIn(Controller controller)
        {
            string loggedin = (string)controller.Session["LoggedIn"];
            controller.Session["warehousedbtype"] = new Settings().warehousedbtype;
            if (string.IsNullOrEmpty(loggedin) || loggedin != "True")
            {
                throw new InvalidOperationException("User Session not Active");
            }
        }

        }


}
