// SCS_Inventory.Controllers.RequestionController
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using SCS_Inventory.Models;
using scs_Project.Models;
using System.Data;


public class RequestionController : Controller
{
	private DataContext db = new DataContext();
	public JsonResult uploadFile()
	{
		// check if the user selected a file to upload
		if (Request.Files.Count > 0)
		{
			try
			{
                HttpFileCollectionBase files = Request.Files;
                HttpPostedFileBase file = files[0];
                string fileName = file.FileName;
				string uploadBy = Request.Form["Requested_by"];

				var Existfile = db.RequestionFileModels.Where(x => x.FileName == fileName && x.FileSize == file.ContentLength && x.Extention == file.ContentType).FirstOrDefault();
				if(Existfile == null)
                {
					Directory.CreateDirectory(Server.MapPath("~/UploadedFiles/"));
					string path = Path.Combine(Server.MapPath("~/UploadedFiles/"), fileName);
					file.SaveAs(path);

					RequestionFileModel FileSave = new RequestionFileModel();
					FileSave.FileName = fileName;
					FileSave.UploadBy = uploadBy;
					FileSave.DateTime = DateTime.Today;
					FileSave.Extention = file.ContentType;
					FileSave.FileSize=file.ContentLength;
					db.RequestionFileModels.Add(FileSave);
					db.SaveChanges();
					int intIdt = db.RequestionFileModels.Select(u => u.id).DefaultIfEmpty(0).Max();
					return Json("File uploaded successfully +" + intIdt);
				}
                else
                {
					return Json("Same File Already in our Server, Please upload another one.");
				}
                //var file = model.AttachFile;
				 //if (file != null)
                //{
                //	var fileName = Path.GetFileName(file.FileName);
                //	var extention = Path.GetExtension(file.FileName);
                //	var filenamewithoutextension = Path.GetFileNameWithoutExtension(file.FileName);
				//	file.SaveAs(Server.MapPath("/UploadedFiles/" + file.FileName));
				//}
				//return Json(file.FileName, JsonRequestBehavior.AllowGet);
            }
			catch (Exception e)
			{
				return Json("error" + e.Message);
			}
		}
		return Json("no files were selected !");
	}
	public JsonResult uploadReferanceFile()
	{
		// check if the user selected a file to upload
		if (Request.Files.Count > 0)
		{
			try
			{
				HttpFileCollectionBase files = Request.Files;
				HttpPostedFileBase file = files[0];
				string fileName = file.FileName;
				int App_Req_ID =Convert.ToInt32( Request.Form["AppReqId"]);
				string uploadBy = Request.Form["Requested_by"];
				//var files = Request.Form.Files;
				//string type = Request.Form.Where(x => x.Key == "type").FirstOrDefault().Value;


				var Existfile = db.RequestionFileModels.Where(x => x.FileName == fileName && x.FileSize == file.ContentLength && x.Extention == file.ContentType).FirstOrDefault();
				if (Existfile == null)
				{
					Directory.CreateDirectory(Server.MapPath("~/UploadedFiles/"));
					string path = Path.Combine(Server.MapPath("~/UploadedFiles/"), fileName);
					file.SaveAs(path);

					RequestionFileModel FileSave = new RequestionFileModel();
					FileSave.FileName = fileName;
					FileSave.DateTime = DateTime.Today;
					FileSave.RequestionNumber = App_Req_ID;
					FileSave.UploadBy = uploadBy;
					FileSave.Extention = file.ContentType;
					FileSave.FileSize = file.ContentLength;
					db.RequestionFileModels.Add(FileSave);
					db.SaveChanges();
					//int intIdt = db.RequestionFileModels.Select(u => u.id).DefaultIfEmpty(0).Max();
					return Json("File uploaded successfully \nPlease Reload this page to view the save file.");
				}
				else
				{
					return Json("Same File Already in our Server, Please upload another one.");
				}				
			}
			catch (Exception e)
			{
				return Json("error" + e.Message);
			}
		}
		return Json("no files were selected !");
	}

	public ActionResult Requsitions()
	{
		UserInfo userInfo = (UserInfo)base.Session["UserProfile"];
		if (userInfo != null)
		{
			HttpCookie useridCookie = base.Request.Cookies["um_apps_userid"];
			if (userInfo.UserName == useridCookie.Values.ToString())
			{
				HttpCookie rolesCookie = base.Request.Cookies["um_apps_roles"];
				HttpCookie branchcodeCookie = base.Request.Cookies["um_apps_branchcode"];
				string appsrole = ((rolesCookie == null) ? "noroles" : rolesCookie.Value.ToString());
				string branchcode = ((branchcodeCookie == null) ? "nobranchcode" : branchcodeCookie.Value.ToString());
				appsrole = appsrole.Replace("%20", " ");
				string controller_action = base.ControllerContext.RouteData.Values["controller"].ToString() + "/" + base.ControllerContext.RouteData.Values["action"].ToString();
				if (db.Database.SqlQuery<int>("select id from RoleSettings where Path='" + controller_action + "' and RoleCaption='" + appsrole + "'", Array.Empty<object>()).FirstOrDefault() != 0)
				{
					int branchId = db.Database.SqlQuery<int>("select BranchID from BranchInfoes where BranchCode='" + branchcode + "'", Array.Empty<object>()).FirstOrDefault();
					base.ViewBag.ItemList = new SelectList(db.ItemInfo, "Id", "ItemName");
					base.ViewBag.SubProductList = new SelectList(db.SubProductList, "SubProductId", "ProductModel");
					base.ViewBag.ReqType = new SelectList(db.ReqTypeName, "Id", "TypeName", "Category");
					base.ViewBag.Req = db.ReqTypeName.Select((ReqTypeName reg) => reg);
					if (!appsrole.Contains("Admin"))
					{
						base.ViewBag.Branch = new SelectList(db.Database.SqlQuery<BranchInfo>("select * from BranchInfoes where BranchId in (select BranchId from BranchUserApprovals where UserId='" + userInfo.UserName + "' group by BranchId union select Branch_Id BranchId from UserInfoes where UserName = '" + userInfo.UserName + "') ", Array.Empty<object>()).ToList(), "BranchID", "BranchName");
					}
					else
					{
						base.ViewBag.Branch = new SelectList(db.Database.SqlQuery<BranchInfo>("select * from BranchInfoes where BranchId in (select BranchId from BranchUserApprovals where UserId='" + userInfo.UserName + "' group by BranchId union select Branch_Id BranchId from UserInfoes where UserName = '" + userInfo.UserName + "') ", Array.Empty<object>()).ToList(), "BranchID", "BranchName");
					}
					base.ViewBag.ReqCategoryList = new SelectList(db.ReqCategoryNames, "Id", "TypeName");
					base.ViewBag.RequestionList = db.Database.SqlQuery<RequesitionList>("select R_master.ReqId,B_info.BranchName,R_master.Edit_Date,R_master.Requisition_Date,R_Type_Name.TypeName, U_Info.Name,case when R_master.Req_Status = 0 then 'Waiting For CMDC' when R_master.Req_Status = 1 then 'CMDC Accept >> Waiting For Procurement' when R_master.Req_Status = 2 then 'Procurement Accept >> Waiting For Price Qurtation' when R_master.Req_Status = 3 then 'Price Qurtation are waiting for CS'when R_master.Req_Status = 4 then 'CS are Waiting for CMDC Approved' when R_master.Req_Status = 5 then 'CS are Waiting for Procurement Approved' when R_master.Req_Status = 6 then 'CS are Waiting for PO' when R_master.Req_Status = 7 then 'PO has been generated, Waiting for Vendor' end as Req_Status, details.Description  from BranchInfoes B_info, RequisitionMasters R_master,ReqTypeNames R_Type_Name, UserInfoes U_Info, (SELECT Req_Id, Description = STUFF((SELECT ', ' + Description FROM RequisitionDetails b WHERE b.Req_Status=0 and b.Req_Id = a.Req_Id FOR XML PATH('')), 1, 2, '') FROM RequisitionDetails a GROUP BY Req_Id) details where R_master.ReqId = details.Req_Id and R_master.BranchID = B_info.BranchID and R_master.Requested_by = U_Info.Id and R_Type_Name.Id=R_master.Requsition_Type and R_master.BranchID=" + userInfo.Branch_Id, Array.Empty<object>()).ToList();
					base.ViewBag.OtherRequestionList = db.Database.SqlQuery<OtherRequistionDetailsVM>("select DE.DepartmentName,Binfo.BranchName,AllData.App_Req_ID,AllData.RequestName TypeName,AllData.Requested_by from DepartmentEntries DE,BranchInfoes Binfo,(select OtherRequistionDetailsNews.App_Req_ID,max(OtherRequistionDetailsNews.AmendmentId)AmendmentId,OtherRequistionDetailsNews.BranchId,OtherRequistionDetailsNews.DepartmentId,OtherRequistionDetailsNews.RequestName,OtherRequistionDetailsNews.Requested_by from OtherRequistionDetailsNews, (select BranchId,DepartmentId from BranchUserApprovals where UserId='" + userInfo.UserName + "' group by DepartmentId,BranchId union select Branch_Id BranchId, Department_Id DepartmentId from UserInfoes where UserName = '" + userInfo.UserName + "') AU where OtherRequistionDetailsNews.DepartmentId = AU.DepartmentId and OtherRequistionDetailsNews.BranchID = AU.BranchId group by OtherRequistionDetailsNews.App_Req_ID,OtherRequistionDetailsNews.BranchId,OtherRequistionDetailsNews.DepartmentId,OtherRequistionDetailsNews.RequestName, OtherRequistionDetailsNews.Requested_by) AllData where AllData.BranchID = Binfo.BranchID and AllData.DepartmentId = DE.DepartmentId", Array.Empty<object>()).ToList();
					return View();
				}
				return RedirectToAction("Index", "Home");
			}
			return RedirectToAction("Index", "Home");
		}
		return RedirectToAction("SmartLogins", "Setings");
	}

	public ActionResult Index()
	{
		return View();
	}

	public JsonResult ItemData(int Id)
	{
		List<SubProductList> data = new List<SubProductList>();
		List<SubProductList> itemData = new List<SubProductList>();
		if (Id != 0)
		{
			data = db.SubProductList.Where((SubProductList p) => p.SubProductId == Id).ToList();
		}
		return new JsonResult
		{
			Data = data,
			JsonRequestBehavior = JsonRequestBehavior.AllowGet
		};
	}

	public JsonResult GetRequestion(int Id)
	{
		List<SubProductList> data = new List<SubProductList>();
		List<SubProductList> itemData = new List<SubProductList>();
		if (Id != 0)
		{
			data = db.SubProductList.Where((SubProductList p) => p.SubProductId == Id).ToList();
		}
		return new JsonResult
		{
			Data = data,
			JsonRequestBehavior = JsonRequestBehavior.AllowGet
		};
	}

