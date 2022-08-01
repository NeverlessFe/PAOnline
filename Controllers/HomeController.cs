using PAOnline.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Mvc;
using Dapper;
using Newtonsoft.Json;
using PAOnline.Scripts.LogicScript;
using System.IO;
using ZXing;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;
using Microsoft.Reporting.WebForms;
using System.Web.UI.WebControls;
using CrystalDecisions.CrystalReports.Engine;
using System.Web.Script.Serialization;

namespace PAOnline.Controllers
{
    public class HomeController : Controller
    {
        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = false)]
        public static extern bool LogonUser(string lpszUsername, string lpszDomain, string lpszPassword, int dwLogonType, int dwLogonProvider, ref IntPtr phToken);
        [DllImport("kernel32.dll")]
        public static extern int FormatMessage(int dwFlags, ref IntPtr lpSource, int dwMessageId, int dwLanguageId, ref string lpBuffer, int nSize, ref IntPtr Arguments);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern bool CloseHandle(IntPtr handle);
        readonly ConnectionStringSettings dbDFIS = ConfigurationManager.ConnectionStrings["ReserverDiscount"];
        DAL DAL = new DAL();
        DataTable dtClaimDetail = new DataTable();
        string Result;
        int flaggenerat=0;
        int flaggenerat2 = 0;

        #region View
        public ActionResult Index()
        {
            if (Session["UserName"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                ViewBag.Username = Session["UserName"];
                return View();
            }
        }
        public ActionResult Login()
        {
            return View();
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult Home()
        {
            if (Session["UserName"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                ViewBag.Username = Session["UserName"];
                return View();
            }
        }
        public ActionResult PADetailApprove(string NoPA)
        {
            if (Session["UserName"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                ViewBag.Username = Session["UserName"];
                ViewBag.NoPA = NoPA;
                return View();
            }
        }
        public ActionResult PAList()
        {
            //if (Session["UserName"] == null)
            //{
            //    return RedirectToAction("Login", "Home");
            //}
            //else
            //{
                ViewBag.Username = Session["UserName"];
                return View();
            //}
        }
        public ActionResult PAMonitoring()
        {
            if (Session["UserName"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                ViewBag.Username = Session["UserName"];
                return View();
            }
        }
        public ActionResult CreateClaim()
        {
            Session["DTDetailClaim"] = null;
            return View();
        }
        public ActionResult CreateClaimSPB()
        {
            return View();
        }
        public ActionResult CreateMemo()
        {
            return View();
        }
        public ActionResult CreatePA()
        {
            return View();
        }
        public ActionResult MonitoringMemo()
        {
            return View();
        }
        public ActionResult MonitoringClaim()
        {
            return View();
        }
        public ActionResult ClaimDetail()
        {
            return View();
        }
        public ActionResult ReportMemo()
        {

            return View();
        }
        public ActionResult CreateDOFDummy()
        {
            return View();
        }
        #endregion

        #region PopulateData
        public ActionResult GetDLL(ClaimAttribute model)
        {
            string Username = "";
            string Type = "";
            if(Session["Role"] != null)
            {
                Username = Session["UserName"].ToString();
                Type = Session["Role"].ToString();
            }
            else
            {
                Username = "";
            }
            var dictionary = new Dictionary<string, object>
            {
                {"Option",model.Option },
                {"RegionGet", model.Region },
                {"CabangGet", model.Cabang },
                {"Username", Username },
                {"TypeLogin", Type },
                {"ExpenseHeaderGet", model.HeaderExpense },
                {"ExpenseItemGet", model.ItemExpense},
                {"ShipIDGet", model.SiteUsedId },
                {"NoMemoGet", model.NoMemo}
            };
            var parameters = new DynamicParameters(dictionary);
            return Json(DAL.StoredProcedure(parameters, model.SP));
        }
        public string PopulateDataGeneral(CreateMemoAttribute Model)
        {
            var dictionary = new Dictionary<string, object>
            {
                {"Option",Model.Option },
                {"NoMemoGet", Model.NoMemo },
                {"KodeSubdistGet", Model.KodeSubdist },
                {"PeriodeGet", Model.Periode },
                {"ItemExpenseGet" , Model.ItemExpense}
            };
            var parameters = new DynamicParameters(dictionary);
            return DAL.StoredProcedure(parameters, Model.SP);
        }
        public class ListtoDataTableConverter
        {
            public DataTable ToDataTable<T>(List<T> items)

            {

                DataTable dataTable = new DataTable(typeof(T).Name);

                //Get all the properties

                PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

                foreach (PropertyInfo prop in Props)

                {

                    //Setting column names as Property names

                    dataTable.Columns.Add(prop.Name);

                }

                foreach (T item in items)

                {

                    var values = new object[Props.Length];

                    for (int i = 0; i < Props.Length; i++)

                    {

                        //inserting property values to datatable rows

                        values[i] = Props[i].GetValue(item, null);

                    }

                    dataTable.Rows.Add(values);

                }

                //put a breakpoint here and check datatable

                return dataTable;

            }
        }
        public ActionResult GenerateUsername()
        {
            List<string> ModelData = new List<string>();
            if (Session["UserName"] != null)
            {
                ModelData.Add(Session["UserName"].ToString());
            }
            else
            {
                ModelData.Add("Unvalid");
            }
            return Json(ModelData);

        }
        public ActionResult GetPAMinAmount(ClaimAttribute model)
        {
            var dictionary = new Dictionary<string, object>{
                { "Option",5 },
                { "NoMemoGet", model.NoMemo }
            };
            var ListAttribute = new DynamicParameters(dictionary);
            string Return = DAL.StoredProcedure(ListAttribute, "STP_CreatePA");
            return Json(Return);
        }

        public ActionResult GetDOFDetailAmount(ClaimAttribute model)
        {
            var dictionary = new Dictionary<string, object>{
                { "Option",2 },
                { "NoRegist", model.NoRegist },
                { "DetailNo", model.No }
            };
            var ListAttribute = new DynamicParameters(dictionary);
            string Return = DAL.StoredProcedure(ListAttribute, "STP_ClaimTransaction");
            return Json(Return);
        }
        #endregion

        #region ExecuteData
        public ActionResult DeleteClaimDetail(ClaimDetailDT Model)
        {
            string Amount = Model.Amount.Replace(",", "");
            if (Session["DTDetailClaim"] != null)
            {
                var list = Session["DTDetailClaim"] as List<ClaimDetailDT>;

                var itemToRemove = list.Single(r => r.Region == Model.Region && r.ItemExpense == Model.ItemExpense && r.NamaRek == Model.NamaRek && r.NoRek == Model.NoRek && r.Remarks == Model.Remarks);
                list.Remove(itemToRemove);

                Session["DTDetailClaim"] = list;
            }
            var JsonReturn = Session["DTDetailClaim"] as List<ClaimDetailDT>;
            return Json(JsonReturn);
        }
        public ActionResult InsertClaimDetail(ClaimDetailDT Model)
        {
            if (Session["DTDetailClaim"] != null)
            {
                var list = Session["DTDetailClaim"] as List<ClaimDetailDT>;
                list.Add(Model);

                Session["DTDetailClaim"] = list;
            }
            else
            {
                Dictionary<string, object> row;
                List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                List<ClaimDetailDT> newList = new List<ClaimDetailDT>();
                newList.Add(Model);

                Session["DTDetailClaim"] = newList;
            }
            var JsonReturn = Session["DTDetailClaim"] as List<ClaimDetailDT>;
            return Json(JsonReturn);
        }
        public ActionResult CreateMemoExec(CreateMemoAttribute Model)
        {
            ListtoDataTableConverter converter = new ListtoDataTableConverter();
            int Counter = Model.NoClaimList.Count;
            int Trav = 0;
            DataTable DetailNoClaimList = new DataTable();
            DetailNoClaimList.Columns.Add("NoClaim");
            for (Trav = 0; Trav < Counter; Trav++)
            {
                DataRow dr = DetailNoClaimList.NewRow();
                dr["NoClaim"] = Model.NoClaimList[Trav];
                DetailNoClaimList.Rows.Add(dr);
            }
            var list = new ClaimDetailDT();
            var dictionary = new Dictionary<string, object>{
                { "Option",2 },
                { "TypeMemo", Model.TypeMemo },
                { "ItemExpenseGet", Model.ItemExpense },
                { "UsernameGet", Model.Username},
                { "PeriodeGet", Model.Periode },
                { "TtdName1",Model.Ttd1 },
                { "TtdName2",Model.Ttd2 },
                { "TtdName3",Model.Ttd3 },
                { "TtdName4",Model.Ttd4 },
                { "TtdName5",Model.Ttd5 },
                { "Jbt1",Model.Jbt1 },
                { "Jbt2",Model.Jbt2 },
                { "Jbt3",Model.Jbt3 },
                { "Jbt4",Model.Jbt4 },
                { "Jbt5",Model.Jbt5 },
                { "ToGet", Model.ToHeader},
                { "SubdistQP1", Model.SubdistQP1},
                { "AlamatQP1", Model.AlamatQP1 },
                { "CabangQP1", Model.CabangQP1 },
                { "SubdistQP2", Model.SubdistQP2},
                { "AlamatQP2", Model.AlamatQP2 },
                { "CabangQP2", Model.CabangQP2 },
                { "@Remarks", Model.Remarks }
            };
            var ListAttribute = new DynamicParameters(dictionary);
            ListAttribute.Add("NoClaimListGet", DetailNoClaimList.AsTableValuedParameter("NoClaimList"));
            string Return = DAL.StoredProcedure(ListAttribute, "STP_Claim_CreateMemo");
            return Json(Return);
        }
        public ActionResult SubmitClaim(ClaimAttribute Model)
        {
            Dictionary<string, object> row;
            var DetailClaimobj = Session["DTDetailClaim"] as List<ClaimDetailDT>;

            ListtoDataTableConverter converter = new ListtoDataTableConverter();
            DataTable DetailClaimobjdt = converter.ToDataTable(DetailClaimobj);

            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            List<ClaimAttribute> newList = new List<ClaimAttribute>();
            newList.Add(Model);
            var list = new ClaimDetailDT();
            //var result = list.ToDataTable<AnalisaKondisiModel>(newList); // Insert to DataTable
            var dictionary = new Dictionary<string, object>{
                { "Option",1 },
                { "Cabang", Model.Cabang },
                { "SiteUsedIdGet", Model.SiteUsedId },
                { "ItemExpenseGet", Model.ItemExpense },
                { "RegionGet", Model.Region },
                { "UsernameGet", Model.Username},
                { "DetailClaimGet", DetailClaimobjdt },
                { "ToGet", Model.ToHeader},
                { "DetailNo", Model.No},
                { "NoRegist", Model.NoMemo}

            };
            var ListAttribute = new DynamicParameters(dictionary);
            ListAttribute.Add("DetailClaimGet", DetailClaimobjdt.AsTableValuedParameter("ClaimDetailDT"));
            string Return = DAL.StoredProcedure(ListAttribute, "STP_ClaimTransaction");
            return Json(Return);
        }
        public ActionResult LoginExec(LoginAttribute Model)
        {
            List<string> ModelData = new List<string>();
            DataTable dt = new DataTable();
            DataTable dt2 = new DataTable();
            string conString = dbDFIS.ConnectionString;
            SqlConnection conn = new SqlConnection(conString);
            try
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand("[dbo].[STP_Transaction]", conn))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add("@Option", System.Data.SqlDbType.Int);
                    command.Parameters["@Option"].Value = 43;

                    command.Parameters.Add("@Username", System.Data.SqlDbType.NVarChar);
                    command.Parameters["@Username"].Value = Model.Username;

                    command.Parameters.Add("@PasswordGet", System.Data.SqlDbType.NVarChar);
                    command.Parameters["@PasswordGet"].Value = Model.Password;

                    SqlDataAdapter dataAdapt = new SqlDataAdapter();
                    dataAdapt.SelectCommand = command;

                    dataAdapt.Fill(dt);
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            if (dt.Rows[0]["Type"].ToString() != "NAD")
            {
                IntPtr tokenHandle = new IntPtr(0);
                try
                {
                    string UserName, MachineName, Pwd = null;

                    //The MachineName property gets the name of your computer.                
                    UserName = Model.Username;
                    Pwd = Model.Password;
                    MachineName = "ONEKALBE";
                    const int LOGON32_PROVIDER_DEFAULT = 0;
                    const int LOGON32_LOGON_INTERACTIVE = 2;
                    tokenHandle = IntPtr.Zero;

                    //Call the LogonUser function to obtain a handle to an access token.
                    bool returnValue = LogonUser(UserName, MachineName, Pwd, LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT, ref tokenHandle);

                    Session["LoginSuccess"] = "True";
                    Session["UserName"] = Model.Username;

                    if (returnValue == false)
                    {
                        //This function returns the error code that the last unmanaged function returned.
                        int ret = Marshal.GetLastWin32Error();
                        if (ret == 1329)
                        {
                            Session["LoginSuccess"] = "True";
                            Session["xUser"] = Model.Username;
                            Session["LoginSuccess"] = "Account Directory tidak Valid";
                        }
                        else
                        {
                            if (dt.Rows[0]["Type"].ToString() == "NAD")
                            {
                                Session["LoginSuccess"] = "True";
                                Session["UserName"] = Model.Username;
                                Session["Role"] = dt.Rows[0]["Role"].ToString();
                                ModelData.Add(dt.Rows[0]["Role"].ToString());
                            }
                            else
                            {
                                ModelData.Add("UNVALID");
                                Session["UserName"] = Model.Username;
                                Session["LoginSuccess"] = "False";
                            }
                        }
                    }
                    else
                    {
                        Session["LoginSuccess"] = "True";
                        Session["UserName"] = Model.Username;
                        Session["Role"] = dt.Rows[0]["Role"].ToString();
                        ModelData.Add(dt.Rows[0]["Role"].ToString());
                    }
                }
                catch (Exception ex)
                {
                    Session["LoginSuccess"] = "False";
                }
            }
            else
            {
                if (dt.Rows[0]["Role"].ToString() == "FA" || dt.Rows[0]["Role"].ToString() == "MKT" || dt.Rows[0]["Role"].ToString() == "FINAR")
                {
                    Session["LoginSuccess"] = "True";
                    Session["UserName"] = Model.Username;
                    Session["xUser"] = Model.Username;
                    Session["Role"] = dt.Rows[0]["Role"].ToString();
                    ModelData.Add(dt.Rows[0]["Role"].ToString());
                }
                else
                {
                    ModelData.Add("UNVALID");
                    Session["LoginSuccess"] = "False";
                    Session["UserName"] = Model.Username;
                }
            }
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row;
            foreach (DataRow dr in dt.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn col in dt.Columns)
                {
                    row.Add(col.ColumnName, dr[col]);
                }
                rows.Add(row);
            }
            ViewBag.Username = Session["UserName"];
            return Json(ModelData);
        }
        public FileResult ExportCustomers(string Option, string TypeMemo, string ItemExpense, string KodeSubdist, string NoMemo)
        {
            ReportDocument rd = new ReportDocument();
           
            CreateMemoAttribute objAttr = new CreateMemoAttribute();
            objAttr.Option ="3";
            objAttr.NoMemo = NoMemo;
            objAttr.KodeSubdist = KodeSubdist;
            objAttr.SP = "STP_Claim_CreateMemo";
            objAttr.TypeMemo = TypeMemo;
            objAttr.ItemExpense = ItemExpense;
            string DataPrintout = PopulateDataGeneral(objAttr);

            JavaScriptSerializer js = new JavaScriptSerializer();
            dynamic responseJson = js.Deserialize<dynamic>(DataPrintout);

            var responseData = responseJson[0];
            TextObject txt;

            if(TypeMemo.Trim() == "TC")
            {
                if(ItemExpense == "QPL")
                {
                    rd.Load(Path.Combine(Server.MapPath("~/Reports"), "MemoReportNonTF.rpt"));
                    rd.SetParameterValue("NoMemo", responseData["NoMemo_PK"]);
                }
                else
                {
                    rd.Load(Path.Combine(Server.MapPath("~/Reports"), "MemoReportNonTF.rpt"));
                    txt = (TextObject)rd.ReportDefinition.ReportObjects["txtNoMemo"];
                    txt.Text = responseData["NoMemo_PK"];

                    txt = (TextObject)rd.ReportDefinition.ReportObjects["txtBody1"];
                    txt.Text = responseData["BodyHTML"];

                    txt = (TextObject)rd.ReportDefinition.ReportObjects["txtPrintDate"];
                    txt.Text = responseData["ApplyDate"];

                    if(responseData["TtdName1"] != null)
                    {
                        txt = (TextObject)rd.ReportDefinition.ReportObjects["txtTtd1"];
                        txt.Text = responseData["TtdName1"];
                    }
                    if (responseData["TtdName2"] != null)
                    {
                        txt = (TextObject)rd.ReportDefinition.ReportObjects["txtTtd2"];
                        txt.Text = responseData["TtdName2"];
                    }
                    if (responseData["TtdName3"] != null)
                    {
                        txt = (TextObject)rd.ReportDefinition.ReportObjects["txtTtd3"];
                        txt.Text = responseData["TtdName3"];
                    }
                    if (responseData["TtdName4"] != null)
                    {
                        txt = (TextObject)rd.ReportDefinition.ReportObjects["txtTtd4"];
                        txt.Text = responseData["TtdName4"];
                    }
                    if(responseData["TtdName5"] != null){
                        txt = (TextObject)rd.ReportDefinition.ReportObjects["txtTtd5"];
                        txt.Text = responseData["TtdName5"];
                    }
                    if (responseData["Jbt1"] != null)
                    {
                        txt = (TextObject)rd.ReportDefinition.ReportObjects["txtJbt1"];
                        txt.Text = responseData["Jbt1"];
                    }
                    if (responseData["Jbt2"] != null)
                    {
                        txt = (TextObject)rd.ReportDefinition.ReportObjects["txtJbt2"];
                        txt.Text = responseData["Jbt2"];
                    }
                    if (responseData["Jbt3"] != null)
                    {
                        txt = (TextObject)rd.ReportDefinition.ReportObjects["txtJbt3"];
                        txt.Text = responseData["Jbt3"];
                    }
                    if (responseData["Jbt4"] != null)
                    {
                        txt = (TextObject)rd.ReportDefinition.ReportObjects["txtJbt4"];
                        txt.Text = responseData["Jbt4"];
                    }
                    if (responseData["Jbt5"] != null)
                    {
                        txt = (TextObject)rd.ReportDefinition.ReportObjects["txtJbt5"];
                        txt.Text = responseData["Jbt5"];
                    }
                    txt = (TextObject)rd.ReportDefinition.ReportObjects["txtToHeader"];
                    txt.Text = responseData["ToHeader"];
                }
            }
            else if(TypeMemo.Trim() == "SM")
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports"), "MemoReportSM.rpt"));
                txt = (TextObject)rd.ReportDefinition.ReportObjects["txtNoMemo"];
                txt.Text = responseData["NoMemo_PK"];

                txt = (TextObject)rd.ReportDefinition.ReportObjects["txtBody1"];
                txt.Text = responseData["BodyHTML"];

                txt = (TextObject)rd.ReportDefinition.ReportObjects["txtPrintDate"];
                txt.Text = responseData["ApplyDate"];

                if (responseData["TtdName1"] != null)
                {
                    txt = (TextObject)rd.ReportDefinition.ReportObjects["txtTtd1"];
                    txt.Text = responseData["TtdName1"];
                }
                if (responseData["TtdName2"] != null)
                {
                    txt = (TextObject)rd.ReportDefinition.ReportObjects["txtTtd2"];
                    txt.Text = responseData["TtdName2"];
                }
                if (responseData["TtdName3"] != null)
                {
                    txt = (TextObject)rd.ReportDefinition.ReportObjects["txtTtd3"];
                    txt.Text = responseData["TtdName3"];
                }
                if (responseData["TtdName4"] != null)
                {
                    txt = (TextObject)rd.ReportDefinition.ReportObjects["txtTtd4"];
                    txt.Text = responseData["TtdName4"];
                }
                if (responseData["TtdName5"] != null)
                {
                    txt = (TextObject)rd.ReportDefinition.ReportObjects["txtTtd5"];
                    txt.Text = responseData["TtdName5"];
                }
                if (responseData["Jbt1"] != null)
                {
                    txt = (TextObject)rd.ReportDefinition.ReportObjects["txtJbt1"];
                    txt.Text = responseData["Jbt1"];
                }
                if (responseData["Jbt2"] != null)
                {
                    txt = (TextObject)rd.ReportDefinition.ReportObjects["txtJbt2"];
                    txt.Text = responseData["Jbt2"];
                }
                if (responseData["Jbt3"] != null)
                {
                    txt = (TextObject)rd.ReportDefinition.ReportObjects["txtJbt3"];
                    txt.Text = responseData["Jbt3"];
                }
                if (responseData["Jbt4"] != null)
                {
                    txt = (TextObject)rd.ReportDefinition.ReportObjects["txtJbt4"];
                    txt.Text = responseData["Jbt4"];
                }
                if (responseData["Jbt5"] != null)
                {
                    txt = (TextObject)rd.ReportDefinition.ReportObjects["txtJbt5"];
                    txt.Text = responseData["Jbt5"];
                }

                txt = (TextObject)rd.ReportDefinition.ReportObjects["txtToHeader"];
                txt.Text = responseData["ToHeader"];
            }
            

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();


            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/pdf", "Memo.pdf");
        }
        public ActionResult SaveEditDetailPA(CreatePAAttribute Model)
        {
            var list = new ClaimDetailDT();
            var dictionary = new Dictionary<string, object>{
                { "Option",3 },
                { "NoMemoGet",  Model.NoMemo},
                { "KodeSubdistGet",  Model.SiteUsedId},
                { "ItemExpenseGet",  Model.ItemExpense},
                { "BankGet",  Model.Bank},
                { "RekeningGet",  Model.Rekening},
                { "NoRekGet",  Model.NoRekening},
                { "EOGet",  Model.EO},
                { "AmountGet",  Model.Amount},
                { "RemarksGet",  Model.Remarks},
                { "AmountChangeGet",  Model.AmountChange},
                { "RemarksChangeGet",  Model.RemarksChange}
            };
            var ListAttribute = new DynamicParameters(dictionary);
            string Return = DAL.StoredProcedure(ListAttribute, "STP_CreatePA");
            return Json(Return);
        }
        public ActionResult CreatePAExec(CreatePAAttribute Model)
        {
            var list = new ClaimDetailDT();
            DataTable DetailNoClaimList = new DataTable();
            DetailNoClaimList.Columns.Add("NoClaim");


            int Counter = Model.NoPool.Count;
            int Trav = 0;
           
            for (Trav = 0; Trav < Counter; Trav++)
            {
                DataRow dr = DetailNoClaimList.NewRow();
                dr["NoClaim"] = Model.NoPool[Trav];
                DetailNoClaimList.Rows.Add(dr);
            }

            var dictionary = new Dictionary<string, object>{
                { "Option",4 },
                { "NoMemoGet",  Model.NoMemo},
                { "KodeSubdistGet",  Model.SiteUsedId},
                { "ItemExpenseGet",  Model.ItemExpense}
            };
            var ListAttribute = new DynamicParameters(dictionary);
            ListAttribute.Add("NoPoolGet", DetailNoClaimList.AsTableValuedParameter("NoClaimList"));
            string Return = DAL.StoredProcedure(ListAttribute, "STP_CreatePA");
            return Json(Return);
        }
        #endregion


    }
}