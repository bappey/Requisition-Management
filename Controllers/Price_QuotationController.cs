using SCS_Inventory.Models;
using scs_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SCS_Inventory.Controllers
{
    public class Price_QuotationController : Controller
    {
        DataContext db = new DataContext();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult RequestionForPriceQuertation(int Id)
        {
            ViewBag.MasterID = Id;
            ViewBag.Vendors = new SelectList(db.Vendor, "Id", "Vendor_Name");
            ViewBag.RequestionForPriceQuertation = from d in db.RequisitionDetail
                                                   join i in db.SubProductList on d.SubProductId equals i.SubProductId
                                                   where d.Req_Id == Id
                                                   select new RequesitionVM { SP_List = i, R_Details = d };
            //ViewBag.RequestionForPriceQuertation= db.Database.SqlQuery<PriceListVM>("select rd.ItemId,I_Info.ItemName,rd.Qty,rd.Unit,rd.Description from RequestionDetails RD, ItemInfoes I_Info where I_Info.Id=RD.ItemId and rd.MasterId="+ Id).ToList<PriceListVM>();

            return View();
        }
        public ActionResult PriceQuertation()
        {
            //ViewBag.Total = db.PriceQuotations.Count();
            //ViewBag.Waiting = db.PriceQuotations.Where(d => d.E_Status == 0).Count();
            //ViewBag.Approved = db.PriceQuotations.Where(d => d.E_Status == 1).Count();
            //ViewBag.Rejected = db.PriceQuotations.Where(d => d.E_Status == 2).Count();
            //ViewBag.OP_Order = db.PriceQuotations.Where(d => d.E_Status == 3).Count();

            //var query = db.Database.SqlQuery<PriceListVM>("select main.*,V_Info.Vendor_Name from VendorInfoes V_Info, (select sum(price)price, max(QuotationID)QuotationID,MasterID,vendorID from PriceQuotations where E_Status!=2 group by MasterID,vendorID) main where V_Info.id=main.vendorID order by MasterID desc").ToList<PriceListVM>();
            ViewBag.PriceQuertationList = db.Database.SqlQuery<PriceListVM>("select(ReqId)MasterID, Requisition_Date,U_Info.Name as Requsitor_Name,B_Info.BranchName  from RequisitionMasters RM, BranchInfoes B_Info,UserInfoes U_Info where RM.BranchID=B_Info.BranchID and RM.Requested_by= U_Info.Id").ToList<PriceListVM>();
            ViewBag.RequestionDetailsList = db.Database.SqlQuery<PriceListVM>("select (RD.Req_Id)MasterID,(RD.SubProductId)ItemId,(Sub_P_List.ProductModel)ItemName,rd.Unit,RD.Qty,RD.Description from RequisitionDetails RD,SubProductLists Sub_P_List where rd.SubProductId=Sub_P_List.SubProductId and RD.Req_Status=0").ToList<PriceListVM>();

            return View();
        }
        public ActionResult Price_QuertationList()
        {
            ViewBag.Total = db.PriceQuotations.Count();
            //ViewBag.Waiting = db.PriceQuotations.Where(d => d.E_Status == 0).Count();
            //ViewBag.Approved = db.PriceQuotations.Where(d => d.E_Status == 1).Count();
            //ViewBag.Rejected = db.PriceQuotations.Where(d => d.E_Status == 2).Count();
            //ViewBag.OP_Order = db.PriceQuotations.Where(d => d.E_Status == 3).Count();
            ViewBag.PriceQuertationList = db.Database.SqlQuery<PriceListVM>("select(ReqId)MasterID,Requisition_Date,U_Info.Name as Requsitor_Name,B_Info.BranchName,0 as QuotationID,0 as price,0 as vendorID from RequisitionMasters RM, BranchInfoes B_Info,UserInfoes U_Info where RM.BranchID=B_Info.id and RM.Requested_by= U_Info.Id").ToList<PriceListVM>();
            return View();
        }
        public ActionResult PQ_CMDC_Authorozation(int MasterID)
        {
            ViewBag.Quartations = from d in db.PriceQuotations
                                  join i in db.ItemInfo on d.ItemID equals i.Id
                                  where d.MasterID == MasterID
                                  select new RequesitionVM { I_info = i, PQ_Info = d };
            return View();
        }
        public ActionResult Show_All_Price(int MasterID)
        {
            ViewBag.VendorNameList = db.Database.SqlQuery<PriceListVM>("select vm.Vendor_Name, vm.Id as MasterID From (select DISTINCT V_Info.Vendor_Name, MIN(pq.Id) AS Id from PriceQuotations pq,VendorInfoes V_Info where V_Info.id = PQ.vendorID and pq.MasterID=" + MasterID + " group by V_Info.Vendor_Name) vm order by vm.Id").ToList<PriceListVM>();
            ViewBag.ItemNameList = db.Database.SqlQuery<PriceListVM>("select i_Name.ItemName,i_Name.Qty  from (SELECT I_info.ItemName,pq.ItemID,pq.Qty,pq.price,pq.MasterID,V_Info.Vendor_Name FROM PriceQuotations PQ, ItemInfoes I_info, VendorInfoes V_Info where I_info.Id = PQ.ItemID and V_Info.id = PQ.vendorID and pq.MasterID=" + MasterID + ") i_Name group by i_Name.ItemName, i_Name.Qty").ToList<PriceListVM>();
            ViewBag.PriceDetails = db.Database.SqlQuery<PriceListVM>("SELECT I_info.ItemName,pq.ItemID as ItemId,pq.Qty,pq.price,pq.MasterID,V_Info.Vendor_Name, pq.Warienty,pq.MadeBy,pq.vendorID FROM PriceQuotations PQ, ItemInfoes I_info, VendorInfoes V_Info where I_info.Id = PQ.ItemID and V_Info.id = PQ.vendorID and pq.MasterID=" + MasterID).ToList<PriceListVM>();
            return View();
        }
        public JsonResult CSAccept(List<PriceListVM> model)
        {
            int insertedRecords = 0;
            foreach (var item in model)
            {
                var existModel = db.PriceQuotations.Where(i => i.MasterID == item.MasterID && i.ItemID == item.ItemId && i.vendorID == item.vendorID).FirstOrDefault();

                existModel.CS_Status = 1;
                existModel.CS_Command = item.CS_Command;
                db.Entry(existModel).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                insertedRecords++;
            }
            return Json(insertedRecords);
        }
        public ActionResult Show_Accept_CS_Data(int MasterID)
        {
            ViewBag.VendorNameList = db.Database.SqlQuery<PriceListVM>("select vm.Vendor_Name, vm.Id as MasterID From (select DISTINCT V_Info.Vendor_Name, MIN(pq.Id) AS Id from PriceQuotations pq,VendorInfoes V_Info where V_Info.id = PQ.vendorID and pq.MasterID=" + MasterID + " group by V_Info.Vendor_Name) vm order by vm.Id").ToList<PriceListVM>();
            //ViewBag.ItemNameList = db.Database.SqlQuery<PriceListVM>("select i_Name.ItemName,i_Name.Qty  from (SELECT I_info.ItemName,pq.ItemID,pq.Qty,pq.price,pq.MasterID,V_Info.Vendor_Name FROM PriceQuotations PQ, ItemInfoes I_info, VendorInfoes V_Info where I_info.Id = PQ.ItemID and V_Info.id = PQ.vendorID and pq.MasterID=" + MasterID + ") i_Name group by i_Name.ItemName, i_Name.Qty").ToList<PriceListVM>();
            ViewBag.ItemNameList = db.Database.SqlQuery<PriceListVM>("select I_info.ItemName, pq.Qty, pq.CS_Command from PriceQuotations pq, ItemInfoes I_info where I_info.Id=pq.ItemID and pq.MasterID= " + MasterID + " and CS_Command is not null group by I_info.ItemName, pq.Qty,pq.CS_Command").ToList<PriceListVM>();
            ViewBag.PriceDetails = db.Database.SqlQuery<PriceListVM>("SELECT I_info.ItemName,pq.ItemID as ItemId,pq.Qty,pq.price,pq.MasterID,V_Info.Vendor_Name, pq.Warienty,pq.MadeBy,pq.vendorID,pq.CS_Status,pq.CMDC_Command FROM PriceQuotations PQ, ItemInfoes I_info, VendorInfoes V_Info where I_info.Id = PQ.ItemID and V_Info.id = PQ.vendorID and pq.MasterID=" + MasterID).ToList<PriceListVM>();
            return View();
        }
        public JsonResult CSSelect(List<PriceListVM> model)
        {
            int insertedRecords = 0;
            foreach (var item in model)
            {
                var existModel = db.PriceQuotations.Where(i => i.MasterID == item.MasterID && i.ItemID == item.ItemId && i.vendorID == item.vendorID).FirstOrDefault();

                existModel.CMDC_Status = 1;
                existModel.CMDC_Command = item.CMDC_Command;
                db.Entry(existModel).State = System.Data.Entity.EntityState.Modified;
                insertedRecords = db.SaveChanges();
            }
            return Json(insertedRecords);
        }
        public ActionResult Show_Accept_CMDC_Data(int MasterID)
        {
            ViewBag.VendorNameList = db.Database.SqlQuery<PriceListVM>("select vm.Vendor_Name, vm.Id as MasterID From (select DISTINCT V_Info.Vendor_Name, MIN(pq.Id) AS Id from PriceQuotations pq,VendorInfoes V_Info where V_Info.id = PQ.vendorID and pq.MasterID=" + MasterID + " group by V_Info.Vendor_Name) vm order by vm.Id").ToList<PriceListVM>();
            //ViewBag.ItemNameList = db.Database.SqlQuery<PriceListVM>("select i_Name.ItemName,i_Name.Qty,i_Name.CMDC_Command from(SELECT I_info.ItemName, pq.ItemID, pq.Qty, pq.price, pq.MasterID, V_Info.Vendor_Name, pq.CMDC_Command FROM PriceQuotations PQ, ItemInfoes I_info, VendorInfoes V_Info where I_info.Id = PQ.ItemID and V_Info.id = PQ.vendorID and pq.MasterID= " + MasterID + " and pq.CMDC_Command IS NOT NULL) i_Name group by i_Name.ItemName, i_Name.Qty,i_Name.CMDC_Command").ToList<PriceListVM>();
            ViewBag.ItemNameList = db.Database.SqlQuery<PriceListVM>("select I_info.ItemName,pq.Qty,pq.CS_Command,pq.CMDC_Command from PriceQuotations pq, ItemInfoes I_info where I_info.Id=pq.ItemID and pq.MasterID= " + MasterID + " and CS_Command is not null group by I_info.ItemName,pq.Qty,pq.CS_Command,pq.CMDC_Command").ToList<PriceListVM>();
            ViewBag.PriceDetails = db.Database.SqlQuery<PriceListVM>("SELECT I_info.ItemName,pq.ItemID as ItemId,pq.Qty,pq.price,pq.MasterID,V_Info.Vendor_Name, pq.Warienty,pq.MadeBy,pq.vendorID,pq.CMDC_Status,pq.CS_Status,pq.CS_Command,pq.CMDC_Command FROM PriceQuotations PQ, ItemInfoes I_info, VendorInfoes V_Info where I_info.Id = PQ.ItemID and V_Info.id = PQ.vendorID and pq.MasterID=" + MasterID).ToList<PriceListVM>();
            return View();
        }
        public JsonResult Proqurement_Accept(List<PriceListVM> model)
        {
            int insertedRecords = 0;
            foreach (var item in model)
            {
                var existModel = db.PriceQuotations.Where(i => i.MasterID == item.MasterID && i.ItemID == item.ItemId && i.vendorID == item.vendorID).FirstOrDefault();

                existModel.Proqurement_Status = 1;
                db.Entry(existModel).State = System.Data.Entity.EntityState.Modified;
                insertedRecords = db.SaveChanges();
            }
            return Json(insertedRecords);
        }
        public ActionResult Initial_Requires(int MasterID)
        {
            ViewBag.MasterID = MasterID;
            return View();
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult PO()
        {
            ViewBag.Subject = Request.Form["Subject"];
            ViewBag.Body = Request.Form["Body"];

            int MasterId = Convert.ToInt32(Request.Form["MasterIds"]);

            ViewBag.VendorNameList = db.Database.SqlQuery<VendorInfoVM>("select V_Info.Address,V_Info.Vendor_Name,V_info.id, V_Info.Dealing_Person,V_Info.Contact_No,pq.MasterID from PriceQuotations pq, VendorInfoes V_Info where V_Info.id=pq.vendorID and pq.MasterID=" + MasterId + " and pq.CMDC_Status=1").ToList<VendorInfoVM>();
            ViewBag.PriceDetails = db.Database.SqlQuery<PriceListVM>("select I_Info.ItemName,pq.Description,pq.Qty,pq.price,pq.vendorID from PriceQuotations pq,ItemInfoes I_Info where I_Info.Id=pq.ItemID and pq.MasterID=" + MasterId + " and pq.CMDC_Status=1").ToList<PriceListVM>();

            return View();
        }
        public ActionResult ProductReceipt()
        {
            //ViewBag.PriceQuertationList = db.Database.SqlQuery<PriceListVM>("select(ReqId)MasterID, Requisition_Date,U_Info.Name as Requsitor_Name,B_Info.BranchName  from RequisitionMasters RM, BranchInfoes B_Info,UserInfoes U_Info where RM.BranchID=B_Info.BranchID and RM.Requested_by= U_Info.Id").ToList<PriceListVM>();
            
            UserInfo userInfo = (UserInfo)Session["UserProfile"];
            if (userInfo != null)
            {
                HttpCookie useridCookie = Request.Cookies["um_apps_userid"];
                if (userInfo.UserName == useridCookie.Value.ToString())
                {
                    HttpCookie rolesCookie = Request.Cookies["um_apps_roles"];
                    string appsrole = rolesCookie == null ? "noroles" : rolesCookie.Value.ToString();
                    appsrole = appsrole.Replace("%20", " ");
                    string controller_action = this.ControllerContext.RouteData.Values["controller"].ToString() + "/" + this.ControllerContext.RouteData.Values["action"].ToString();
                    int MenuId = db.Database.SqlQuery<int>("select id from RoleSettings where Path='" + controller_action + "' and RoleCaption='" + appsrole + "'").FirstOrDefault();
                    if (MenuId != 0)
                    {
                        ViewBag.ReceiptPendingList = db.Database.SqlQuery<ReceiptVM>("select T1.ReceiveDate,T2.* from (select top 1 SubmissionDate ReceiveDate, MasterID from PriceQuotations where CS_Status = 1 and CMDC_Status = 1 and Proqurement_Status = 1) T1,(" +
                        "select PQ.MasterID RequeisitionID,sum(PQ.qty) Qty,Vinfo.Vendor_Name from PriceQuotations PQ, VendorInfoes Vinfo " +
                        "where PQ.CS_Status = 1 and PQ.CMDC_Status = 1 and PQ.Proqurement_Status = 1 and Vinfo.id = PQ.vendorID and PQ.MasterID not in (select ReqID from ReceiptDetails where ReceiveStatus = 0 group by ReqID) " +
                        "group by PQ.MasterID,Vinfo.Vendor_Name) T2 where T1.MasterID=T2.RequeisitionID " +
                        "union select RD.ReceiveDate,RD.ReqID RequeisitionID,COUNT(RD.ProductID)Qty,Vinfo.Vendor_Name from ReceiptDetails RD, VendorInfoes Vinfo where Vinfo.id=RD.VendorID and RD.ReceiveStatus=1 " +
                        "group by RD.ReqID, Vinfo.Vendor_Name, RD.ReceiveDate").ToList<ReceiptVM>();
                        //ViewBag.RequestionDetailsList = db.Database.SqlQuery<PriceListVM>("select (RD.Req_Id)MasterID,(RD.SubProductId)ItemId,(Sub_P_List.ProductModel)ItemName,rd.Unit,RD.Qty,RD.Description from RequisitionDetails RD,SubProductLists Sub_P_List where rd.SubProductId=Sub_P_List.SubProductId and RD.Req_Status=0").ToList<PriceListVM>();
                        ViewBag.ReceiveList = db.Database.SqlQuery<ReceiptVM>("select Vinfo.Vendor_Name,RD.ReqID RequeisitionID,RD.ReceiveDate,COUNT(RD.ProductID)Qty,sum(RD.price)TotalPrice,RD.PurchageDate,Uinfo.Name,'' as Status from UserInfoes Uinfo, ReceiptDetails RD, VendorInfoes Vinfo where RD.InputUser=Uinfo.Id and Vinfo.id=RD.VendorID and RD.ReceiveStatus=1 group by RD.ReqID, Vinfo.Vendor_Name, RD.ReceiveDate, RD.PurchageDate, Uinfo.Name").ToList<ReceiptVM>();
                        return View();
                    }
                    else
                    {
                        return this.RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    return this.RedirectToAction("Index", "Home");
                }
            }
            else
            {
                return RedirectToAction("SmartLogins", "Setings");
            }

        }
        public ActionResult RequestionProductPrice(int Id)
        {
            ViewBag.MasterID = Id;
            //List<PriceListVM> PriceList = new  List<PriceListVM>();
            var ReceiveList = db.Database.SqlQuery<ReceiptDetailsVM>("select RD.SubProductID,RD.Unit,RD.price,RD.vendorID,RD.MadeBy,RD.Warranty,RD.ProductID,RD.RequeisitionID as MasterID,SubP_List.ProductModel, V_Info.Vendor_Name,RD.ReceiveStatus,RD.ReceiveStatus, RD.ExpireDate, rd.PurchageDate,RD.ReceiveDate  from ReceiptDetails RD ,SubProductLists SubP_List, VendorInfoes V_Info where RD.vendorID=V_Info.id and SubP_List.SubProductId=RD.SubProductID and RD.RequeisitionID=" + Id).ToList();
            if (ReceiveList.Count <= 0)
            {
                ReceiveList = db.Database.SqlQuery<ReceiptDetailsVM>("select PQ.ItemID SubProductID,PQ.Unit,pq.price,PQ.vendorID,PQ.MadeBy,pq.Warienty Warranty,SubP_List.ProductId,PQ.MasterID,SubP_List.ProductModel,PQ.Qty,PQ.Description, V_Info.Vendor_Name from PriceQuotations PQ, SubProductLists SubP_List, VendorInfoes V_Info where PQ.vendorID=V_Info.id and PQ.itemID=SubP_List.SubProductId and PQ.Proqurement_Status=1 and PQ.MasterID=" + Id).ToList();
            }
            ViewBag.PriceQuertationList = ReceiveList;
            //ViewBag.PriceQuertationList= db.Database.SqlQuery<PriceListVM>("select PQ.MasterID,PQ.price,PQ.vendorID,pq.ItemId,pq.Unit,pq.Qty,pq.Description,pq.Warienty,pq.MadeBy,pq.CS_Command,pq.CMDC_Command,I_Info.ProductModel as ItemName,V_Info.Vendor_Name from PriceQuotations PQ,SubProductLists I_Info, VendorInfoes V_Info where PQ.Proqurement_Status=1 and PQ.MasterID=" + Id+" and PQ.ItemID=I_Info.SubProductId and V_Info.id=PQ.vendorID").ToList<PriceListVM>();
            return View();
        }
        public JsonResult SaveReceipt(Common model)
        {
            UserInfo userInfo = (UserInfo)Session["UserProfile"];
            List<ReceiptMaster> recptMaster = model.ReceiptMaster;
            if (model.ReceiptMaster == null)
            {
                recptMaster = new List<ReceiptMaster>();
            }
            foreach (ReceiptMaster details in recptMaster)
            {
                details.ReceiveDate = DateTime.Now;
                details.Input_Date = DateTime.Now;
                db.ReceiptMaster.Add(details);
                db.SaveChanges();
            }
            List<ReceiptDetails> recptDetails = model.ReceiptDetails;
            if (model.ReceiptDetails == null)
            {
                recptDetails = new List<ReceiptDetails>();
            }
            //Loop and insert records.
            foreach (ReceiptDetails details in recptDetails)
            {
                //details.Barcode = null;
                db.ReceiptDetails.Add(details);
            }
            int insertedRecords = db.SaveChanges();
            return Json(insertedRecords);
        }
        public JsonResult ReceiptSave(List<ReceiptDetails> model)
        {
            UserInfo userInfo = (UserInfo)Session["UserProfile"];
            foreach (var data in model)
            {
                //var existModel = db.PriceQuotations.Where(i => i.MasterID == item.MasterID && i.ItemID == item.ItemId && i.vendorID == item.vendorID).FirstOrDefault();
                var ExistData = db.ReceiptDetails.Where(i => i.ReqId == data.ReqId && i.SubProductID == data.SubProductID && i.ProductID == data.ProductID).FirstOrDefault();
                if (ExistData != null)
                {
                    if (ExistData.ReceiveStatus != true)
                    {
                        ExistData.ReceiveStatus = data.ReceiveStatus;
                        ExistData.ExpireDate = data.ExpireDate;
                        ExistData.PurchageDate = data.PurchageDate;
                        ExistData.ReceiveDate = data.ReceiveDate;
                        ExistData.EditUser = userInfo.Id;
                        db.Entry(ExistData).State = System.Data.Entity.EntityState.Modified;
                    }
                }
                else
                {
                    data.InputDate = DateTime.Now;
                    data.InputUser = userInfo.Id;
                    db.ReceiptDetails.Add(data);
                }
            }
            int insertedRecords = db.SaveChanges();
            return Json(insertedRecords);
        }
        public ActionResult ReceiptDetails(int Id)
        {
            ViewBag.MasterID = Id;
            ViewBag.ReceiptDetailsList = db.Database.SqlQuery<ReceiptDetailsVM>("select rd.ReceiveStatus, RD.SubProductID,RD.ReqID ReqId,SubProList.SubProductName,SubProList.ProductModel,Vinfo.Vendor_Name, count(RD.SubProductID)Qty, Cast(sum(RD.price) as decimal) AS Total_Price  from ReceiptDetails RD,SubProductLists SubProList, VendorInfoes Vinfo where Vinfo.id=RD.vendorID and RD.SubProductID=SubProList.SubProductId and RD.ProductID=SubProList.ProductId and RD.ReqID=" + Id + " group by RD.SubProductID,RD.ReqID,SubProList.SubProductName,SubProList.ProductModel,Vinfo.Vendor_Name,rd.ReceiveStatus").ToList();
            //ViewBag.PriceQuertationList = ReceiveList;
            return View();
        }
    }
}