	public JsonResult SaveRequestion(Common model)
	{
		int intIdt = db.RequisitionMaster.Select((RequisitionMaster u) => u.ReqId).DefaultIfEmpty(0).Max();
		intIdt++;
		UserInfo userInfo = (UserInfo)base.Session["UserProfile"];
		List<RequisitionMaster> reqMaster = model.R_Master;
		if (model.R_Master == null)
		{
			reqMaster = new List<RequisitionMaster>();
		}
		foreach (RequisitionMaster details2 in reqMaster)
		{
			details2.Input_User = userInfo.Id;
			details2.ReqId = intIdt;
			details2.Req_Status = 1;
			details2.Requisition_Date = DateTime.Now;
			details2.Input_Date = DateTime.Now;
			db.RequisitionMaster.Add(details2);
			db.SaveChanges();
		}
		List<RequisitionDetails> reqDetails = model.R_Details;
		if (model.R_Details == null)
		{
			reqDetails = new List<RequisitionDetails>();
		}
		foreach (RequisitionDetails details in reqDetails)
		{
			details.AmendmendId = 1;
			details.EntryType = "New";
			details.Req_Id = intIdt;
			details.Input_User = userInfo.UserName;
			details.Input_Date = DateTime.Now;
			db.RequisitionDetail.Add(details);
		}
		int insertedRecords = db.SaveChanges();
			
		return Json(insertedRecords);
	}
	private static int[] StringToIntArray(string myNumbers)
	{
		List<int> myIntegers = new List<int>();
		Array.ForEach(myNumbers.Split(",".ToCharArray()), s =>
		{
			int currentInt;
			if (Int32.TryParse(s, out currentInt))
				myIntegers.Add(currentInt);
		});
		return myIntegers.ToArray();
	}
	public JsonResult SaveOtherRequestion(List<OtherRequistionDetailsNew> OtherRequistionDetailsNew, string RFM)
	{
		int ReqSeqID = db.Database.SqlQuery<int>("select isnull(max(BRSA.ReqSeqID),0)ReqSeqID from BranchReqSeqAssigns BRSA, ReqCategoryNames RCN where RCN.Id = BRSA.CategoryID  and RCN.TypeName = '" + OtherRequistionDetailsNew[0].RequestName + "'", Array.Empty<object>()).FirstOrDefault();

		//int ReqSeqID = db.Database.SqlQuery<int>("select isnull(max(BRSA.ReqSeqID),0)ReqSeqID from BranchReqSeqAssigns BRSA, ReqCategoryNames RCN where RCN.Id = BRSA.CategoryID and BRSA.BranchID = " + OtherRequistionDetailsNew[0].BranchID + " and RCN.TypeName = '" + OtherRequistionDetailsNew[0].RequestName + "'", Array.Empty<object>()).FirstOrDefault();
		if (ReqSeqID == 0)
		{
			return Json("No Requestion Sequence created for this Branch. Please communicate with authorize person for creating Requestion Sequence...");
		}
		int intIdt = db.OtherRequistionDetailsNew.Select((OtherRequistionDetailsNew u) => u.App_Req_ID).DefaultIfEmpty(1000).Max();
		intIdt++;
		UserInfo userInfo = (UserInfo)base.Session["UserProfile"];
		List<OtherRequistionDetailsNew> Other_req_Details = OtherRequistionDetailsNew;
		if (OtherRequistionDetailsNew == null)
		{
			Other_req_Details = new List<OtherRequistionDetailsNew>();
		}
		
		if (RFM!="")
		{
            List<string> names = new List<string>(RFM.Split('+'));

            //string myNumbers = RFM;
            //int[] myArray = StringToIntArray(RFM);

			foreach (string item in names)
            {
				var num = (int.Parse(item));

				var FileData = db.RequestionFileModels.Where(x => x.id == num).FirstOrDefault();
				FileData.RequestionNumber = intIdt;
				db.Entry(FileData).State = EntityState.Modified;
				db.SaveChanges();
			}				
		}
		foreach (OtherRequistionDetailsNew details in Other_req_Details)
		{
			int CheckId = db.Database.SqlQuery<int>("select ISNULL(max(CheckId),0) CheckId from BranchUserApprovals where UserId='" + details.Requested_by + "' and BranchId=" + details.BranchID + " and DepartmentId=" + details.DepartmentId, Array.Empty<object>()).FirstOrDefault();
			details.BranchReqSeqAssignsId = ReqSeqID;
			details.App_Req_ID = intIdt;
			details.Status = CheckId;
			details.RequestedDate = DateTime.Now;
			db.OtherRequistionDetailsNew.Add(details);
			List<BranchReqSeqAssign> UserFartherApproval = db.Database.SqlQuery<BranchReqSeqAssign >("select BRSA.* from BranchUserApprovals BUA,BranchReqSeqAssigns BRSA,(select max(Serial)Serials from BranchUserApprovals BUA,BranchReqSeqAssigns BRSA where Category='" + details.RequestName + "' and BranchId=" + details.BranchID + " and BRSA.ReqSeqID=" + ReqSeqID + " and UserId='"+ userInfo.UserName + "' and BUA.CheckId=BRSA.CheckID group by Serial"+
														   ")MaxSerial where Category ='" + details.RequestName + "' and BranchId = "+details.BranchID+ " and BRSA.ReqSeqID =" + ReqSeqID + " and UserId = '" + userInfo.UserName + "' and BUA.CheckId = BRSA.CheckID and MaxSerial.Serials = BRSA.Serial", Array.Empty<object>()).ToList();
            if (UserFartherApproval.Count > 0)
			{
				details.BranchReqSeqAssignsId = ReqSeqID;
				details.App_Req_ID = intIdt;
				details.Status = UserFartherApproval[1].CheckID;
				details.AmendmentId= UserFartherApproval[1].Serial;
				details.Description1 = details.Description;
				details.ApprovedBy = userInfo.UserName; 
				db.OtherRequistionDetailsNew.Add(details); 
			}
		}
		//var BranchReqSeqAssigns = db.BranchReqSeqAssign.Where((BranchReqSeqAssign i) => i.ReqSeqID == ReqSeqID).FirstOrDefault();
		//select App_Req_ID, ORDN.BranchID,BInfo.BranchName,Requested_by,ORDN.DepartmentId,DEntry.DepartmentName,max(AmendmentId)AmendmentId,FinalSerial.Serial,CompleteBy, CONVERT(varchar, ORDN.CompleteDate, 103)CompleteDate from OtherRequistionDetailsNews ORDN,(select max(Serial)Serial, ReqSeqID from BranchReqSeqAssigns group by ReqSeqID) FinalSerial,BranchInfoes BInfo, DepartmentEntries DEntry where ORDN.AmendmentId = FinalSerial.Serial and ORDN.BranchReqSeqAssignsId = FinalSerial.ReqSeqID and BInfo.BranchID = ORDN.BranchID and DEntry.DepartmentId = ORDN.DepartmentId group by App_Req_ID,BranchReqSeqAssignsId,Serial,ORDN.BranchID,Requested_by,ORDN.DepartmentId,BInfo.BranchName,DEntry.DepartmentName,CompleteBy,CompleteDate
				return Json(db.SaveChanges() + " record(s) inserted.");
	}

	public ActionResult RequestionList()
	{
		UserInfo userInfo = (UserInfo)base.Session["UserProfile"];
		if (userInfo != null)
		{
			HttpCookie useridCookie = base.Request.Cookies["um_apps_userid"];
			if (userInfo.UserName == useridCookie.Values.ToString())
			{
				HttpCookie rolesCookie = base.Request.Cookies["um_apps_roles"];
				string appsrole = ((rolesCookie == null) ? "noroles" : rolesCookie.Value.ToString());
				appsrole = appsrole.Replace("%20", " ");
				string controller_action = base.ControllerContext.RouteData.Values["controller"].ToString() + "/" + base.ControllerContext.RouteData.Values["action"].ToString();
				if (db.Database.SqlQuery<int>("select id from RoleSettings where Path='" + controller_action + "' and RoleCaption='" + appsrole + "'", Array.Empty<object>()).FirstOrDefault() != 0)
				{
					List<ReqCheckPermissionVM> rqVM = db.Database.SqlQuery<ReqCheckPermissionVM>("select CheckId,CheckName from ReqCheckLists where CheckId in(select CheckListId from ReqCheckPermissions where UserName='" + userInfo.UserName + "')", Array.Empty<object>()).ToList();
					List<SelectListItem> selectlist = new List<SelectListItem>();
					foreach (ReqCheckPermissionVM emp in rqVM)
					{
						selectlist.Add(new SelectListItem
						{
							Text = emp.CheckName,
							Value = emp.CheckId.ToString()
						});
					}
					base.ViewBag.CheckList = selectlist;
					return View();
				}
				return RedirectToAction("Index", "Home");
			}
			return RedirectToAction("Index", "Home");
		}
		return RedirectToAction("SmartLogins", "Setings");
	}

	public ActionResult Procurement_Authorization(int id)
	{
		RequisitionMaster Update = db.RequisitionMaster.Where((RequisitionMaster i) => i.ReqId == id).FirstOrDefault();
		Update.Req_Status = 2;
		Update.Procurement_Authorization_Date_Time = DateTime.Now;
		if (Update != null)
		{
			try
			{
				db.Entry(Update).State = EntityState.Modified;
				db.SaveChanges();
			}
			catch (Exception)
			{
				throw;
			}
		}
		return RedirectToAction("RequestionList");
	}

	public ActionResult CMDC_Authorozation(int id)
	{
		base.ViewBag.RequstionMaster = from m in db.RequisitionMaster
									   join b in db.Branch on m.BranchID equals b.BranchID
									   where m.ReqId == id
									   select new RequesitionVM
									   {
										   B_info = b,
										   R_master = m
									   };
		base.ViewBag.RequisitionDetail = from d in db.RequisitionDetail
										 join i in db.SubProductList on d.SubProductId equals i.SubProductId
										 where d.Req_Id == id
										 select new RequesitionVM
										 {
											 SP_List = i,
											 R_Details = d
										 };
		return View();
	}

	public JsonResult CCMDC_AuthorazationUpdate(Common model)
	{
		foreach (RequisitionMaster masterData in model.R_Master)
		{
			RequisitionMaster Update = db.RequisitionMaster.Find(masterData.id);
			Update.Req_Status = 1;
			Update.CMDC_Authorization_Date_Time = DateTime.Now;
			db.Entry(Update).State = EntityState.Modified;
			db.SaveChanges();
		}
		int insertedRecords = 0;
		foreach (RequisitionDetails item in model.R_Details)
		{
			RequisitionDetails existModel = db.RequisitionDetail.Where((RequisitionDetails i) => i.id == item.id && i.SubProductId == item.SubProductId).FirstOrDefault();
			existModel.ApprovedQty = item.ApprovedQty;
			existModel.UserCommend = item.UserCommend;
			db.Entry(existModel).State = EntityState.Modified;
			insertedRecords = db.SaveChanges();
		}
		return Json(insertedRecords);
	}

	public ActionResult Quotations(int id)
	{
		base.ViewBag.VendorEmail = new SelectList(db.Vendor.ToList(), "Vendor_Name", "Email");
		base.ViewBag.ID = id;
		RequisitionMaster Update = db.RequisitionMaster.Where((RequisitionMaster i) => i.ReqId == id).FirstOrDefault();
		Update.Req_Status = 1;
		if (Update != null)
		{
			try
			{
				db.Entry(Update).State = EntityState.Modified;
				db.SaveChanges();
				base.ViewBag.RequisitionDetail = from d in db.RequisitionDetail
												 join i in db.SubProductList on d.SubProductId equals i.SubProductId
												 where d.Req_Id == id
												 select new RequesitionVM
												 {
													 SP_List = i,
													 R_Details = d
												 };
			}
			catch (Exception)
			{
				throw;
			}
		}
		return View();
	}

	public ActionResult GetEmailAddress(int id)
	{
		base.ViewBag.VendorEmail = new SelectList(db.Vendor.ToList(), "id", "Email");
		return View();
	}

	public JsonResult SendEmail(string subject, string body, string[] email, int masterId)
	{
		IQueryable<RequesitionVM> RequisitionDetail = from d in db.RequisitionDetail
													  join i in db.ItemInfo on d.SubProductId equals i.Id
													  where d.Req_Id == masterId
													  select new RequesitionVM
													  {
														  I_info = i,
														  R_Details = d
													  };
		using (StringWriter sw = new StringWriter())
		{
			using (new HtmlTextWriter(sw))
			{
				StringBuilder sb = new StringBuilder();
				sb.Append("<div style='padding: 20px;'> ");
				sb.Append("<table class='table table-borderless'>");
				sb.Append("<tr>");
				sb.Append("<td class='col-md-4'><img src=' " + base.Server.MapPath("~") + "\\Picture\\839448blob.png' class='avatar' alt='avatar' style='height: auto; width: 100px; margin-bottom: 20px;'></td>");
				sb.Append("<td class='col-md-8'><div><h3 style = 'padding: 0px!important; margin: 0px!important'>Sundorban Courier Service</h3><div><p>Address:XYZ</p><p>Phone:01124578235</p></div></div></td>");
				sb.Append("</tr>");
				sb.Append("</table>");
				sb.Append("<br/><br/><div><h3 style='font-weight:bold; text-align:center'>Product Quertation</h3></div><br/><br/>");
				sb.Append("<table class='table'>");
				sb.Append("<tr>");
				sb.Append("<td>");
				sb.Append("<div>");
				sb.Append("<p>To,</p>");
				sb.Append("<b>Rain Computers<b>");
				sb.Append("<p>317 CPDL, Paragon City, 11-13 Sirajudulla Road Andarkilla<p>");
				sb.Append("<p>Chittagong-4000, Bangladesh<p>");
				sb.Append("</div>");
				sb.Append("</td>");
				sb.Append("<td>");
				sb.Append("<div>");
				sb.Append("<strong>Quertation No: </strong>");
				sb.Append("<b>2F63B1<b>");
				sb.Append("</div>");
				sb.Append("</td>");
				sb.Append("<td>");
				sb.Append("<div>");
				sb.Append("<strong>Submit Date:</strong>");
				sb.Append(DateTime.Now.ToString("dd-MM-yyyy"));
				sb.Append("<br/><strong>Prepared By:</strong>");
				sb.Append("Nadim");
				sb.Append("</div>");
				sb.Append("</td>");
				sb.Append("</tr>");
				sb.Append("</table>");
				sb.Append("<br />");
				sb.Append("<table border = '1'>");
				sb.Append("<tr>");
				sb.Append("<th style='font-weight: bold; background-color: coral;'>Serial</th><th>Product Name</th><th>Description</th><th>Quantity</th>");
				int serialNo = 0;
				sb.Append("</tr>");
				foreach (RequesitionVM items in RequisitionDetail)
				{
					serialNo++;
					sb.Append("<tr>");
					sb.Append("<td>" + serialNo + "</td><td>" + items.I_info.ItemName + "</td><td>" + items.R_Details.Description + "</td><td>" + items.R_Details.Qty + "</td><td>");
					sb.Append("</tr>");
				}
				sb.Append("</table>");
				sb.Append("<table class='table'>");
				sb.Append("<tr style='font-weight: bold;'><th> Name </th><th> Signature </th><th> Date </th></tr> ");
				sb.Append("<tr><td> Abdul hakim </td><td> Abdul hakim </td><td class='text-right'>" + DateTime.Today.ToString("dd-MM-yyyy") + "</td></tr>");
				sb.Append("<tr><td style='margin-bottom:2cm;' colspan='5'><small>** This is a system generated requestion, and it does not  required any seal or singnature.</small></td></tr>");
				sb.Append("</table>");
				sb.Append("</div>");
				sb.Append("<script src='https://cdnjs.cloudflare.com/ajax/libs/jquery/3.3.1/jquery.min.js'></script>");
				StringReader sr = new StringReader(sb.ToString());
				Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
				HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
				using (MemoryStream memoryStream = new MemoryStream())
                {
					PdfWriter writer = PdfWriter.GetInstance(pdfDoc, memoryStream);
					pdfDoc.Open();
					htmlparser.Parse(sr);
					pdfDoc.Close();
					byte[] bytes = memoryStream.ToArray();
					memoryStream.Close();
					for (int j = 0; j < email.Length; j++)
					{
						MailMessage mm = new MailMessage("mailuttam3@gmail.com", email[j]);
						mm.Subject = subject;
						mm.Body = body;
						mm.Attachments.Add(new Attachment(new MemoryStream(bytes), "Product-Requestion.pdf"));
						mm.IsBodyHtml = true;
						SmtpClient smtp = new SmtpClient();
						smtp.Host = "smtp.gmail.com";
						smtp.Port = 587;
						smtp.UseDefaultCredentials = false;
						smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
						NetworkCredential NetworkCred = new NetworkCredential();
						NetworkCred.UserName = "mailuttam3@gmail.com";
						NetworkCred.Password = "mailuttam3@";
						smtp.Credentials = NetworkCred;
						smtp.EnableSsl = true;
						smtp.Send(mm);
					}
				}				
			}
		}
		return Json("Send Successfully");
	}

