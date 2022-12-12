using SCS_Inventory.Models;
using scs_Project.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;

using System.Web.Security;


namespace SCS_Inventory.Controllers
{
    public class HomeController : Controller
    {
        DataContext db = new DataContext();

        //[AllowAnonymous]
        public ActionResult Index()
        {
            HttpCookie useridCookie = Request.Cookies["um_apps_userid"];
            string appsuser = useridCookie == null ? "nouser" : useridCookie.Value.ToString();
            var userList = db.UserInfo.Where(p => p.UserName == appsuser).FirstOrDefault();
            if (userList.RoleId == 0 || userList.RoleId == null)
            {
                ViewBag.UserExist = "You are New User in this Site. Please wait for your menu permission by IT";
            }

            ViewBag.OwnSubmited = db.Database.SqlQuery<int>("select Count(ST.Status)OwnSubmited from(Select Requested_by, min(Status) Status, App_Req_ID from OtherRequistionDetailsNews where Requested_by= '" + appsuser + "' group by Requested_by, App_Req_ID) ST").FirstOrDefault();
            ViewBag.Pending = db.Database.SqlQuery<int>("select Count(ST.Status)Pending from(Select Requested_by, min(Status) Status, App_Req_ID from OtherRequistionDetailsNews where Requested_by= '" + appsuser + "' and CompleteBy is NULL group by Requested_by, App_Req_ID) ST").FirstOrDefault();
            ViewBag.Approval = db.Database.SqlQuery<int>("select Count(ST.Status)Approval from(Select Requested_by, min(Status) Status, App_Req_ID from OtherRequistionDetailsNews where Requested_by= '" + appsuser + "' and CompleteBy is not NULL group by Requested_by, App_Req_ID) ST").FirstOrDefault();

            ViewBag.TotalApproval = db.Database.SqlQuery<int>("select count(BranchId)TotalApproval from BranchUserApprovals where UserId='"+ appsuser + "'").FirstOrDefault();

            ViewBag.TotalRequistion = db.Database.SqlQuery<int>("select Count(FT.App_Req_ID)TotalRequistion from (Select Count(App_Req_ID)App_Req_ID from OtherRequistionDetailsNews group by App_Req_ID) FT").FirstOrDefault();
            ViewBag.ApprovedByYou = db.Database.SqlQuery<int>("select Count(ST.Status)ApprovedByYou from (Select Requested_by, min(Status)Status,App_Req_ID from OtherRequistionDetailsNews  where ApprovedBy='" + appsuser + "' group by Requested_by, App_Req_ID) ST").FirstOrDefault();
            ViewBag.WaitingForApproval = db.Database.SqlQuery<int>("select count(NextLevel.App_Req_ID) Number_of_Req from(select RawData.*, ISNULL(ExtraData.CheckName,'Plased_Requsition') AS CurrentStatus  from (select App_Req_ID, max(AmendmentId)AmendmentId,BranchReqSeqAssignsId from OtherRequistionDetailsNews group by App_Req_ID,BranchReqSeqAssignsId)RawData left outer join (select BRSA.*,RCL.CheckName from BranchReqSeqAssigns BRSA,ReqCheckLists RCL where BRSA.CheckID=RCL.CheckId)ExtraData on RawData.BranchReqSeqAssignsId=ExtraData.ReqSeqID and RawData.AmendmentId=ExtraData.Serial) CurrentLevel,(select NextSteps.*,AllReq.*,BInfo.BranchName,DE.DepartmentName from (select ReqSeqID,Serial,CheckName,RCL.CheckId from BranchReqSeqAssigns BRSA,ReqCheckLists RCL where RCL.CheckId=BRSA.CheckID group by ReqSeqID,Serial,CheckName,RCL.CheckId ) NextSteps,(select App_Req_ID, max(AmendmentId)AmendmentId,RequestName,BranchReqSeqAssignsId,BranchID,DepartmentId,Requested_by from OtherRequistionDetailsNews group by App_Req_ID,RequestName, BranchReqSeqAssignsId,BranchID,DepartmentId,Requested_by) AllReq, BranchUserApprovals BUA,BranchInfoes BInfo,DepartmentEntries DE where AllReq.BranchReqSeqAssignsId=NextSteps.ReqSeqID and NextSteps.Serial=AllReq.AmendmentId+1 and BUA.CheckId=NextSteps.CheckId and AllReq.BranchID=BUA.BranchId and AllReq.DepartmentId=BUA.DepartmentId and BUA.Category=AllReq.RequestName and AllReq.DepartmentId=de.DepartmentId and AllReq.BranchID=BInfo.BranchID and BUA.UserId='" + appsuser + "')NextLevel where CurrentLevel.App_Req_ID=NextLevel.App_Req_ID and CurrentLevel.BranchReqSeqAssignsId=NextLevel.BranchReqSeqAssignsId").FirstOrDefault();
            ViewBag.PendingForDownLevel = db.Database.SqlQuery<int>("select Count (AllData.App_Req_ID)App_Req_ID from DepartmentEntries DE,BranchInfoes Binfo,(select distinct App_Req_ID,RequestName TypeName,Requested_by,ORDN.BranchID, ordn.DepartmentId from OtherRequistionDetailsNews ORDN,(select BranchId,DepartmentId from BranchUserApprovals where UserId='" + appsuser + "' group by BranchId,DepartmentId)RawDatas,(select ReqSeqID,max(Serial) Serial from BranchReqSeqAssigns group by ReqSeqID)SeqData where ORDN.BranchID = RawDatas.BranchId and ORDN.DepartmentId = RawDatas.DepartmentId and ORDN.BranchReqSeqAssignsId = SeqData.ReqSeqID and ORDN.Status != SeqData.Serial) AllData where AllData.BranchID = Binfo.BranchID and AllData.DepartmentId = DE.DepartmentId").FirstOrDefault();

            //ViewBag.WaitingForApproval = db.Database.SqlQuery<int>("select Count(FT.App_Req_ID)WaitingForApproval from (select DISTINCT DL2.App_Req_ID, DL2.BranchID, Ol2.BranchName, DL2.Requested_by, DL2.DepartmentId, Ol2.DepartmentName, Dl2.Status, Serial = 0, CurrentStatus = '', DL2.NextStatus from(select Dl1.*, Ol1.CheckName as NextStatus from(select ORDN.*, BRSA.CheckID from OtherRequistionDetailsNews ORDN " +
            //                                    "inner join(select D1.*from(select bra.CheckID, BRA.BranchID, ORD.BranchReqSeqAssignsId, ORD.RequestName, ORD.DepartmentId from BranchReqSeqAssigns BRA, OtherRequistionDetailsNews ORD, (select Max(AmendmentId)AmendmentId, App_Req_ID from OtherRequistionDetailsNews group by " +
            //                                     "App_Req_ID)MaxORD where ORD.BranchReqSeqAssignsId = BRA.ReqSeqID and ORD.BranchID = BRA.BranchID and BRA.Serial = ORD.Status + 1 and ORD.IsCancel = 0 and ORD.AmendmentId = MaxORD.AmendmentId and ORD.App_Req_ID = MaxORD.App_Req_ID ) D1 inner join BranchUserApprovals BUA on BUA.UserId ='" + appsuser + "' and BUA.Category = " +
            //                                     "D1.RequestName and BUA.BranchId = D1.BranchID and BUA.CheckId = D1.CheckID) BRSA on ORDN.BranchReqSeqAssignsId = BRSA.BranchReqSeqAssignsId and ORDN.BranchID = BRSA.BranchID and ORDN.IsCancel = 0) Dl1 inner join(select ReqCheck.CheckName,BranchReqSeq.Serial,BranchReqSeq.CheckID, BranchReqSeq.ReqSeqID, " +
            //                                     "BranchReqSeq.BranchID from ReqCheckLists ReqCheck, BranchReqSeqAssigns BranchReqSeq where ReqCheck.CheckId = BranchReqSeq.CheckID) Ol1 on Dl1.BranchID = Ol1.BranchID and Dl1.BranchReqSeqAssignsId = Ol1.ReqSeqID and Dl1.CheckID = Ol1.CheckID inner join(select Max(AmendmentId) AmendmentId, App_Req_ID from " +
            //                                     "OtherRequistionDetailsNews group by App_Req_ID)MaxORD on Dl1.AmendmentId = MaxORD.AmendmentId and Dl1.App_Req_ID = MaxORD.App_Req_ID) DL2 inner join(select ORDN.BranchID, BInfo.BranchName,ORDN.DepartmentId, DE.DepartmentName, ORDN.App_Req_ID from OtherRequistionDetailsNews ORDN, DepartmentEntries DE, " +
            //                                     "BranchInfoes BInfo, (select Max(AmendmentId)AmendmentId, App_Req_ID from OtherRequistionDetailsNews group by App_Req_ID)MaxORD where ORDN.BranchID = BInfo.BranchID and ORDN.DepartmentId = DE.DepartmentId and ORDN.AmendmentId = MaxORD.AmendmentId and ORDN.App_Req_ID = MaxORD.App_Req_ID) Ol2 on DL2.BranchID = Ol2.BranchID " +
            //                                     "and DL2.DepartmentId = Ol2.DepartmentId and dl2.App_Req_ID = Ol2.App_Req_ID) FT").FirstOrDefault();


            //Response.Cookies["um_apps_userid"].Value = "dhk00112";
            //Response.Cookies["um_apps_roles"].Value = "Super Admin";
            //Response.Cookies["um_apps_id"].Value = "scsapps_inventory";
            //Response.Cookies["um_apps_branchcode"].Value = "DHK";

            //HttpCookie useridCookie = Request.Cookies["um_apps_userid"];
            //HttpCookie rolesCookie = Request.Cookies["um_apps_roles"];
            //HttpCookie appsidCookie = Request.Cookies["um_apps_id"];
            //HttpCookie branchcodeCookie = Request.Cookies["um_apps_branchcode"];

            //string appsid = appsidCookie == null ? "nopermit" : appsidCookie.Value.ToString();
            //string appsuser = useridCookie == null ? "nouser" : useridCookie.Value.ToString();
            //string appsrole = rolesCookie == null ? "noroles" : rolesCookie.Value.ToString();
            //string branchcode = branchcodeCookie == null ? "nobranchcode" : branchcodeCookie.Value.ToString();

            ////HttpCookie useridCookie = new HttpCookie("um_apps_userid");
            ////HttpCookie rolesCookie = new HttpCookie("um_apps_roles");
            ////HttpCookie appsidCookie = new HttpCookie("um_apps_id");
            ////HttpCookie branchcodeCookie = new HttpCookie("um_apps_branchcode");

            ////Request.Cookies["um_apps_userid"].Value = "uttam";
            ////Request.Cookies["um_apps_roles"].Value = "Branch Admin";
            ////Request.Cookies["um_apps_id"].Value = "scsapps_inventory";
            ////Request.Cookies["um_apps_branchcode"].Value = "SCS";

            //var UserExist = db.UserInfo.Where(r => r.UserName == appsuser).FirstOrDefault();
            //var RoleExist = db.RoleName.Where(r => r.RoleCaption == appsrole).FirstOrDefault();
            //if (RoleExist == null)
            //{
            //    RoleName roleName = new RoleName();
            //    roleName.RoleCaption = appsrole;
            //    roleName.Status = true;
            //    roleName.SL = db.RoleName.Count() + 1;
            //    db.RoleName.Add(roleName);
            //}                       


            //int BranchID = db.Branch.Where(r => r.BranchCode == branchcode).Select(s => s.BranchID).SingleOrDefault();

            //if (UserExist == null)
            //{

            //    var std = new UserInfo()
            //    {
            //        UserName = appsuser,
            //        Branch_Id = BranchID,
            //        BranchCode = branchcode,
            //        Activation_Date = DateTime.Now
            //    };
            //    db.UserInfo.Add(std);
            //    db.SaveChanges();

            //}
            //else
            //{
            //    var UserExist1 = db.ReqApprovSeqPermission.Where(r => r.UserID == UserExist.Id).FirstOrDefault();
            //    ViewBag.UserExist = "Please contact with your IT person for your Requesition Menu permission";
            //}
            //if (appsuser != "nouser" && appsrole != "noroles" && branchcode != "nobranchcode" && appsid == "scsapps_inventory")
            //{
            //    UserInfo uinfo = new UserInfo();
            //    uinfo.UserName = "dhk00112";
            //    uinfo.BranchCode = "DHK";
            //    uinfo.Id = 1;

            //    this.Session["UserProfile"] = uinfo;

            //    HttpCookie cookie = new HttpCookie("SCSLogin");
            //    cookie.Values.Add("UserName", uinfo.UserName);
            //    cookie.Values.Add("Branch_Id", uinfo.BranchCode);
            //    cookie.Values.Add("Id", "1");
            //    cookie.Expires = DateTime.MaxValue;
            //    Response.Cookies.Add(cookie);
            return View();
            //}
            //else
            //{
            //    return Redirect("https://scsapps.xyz");
            //}


            //HttpCookie useridCookie = Request.Cookies["um_apps_userid"];
            //HttpCookie rolesCookie = Request.Cookies["um_apps_roles"];
            //HttpCookie appsidCookie = Request.Cookies["um_apps_id"];
            //HttpCookie branchcodeCookie = Request.Cookies["um_apps_branchcode"];

            //string appsid = appsidCookie == null ? "nopermit" : appsidCookie.Value.ToString();
            //string appsuser = useridCookie == null ? "nouser" : useridCookie.Value.ToString();
            //string appsrole = rolesCookie == null ? "noroles" : rolesCookie.Value.ToString();
            //string branchcode = branchcodeCookie == null ? "nobranchcode" : branchcodeCookie.Value.ToString();

            ////Request.Cookies["um_apps_userid"].Value = "uttam";
            ////Request.Cookies["um_apps_roles"].Value = "Branch Admin";
            ////Request.Cookies["um_apps_id"].Value = "scsapps_inventory";
            ////Request.Cookies["um_apps_branchcode"].Value = "SCS";

            //if (appsuser != "nouser" && appsrole != "noroles" && branchcode != "nobranchcode" && appsid == "scsapps_inventory")
            //{
            //    //var result = db.UserInfo.Where(s => s.UserName == "uttam").FirstOrDefault();
            //    //if (result != null)
            //    //{

            //    var UserExist = db.UserInfo.Where(r => r.UserName == appsuser).FirstOrDefault();
            //    var RoleExist = db.RoleName.Where(r => r.RoleCaption == appsrole).FirstOrDefault();
            //    if (RoleExist == null)
            //    {
            //        RoleName roleName = new RoleName();
            //        roleName.RoleCaption = appsrole;
            //        roleName.Status = true;
            //        roleName.SL = db.RoleName.Count() + 1;
            //        db.RoleName.Add(roleName);
            //    }

            //    UserInfo uinfo = new UserInfo();
            //    uinfo.UserName = appsuser;
            //    uinfo.BranchCode = branchcode;

            //    //System.Collections.Hashtable ht = new System.Collections.Hashtable();
            //    //ht.Add("UserName", appsuser);
            //    //ht.Add("Branch_Id", branchcode);
            //    this.Session["UserProfile"] = uinfo;

            //    HttpCookie cookie = new HttpCookie("SCSLogin");
            //    cookie.Values.Add("UserName", uinfo.UserName);
            //    //cookie.Values.Add("Id", Convert.ToString(result.Id));
            //    cookie.Values.Add("Branch_Id", uinfo.BranchCode);
            //    //cookie.Values.Add("Password", result.Password);
            //    cookie.Expires = DateTime.MaxValue;
            //    Response.Cookies.Add(cookie);
            //    //return RedirectToAction("Index", "Home");
            //    //}
            //    //else
            //    //{
            //    return View();
            //    //    return Redirect("https://scsapps.xyz");
            //    //}
            //}
            //else
            //{
            //    return Redirect("https://scsapps.xyz");
            //}

            //HttpCookie useridCookie = Request.Cookies["um_apps_userid"];
            //HttpCookie rolesCookie = Request.Cookies["um_apps_roles"];
            //HttpCookie appsidCookie = Request.Cookies["um_apps_id"];
            //HttpCookie branchcodeCookie = Request.Cookies["um_apps_branchcode"];

            //string appsid = appsidCookie == null ? "nopermit" : appsidCookie.Value.ToString();
            //string appsuser = useridCookie == null ? "nouser" : useridCookie.Value.ToString();
            //string appsrole = rolesCookie == null ? "noroles" : rolesCookie.Value.ToString();
            //string branchcode = branchcodeCookie == null ? "nobranchcode" : branchcodeCookie.Value.ToString();

            //appsrole = appsrole.Replace("%20", " ");

            //ViewBag.UserID = appsuser + " " + appsid + " " + appsrole + " " + branchcode ;

            //ViewBag.useridCookie = useridCookie;
            //    ViewBag.rolesCookie = rolesCookie;
            //    ViewBag.appsidCookie = appsidCookie;

            //ViewBag.PO = db.PriceQuotations.Where(d => d.Proqurement_Status == 1).Count();
            //ViewBag.CMDC_Approved = db.PriceQuotations.Where(d => d.CMDC_Status == 1).Count();
            //ViewBag.Procurement_Pending = db.Database.SqlQuery<int>("select count(ReqId)Procurement_Pending from RequisitionMasters where ReqId not in (select ReqId from PriceQuotations) and  Req_Status = 1").FirstOrDefault();
            //ViewBag.CMDC_Othorize_Pending = db.Database.SqlQuery<int>("select count(ReqId)CMDC_Othorize_Pending from RequisitionMasters where ReqId not in (select ReqId from PriceQuotations) and Req_Status = 0").FirstOrDefault();
            //return View();
            //return RedirectToAction("Index", "Home");
            //}
            //else
            //{
            //    return Redirect("https://scsapps.xyz");
            //}

            //UserInfo userInfo = (UserInfo)Session["UserProfile"];
            //if (userInfo != null)
            //{ 
            //ViewBag.PO = db.PriceQuotations.Where(d => d.Proqurement_Status == 1).Count();
            //ViewBag.CMDC_Approved = db.PriceQuotations.Where(d => d.CMDC_Status == 1).Count();            
            //ViewBag.Procurement_Pending = db.Database.SqlQuery<int>("select count(ReqId)Procurement_Pending from RequisitionMasters where ReqId not in (select ReqId from PriceQuotations) and  Req_Status = 1").FirstOrDefault();
            //ViewBag.CMDC_Othorize_Pending = db.Database.SqlQuery<int>("select count(ReqId)CMDC_Othorize_Pending from RequisitionMasters where ReqId not in (select ReqId from PriceQuotations) and Req_Status = 0").FirstOrDefault();
            //    return View();
            //}
            //else
            //{
            //    return RedirectToAction("SmartLogins", "Setings");
            //}
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
        public ActionResult GetRequestionList()
        {
            try
            {                
                var listData = db.Database.SqlQuery< RequsitionStatus>("select RM.ReqId,RM.Requisition_Date,RM.Requsition_Type, case when RM.Req_Status = 0 then 'Waiting For CMDC' when RM.Req_Status = 1 then 'CMDC Accept, Waiting For Procurement' when RM.Req_Status = 2 then 'Procurement Accept, Waiting For Price Qurtation' when RM.Req_Status = 3 then 'Price Qurtation are waiting for CS' when RM.Req_Status = 4 then 'CS are Waiting for CMDC Approved' when RM.Req_Status = 5 then 'CS are Waiting for Procurement Approved' when RM.Req_Status = 6 then 'CS are Waiting for PO' when RM.Req_Status = 7 then 'PO has been generated, Waiting for Vendor' end as Status, B_Info.BranchName, U_Info.Name from RequisitionMasters RM, BranchInfoes B_Info, UserInfoes U_Info where B_Info.id = rm.BranchId and U_Info.Id = RM.Requested_by").ToList<RequsitionStatus>();
                return Json(new { data = listData }, JsonRequestBehavior.AllowGet);
            }

            catch (EntityException ex)
            {
                return Content(" Connection to Database Failed." + ex);
            }
        }

    }
}