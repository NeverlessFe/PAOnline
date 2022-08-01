using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PAOnline.Models
{
    public class HomeModel
    {
        public int Option { get; set; }
    }
    public class LoginAttribute
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
    public class NoPAPool
    {
        public string NoPA { get; set; }
    }
    public class MultiplePAProcess
    {
        public List<string> NoPAList { get; set; }
        public string Type { get; set; }
        public string RejectReason { get; set; }
    }
    public class ClaimAttribute
    {
        public string Option { get; set; }
        public string ApplyDate { get; set; }
        public string SP { get; set; }
        public string Region { get; set; }
        public string Cabang { get; set; }
        public string Username { get; set; }
        public string HeaderExpense { get; set; }
        public string ItemExpense { get; set; }
        public string SiteUsedId { get; set; }
        public string ToHeader { get; set; }
        public string NoMemo { get; set; }
        public string NoRegist { get; set; }
        public string No { get; set; }
    }
    public class CreatePAAttribute
    {
        public string Option { get; set; }
        public string NoMemo { get; set; }
        public string SiteUsedId { get; set; }
        public string ItemExpense { get; set; }
        public string Bank { get; set; }
        public string Rekening { get; set; }
        public string NoRekening { get; set; }
        public string EO { get; set; }
        public string Amount { get; set; }
        public string AmountChange { get; set; }
        public string Remarks { get; set; }
        public string RemarksChange { get; set; }
        public List<string> NoPool { get; set; }
    }
    public class CreateMemoAttribute
    {
        public string Option { get; set; }
        public string ApplyDate { get; set; }
        public string SP { get; set; }
        public string Username { get; set; }
        public string ItemExpense { get; set; }
        public string TypeMemo { get; set; }
        public List<string> NoClaimList { get; set; }
        public string NoMemo { get; set; }
        public string KodeSubdist { get; set; }
        public string Periode { get; set; }
        public string Ttd1 { get; set; }
        public string Ttd2 { get; set; }
        public string Ttd3 { get; set; }
        public string Ttd4 { get; set; }
        public string Ttd5 { get; set; }
        public string Jbt1 { get; set; }
        public string Jbt2 { get; set; }
        public string Jbt3 { get; set; }
        public string Jbt4 { get; set; }
        public string Jbt5 { get; set; }
        public string ToHeader { get; set; }
        public string SubdistQP1 { get; set; }
        public string AlamatQP1 { get; set; }
        public string CabangQP1 { get; set; }
        public string SubdistQP2 { get; set; }
        public string AlamatQP2 { get; set; }
        public string CabangQP2 { get; set; }
        public string Remarks { get; set; }
    }
    public class ClaimDetailDT
    {
        public string No { get; set; }
        public string Region { get; set; }
        public string Cabang { get; set; }
        public string Subdist { get; set; }
        public string ItemExpense { get; set; }
        public string Amount { get; set; }
        public string Remarks { get; set; }
        public string Bank { get; set; }
        public string EO { get; set; }
        public string NamaRek { get; set; }
        public string NoRek { get; set; }
        public string SubdistName { get; set; }
        public string AmountShow { get; set; }
    }


}