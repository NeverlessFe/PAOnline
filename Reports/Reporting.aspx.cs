using CrystalDecisions.CrystalReports.Engine;
using Dapper;
using PAOnline.Scripts.LogicScript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PAOnline.Reports
{
    public partial class Reporting : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            DAL DAL = new DAL();
            CrystalReportViewer1.ToolPanelView = CrystalDecisions.Web.ToolPanelViewType.None;
            MemoReportNonTF crystalReport = new MemoReportNonTF();
            (crystalReport.ReportDefinition.ReportObjects["txtNoMemo"] as TextObject).Text = "YorStringValue";

            CrystalReportViewer1.ReportSource = crystalReport;
            Response.ClearContent();

            var dictionary = new Dictionary<string, object>
            {
                {"Option",2 },
                {"NoMemo", Request.QueryString.Get("NoMemo")}
            };
            var parameters = new DynamicParameters(dictionary);
            string Result = DAL.StoredProcedure(parameters, "STP_DOF");

            //List<User> UserList = JsonConvert.DeserializeObject<List<User>>(jsonString);

            //var data = new ReportDocument();
            //data.Load(Server.MapPath("~/Report/ClaimKehamilanReport.rpt"));
            ////data.SetDatabaseLogon("sab7", "Welcome123");
            //data.SetDataSource(dt);
            //data.VerifyDatabase();






            //CrystalReportViewer1.ReportSource = data;
            CrystalReportViewer1.PrintMode = CrystalDecisions.Web.PrintMode.ActiveX;
        }
    }
}