	public JsonResult RequestionPriceQuertationUpdate(Common model)
	{
		List<PriceQuotation> pq_Details = model.PQ_Details;
		if (model.PQ_Details == null)
		{
			pq_Details = new List<PriceQuotation>();
		}
		int MasterId = 0;
		using (List<PriceQuotation>.Enumerator enumerator = pq_Details.GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				PriceQuotation checkEditOrNew = enumerator.Current;
				MasterId = (from q in db.PriceQuotations
							where q.MasterID == checkEditOrNew.MasterID && q.vendorID == checkEditOrNew.vendorID
							select q into O
							select O.MasterID).FirstOrDefault();
			}
		}
		if (MasterId != 0)
		{
			foreach (PriceQuotation details2 in pq_Details)
			{
				PriceQuotation exCategory = db.PriceQuotations.Where((PriceQuotation c) => c.MasterID == details2.MasterID && c.ItemID == details2.ItemID && c.vendorID == details2.vendorID).FirstOrDefault();
				exCategory.SubmissionDate = DateTime.Now;
				exCategory.price = details2.price;
				exCategory.Warienty = details2.Warienty;
				exCategory.MadeBy = details2.MadeBy;
				db.Entry(exCategory).State = EntityState.Modified;
				db.SaveChanges();
			}
		}
		else
		{
			foreach (PriceQuotation details in pq_Details)
			{
				details.Unit = details.Unit.Replace("@\t\r\n", "").Trim();
				details.Description = details.Description.Replace("@\t\r\n", "").Trim();
				details.SubmissionDate = DateTime.Now;
				db.PriceQuotations.Add(details);
			}
		}
		int insertedRecords = db.SaveChanges();
		return Json(insertedRecords);
	}

	public ActionResult DeclinePriceQuertation(int MasterID, int QuotationID, int vendorID)
	{
		List<PriceQuotation> Update = db.PriceQuotations.Where((PriceQuotation o) => o.MasterID == MasterID && o.vendorID == vendorID).ToList();
		foreach (PriceQuotation item in Update)
		{
			item.E_Status = 2;
			db.Entry(item).State = EntityState.Modified;
			db.SaveChanges();
		}
		return RedirectToAction("PriceQuertation");
	}

	public ActionResult PQ_CMDC_Authorozation(int MasterID, int QuotationID, int vendorID)
	{
		base.ViewBag.Quartations = from d in db.PriceQuotations
								   join i in db.ItemInfo on d.ItemID equals i.Id
								   where d.MasterID == MasterID && d.vendorID == vendorID
								   select new RequesitionVM
								   {
									   I_info = i,
									   PQ_Info = d
								   };
		return View();
	}

	public ActionResult PQ_CMDC_Accept(int MasterID, int QuotationID, int vendorID)
	{
		List<PriceQuotation> accept = db.PriceQuotations.Where((PriceQuotation o) => o.MasterID == MasterID && o.vendorID == vendorID).ToList();
		foreach (PriceQuotation item in accept)
		{
			item.E_Status = 1;
			db.Entry(item).State = EntityState.Modified;
			db.SaveChanges();
		}
		return RedirectToAction("PriceQuertation");
	}

	public ActionResult GetPriceQuertation()
	{
		try
		{
			List<PriceListVM> query = db.Database.SqlQuery<PriceListVM>("select(MasterId)MasterID, Requisition_Date,U_Info.Name as Requsitor_Name,B_Info.BranchName  from RequisitionMasters RM, BranchInfoes B_Info,UserInfoes U_Info where RM.Requested_Branch=B_Info.id and RM.Requested_by= U_Info.Id", Array.Empty<object>()).ToList();
			base.ViewBag.PriceQuertationList = query;
			return Json(new
			{
				data = query
			}, JsonRequestBehavior.AllowGet);
		}
		catch (EntityException ex)
		{
			return Content(" Connection to Database Failed." + ex);
		}
	}

	public ActionResult ReqCheckIndex()
	{
		UserInfo userInfo = (UserInfo)base.Session["UserProfile"];
		if (userInfo != null)
		{
			HttpCookie useridCookie = base.Request.Cookies["um_apps_userid"];
			if (userInfo.UserName == useridCookie.Values.ToString())
			{
				HttpCookie rolesCookie = base.Request.Cookies["um_apps_roles"];
				string appsrole = ((rolesCookie == null) ? "noroles" : rolesCookie.Value.ToString());
				appsrole = appsrole.Replace("%20", " ");
				string controller_action = base.ControllerContext.RouteData.Values["controller"].ToString() + "/" + base.ControllerContext.RouteData.Values["action"].ToString();
				if (db.Database.SqlQuery<int>("select id from RoleSettings where Path='" + controller_action + "' and RoleCaption='" + appsrole + "'", Array.Empty<object>()).FirstOrDefault() != 0)
				{
					List<ReqCheckList> ReqCheckList = db.ReqCheckList.ToList();
					base.ViewBag.TypeName = new SelectList(db.ReqTypeName.ToList(), "id", "TypeName");
					return View(ReqCheckList);
				}
				return RedirectToAction("Index", "Home");
			}
			return RedirectToAction("Index", "Home");
		}
		return RedirectToAction("SmartLogins", "Setings");
	}

	[HttpPost]
	public ActionResult AddReqCheck(ReqCheckList objReqCheck)
	{
		UserInfo userInfo = (UserInfo)base.Session["UserProfile"];
		if (userInfo != null)
		{
			int intIdt = db.ReqCheckList.Select((ReqCheckList u) => u.CheckId).DefaultIfEmpty(0).Max();
			objReqCheck.CheckId = intIdt + 1;
			objReqCheck.Input_Date = DateTime.Now;
			objReqCheck.Input_User = userInfo.Id;
			db.ReqCheckList.Add(objReqCheck);
			db.SaveChanges();
			long id = objReqCheck.id;
		}			
		return RedirectToAction("ReqCheckIndex");
	}

	public ActionResult EditReqCheck(int id)
	{
		ReqCheckList ReqCheck = db.ReqCheckList.Single((ReqCheckList p) => p.id == id);
		return View(ReqCheck);
	}

	[HttpPost]
	public ActionResult EditReqCheck(ReqCheckList model)
	{
		if (base.ModelState.IsValid)
		{
			UserInfo userInfo = (UserInfo)base.Session["UserProfile"];
			model.Edit_Date = DateTime.Now;
			model.Edit_User = userInfo.Id;
			db.Entry(model).State = EntityState.Modified;
			db.SaveChanges();
			return RedirectToAction("ReqCheckIndex");
		}
		return View(model);
	}

	public ActionResult RequsitionDetails(int id)
	{
		base.ViewBag.RequisitionDetais = from d in db.RequisitionDetail
										 join i in db.SubProductList on d.SubProductId equals i.SubProductId
										 where d.Req_Id == id
										 select new RequesitionVM
										 {
											 SP_List = i,
											 R_Details = d
										 };
		base.ViewBag.RequstionMaster = from m in db.RequisitionMaster
									   join b in db.Branch on m.BranchID equals b.BranchID
									   join r in db.ReqTypeName on m.Requsition_Type equals r.id
									   where m.ReqId == id
									   select new RequesitionVM
									   {
										   B_info = b,
										   R_master = m,
										   Req_Type = r
									   };
		return View();
	}

	public ActionResult RequsitionEdit(int ReqId)
	{
		base.ViewBag.SubProductList2 = new SelectList(db.SubProductList, "SubProductId", "ProductModel");
		base.ViewBag.ReqType = new SelectList(db.ReqTypeName, "Id", "TypeName");
		base.ViewBag.Branch = new SelectList(db.Branch, "BranchID", "BranchName");
		base.ViewBag.Departments = new SelectList(db.DepartmentEntry, "DepartmentId", "DepartmentName");
		int maxAmendmendId = (from f in db.RequisitionDetail
							  where f.Req_Id == ReqId
							  select f into u
							  select u.AmendmendId).DefaultIfEmpty(0).Max();
		base.ViewBag.RequisitionDetais = from d in db.RequisitionDetail
										 join i in db.SubProductList on d.SubProductId equals i.SubProductId
										 where d.Req_Id == ReqId && d.Req_Status == 0
										 select new RequesitionVM
										 {
											 SP_List = i,
											 R_Details = d
										 };
		base.ViewBag.RequstionMaster = from m in db.RequisitionMaster
									   join b in db.Branch on m.BranchID equals b.BranchID
									   join r in db.ReqTypeName on m.Requsition_Type equals r.id
									   where m.ReqId == ReqId
									   select new RequesitionVM
									   {
										   B_info = b,
										   R_master = m,
										   Req_Type = r
									   };
		return View();
	}

	public JsonResult EditRequestion(Common model)
	{
		int insertedRecords = 0;
		int ReqId = 0;
		UserInfo userInfo = (UserInfo)base.Session["UserProfile"];
		List<RequisitionMaster> reqMaster = model.R_Master;
		if (model.R_Master == null)
		{
			reqMaster = new List<RequisitionMaster>();
		}
		foreach (RequisitionMaster details2 in reqMaster)
		{
			ReqId = details2.ReqId;
			details2.Edit_Date = DateTime.Now;
			details2.Edit_User = userInfo.Id;
			db.Entry(details2).State = EntityState.Modified;
		}
		List<RequisitionDetails> reqDetails = model.R_Details;
		if (model.R_Details == null)
		{
			reqDetails = new List<RequisitionDetails>();
		}
		int subProductIds = model.R_Details[0].SubProductId;
		foreach (RequisitionDetails some in db.RequisitionDetail.Where((RequisitionDetails x) => x.Req_Id == ReqId && x.Req_Status == 0 && x.SubProductId == subProductIds).ToList())
		{
			some.Req_Status = 1;
			db.Entry(some).State = EntityState.Modified;
			db.SaveChanges();
		}
		int intIdt = (from f in db.RequisitionDetail
					  where f.Req_Id == ReqId && f.SubProductId == subProductIds
					  select f into u
					  select u.AmendmendId).DefaultIfEmpty(0).Max();
		intIdt++;
		foreach (RequisitionDetails details in reqDetails)
		{
			details.Req_Id = ReqId;
			details.Req_Status = 0;
			details.AmendmendId = intIdt;
			details.Input_User = userInfo.UserName;
			details.Input_Date = DateTime.Now;
			db.RequisitionDetail.Add(details);
			insertedRecords += db.SaveChanges();
		}
		return Json(insertedRecords);
	}

	public JsonResult AmendmendEditRequestion(Common model)
	{
		int insertedRecords = 0;
		int ReqId = 0;
		UserInfo userInfo = (UserInfo)base.Session["UserProfile"];
		List<RequisitionMaster> reqMaster = model.R_Master;
		if (model.R_Master == null)
		{
			reqMaster = new List<RequisitionMaster>();
		}
		foreach (RequisitionMaster details2 in reqMaster)
		{
			ReqId = details2.ReqId;
			details2.Edit_Date = DateTime.Now;
			details2.Edit_User = userInfo.Id;
			db.Entry(details2).State = EntityState.Modified;
		}
		List<RequisitionDetails> reqDetails = model.R_Details;
		if (model.R_Details == null)
		{
			reqDetails = new List<RequisitionDetails>();
		}
		foreach (RequisitionDetails details in reqDetails)
		{
			details.Req_Id = ReqId;
			details.Req_Status = 0;
			details.AmendmendId = 1;
			details.Input_User = userInfo.UserName;
			details.Input_Date = DateTime.Now;
			db.RequisitionDetail.Add(details);
			insertedRecords += db.SaveChanges();
		}
		return Json(insertedRecords);
	}

	public JsonResult AmendmendDeleteItem(Common model)
	{
		int insertedRecords = 0;
		int ReqId = 0;
		UserInfo userInfo = (UserInfo)base.Session["UserProfile"];
		List<RequisitionMaster> reqMaster = model.R_Master;
		if (model.R_Master == null)
		{
			reqMaster = new List<RequisitionMaster>();
		}
		foreach (RequisitionMaster details2 in reqMaster)
		{
			ReqId = details2.ReqId;
			details2.Edit_Date = DateTime.Now;
			details2.Edit_User = 1;
			db.Entry(details2).State = EntityState.Modified;
		}
		List<RequisitionDetails> reqDetails = model.R_Details;
		if (model.R_Details == null)
		{
			reqDetails = new List<RequisitionDetails>();
		}
		foreach (RequisitionDetails details in reqDetails)
		{
			details.Req_Status = 1;
			details.Input_User = userInfo.UserName;
			details.Input_Date = DateTime.Now;
			db.Entry(details).State = EntityState.Modified;
			insertedRecords += db.SaveChanges();
		}
		return Json(insertedRecords);
	}

	public ActionResult ReqCheckPermissionIndex()
	{
		base.ViewBag.UserList = new SelectList(db.UserInfo, "UserName", "UserName");
		return View();
	}

	public JsonResult RequestionListData(int checkId)
	{
		UserInfo userInfo = (UserInfo)base.Session["UserProfile"];
		List<RequesitionList> RequestionList = db.Database.SqlQuery<RequesitionList>("select R_master.ReqId,B_info.BranchName,R_master.Requisition_Date,R_Type_Name.TypeName, U_Info.Name,case when R_master.Req_Status = 0 then 'Waiting For CMDC' when R_master.Req_Status = 1 then 'CMDC Accept >> Waiting For Procurement' when R_master.Req_Status = 2 then 'Procurement Accept >> Waiting For Price Qurtation' when R_master.Req_Status = 3 then 'Price Qurtation are waiting for CS'when R_master.Req_Status = 4 then 'CS are Waiting for CMDC Approved' when R_master.Req_Status = 5 then 'CS are Waiting for Procurement Approved' when R_master.Req_Status = 6 then 'CS are Waiting for PO' when R_master.Req_Status = 7 then 'PO has been generated, Waiting for Vendor' end as Req_Status, details.Description  from BranchInfoes B_info, RequisitionMasters R_master,ReqTypeNames R_Type_Name, UserInfoes U_Info,(select CheckListId-1 as CheckListId from ReqCheckPermissions where CheckListId=" + checkId + " and UserName='" + userInfo.UserName + "') CheckReq, (SELECT Req_Id, Description = STUFF((SELECT ', ' + Description FROM RequisitionDetails b WHERE b.Req_Id = a.Req_Id FOR XML PATH('')), 1, 2, '') FROM RequisitionDetails a GROUP BY Req_Id) details where R_master.ReqId = details.Req_Id and R_master.BranchId = B_info.BranchID and R_master.Requested_by = U_Info.Id and R_Type_Name.Id=R_master.Requsition_Type and R_master.Req_Status=CheckReq.CheckListId", Array.Empty<object>()).ToList();
		return new JsonResult
		{
			Data = RequestionList,
			JsonRequestBehavior = JsonRequestBehavior.AllowGet
		};
	}

	public ActionResult RequestionPurchageReport()
	{
		UserInfo userInfo = (UserInfo)base.Session["UserProfile"];
		if (userInfo != null)
		{
			base.ViewBag.RequestionList = db.Database.SqlQuery<RequesitionList>("select R_master.ReqId,B_info.BranchName,R_master.Edit_Date,R_master.Requisition_Date,R_Type_Name.TypeName, U_Info.Name,case when R_master.Req_Status = 0 then 'Waiting For CMDC' when R_master.Req_Status = 1 then 'CMDC Accept >> Waiting For Procurement' when R_master.Req_Status = 2 then 'Procurement Accept >> Waiting For Price Qurtation' when R_master.Req_Status = 3 then 'Price Qurtation are waiting for CS'when R_master.Req_Status = 4 then 'CS are Waiting for CMDC Approved' when R_master.Req_Status = 5 then 'CS are Waiting for Procurement Approved' when R_master.Req_Status = 6 then 'CS are Waiting for PO' when R_master.Req_Status = 7 then 'PO has been generated, Waiting for Vendor' end as Req_Status, details.Description  from BranchInfoes B_info, RequisitionMasters R_master,ReqTypeNames R_Type_Name, UserInfoes U_Info, (SELECT Req_Id, Description = STUFF((SELECT ', ' + Description FROM RequisitionDetails b WHERE b.Req_Status=0 and b.Req_Id = a.Req_Id FOR XML PATH('')), 1, 2, '') FROM RequisitionDetails a GROUP BY Req_Id) details where R_master.ReqId = details.Req_Id and R_master.BranchID = B_info.BranchID and R_master.Requested_by = U_Info.Id and R_Type_Name.Id=R_master.Requsition_Type and R_master.BranchID=" + userInfo.Branch_Id, Array.Empty<object>()).ToList();
		}
		return View();
	}

	public ActionResult OtherRequestionReport()
	{
		UserInfo userInfo = (UserInfo)base.Session["UserProfile"];
		if (userInfo != null)
		{
			base.ViewBag.OthersRequisition = db.Database.SqlQuery<OtherRequistionDetailsVM>("select DE.DepartmentName, Details.Requested_by UserName,Binfo.BranchName,RTN.TypeName,Details.App_Req_ID,Details.CaptionStatus,details.RequestedDate from  DepartmentEntries DE, ReqTypeNames RTN,BranchInfoes Binfo, (select App_Req_ID, BranchID, Requested_by, Requsition_Type, DepartmentId,CaptionStatus,CONVERT(varchar, RequestedDate, 103)RequestedDate from OtherRequistionDetails group by CaptionStatus,App_Req_ID, BranchID, Requested_by, Requsition_Type, DepartmentId,CONVERT(varchar, RequestedDate, 103) ) Details where DE.DepartmentId = Details.DepartmentId and RTN.id = Details.Requsition_Type and Binfo.BranchID = Details.BranchID", Array.Empty<object>()).ToList();
			return View();
		}
		return RedirectToAction("SmartLogins", "Setings");
	}

	public ActionResult requestionReport(int detailsId)
	{
		UserInfo userInfo = (UserInfo)base.Session["UserProfile"];
		if (userInfo != null)
		{
			base.ViewBag.OthersRequisition = db.Database.SqlQuery<OtherRequistionDetailsVM>("select ORD.App_Req_ID,ORD.RequestName,ORD.Description,binfo.BranchName,ORD.Requested_by,ORD.ApprovedBy,ORD.CompleteBy,DE.DepartmentName,RTn.TypeName,RTN.Destination,RTN.Designation,RTN.Address,ORD.ItContactPerson,CONVERT(varchar, ORD.RequestedDate, 103)RequestedDate,ORD.ApprovedDate,ORD.CompleteDate from OtherRequistionDetails ORD, DepartmentEntries DE, ReqTypeNames RTN, BranchInfoes Binfo where DE.DepartmentId = ORD.DepartmentId and RTN.id = ORD.Requsition_Type and Binfo.BranchID = ORD.BranchID  and ORD.App_Req_ID=" + detailsId, Array.Empty<object>()).ToList();
		}
		return View();
	}

	public ActionResult PurchageRequestionReport(int id)
	{
		UserInfo userInfo = (UserInfo)base.Session["UserProfile"];
		if (userInfo != null)
		{
			base.ViewBag.RequisitionDetais = from d in db.RequisitionDetail
											 join i in db.SubProductList on d.SubProductId equals i.SubProductId
											 where d.Req_Id == id
											 select new RequesitionVM
											 {
												 SP_List = i,
												 R_Details = d
											 };
			base.ViewBag.RequstionMaster = from m in db.RequisitionMaster
										   join b in db.Branch on m.BranchID equals b.BranchID
										   join r in db.ReqTypeName on m.Requsition_Type equals r.id
										   where m.ReqId == id
										   select new RequesitionVM
										   {
											   B_info = b,
											   R_master = m,
											   Req_Type = r
										   };
		}
		return View();
	}

	[RequireHttps]
	public ActionResult BranchApprovalPending()
	{
		UserInfo userInfo = (UserInfo)base.Session["UserProfile"];
		if (userInfo != null)
		{
			HttpCookie useridCookie = base.Request.Cookies["um_apps_userid"];
			if (userInfo.UserName == useridCookie.Value.ToString())
			{
				HttpCookie rolesCookie = base.Request.Cookies["um_apps_roles"];
				HttpCookie branchcodeCookie = base.Request.Cookies["um_apps_branchcode"];
				string appsrole = ((rolesCookie == null) ? "noroles" : rolesCookie.Value.ToString());
				string branchcode = ((branchcodeCookie == null) ? "nobranchcode" : branchcodeCookie.Value.ToString());
				appsrole = appsrole.Replace("%20", " ");
				int branchId = db.Database.SqlQuery<int>("select BranchID from BranchInfoes where BranchCode='" + branchcode + "'", Array.Empty<object>()).FirstOrDefault();
				string controller_action = base.ControllerContext.RouteData.Values["controller"].ToString() + "/" + base.ControllerContext.RouteData.Values["action"].ToString();
				if (db.Database.SqlQuery<int>("select id from RoleSettings where Path='" + controller_action + "' and RoleCaption='" + appsrole + "'", Array.Empty<object>()).FirstOrDefault() != 0)
				{
					base.ViewBag.OthersRequisition = db.Database.SqlQuery<OtherRequistionDetailsVM>("select DE.DepartmentName,Details.Requested_by UserName,Binfo.BranchName,RTN.TypeName,Details.App_Req_ID from DepartmentEntries DE,BranchInfoes Binfo,ReqTypeNames RTN,(select App_Req_ID, BranchID, Requested_by, Requsition_Type, DepartmentId from OtherRequistionDetails where CaptionStatus = 'Branch Approval' and BranchID=" + branchId + " group by App_Req_ID, BranchID, Requested_by, Requsition_Type, DepartmentId) Details where Details.DepartmentId = DE.DepartmentId and Binfo.BranchID = Details.BranchID and RTN.id = Details.Requsition_Type", Array.Empty<object>()).ToList();
					return View();
				}
				return RedirectToAction("Index", "Home");
			}
			return RedirectToAction("Index", "Home");
		}
		return RedirectToAction("SmartLogins", "Setings");
	}

	public JsonResult BranchApprovalPendingData(string fDate, string tDate, string branchCode)
	{
		Convert.ToDateTime(fDate);
		Convert.ToDateTime(tDate);
		int branchId = db.Database.SqlQuery<int>("select BranchID from BranchInfoes where BranchCode='" + branchCode + "'", Array.Empty<object>()).FirstOrDefault();
		List<OtherRequistionDetailsVM> data = new List<OtherRequistionDetailsVM>();
		List<OtherRequistionDetailsVM> itemData = new List<OtherRequistionDetailsVM>();
		data = db.Database.SqlQuery<OtherRequistionDetailsVM>("select DE.DepartmentName,Details.Requested_by UserName,Binfo.BranchName,RTN.TypeName,Details.App_Req_ID from DepartmentEntries DE,BranchInfoes Binfo,ReqTypeNames RTN, (select App_Req_ID, BranchID, Requested_by, Requsition_Type, DepartmentId from OtherRequistionDetails where CaptionStatus = 'Branch Approval' and RequestedDate between '" + fDate + "' AND '" + tDate + "' and BranchID = " + branchId + " group by App_Req_ID, BranchID, Requested_by, Requsition_Type, DepartmentId) Details where Details.DepartmentId = DE.DepartmentId and Binfo.BranchID = Details.BranchID and RTN.id = Details.Requsition_Type ", Array.Empty<object>()).ToList();
		return new JsonResult
		{
			Data = data,
			JsonRequestBehavior = JsonRequestBehavior.AllowGet
		};
	}

	public ActionResult BranchApprovalPendingDetails(int AppReqID, string BranchCode)
	{
		UserInfo userInfo = (UserInfo)base.Session["UserProfile"];
		int branchId = db.Database.SqlQuery<int>("select BranchID from BranchInfoes where BranchCode='" + BranchCode + "'", Array.Empty<object>()).FirstOrDefault();
		if (userInfo == null)
		{
			return RedirectToAction("SmartLogins", "Setings");
		}
		base.ViewBag.AppReqID = AppReqID;
		base.ViewBag.OthersRequisitionByID = db.Database.SqlQuery<OtherRequistionDetailsVM>("select DE.DepartmentName,ORD.Requested_by,Binfo.BranchName,RTN.TypeName,ORD.Description,ORD.RequestName,ORD.App_Req_ID from OtherRequistionDetails ORD,DepartmentEntries DE,ReqTypeNames RTN, BranchInfoes Binfo where DE.DepartmentId = ORD.DepartmentId and RTN.id = ORD.Requsition_Type and Binfo.BranchID = ORD.BranchID and ORD.CaptionStatus = 'Branch Approval' and ORD.BranchID=" + branchId + " and ORD.App_Req_ID = " + AppReqID, Array.Empty<object>()).ToList();
		return View();
	}

	[RequireHttps]
	public ActionResult ApplicationRequestionPending()
	{
		UserInfo userInfo = (UserInfo)base.Session["UserProfile"];
		if (userInfo != null)
		{
			HttpCookie useridCookie = base.Request.Cookies["um_apps_userid"];
			if (userInfo.UserName == useridCookie.Value.ToString())
			{
				HttpCookie rolesCookie = base.Request.Cookies["um_apps_roles"];
				string appsrole = ((rolesCookie == null) ? "noroles" : rolesCookie.Value.ToString());
				appsrole = appsrole.Replace("%20", " ");
				string controller_action = base.ControllerContext.RouteData.Values["controller"].ToString() + "/" + base.ControllerContext.RouteData.Values["action"].ToString();
				if (db.Database.SqlQuery<int>("select id from RoleSettings where Path='" + controller_action + "' and RoleCaption='" + appsrole + "'", Array.Empty<object>()).FirstOrDefault() != 0)
				{
					base.ViewBag.OthersRequisition = db.Database.SqlQuery<OtherRequistionDetailsVM>("select DE.DepartmentName,Details.Requested_by UserName,Binfo.BranchName,RTN.TypeName,Details.App_Req_ID from DepartmentEntries DE,BranchInfoes Binfo,ReqTypeNames RTN,(select App_Req_ID, BranchID, Requested_by, Requsition_Type, DepartmentId from OtherRequistionDetails where CaptionStatus = 'Pending' group by App_Req_ID, BranchID, Requested_by, Requsition_Type, DepartmentId) Details where Details.DepartmentId = DE.DepartmentId and Binfo.BranchID = Details.BranchID and RTN.id = Details.Requsition_Type", Array.Empty<object>()).ToList();
					return View();
				}
				return RedirectToAction("Index", "Home");
			}
			return RedirectToAction("Index", "Home");
		}
		return RedirectToAction("SmartLogins", "Setings");
	}

	public JsonResult ApplicationRequestionPendingData(string fDate, string tDate)
	{
		Convert.ToDateTime(fDate);
		Convert.ToDateTime(tDate);
		List<OtherRequistionDetailsVM> data = new List<OtherRequistionDetailsVM>();
		List<OtherRequistionDetailsVM> itemData = new List<OtherRequistionDetailsVM>();
		data = db.Database.SqlQuery<OtherRequistionDetailsVM>("select DE.DepartmentName,Uinfo.UserName,Binfo.BranchName,RTN.TypeName,Details.App_Req_ID,Details.RequestedDate from  DepartmentEntries DE, ReqTypeNames RTN, UserInfoes Uinfo, BranchInfoes Binfo, (select App_Req_ID, BranchID, Requested_by, Requsition_Type, DepartmentId, RequestedDate from OtherRequistionDetails where CaptionStatus='Pending' group by RequestedDate,App_Req_ID, BranchID, Requested_by, Requsition_Type, DepartmentId) Details where DE.DepartmentId = Details.DepartmentId and RTN.id = Details.Requsition_Type and Uinfo.Id = Details.Requested_by and Binfo.BranchID = Details.BranchID and Details.RequestedDate between '" + fDate + "' AND '" + tDate + "'", Array.Empty<object>()).ToList();
		return new JsonResult
		{
			Data = data,
			JsonRequestBehavior = JsonRequestBehavior.AllowGet
		};
	}

	public ActionResult RequestionPendingByID(int AppReqID)
	{
		UserInfo userInfo = (UserInfo)base.Session["UserProfile"];
		if (userInfo == null)
		{
			return RedirectToAction("SmartLogins", "Setings");
		}
		base.ViewBag.AppReqID = AppReqID;
		base.ViewBag.OthersRequisitionByID = db.Database.SqlQuery<OtherRequistionDetailsVM>("select DE.DepartmentName,ORD.Requested_by,Binfo.BranchName,RTN.TypeName,ORD.Description,ORD.RequestName,ORD.App_Req_ID from OtherRequistionDetails ORD,DepartmentEntries DE,ReqTypeNames RTN, BranchInfoes Binfo where DE.DepartmentId = ORD.DepartmentId and RTN.id = ORD.Requsition_Type and Binfo.BranchID = ORD.BranchID and ORD.CaptionStatus = 'Pending' and ORD.App_Req_ID = " + AppReqID, Array.Empty<object>()).ToList();
		return View();
	}

	public JsonResult ReqAuthorization(List<OtherRequistionDetails> model)
	{
		int insertedRecords = 0;
		if (base.ModelState.IsValid)
		{
			UserInfo userInfo = (UserInfo)base.Session["UserProfile"];
			foreach (OtherRequistionDetails modeldata in model)
			{
				OtherRequistionDetails existModel = db.OtherRequisitionDetail.Where((OtherRequistionDetails i) => i.App_Req_ID == modeldata.App_Req_ID && i.RequestName == modeldata.RequestName).FirstOrDefault();
				existModel.Description1 = modeldata.Description1;
				existModel.ApprovedDate = DateTime.Now;
				existModel.ApprovedBy = userInfo.UserName;
				existModel.CaptionStatus = "Approved";
				existModel.AmendmentID = 0;
				db.Entry(existModel).State = EntityState.Modified;
				db.SaveChanges();
				insertedRecords++;
			}
		}
		return Json(insertedRecords);
	}

	public JsonResult BranchAuthorization(List<OtherRequistionDetails> model)
	{
		int insertedRecords = 0;
		if (base.ModelState.IsValid)
		{
			UserInfo userInfo = (UserInfo)base.Session["UserProfile"];
			foreach (OtherRequistionDetails modeldata in model)
			{
				OtherRequistionDetails existModel = db.OtherRequisitionDetail.Where((OtherRequistionDetails i) => i.App_Req_ID == modeldata.App_Req_ID && i.RequestName == modeldata.RequestName).FirstOrDefault();
				existModel.Description1 = modeldata.Description1;
				existModel.BranchApprovedDate = DateTime.Now;
				existModel.BranchApprovedBy = userInfo.UserName;
				existModel.CaptionStatus = "Pending";
				existModel.AmendmentID = 0;
				db.Entry(existModel).State = EntityState.Modified;
				db.SaveChanges();
				insertedRecords++;
			}
		}
		return Json(insertedRecords);
	}

	[RequireHttps]
	public ActionResult ApplicationRequestionApprovedList()
	{
		UserInfo userInfo = (UserInfo)base.Session["UserProfile"];
		if (userInfo != null)
		{
			HttpCookie useridCookie = base.Request.Cookies["um_apps_userid"];
			if (userInfo.UserName == useridCookie.Value.ToString())
			{
				HttpCookie rolesCookie = base.Request.Cookies["um_apps_roles"];
				string appsrole = ((rolesCookie == null) ? "noroles" : rolesCookie.Value.ToString());
				appsrole = appsrole.Replace("%20", " ");
				string controller_action = base.ControllerContext.RouteData.Values["controller"].ToString() + "/" + base.ControllerContext.RouteData.Values["action"].ToString();
				if (db.Database.SqlQuery<int>("select id from RoleSettings where Path='" + controller_action + "' and RoleCaption='" + appsrole + "'", Array.Empty<object>()).FirstOrDefault() != 0)
				{
					base.ViewBag.OthersRequisition = db.Database.SqlQuery<OtherRequistionDetailsVM>("select DE.DepartmentName,Details.Requested_by UserName,Binfo.BranchName,RTN.TypeName,Details.App_Req_ID from  DepartmentEntries DE, ReqTypeNames RTN, BranchInfoes Binfo, (select App_Req_ID, BranchID, Requested_by, Requsition_Type, DepartmentId from OtherRequistionDetails where CaptionStatus='Approved' group by App_Req_ID, BranchID, Requested_by, Requsition_Type, DepartmentId) Details where DE.DepartmentId = Details.DepartmentId and RTN.id = Details.Requsition_Type and Binfo.BranchID = Details.BranchID", Array.Empty<object>()).ToList();
					return View();
				}
				return RedirectToAction("Index", "Home");
			}
			return RedirectToAction("Index", "Home");
		}
		return RedirectToAction("SmartLogins", "Setings");
	}

	public JsonResult ApplicationRequestionApprovedData(string fDate, string tDate)
	{
		Convert.ToDateTime(fDate);
		Convert.ToDateTime(tDate);
		List<OtherRequistionDetailsVM> data = new List<OtherRequistionDetailsVM>();
		List<OtherRequistionDetailsVM> itemData = new List<OtherRequistionDetailsVM>();
		data = db.Database.SqlQuery<OtherRequistionDetailsVM>("select DE.DepartmentName,Uinfo.UserName,Binfo.BranchName,RTN.TypeName,Details.App_Req_ID,Details.RequestedDate from  DepartmentEntries DE, ReqTypeNames RTN, UserInfoes Uinfo, BranchInfoes Binfo, (select App_Req_ID, BranchID, Requested_by, Requsition_Type, DepartmentId, RequestedDate from OtherRequistionDetails where CaptionStatus='Approved' and AmendmentID=1 group by RequestedDate,App_Req_ID, BranchID, Requested_by, Requsition_Type, DepartmentId) Details where DE.DepartmentId = Details.DepartmentId and RTN.id = Details.Requsition_Type and Uinfo.Id = Details.Requested_by and Binfo.BranchID = Details.BranchID and Details.RequestedDate between '" + fDate + "' AND '" + tDate + "'", Array.Empty<object>()).ToList();
		return new JsonResult
		{
			Data = data,
			JsonRequestBehavior = JsonRequestBehavior.AllowGet
		};
	}

	public ActionResult RequestionApprovedByID(int AppReqID)
	{
		UserInfo userInfo = (UserInfo)base.Session["UserProfile"];
		if (userInfo == null)
		{
			return RedirectToAction("SmartLogins", "Setings");
		}
		base.ViewBag.AppReqID = AppReqID;
		base.ViewBag.OthersRequisitionByID = db.Database.SqlQuery<OtherRequistionDetailsVM>("select DE.DepartmentName,ORD.Requested_by UserName,Binfo.BranchName,RTN.TypeName,ORD.Description,ORD.RequestName,ORD.App_Req_ID,ORD.Description1 from OtherRequistionDetails ORD,DepartmentEntries DE,ReqTypeNames RTN, BranchInfoes Binfo where DE.DepartmentId = ORD.DepartmentId and RTN.id = ORD.Requsition_Type  and Binfo.BranchID = ORD.BranchID and ORD.CaptionStatus = 'Approved' and ORD.AmendmentID=0 and ORD.App_Req_ID = " + AppReqID, Array.Empty<object>()).ToList();
		return View();
	}

	public JsonResult AppReqDone(List<OtherRequistionDetails> model)
	{
		int insertedRecords = 0;
		if (base.ModelState.IsValid)
		{
			UserInfo userInfo = (UserInfo)base.Session["UserProfile"];
			foreach (OtherRequistionDetails modeldata in model)
			{
				OtherRequistionDetails existModel = db.OtherRequisitionDetail.Where((OtherRequistionDetails i) => i.App_Req_ID == modeldata.App_Req_ID && i.RequestName == modeldata.RequestName).FirstOrDefault();
				existModel.CompleteDate = DateTime.Now;
				existModel.CompleteBy = userInfo.UserName;
				existModel.CaptionStatus = "Complete";
				db.Entry(existModel).State = EntityState.Modified;
				db.SaveChanges();
				insertedRecords++;
			}
		}
		return Json(insertedRecords);
	}

	public JsonResult MenuApprovebyRole(string fDate, string tDate)
	{
		List<OtherRequistionDetailsVM> data = new List<OtherRequistionDetailsVM>();
		List<OtherRequistionDetailsVM> itemData = new List<OtherRequistionDetailsVM>();
		data = db.Database.SqlQuery<OtherRequistionDetailsVM>("select DE.DepartmentName,Uinfo.UserName,Binfo.BranchName,RTN.TypeName,Details.App_Req_ID,Details.RequestedDate from  DepartmentEntries DE, ReqTypeNames RTN, UserInfoes Uinfo, BranchInfoes Binfo, (select App_Req_ID, BranchID, Requested_by, Requsition_Type, DepartmentId, RequestedDate from OtherRequistionDetails where CaptionStatus='Approved' and AmendmentID=1 group by RequestedDate,App_Req_ID, BranchID, Requested_by, Requsition_Type, DepartmentId) Details where DE.DepartmentId = Details.DepartmentId and RTN.id = Details.Requsition_Type and Uinfo.Id = Details.Requested_by and Binfo.BranchID = Details.BranchID and Details.RequestedDate between '" + fDate + "' AND '" + tDate + "'", Array.Empty<object>()).ToList();
		return new JsonResult
		{
			Data = data,
			JsonRequestBehavior = JsonRequestBehavior.AllowGet
		};
	}

	public JsonResult GetDepartmentNameByBranch(int BranchId)
	{
		List<DepartmentEntry> item = db.Database.SqlQuery<DepartmentEntry>("select DepartmentName.* from DepartmentInfoes Deapartment,DepartmentEntries DepartmentName where DepartmentName.DepartmentId=Deapartment.DepartmentId and Deapartment.BranchId=" + BranchId, Array.Empty<object>()).ToList();
		return new JsonResult
		{
			Data = item,
			JsonRequestBehavior = JsonRequestBehavior.AllowGet
		};
	}

	public ActionResult RequestionApproval()
	{
		UserInfo userInfo = (UserInfo)base.Session["UserProfile"];
		if (userInfo != null)
		{
			HttpCookie useridCookie = base.Request.Cookies["um_apps_userid"];
			if (userInfo.UserName == useridCookie.Values.ToString())
			{
				HttpCookie rolesCookie = base.Request.Cookies["um_apps_roles"];
				string appsrole = ((rolesCookie == null) ? "noroles" : rolesCookie.Value.ToString());
				appsrole = appsrole.Replace("%20", " ");
				string controller_action = base.ControllerContext.RouteData.Values["controller"].ToString() + "/" + base.ControllerContext.RouteData.Values["action"].ToString();
				if (db.Database.SqlQuery<int>("select id from RoleSettings where Path='" + controller_action + "' and RoleCaption='" + appsrole + "'", Array.Empty<object>()).FirstOrDefault() != 0)
				{
					//base.ViewBag.RequestionList = db.Database.SqlQuery<OtherRequistionDetailsNewVM>("select DL2.App_Req_ID,DL2.BranchID,Ol2.BranchName,DL2.Requested_by,DL2.DepartmentId,Ol2.DepartmentName,Dl2.Status,Serial=0,CurrentStatus='',DL2.NextStatus from (select Dl1.*,Ol1.CheckName as NextStatus from (select distinct ORDN.id, ORDN.App_Req_ID,ORDN.RequestName,ORDN.Description,ORDN.Description1,ordn.Status,ordn.Requested_by,ORDN.BranchID,ORDN.DepartmentId,ORDN.ItContactPerson,ORDN.ApprovedBy,ORDN.ApprovedDate,ORDN.CompleteBy,ORDN.CompleteDate,ordn.RequestedDate,ORDN.Remarks,ORDN.Requsition_Type,ORDN.AmendmentId,ORDN.BranchReqSeqAssignsId,ORDN.IsCancel,BRSA.CheckID from OtherRequistionDetailsNews ORDN inner join(select D1.*from(select bra.CheckID, BRA.BranchID, ORD.BranchReqSeqAssignsId, ORD.RequestName, ORD.DepartmentId from BranchReqSeqAssigns BRA, OtherRequistionDetailsNews ORD, (select Max(AmendmentId)AmendmentId, App_Req_ID from OtherRequistionDetailsNews group by App_Req_ID)MaxORD where ORD.BranchReqSeqAssignsId = BRA.ReqSeqID and ORD.BranchID = BRA.BranchID and BRA.Serial = ORD.Status + 1 and ORD.IsCancel = 0 and ORD.AmendmentId = MaxORD.AmendmentId and ORD.App_Req_ID = MaxORD.App_Req_ID ) D1 inner join BranchUserApprovals BUA on BUA.UserId ='" + userInfo.UserName + "' and BUA.Category = D1.RequestName and BUA.BranchId = D1.BranchID and BUA.CheckId = D1.CheckID) BRSA on ORDN.BranchReqSeqAssignsId = BRSA.BranchReqSeqAssignsId and ORDN.BranchID = BRSA.BranchID and ORDN.IsCancel = 0) Dl1 inner join(select ReqCheck.CheckName,BranchReqSeq.Serial,BranchReqSeq.CheckID, BranchReqSeq.ReqSeqID, BranchReqSeq.BranchID from ReqCheckLists ReqCheck, BranchReqSeqAssigns BranchReqSeq where ReqCheck.CheckId = BranchReqSeq.CheckID) Ol1 on Dl1.BranchID = Ol1.BranchID and Dl1.BranchReqSeqAssignsId = Ol1.ReqSeqID and Dl1.CheckID = Ol1.CheckID inner join(select Max(AmendmentId) AmendmentId, App_Req_ID from OtherRequistionDetailsNews group by App_Req_ID)MaxORD on Dl1.AmendmentId = MaxORD.AmendmentId and Dl1.App_Req_ID = MaxORD.App_Req_ID) DL2 inner join(select ORDN.BranchID, BInfo.BranchName,ORDN.DepartmentId, DE.DepartmentName, ORDN.App_Req_ID from OtherRequistionDetailsNews ORDN, DepartmentEntries DE, BranchInfoes BInfo, (select Max(AmendmentId)AmendmentId, App_Req_ID from OtherRequistionDetailsNews group by App_Req_ID)MaxORD where ORDN.BranchID = BInfo.BranchID and ORDN.DepartmentId = DE.DepartmentId and ORDN.AmendmentId = MaxORD.AmendmentId and ORDN.App_Req_ID = MaxORD.App_Req_ID) Ol2 on DL2.BranchID = Ol2.BranchID and DL2.DepartmentId = Ol2.DepartmentId and dl2.App_Req_ID = Ol2.App_Req_ID", Array.Empty<object>()).ToList();
					base.ViewBag.RequestionList = db.Database.SqlQuery<OtherRequistionDetailsNewVM>("select NextLevel.App_Req_ID,NextLevel.BranchID,NextLevel.BranchName,NextLevel.Requested_by,NextLevel.DepartmentId,NextLevel.DepartmentName,NextLevel.AmendmentId as Status,NextLevel.Serial,CurrentLevel.CurrentStatus,NextLevel.CheckName as NextStatus from(select RawData.*, ISNULL(ExtraData.CheckName,'Plased_Requsition') AS CurrentStatus  from (select App_Req_ID, max(AmendmentId)AmendmentId,BranchReqSeqAssignsId from OtherRequistionDetailsNews group by App_Req_ID,BranchReqSeqAssignsId)RawData left outer join (select BRSA.*,RCL.CheckName from BranchReqSeqAssigns BRSA,ReqCheckLists RCL where BRSA.CheckID=RCL.CheckId)ExtraData on RawData.BranchReqSeqAssignsId=ExtraData.ReqSeqID and RawData.AmendmentId=ExtraData.Serial) CurrentLevel,(select NextSteps.*,AllReq.*,BInfo.BranchName,DE.DepartmentName from (select ReqSeqID,Serial,CheckName,RCL.CheckId from BranchReqSeqAssigns BRSA,ReqCheckLists RCL where RCL.CheckId=BRSA.CheckID group by ReqSeqID,Serial,CheckName,RCL.CheckId ) NextSteps,(select App_Req_ID, max(AmendmentId)AmendmentId,RequestName,BranchReqSeqAssignsId,BranchID,DepartmentId,Requested_by from OtherRequistionDetailsNews group by App_Req_ID,RequestName, BranchReqSeqAssignsId,BranchID,DepartmentId,Requested_by) AllReq, BranchUserApprovals BUA,BranchInfoes BInfo,DepartmentEntries DE where AllReq.BranchReqSeqAssignsId=NextSteps.ReqSeqID and NextSteps.Serial=AllReq.AmendmentId+1 and BUA.CheckId=NextSteps.CheckId and AllReq.BranchID=BUA.BranchId and AllReq.DepartmentId=BUA.DepartmentId and BUA.Category=AllReq.RequestName and AllReq.DepartmentId=de.DepartmentId and AllReq.BranchID=BInfo.BranchID and BUA.UserId='" + userInfo.UserName + "')NextLevel where CurrentLevel.App_Req_ID=NextLevel.App_Req_ID and CurrentLevel.BranchReqSeqAssignsId=NextLevel.BranchReqSeqAssignsId", Array.Empty<object>()).ToList();
					return View();
				}
				return RedirectToAction("Index", "Home");
			}
			return RedirectToAction("Index", "Home");
		}
		return RedirectToAction("SmartLogins", "Setings");
	}

	public ActionResult RequestionPendingDetails(int AppReqID)
	{
		base.ViewBag.AppReqID = AppReqID;
		base.ViewBag.OthersRequisitionByID = db.Database.SqlQuery<OtherRequistionDetailsNewApprovalVM>("select TOP 1 NextLevel.DepartmentName,NextLevel.Requested_by,NextLevel.BranchName,Description,Description1,RequestName,NextLevel.App_Req_ID,NextLevel.BranchID,NextLevel.AmendmentId as Status,NextLevel.Serial,CurrentLevel.CurrentStatus as 'Current_Status',NextLevel.CheckName as Next_Status from(select RawData.*, ISNULL(ExtraData.CheckName,'Plased_Requsition') AS CurrentStatus from (select App_Req_ID, max(AmendmentId)AmendmentId,BranchReqSeqAssignsId from OtherRequistionDetailsNews group by App_Req_ID,BranchReqSeqAssignsId)RawData left outer join (select BRSA.*,RCL.CheckName from BranchReqSeqAssigns BRSA,ReqCheckLists RCL where BRSA.CheckID=RCL.CheckId)ExtraData on RawData.BranchReqSeqAssignsId=ExtraData.ReqSeqID and RawData.AmendmentId=ExtraData.Serial) CurrentLevel,(select NextSteps.*,AllReq.*,BInfo.BranchName,DE.DepartmentName from (select ReqSeqID,Serial,CheckName,RCL.CheckId from BranchReqSeqAssigns BRSA,ReqCheckLists RCL where RCL.CheckId=BRSA.CheckID group by ReqSeqID,Serial,CheckName,RCL.CheckId ) NextSteps,(select App_Req_ID, max(AmendmentId)AmendmentId,RequestName,BranchReqSeqAssignsId,BranchID,DepartmentId,Requested_by,Description,Description1 from OtherRequistionDetailsNews group by Description,Description1,App_Req_ID,RequestName, BranchReqSeqAssignsId,BranchID,DepartmentId,Requested_by) AllReq, BranchUserApprovals BUA,BranchInfoes BInfo,DepartmentEntries DE where AllReq.BranchReqSeqAssignsId=NextSteps.ReqSeqID and NextSteps.Serial=AllReq.AmendmentId+1 and BUA.CheckId=NextSteps.CheckId and AllReq.BranchID=BUA.BranchId and AllReq.DepartmentId=BUA.DepartmentId and BUA.Category=AllReq.RequestName and AllReq.DepartmentId=de.DepartmentId and AllReq.BranchID=BInfo.BranchID)NextLevel where CurrentLevel.App_Req_ID=NextLevel.App_Req_ID and CurrentLevel.BranchReqSeqAssignsId=NextLevel.BranchReqSeqAssignsId and CurrentLevel.App_Req_ID=" + AppReqID + " ORDER BY serial DESC", Array.Empty<object>()).ToList();
		//base.ViewBag.OthersRequisitionByID = db.Database.SqlQuery<OtherRequistionDetailsNewApprovalVM>("select Datas.DepartmentName,Datas.Requested_by,Datas.BranchName,Datas.Description,Datas.RequestName,Datas.App_Req_ID,Datas.BranchID,Datas.Currents 'Status',Datas.Next 'Serial',(CASE WHEN Datas.Currents=0 then 'Requisition Entered' else (select RCL.CheckName from BranchReqSeqAssigns BRSA, ReqCheckLists RCL where BRSA.ReqSeqID = Datas.BranchReqSeqAssignsId and BRSA.Serial = Datas.Currents and RCL.CheckId = BRSA.CheckID) END) as Current_Status,RCL.CheckName 'Next_Status' from (select DE.DepartmentName, ORDN.Requested_by,BInfo.BranchName, ORDN.Description, ORDN.RequestName, ORDN.App_Req_ID, ORDN.BranchID, ORDN.Status as Currents, ORDN.Status + 1 'Next',ORDN.BranchReqSeqAssignsId from OtherRequistionDetailsNews ORDN, DepartmentEntries DE, BranchInfoes BInfo ,(select max(AmendmentId)AmendmentId from OtherRequistionDetailsNews where App_Req_ID=" + AppReqID + ")MaxAmendmentID where MaxAmendmentID.AmendmentId=ORDN.AmendmentId and  ORDN.DepartmentId = DE.DepartmentId and ORDN.BranchID = BInfo.BranchID and ORDN.App_Req_ID = " + AppReqID + ") Datas left join BranchReqSeqAssigns BRSA on Datas.Next = BRSA.Serial and BRSA.ReqSeqID = Datas.BranchReqSeqAssignsId left join ReqCheckLists RCL on BRSA.CheckID = RCL.CheckId", Array.Empty<object>()).ToList();
		base.ViewBag.BranchName = base.ViewBag.OthersRequisitionByID[0].BranchName;
		base.ViewBag.Requested_by = base.ViewBag.OthersRequisitionByID[0].Requested_by;
		base.ViewBag.Next_Status = base.ViewBag.OthersRequisitionByID[0].Next_Status;
		base.ViewBag.Current_Status = base.ViewBag.OthersRequisitionByID[0].Current_Status;


		var fileName = db.RequestionFileModels.Where(x => x.RequestionNumber == AppReqID).ToList();
		base.ViewBag.FileData = db.Database.SqlQuery<RequestionFileModel>("select * from RequestionFileModels where RequestionNumber=" + AppReqID).ToList();
		string[] filePaths = Directory.GetFiles(Server.MapPath("~/UploadedFiles/"));
		List<RequestionFileModel> files = new List<RequestionFileModel>();
		foreach (string filePath in filePaths)
		{
			foreach (RequestionFileModel modeldata3 in fileName)
			{
				if (modeldata3.FileName == Path.GetFileName(filePath))
				{
					files.Add(new RequestionFileModel { FileName = Path.GetFileName(filePath) });
				}
			}
		}
		return View(files);
	}	

	public JsonResult OtherRequestionApprovedData(List<OtherRequistionDetailsNew> ReqPending_Details, string param)
	{
		int insertedRecords = 0;
		UserInfo userInfo = (UserInfo)base.Session["UserProfile"];
		if (param == "Approved")
		{
			if (base.ModelState.IsValid)
			{
				OtherRequistionDetailsNew OldData2 = db.Database.SqlQuery<OtherRequistionDetailsNew>("select * from OtherRequistionDetailsNews where App_Req_ID=" + ReqPending_Details[0].App_Req_ID + " and AmendmentId=(select max(AmendmentId) from OtherRequistionDetailsNews where App_Req_ID=" + ReqPending_Details[0].App_Req_ID + ")", Array.Empty<object>()).FirstOrDefault();
				foreach (OtherRequistionDetailsNew modeldata3 in ReqPending_Details)
				{
					OldData2.ApprovedDate = DateTime.Now;
					OldData2.ApprovedBy = userInfo.UserName;
					OldData2.AmendmentId++;
					OldData2.Description1 = modeldata3.Description1;
					OldData2.Remarks = modeldata3.Remarks;
					OldData2.Status++;
					db.OtherRequistionDetailsNew.Add(OldData2);
					db.SaveChanges();
					insertedRecords++;
				}
			}
		}
		else if (base.ModelState.IsValid)
		{
			List<OtherRequistionDetailsNew> OldData = db.Database.SqlQuery<OtherRequistionDetailsNew>("select * from OtherRequistionDetailsNews where App_Req_ID=" + ReqPending_Details[0].App_Req_ID, Array.Empty<object>()).ToList();
			foreach (OtherRequistionDetailsNew modeldata2 in OldData)
			{
				modeldata2.IsCancel = true;
				db.Entry(modeldata2).State = EntityState.Modified;
				db.SaveChanges();
			}
			OtherRequistionDetailsNew OldData3 = db.Database.SqlQuery<OtherRequistionDetailsNew>("select * from OtherRequistionDetailsNews where App_Req_ID=" + ReqPending_Details[0].App_Req_ID + " and AmendmentId=(select max(AmendmentId) from OtherRequistionDetailsNews where App_Req_ID=" + ReqPending_Details[0].App_Req_ID + ")", Array.Empty<object>()).FirstOrDefault();
			foreach (OtherRequistionDetailsNew modeldata in ReqPending_Details)
			{
				OldData3.ApprovedDate = DateTime.Now;
				OldData3.ApprovedBy = userInfo.UserName;
				OldData3.AmendmentId++;
				OldData3.Description1 = modeldata.Description1;
				OldData3.Remarks = modeldata.Remarks;
				OldData3.IsCancel = true;
				db.OtherRequistionDetailsNew.Add(OldData3);
				db.SaveChanges();
				insertedRecords++;
			}
		}
		return new JsonResult
		{
			Data = insertedRecords,
			JsonRequestBehavior = JsonRequestBehavior.AllowGet
		};
	}

	public ActionResult RequestionDone()
	{
		UserInfo userInfo = (UserInfo)base.Session["UserProfile"];
		if (userInfo != null)
		{
			HttpCookie useridCookie = base.Request.Cookies["um_apps_userid"];
			if (userInfo.UserName == useridCookie.Value.ToString())
			{
				HttpCookie rolesCookie = base.Request.Cookies["um_apps_roles"];
				string appsrole = ((rolesCookie == null) ? "noroles" : rolesCookie.Value.ToString());
				appsrole = appsrole.Replace("%20", " ");
				string controller_action = base.ControllerContext.RouteData.Values["controller"].ToString() + "/" + base.ControllerContext.RouteData.Values["action"].ToString();
				if (db.Database.SqlQuery<int>("select id from RoleSettings where Path='" + controller_action + "' and RoleCaption='" + appsrole + "'", Array.Empty<object>()).FirstOrDefault() != 0)
				{
					base.ViewBag.OthersRequisition = db.Database.SqlQuery<OtherRequistionDetailsNewVM>("select AllData.App_Req_ID,AllData.BranchID,BInfo.BranchName,AllData.Requested_by,AllData.DepartmentId,DEntry.DepartmentName,AllData.Status,Serial=0,CurrentStatus='',NextStatus='' from BranchInfoes BInfo,DepartmentEntries DEntry,(select MaxStatus.* from OtherRequistionDetailsNews MaxStatus, (select max(Serial)Serial, ReqSeqID from BranchReqSeqAssigns group by ReqSeqID) FinalSerial where MaxStatus.AmendmentId = FinalSerial.Serial and MaxStatus.BranchReqSeqAssignsId = FinalSerial.ReqSeqID and MaxStatus.IsCancel = 0 and MaxStatus.CompleteBy IS not NULL)AllData where BInfo.BranchID = AllData.BranchID and DEntry.DepartmentId = AllData.DepartmentId", Array.Empty<object>()).ToList();
					return View();
				}
				return RedirectToAction("Index", "Home");
			}
			return RedirectToAction("Index", "Home");
		}
		return RedirectToAction("SmartLogins", "Setings");
	}

	public ActionResult RequestionToBeClearDetails(int AppReqID)
	{
		base.ViewBag.AppReqID = AppReqID;
		base.ViewBag.OthersRequisitionByID = db.Database.SqlQuery<OtherRequistionDetailsNewApprovalVM>("select DEntry.DepartmentName,AllData.Requested_by,BInfo.BranchName,AllData.Description,AllData.Description1,AllData.Remarks,AllData.RequestName,AllData.App_Req_ID,AllData.BranchID,AllData.Status,Serial=0,Current_Status='',Next_Status='' from BranchInfoes BInfo, DepartmentEntries DEntry, (select MaxStatus.* from OtherRequistionDetailsNews MaxStatus, (select max(Serial)Serial, ReqSeqID from BranchReqSeqAssigns group by ReqSeqID) FinalSerial where MaxStatus.AmendmentId = FinalSerial.Serial and MaxStatus.BranchReqSeqAssignsId = FinalSerial.ReqSeqID and MaxStatus.IsCancel = 0 and MaxStatus.CompleteBy IS not NULL and MaxStatus.App_Req_ID = " + AppReqID + ")AllData where BInfo.BranchID = AllData.BranchID and DEntry.DepartmentId = AllData.DepartmentId", Array.Empty<object>()).ToList();
		base.ViewBag.BranchName = base.ViewBag.OthersRequisitionByID[0].BranchName;
		base.ViewBag.Requested_by = base.ViewBag.OthersRequisitionByID[0].Requested_by;
		return View();
	}
	public ActionResult RequestionDoneByID(int AppReqID)
	{
		UserInfo userInfo = (UserInfo)base.Session["UserProfile"];
		if (base.ModelState.IsValid)
		{
			List<OtherRequistionDetailsNew> OldData = db.Database.SqlQuery<OtherRequistionDetailsNew>("select * from OtherRequistionDetailsNews where App_Req_ID=" + AppReqID, Array.Empty<object>()).ToList();
			foreach (OtherRequistionDetailsNew modeldata in OldData)
			{
				modeldata.CompleteBy = userInfo.UserName;
				modeldata.CompleteDate = DateTime.Now;
				db.Entry(modeldata).State = EntityState.Modified;
				db.SaveChanges();
			}
		}
		return RedirectToAction("RequestionDone", "Requestion");
	}
	public ActionResult OtherRequestionRptList()
	{
		UserInfo userInfo = (UserInfo)base.Session["UserProfile"];
		if (userInfo != null)
		{
			HttpCookie useridCookie = base.Request.Cookies["um_apps_userid"];
			if (userInfo.UserName == useridCookie.Value.ToString())
			{
				HttpCookie rolesCookie = base.Request.Cookies["um_apps_roles"];
				string appsrole = ((rolesCookie == null) ? "noroles" : rolesCookie.Value.ToString());
				appsrole = appsrole.Replace("%20", " ");
				string controller_action = base.ControllerContext.RouteData.Values["controller"].ToString() + "/" + base.ControllerContext.RouteData.Values["action"].ToString();
				if (db.Database.SqlQuery<int>("select id from RoleSettings where Path='" + controller_action + "' and RoleCaption='" + appsrole + "'", Array.Empty<object>()).FirstOrDefault() != 0)
				{
					//base.ViewBag.OthersRequisition = db.Database.SqlQuery<OtherRequistionDetailsNewVM>("select AllData.App_Req_ID,AllData.BranchID,BInfo.BranchName,AllData.Requested_by,AllData.DepartmentId,DEntry.DepartmentName,AllData.Status,Serial=0,CurrentStatus='',NextStatus='',convert(varchar, AllData.CompleteDate, 103)CompleteDate,AllData.CompleteBy from BranchInfoes BInfo,DepartmentEntries DEntry,(select MaxStatus.* from OtherRequistionDetailsNews MaxStatus, (select max(Serial)Serial, ReqSeqID from BranchReqSeqAssigns group by ReqSeqID) FinalSerial where MaxStatus.Status = FinalSerial.Serial and MaxStatus.BranchReqSeqAssignsId = FinalSerial.ReqSeqID and MaxStatus.IsCancel = 0 and MaxStatus.CompleteBy IS not NULL)AllData where BInfo.BranchID = AllData.BranchID and DEntry.DepartmentId = AllData.DepartmentId", Array.Empty<object>()).ToList();
					base.ViewBag.OthersRequisition = db.Database.SqlQuery<OtherRequistionDetailsNewVM>("select App_Req_ID,ORDN.BranchID,BInfo.BranchName,Requested_by,ORDN.DepartmentId,DEntry.DepartmentName,max(AmendmentId)AmendmentId,FinalSerial.Serial,CompleteBy,CONVERT(varchar,ORDN.CompleteDate,103)CompleteDate from OtherRequistionDetailsNews ORDN,(select max(Serial)Serial,ReqSeqID from BranchReqSeqAssigns group by ReqSeqID) FinalSerial,BranchInfoes BInfo,DepartmentEntries DEntry where ORDN.AmendmentId=FinalSerial.Serial and ORDN.BranchReqSeqAssignsId=FinalSerial.ReqSeqID and BInfo.BranchID=ORDN.BranchID and DEntry.DepartmentId=ORDN.DepartmentId group by App_Req_ID,BranchReqSeqAssignsId,Serial,ORDN.BranchID,Requested_by,ORDN.DepartmentId,BInfo.BranchName,DEntry.DepartmentName,CompleteBy,CompleteDate", Array.Empty<object>()).ToList();
					return View();
				}
				return RedirectToAction("Index", "Home");
			}
			return RedirectToAction("Index", "Home");
		}
		return View();
	}
	public ActionResult OtherRequestionReportDetails(int ReqID)
	{
		UserInfo userInfo = (UserInfo)base.Session["UserProfile"];
		List<RequestionFileModel> files = new List<RequestionFileModel>();
		if (userInfo != null)
		{
			base.ViewBag.OthersRequisition = db.Database.SqlQuery<OtherRequistionDetailsNew>("select * from OtherRequistionDetailsNews where App_Req_ID=" + ReqID + " order by AmendmentId", Array.Empty<object>()).ToList();
			base.ViewBag.RequisitionInitialData = db.Database.SqlQuery<OtherRequistionDetailsNewVM>("select ORDN.Requested_by,ORDN.CompleteBy, CONVERT(varchar,ORDN.CompleteDate,103)CompleteDate,BInfo.BranchName,DEntry.DepartmentName from OtherRequistionDetailsNews ORDN,BranchInfoes BInfo,DepartmentEntries DEntry where ORDN.App_Req_ID=" + ReqID + " and ORDN.BranchID=BInfo.BranchID and DEntry.DepartmentId=ORDN.DepartmentId", Array.Empty<object>()).ToList();
			var fileName = db.RequestionFileModels.Where(x => x.RequestionNumber == ReqID).ToList();
			base.ViewBag.FileData = db.Database.SqlQuery<RequestionFileModel>("select * from RequestionFileModels where RequestionNumber=" + ReqID).ToList();
			string[] filePaths = Directory.GetFiles(Server.MapPath("~/UploadedFiles/"));

			foreach (string filePath in filePaths)
			{
				foreach (RequestionFileModel modeldata3 in fileName)
				{
					if (modeldata3.FileName == Path.GetFileName(filePath))
					{
						files.Add(new RequestionFileModel { FileName = Path.GetFileName(filePath) });
					}
				}
			}
		}
		return View(files);
	}
	public ActionResult ViewRequestionDetails(int AppReqID)
	{
		UserInfo userInfo = (UserInfo)base.Session["UserProfile"];
		if (userInfo == null)
		{
			base.Response.Cookies["um_apps_userid"].Expires = DateTime.Now;
			base.Response.Cookies["um_apps_roles"].Expires = DateTime.Now;
			base.Response.Cookies["um_apps_branchcode"].Expires = DateTime.Now;
			FormsAuthentication.SignOut();
			FormsAuthentication.RedirectToLoginPage();
			return RedirectToAction("SmartLogins", "Setings");
		}

        var fileName = db.RequestionFileModels.Where(x => x.RequestionNumber == AppReqID).ToList();
		base.ViewBag.FileData = db.Database.SqlQuery<RequestionFileModel>("select * from RequestionFileModels where RequestionNumber=" + AppReqID).ToList();
		string[] filePaths = Directory.GetFiles(Server.MapPath("~/UploadedFiles/"));
        List<RequestionFileModel> files = new List<RequestionFileModel>();
        foreach (string filePath in filePaths)
        {
			foreach(RequestionFileModel modeldata3 in fileName)
            {
                if (modeldata3.FileName ==  Path.GetFileName(filePath))
                {
					files.Add(new RequestionFileModel { FileName = Path.GetFileName(filePath) });
				}
            }
        }

        //      string partialName = "webapi";
        //DirectoryInfo hdDirectoryInWhichToSearch = new DirectoryInfo(@"c:\");
        //FileInfo[] filesInDir = hdDirectoryInWhichToSearch.GetFiles("*" + partialName + "*.*");

        //foreach (FileInfo foundFile in filesInDir)
        //{
        //	string fullName = foundFile.FullName;
        //	Console.WriteLine(fullName);        //}

        //base.ViewBag.OthersRequisitionByID = db.OtherRequistionDetailsNew.Where((OtherRequistionDetailsNew i) => i.App_Req_ID == AppReqID).ToList(); base.ViewBag.OthersRequisition = db.Database.SqlQuery<OtherRequistionDetailsVM>("select ORDN.App_Req_ID, BInfo.BranchName,DEntry.DepartmentName,ORDN.RequestName TypeName from OtherRequistionDetailsNews ORDN,BranchInfoes BInfo,DepartmentEntries DEntry where DEntry.DepartmentId=ORDN.DepartmentId and ORDN.ApprovedBy='" + userInfo.UserName + "' and BInfo.BranchID=ORDN.BranchID group by DEntry.DepartmentName,ORDN.App_Req_ID,BInfo.BranchName,ORDN.RequestName", Array.Empty<object>()).ToList();
		base.ViewBag.OthersRequisitionByID = db.Database.SqlQuery<OtherRequistionDetailsNew>("select id,App_Req_ID,RequestName,Description,Description1,Status,Requested_by,BranchID,DepartmentId,ItContactPerson,ApprovedBy,ApprovedDate,CompleteBy,CompleteDate,RequestedDate,Requsition_Type,AmendmentId,BranchReqSeqAssignsId,IsCancel,ApprovedDate,(case when ApprovedDate is NULL then concat('Request Date: ',(convert(varchar,RequestedDate,103)),' (Which is ',datediff(day, RequestedDate, getdate()),' Days Ago)') else concat((convert(varchar, ApprovedDate, 103)),'(', datediff(day, ApprovedDate, getdate()),' Days Ago)')end)Remarks from OtherRequistionDetailsNews where App_Req_ID=" + AppReqID, Array.Empty<object>()).ToList();

		return View(files);
	}
    //public ActionResult Index()
    //{
    //    //Fetch all files in the Folder (Directory).
    //    string[] filePaths = Directory.GetFiles(Server.MapPath("~/Files/"));

    //    //Copy File names to Model collection.
    //    List<FileModel> files = new List<FileModel>();
    //    foreach (string filePath in filePaths)
    //    {
    //        files.Add(new FileModel { FileName = Path.GetFileName(filePath) });
    //    }

    //    return View(files);
    //}

    public FileResult DownloadFile(string fileName)
    {
        //Build the File Path.
        string path = Server.MapPath("~/UploadedFiles/") + fileName;

        //Read the File data into Byte Array.
        byte[] bytes = System.IO.File.ReadAllBytes(path);

        //Send the File to Download.
        return File(bytes, "application/octet-stream", fileName);
    }
    public ActionResult RequestionApprovedList()
	{
		UserInfo userInfo = (UserInfo)base.Session["UserProfile"];
		if (userInfo == null)
		{
			base.Response.Cookies["um_apps_userid"].Expires = DateTime.Now;
			base.Response.Cookies["um_apps_roles"].Expires = DateTime.Now;
			base.Response.Cookies["um_apps_branchcode"].Expires = DateTime.Now;
			FormsAuthentication.SignOut();
			FormsAuthentication.RedirectToLoginPage();
			return RedirectToAction("SmartLogins", "Setings");
		}
		base.ViewBag.OthersRequisition = db.Database.SqlQuery<OtherRequistionDetailsVM>("select ORDN.App_Req_ID, BInfo.BranchName,DEntry.DepartmentName,ORDN.RequestName TypeName from OtherRequistionDetailsNews ORDN,BranchInfoes BInfo,DepartmentEntries DEntry where DEntry.DepartmentId=ORDN.DepartmentId and ORDN.ApprovedBy='" + userInfo.UserName + "' and BInfo.BranchID=ORDN.BranchID group by DEntry.DepartmentName,ORDN.App_Req_ID,BInfo.BranchName,ORDN.RequestName", Array.Empty<object>()).ToList();
		return View();
	}
	public ActionResult ViewRequestionApprovedDetails(int AppReqID)
	{
		UserInfo userInfo = (UserInfo)base.Session["UserProfile"];
		if (userInfo == null)
		{
			base.Response.Cookies["um_apps_userid"].Expires = DateTime.Now;
			base.Response.Cookies["um_apps_roles"].Expires = DateTime.Now;
			base.Response.Cookies["um_apps_branchcode"].Expires = DateTime.Now;
			FormsAuthentication.SignOut();
			FormsAuthentication.RedirectToLoginPage();
			return RedirectToAction("SmartLogins", "Setings");
		}
		base.ViewBag.OthersRequisitionByID = db.OtherRequistionDetailsNew.Where((OtherRequistionDetailsNew i) => i.App_Req_ID == AppReqID && i.ApprovedBy == userInfo.UserName).ToList();
		return View();
	}
	public ActionResult RequestionDownlevelPending()
	{
		UserInfo userInfo = (UserInfo)base.Session["UserProfile"];
		if (userInfo == null)
		{
			base.Response.Cookies["um_apps_userid"].Expires = DateTime.Now;
			base.Response.Cookies["um_apps_roles"].Expires = DateTime.Now;
			base.Response.Cookies["um_apps_branchcode"].Expires = DateTime.Now;
			FormsAuthentication.SignOut();
			FormsAuthentication.RedirectToLoginPage();
			return RedirectToAction("SmartLogins", "Setings");
		}
		
		base.ViewBag.OtherRequestionList = db.Database.SqlQuery<OtherRequistionDetailsVM>("select DE.DepartmentName,Binfo.BranchName,AllData.* from DepartmentEntries DE,BranchInfoes Binfo,(select distinct App_Req_ID, RequestName TypeName, Requested_by, ORDN.BranchID, ordn.DepartmentId, concat((convert(varchar, RequestedDate, 103)), ' (', datediff(day, RequestedDate, getdate()),' Days Ago)')RequestedDate  from OtherRequistionDetailsNews ORDN, (select BranchId, DepartmentId from BranchUserApprovals where UserId = '" + userInfo.UserName + "' group by BranchId, DepartmentId)RawDatas,(select ReqSeqID, max(Serial) Serial from BranchReqSeqAssigns group by ReqSeqID)SeqData where ORDN.BranchID = RawDatas.BranchId and ORDN.DepartmentId = RawDatas.DepartmentId and ORDN.BranchReqSeqAssignsId = SeqData.ReqSeqID and ORDN.Status != SeqData.Serial) AllData where AllData.BranchID = Binfo.BranchID and AllData.DepartmentId = DE.DepartmentId", Array.Empty<object>()).ToList();
		//base.ViewBag.OtherRequestionList = db.Database.SqlQuery<OtherRequistionDetailsVM>("select DE.DepartmentName,Binfo.BranchName,AllData.* from DepartmentEntries DE,BranchInfoes Binfo,(select distinct App_Req_ID,RequestName TypeName,Requested_by,ORDN.BranchID, ordn.DepartmentId,, concat((convert(varchar,RequestedDate,103)),' (',datediff(day, RequestedDate, getdate()),' Days Ago)')RequestedDate from OtherRequistionDetailsNews ORDN,(select BranchId,DepartmentId from BranchUserApprovals where UserId='" + userInfo.UserName + "' group by BranchId,DepartmentId)RawDatas,(select ReqSeqID,max(Serial) Serial from BranchReqSeqAssigns group by ReqSeqID)SeqData where ORDN.BranchID = RawDatas.BranchId and ORDN.DepartmentId = RawDatas.DepartmentId and ORDN.BranchReqSeqAssignsId = SeqData.ReqSeqID and ORDN.Status != SeqData.Serial) AllData where AllData.BranchID = Binfo.BranchID and AllData.DepartmentId = DE.DepartmentId", Array.Empty<object>()).ToList();

		//base.ViewBag.OthersRequisition = db.Database.SqlQuery<OtherRequistionDetailsVM>("select ORDN.App_Req_ID, BInfo.BranchName,DEntry.DepartmentName,ORDN.RequestName TypeName from OtherRequistionDetailsNews ORDN,BranchInfoes BInfo,DepartmentEntries DEntry where DEntry.DepartmentId=ORDN.DepartmentId and ORDN.ApprovedBy='" + userInfo.UserName + "' and BInfo.BranchID=ORDN.BranchID group by DEntry.DepartmentName,ORDN.App_Req_ID,BInfo.BranchName,ORDN.RequestName", Array.Empty<object>()).ToList();
		return View();
	}
}