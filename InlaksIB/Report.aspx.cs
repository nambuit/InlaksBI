using BackBone;
using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace InlaksIB
{
    public partial class Report : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            showReport();
        }

        private void showReport()
        {

            ReportDocument crT = new ReportDocument();

            if (Request.QueryString["id"] != null)
                crT = (ReportDocument)Session[Request.QueryString["id"]];
            else
                crT = (ReportDocument)Session["report"];
           
            CrystalReportViewer1.ReportSource = crT;

            var reportparams = (List<ValuePair>)Session["reportparams"];

            foreach(var p in reportparams)
            {
                crT.SetParameterValue(p.ID, p.Value);
            }

        }

        protected void crVwTeller_Navigate(object source, CrystalDecisions.Web.NavigateEventArgs e)
        {
            showReport();
        }
    }
}