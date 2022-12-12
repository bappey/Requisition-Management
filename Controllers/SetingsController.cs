using SCS_Inventory.Models;
using scs_Project.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity.Core;
using System.Web.Security;


namespace SCS_Inventory.Controllers
{
    
    public class SetingsController : Controller
    {       

        DataContext db = new DataContext(); 
        //
        // GET: /Setings/
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult CreteUser()
        {
            ViewBag.BranchName = new SelectList(db.Branch.ToList(), "BranchCode", "BranchName");
            ViewBag.DepartmentName = new SelectList(db.DepartmentEntry.ToList(), "Id", "DepartmentName");
            ViewBag.Designation = new SelectList(db.Designations.ToList(), "Id", "DesignationName");
            return View();
        }
        [HttpPost]
        public ActionResult CreteUser(UserInfo model)
        {
            using (DataContext db = new DataContext())
            {
                model.Activation_Date = DateTime.Now;
                string pass = "";
                for (int i = 1; i >= 3; i++)
                {
                    pass += getEncode(Convert.ToChar(i));
                }
                model.Password = pass;
                db.UserInfo.Add(model);
                db.SaveChanges();
            }
            ViewBag.BranchName = new SelectList(db.Branch.ToList(), "BranchCode", "BranchName");
            ViewBag.DepartmentName = new SelectList(db.DepartmentEntry.ToList(), "Id", "DepartmentName");
            return View();
        }
        public JsonResult GetReqCheckList (string UserID)
        {
            List<ReqCheckPermissionVM> item = new List<ReqCheckPermissionVM > ();
            if (UserID != "")
            {
                item = db.Database.SqlQuery<ReqCheckPermissionVM>("select isnull(ReqCheckPermissions.Status,0)Status, isnull(ReqCheckPermissions.Id,0)Id,isnull(ReqCheckPermissions.UserName,'')UserName ,isnull(ReqCheckLists.CheckId,0)CheckId,isnull(ReqCheckLists.CheckName,'')CheckName,isnull(ReqCheckPermissions.AuthorizedUserName,'')AuthorizedUserName, isnull(ReqCheckPermissions.AuthorizeDate,0)AuthorizeDate from ReqCheckLists left join ReqCheckPermissions on ReqCheckLists.CheckId=ReqCheckPermissions.CheckListId and ReqCheckPermissions.UserName='" + UserID + "'").ToList();
            }
            return new JsonResult { Data = item, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public JsonResult GiveReqCheckPermission(int CheckId, string UserName,string ApprovedBy)
        {
            string Message="";
            var order = db.ReqCheckPermission.Where(x => x.CheckListId == CheckId && x.UserName == UserName).FirstOrDefault();
            if (order != null) 
            {
                order.Status = false;
                order.AuthorizedUserName = ApprovedBy;
                order.AuthorizeDate = DateTime.Now;
                db.Entry(order).State = EntityState.Modified;
                Message = "Update Successfully";
            }
            else
            {
                ReqCheckPermission ReqPermission = new ReqCheckPermission();

                ReqPermission.AuthorizeDate = DateTime.Now;
                ReqPermission.CheckListId = CheckId;
                ReqPermission.AuthorizedUserName = ApprovedBy;
                ReqPermission.UserName = UserName;
                ReqPermission.Status = true;
                db.ReqCheckPermission.Add(ReqPermission);
                Message = "Added Successfully";
            }
            db.SaveChanges();
            return Json(Message);
        }
        public ActionResult ReqSequence()
        {
            ViewBag.ReqType = new SelectList(db.ReqTypeName.ToList(), "id", "TypeName");
            ViewBag.ReqCheckLists = new SelectList(db.ReqCheckList.ToList(), "CheckId", "CheckName");
            return View();
        }
        public JsonResult SaveReqSequence(List<ReqStatusSequence> ReqSequence)
        {
            string Message = "";

            //int intIdt = db.ReqStatusSequence.Where(f => f.typeId == typeid).Select(u => u.ReqStatus).DefaultIfEmpty(0).Max();
            //intIdt = intIdt + 1;           
            //for (int i=0;i<checks.Length; i++)
            //{
            //    ReqStatusSequence ReqSequence = new ReqStatusSequence();
            //    ReqSequence.AuthorizeDate = DateTime.Now;
            //    ReqSequence.typeId = typeid;
            //    ReqSequence.AuthorizedUserName = authorizedUserName;
            //    ReqSequence.ReqStatus = intIdt;
            //    ReqSequence.CheckId = checks[i];                
            //    db.ReqStatusSequence.Add(ReqSequence);
            //    db.SaveChanges();
            //    Message = "Added Successfully!";
            //}
            
            foreach (ReqStatusSequence details in ReqSequence)
            {
                details.AuthorizeDate = DateTime.Now;               
                details.ReqStatus = 1;
                db.ReqStatusSequence.Add(details);
                db.SaveChanges();
                Message = "Added Successfully!";
            }
            return Json(Message);
        }
        public JsonResult GetReqSequence()
        {
            var StatusVM = new ObservableCollection<ReqStatusSequenceVM>();
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SCSInventoryEntities"].ConnectionString);
            conn.Open();
            SqlCommand cmd = new SqlCommand("tempData", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                var reqStatusVM = new ReqStatusSequenceVM
                {
                    TypeName = rdr["TypeName"].ToString(),
                    AuthorizedUserName = rdr["AuthorizedUserName"].ToString(),
                    AuthorizeDate = rdr["AuthorizeDate"].ToString(),
                    CheckName = rdr["CheckName"].ToString()
                };
                StatusVM.Add(reqStatusVM);
            }                    
            return Json(new { data = StatusVM }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SmartLogins()
        {
            //string[] myCookies = Request.Cookies.AllKeys;
            //foreach (string cookie in myCookies)
            //{
            //    ////HttpCookie roleCookie = new HttpCookie("um_apps_roles");
            //    ////roleCookie.Value = string.Empty;
            //    ////this.ControllerContext.HttpContext.Response.Cookies.Set(roleCookie);

            //    HttpContext.Response.Cookies.Set(new HttpCookie(cookie) { Value = string.Empty });
            //    //HttpContext.Response.Cookies.Set(new HttpCookie("um_apps_branchcode") { Value = string.Empty });
            //    //HttpContext.Response.Cookies.Set(new HttpCookie("um_apps_userid") { Value = string.Empty });

            //    //Response.Cookies[cookie].Expires = DateTime.Now.AddSeconds(1);
            //    //Response.Cookies.Clear();
            //}

            //if (this.ControllerContext.HttpContext.Request.Cookies.AllKeys.Contains("um_apps_userid"))
            //{
            //    HttpCookie cookie = this.ControllerContext.HttpContext.Request.Cookies["um_apps_userid"];
            //    cookie.Expires = DateTime.Now.AddDays(-1);
            //    this.ControllerContext.HttpContext.Response.Cookies.Add(cookie);
            //}
            //if (this.ControllerContext.HttpContext.Request.Cookies.AllKeys.Contains("um_apps_roles"))
            //{
            //    HttpCookie cookie = this.ControllerContext.HttpContext.Request.Cookies["um_apps_roles"];
            //    cookie.Expires = DateTime.Now.AddDays(-1);
            //    this.ControllerContext.HttpContext.Response.Cookies.Add(cookie);
            //}
            //if (this.ControllerContext.HttpContext.Request.Cookies.AllKeys.Contains("um_apps_branchcode"))
            //{
            //    HttpCookie cookie = this.ControllerContext.HttpContext.Request.Cookies["um_apps_branchcode"];
            //    cookie.Expires = DateTime.Now.AddDays(-1);
            //    this.ControllerContext.HttpContext.Response.Cookies.Add(cookie);
            //}

            //HttpCookie roleCookie = new HttpCookie("um_apps_roles");
            //roleCookie.Value = "";
            //Response.Cookies.Add(roleCookie);

            //HttpCookie branchcodeCookie = new HttpCookie("um_apps_branchcode");
            //branchcodeCookie.Value = "";
            //Response.Cookies.Add(branchcodeCookie);

            //HttpCookie useridCookie = new HttpCookie("um_apps_userid");
            //useridCookie.Value = "";
            //Response.Cookies.Add(useridCookie);


            Response.Cookies["um_apps_userid"].Expires = DateTime.Now;
            Response.Cookies["um_apps_roles"].Expires = DateTime.Now;
            Response.Cookies["um_apps_branchcode"].Expires = DateTime.Now;



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

            //if (appsuser != "nouser" && appsrole != "noroles" && branchcode != "nobranchcode" && appsid == "scsapps_inventory")
            //{
            //    UserInfo uinfo = new UserInfo();
            //    uinfo.UserName = "dhk00112";
            //    uinfo.BranchCode = "KLP";

            //    this.Session["UserProfile"] = uinfo;

            //    HttpCookie cookie = new HttpCookie("SCSLogin");
            //    cookie.Values.Add("UserName", uinfo.UserName);
            //    cookie.Values.Add("Branch_Id", uinfo.BranchCode);
            //    cookie.Expires = DateTime.MaxValue;
            //    Response.Cookies.Add(cookie);
            //    return RedirectToAction("Index", "Home");
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
            //    return RedirectToAction("Index", "Home");
            //    //}
            //    //else
            //    //{
            //    //    return Redirect("https://scsapps.xyz");
            //    //}
            //}
            //else
            //{
            //    return Redirect("https://scsapps.xyz");
            //}

            //, um_apps_roles, um_apps_id = scsapps_inventory

            //string apps_userid = "Uttam";
            //string apps_roles = "123";
            //string apps_id = "scsapps_inventory";

            //HttpCookieCollection MyCookieCollection = HttpContext.Current.Request.Cookies;

            //HttpCookie apps_userid = MyCookieCollection.Get("um_apps_userid");
            //MyCookie.Value = DateTime.Now.ToString();
            //MyCookieCollection.Set(MyCookie);


            //HttpCookie apps_userid = HttpContext.Request.Cookies["um_apps_userid"];

            //this.Response.Cookies["um_apps_userid"] != null;

            //Request.Cookies["um_apps_userid"]?.Value;
            //var appname1 = "scsapps_inventory";

            //HttpCookie uid = Request.Cookies["um_apps_userid"];
            //HttpCookie urole = Request.Cookies["um_apps_roles"];
            //HttpCookie a = Request.Cookies["um_apps_id"];
            //var appname = Convert.ToString(a);

            //HttpCookie apps_userid = HttpContext.Request.Cookies["um_apps_userid"] as HttpCookie;
            //HttpCookie apps_roles = HttpContext.Request.Cookies["um_apps_roles"] as HttpCookie;
            //HttpCookie apps_id = HttpContext.Request.Cookies["um_apps_id"] as HttpCookie;

            //string apps_userid = "";

            //HttpCookieCollection MyCookieColl;
            //HttpCookie MyCookie;

            //MyCookieColl = Request.Cookies;

            //// Capture all cookie names into a string array.
            //String[] arr1 = MyCookieColl.AllKeys;

            //// Grab individual cookie objects by cookie name.
            //for (int i = 0; i < arr1.Length; i++)
            //{
            //    MyCookie = MyCookieColl[arr1[i]];
            //    apps_userid= MyCookie.Domain.ToString();

            //    //Debug.WriteLine("Expires: " + MyCookie.um_apps_roles);
            //    //Debug.WriteLine("Secure:" + MyCookie.Secure);
            //}




            /*string apps_userid = "";*/
            //string apps_roles = ""; string apps_id = "";
            //if (HttpContext.Request.Cookies["um_apps_userid"] == null)
            //{
            //apps_userid = Request.Cookies["um_apps_userid"].ToString();
            //}
            //if (HttpContext.Request.Cookies["um_apps_roles"] == null)
            //{
            //apps_roles = HttpContext.Request.Cookies["um_apps_roles"].Value;
            //}
            //if (HttpContext.Request.Cookies["um_apps_id"] == null)
            //{
            //apps_id = HttpContext.Request.Cookies["um_apps_id"].Value;
            //}



            //Request.Cookies["um_apps_userid"];
            //apps_roles = Convert.ToString(Request.Cookies["um_apps_roles"]);
            //apps_id = Convert.ToString(Request.Cookies["um_apps_id"]);

            //if (apps_userid != null && apps_roles !=null && apps_id == "scsapps_inventory")
            //{
            //    return RedirectToAction("Index", "Home");
            //}
            //else
            //{
            //    return Redirect("https://scsapps.xyz");
            //}

            //UserInfo userInfo = (UserInfo)Session["UserProfile"];
            //if (userInfo != null)
            //{
            //    var saveData = db.UserInfo.Where(s => s.UserName == userInfo.UserName && s.Password == userInfo.Password).FirstOrDefault();

            //    if (saveData == null)
            //    {
            //        ViewData["Message"] = "User Information Invalid";
            //        return View();
            //    }
            //    else
            //    {
            //        if (model.RememberMe)
            //        {
            //            HttpCookie cookie = new HttpCookie("SCSLogin");
            //            cookie.Values.Add("UserName", model.UserName);
            //            cookie.Values.Add("Password", model.Password);
            //            cookie.Expires = DateTime.MaxValue;
            //            Response.Cookies.Add(cookie);
            //        }
            //        else
            //        {
            //            HttpCookie cookie = new HttpCookie("SCSLogin");
            //            cookie.Expires = DateTime.Now.AddDays(-1d);
            //            Response.Cookies.Add(cookie);
            //        }
            //        this.Session["UserProfile"] = saveData;
            //        return RedirectToAction("Index", "Home");
            //    }
            //}

            //LoginViewModel model = new LoginViewModel();

            //if (Request.Cookies["SCSLogin"] != null)
            //{
            //    ViewBag.UserName = Request.Cookies["SCSLogin"].Values["UserName"];
            //    ViewBag.Password = Request.Cookies["SCSLogin"].Values["Password"];
            //    ViewBag.RememberMe = true;
            //}
            return View();
        }
        [HttpPost]
        public ActionResult SmartLogins(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("SmartLogins");                
            }
            if(model.UserName != null)
            {
                var result = db.UserInfo.Where(s => s.UserName == model.UserName && s.Password == model.Password).FirstOrDefault();
                if (result == null)
                {
                    //ModelState.AddModelError(string.Empty, "Invalid Credentials Supplied");              
                    ViewData["Message"] = "User Information Invalid";
                    return View();
                }
                else
                {                                        
                    var branchInfo = db.Branch.Where(p => p.BranchID == result.Branch_Id).FirstOrDefault();

                    //CookieOptions option = new CookieOptions("");
                    //option.Expires = DateTime.Now.AddHours(24);
                    //Response.Cookies.Append("um_apps_userid", userModel.LoginID, option);

                    //HttpCookie myCookie = new HttpCookie("");
                    //myCookie.Expires = DateTime.Now.AddHours(24);
                    //Response.Cookies["um_apps_userid"].Value= model.UserName;
                    //Response.Cookies.Add(myCookie);

                    //HttpCookie useridCookie = new HttpCookie("um_apps_userid");                
                    // Request.Cookies["um_apps_userid"].Value = model.UserName;
                    //useridCookie.Expires = DateTime.Now.AddHours(24);
                    //Response.Cookies.Add(useridCookie);

                    //HttpCookie rolesCookie = new HttpCookie("um_apps_roles");
                    //Request.Cookies["um_apps_roles"].Value = UserRole.RoleCaption;
                    //rolesCookie.Expires = DateTime.Now.AddHours(24);
                    //Response.Cookies.Add(rolesCookie);

                    //HttpCookie appsidCookie = new HttpCookie("um_apps_id");
                    //Request.Cookies["um_apps_id"].Value = "scsapps_inventory";
                    //appsidCookie.Expires = DateTime.Now.AddHours(24);
                    //Response.Cookies.Add(appsidCookie);

                    //HttpCookie branchcodeCookie = new HttpCookie("um_apps_branchcode");
                    //Request.Cookies["um_apps_branchcode"].Value = branchInfo.BranchCode;
                    //branchcodeCookie.Expires = DateTime.Now.AddHours(24);
                    //Response.Cookies.Add(branchcodeCookie);
                    //Session.Clear();
                    //Session.Abandon();
                    //this.ControllerContext.HttpContext.Response.Cookies.Clear();

                    HttpCookie useridCookie = new HttpCookie("um_apps_userid");
                    useridCookie.Value = model.UserName;
                    this.ControllerContext.HttpContext.Response.Cookies.Add(useridCookie);
                    
                    if (result.RoleId != null )
                    {
                        //int roleID = db.Database.SqlQuery<int>("select isnull(RoleId,0)RoleId from UserInfoes where Id=" + result.RoleId + "").FirstOrDefault();

                        var UserRole = db.RoleName.Where(p => p.Id == result.RoleId).FirstOrDefault();
                     
                        HttpCookie rolesCookie = new HttpCookie("um_apps_roles");
                        rolesCookie.Value = UserRole.RoleCaption;
                        this.ControllerContext.HttpContext.Response.Cookies.Add(rolesCookie);
                    }
                    //else
                    //{
                    //    HttpCookie rolesCookie = Request.Cookies["um_apps_roles"];
                    //    string appsrole = rolesCookie == null ? "noroles" : rolesCookie.Value.ToString();
                    //}
                    
                    HttpCookie branchcodeCookie = new HttpCookie("um_apps_branchcode");
                    branchcodeCookie.Value = branchInfo.BranchCode;
                    this.ControllerContext.HttpContext.Response.Cookies.Add(branchcodeCookie);

                    HttpCookie cookie = new HttpCookie("SCSLogin");
                    cookie.Values.Add("um_apps_userid", model.UserName);
                    if (result.RoleId!= null)
                    {
                        var UserRole = db.RoleName.Where(p => p.Id == result.RoleId).FirstOrDefault();
                        cookie.Values.Add("um_apps_roles", UserRole.RoleCaption);
                    }
                    else
                    {
                        HttpCookie roleCookie = new HttpCookie("um_apps_roles");
                        roleCookie.Value = "";
                        this.ControllerContext.HttpContext.Response.Cookies.Add(roleCookie);
                    }

                    cookie.Values.Add("um_apps_id", "scsapps_inventory");
                    cookie.Values.Add("um_apps_branchcode", branchInfo.BranchCode);
                    cookie.Expires = DateTime.Now.AddDays(-1d);
                    Response.Cookies.Add(cookie);

                    //if (model.RememberMe)
                    //{   
                    //    //HttpCookie cookie = new HttpCookie("SCSLogin");
                    //    //cookie.Values.Add("um_apps_userid", model.UserName);
                    //    //cookie.Values.Add("um_apps_roles", UserRole.RoleCaption);
                    //    //cookie.Values.Add("um_apps_id", "scsapps_inventory");
                    //    //cookie.Values.Add("um_apps_branchcode", branchInfo.BranchCode);
                    //    //cookie.Expires = DateTime.Now.AddHours(24);
                    //    //Response.Cookies.Add(cookie);
                    //}
                    //else
                    //{
                    //}                    
                    this.Session["UserProfile"] = result;
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                ViewData["Message"] = "";
                return View();
            }
            
        }
        public ActionResult UserRegistration()
        {
            ViewBag.BranchName = new SelectList(db.Branch.ToList(), "BranchID", "BranchName");
            ViewBag.DepartmentName = new SelectList(db.DepartmentEntry.ToList(), "DepartmentId", "DepartmentName");
            ViewBag.Designation = new SelectList(db.Designations.ToList(), "Id", "DesignationName");
            return View();
        }
        [HttpPost]
        public ActionResult UserRegistration(UserInfo model)
        {

            using (DataContext db = new DataContext())
            {
                try
                {
                    if (model.Name == null && model.Password == null && model.Phone == null && model.UserName == null)
                    {
                        TempData["Message"] = "User Information Required";
                    }
                    else
                    {
                        string pass = "";
                        for (int i = 1; i >= 3; i++)
                        {
                            pass += getEncode(Convert.ToChar(i));
                        }
                        string branchCode = db.Branch.Where(f => f.BranchID == model.Branch_Id).Select(u => u.BranchCode).FirstOrDefault();
                        int intIdt = db.UserInfo.Where(f => f.UserName == model.UserName && f.Name == model.Name && f.Branch_Id == model.Branch_Id && f.Department_Id == model.Department_Id).Select(u => u.Id).DefaultIfEmpty(0).Max();
                        if (intIdt == 0)
                        {
                            model.BranchCode = branchCode;
                            model.Activation_Date = DateTime.Now;
                            //model.RoleId = 0;
                            //model.Authorizedby = 0;
                            //model.AuthorizedDate = DateTime.Now;
                            //model.Old_Password = pass;
                            db.UserInfo.Add(model);
                            db.SaveChanges();
                            ModelState.Clear();
                            TempData["Message"] = "Save Successfully";
                        }
                        else
                        {
                            ModelState.Clear();
                            //model.Errors = "User name already taken";
                            TempData["Message"] = "User name already taken";
                        }
                    }
                    ViewBag.BranchName = new SelectList(db.Branch.ToList(), "BranchID", "BranchName");
                    ViewBag.DepartmentName = new SelectList(db.DepartmentEntry.ToList(), "DepartmentId", "DepartmentName");
                    ViewBag.Designation = new SelectList(db.Designations.ToList(), "Id", "DesignationName");
                }
                catch (Exception e)
                {

                }
            }
            return View(model);
        }
        public ActionResult LogOut()
        {
            //int limit = Request.Cookies.Count;                                                
            //HttpCookie aCookie;   
            //string cookieName;

            //for (int i = 0; i < limit; i++)
            //{
            //    cookieName = Request.Cookies[i].Name;    //get the name of the current cookie
            //    aCookie = new HttpCookie(cookieName);    //create a new cookie with the same
            //                                             // name as the one you're deleting
            //    aCookie.Value = "";    //set a blank value to the cookie 
            //    aCookie.Expires = DateTime.Now.AddSeconds(1);    
            //    Response.Cookies.Add(aCookie);    
            //}

            //HttpContext.Response.Cookies.Set(new HttpCookie("um_apps_roles") { Value = string.Empty });
            //HttpContext.Response.Cookies.Set(new HttpCookie("um_apps_branchcode") { Value = string.Empty });
            //HttpContext.Response.Cookies.Set(new HttpCookie("um_apps_userid") { Value = string.Empty });
            
            Response.Cookies["um_apps_userid"].Expires = DateTime.Now;
            Response.Cookies["um_apps_roles"].Expires = DateTime.Now;
            Response.Cookies["um_apps_branchcode"].Expires = DateTime.Now;

            FormsAuthentication.SignOut();
            FormsAuthentication.RedirectToLoginPage();

            //HttpCookie roleCookie = new HttpCookie("um_apps_roles");
            //roleCookie.Value = "";
            //Response.Cookies.Add(roleCookie);


            //HttpCookie roleCookie = new HttpCookie("um_apps_roles");
            //roleCookie.Value = null;
            //roleCookie.Expires = DateTime.Now;
            //Response.Cookies.Add(roleCookie);

            //HttpCookie branchcodeCookie = new HttpCookie("um_apps_branchcode");
            //branchcodeCookie.Value = "";
            //this.ControllerContext.HttpContext.Response.Cookies.Add(branchcodeCookie);

            //HttpCookie useridCookie = new HttpCookie("um_apps_userid");
            //useridCookie.Value = "";
            //Response.Cookies.Add(useridCookie);

            //if (this.ControllerContext.HttpContext.Request.Cookies.AllKeys.Contains("um_apps_userid"))
            //{
            //    HttpCookie cookie = this.ControllerContext.HttpContext.Request.Cookies["um_apps_userid"];
            //    cookie.Expires = DateTime.Now;
            //    this.ControllerContext.HttpContext.Response.Cookies.Add(cookie);
            //}
            //if (this.ControllerContext.HttpContext.Request.Cookies.AllKeys.Contains("um_apps_roles"))
            //{
            //    HttpCookie cookie = this.ControllerContext.HttpContext.Request.Cookies["um_apps_roles"];
            //    cookie.Expires = DateTime.Now;
            //    this.ControllerContext.HttpContext.Response.Cookies.Add(cookie);
            //}
            //if (this.ControllerContext.HttpContext.Request.Cookies.AllKeys.Contains("um_apps_branchcode"))
            //{
            //    HttpCookie cookie = this.ControllerContext.HttpContext.Request.Cookies["um_apps_branchcode"];
            //    cookie.Expires = DateTime.Now;
            //    this.ControllerContext.HttpContext.Response.Cookies.Add(cookie);
            //}

            //this.ControllerContext.HttpContext.Response.Cookies.Clear(); 
            //Session.Clear();
            //Session.Abandon();

            //this.ControllerContext.HttpContext.Response.Cookies.Clear();
            //HttpCookie rolesCookie = new HttpCookie("um_apps_roles");
            //rolesCookie.Expires = DateTime.Now.AddDays(-1);



            //string[] myCookies = Request.Cookies.AllKeys;
            //foreach (string cookie in myCookies)
            //{
            //    Response.Cookies[cookie].Expires = DateTime.Now;
            //    Response.Cookies.Remove(cookie);
            //}
            //Response.Cookies.Clear();
            //Response.Cache.SetCacheability(HttpCacheability.NoCache);
            //Response.Cache.SetExpires(DateTime.UtcNow);
            //Response.Cache.SetNoStore();



            //return Redirect("https://scsapps.xyz");
            //return Redirect("https://scsapps.xyz/Bypass");

            //if (Request.Cookies["um_apps_roles"] != null)
            //{
            //    HttpCookie myCookie = new HttpCookie("um_apps_roles");
            //    myCookie.Expires = DateTime.Now.AddDays(-1);
            //    Response.Cookies.Add(myCookie);

            //    HttpCookie useridCookie = new HttpCookie("um_apps_userid");
            //    useridCookie.Expires = DateTime.Now.AddDays(-1);
            //    Response.Cookies.Add(useridCookie);

            //    HttpCookie branchcodeCookie = new HttpCookie("um_apps_branchcode");
            //    branchcodeCookie.Expires = DateTime.Now.AddDays(-1);
            //    Response.Cookies.Add(branchcodeCookie);
            //}

            return RedirectToAction("SmartLogins", "Setings");
            //Response.Redirect("SmartLogins");
            //return View();

        }
        public ActionResult MenuEntry()
        {
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
                        ViewBag.RoleName = new SelectList(db.RoleName.ToList(), "RoleCaption", "RoleCaption");
                        List<RoleSetting> RoleSetting = db.RoleSetting.ToList();
                        return View(RoleSetting);
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
                //return Redirect("https://scsapps.xyz/Bypass");
                return this.RedirectToAction("SmartLogins", "Setings");
            }

        }
        [HttpPost]
        public ActionResult AddMenu(RoleSetting objMenu)
        {
            UserInfo userInfo = (UserInfo)Session["UserProfile"];
            objMenu.PermittedBy = userInfo.UserName;
            objMenu.AssignDate = DateTime.Today;
            int intIdt = db.RoleSetting.Select(u => u.Id).DefaultIfEmpty(0).Max();
            objMenu.Id = intIdt + 1;            
            db.RoleSetting.Add(objMenu);
            db.SaveChanges();
            Int64 id = objMenu.Id;
            return RedirectToAction("MenuEntry");
        }
        public ActionResult DeleteMenu(int Id)
        {
            RoleSetting menu = db.RoleSetting.Find(Id);
            db.RoleSetting.Remove(menu);
            db.SaveChanges();
            return RedirectToAction("MenuEntry");
        }
        public ActionResult EditMenu(int Id)
        {
            var menu = db.RoleSetting.Single(p => p.Id == Id);
            ViewBag.RoleName = new SelectList(db.RoleName.ToList(), "RoleCaption", "RoleCaption");
            return View(menu);
        }
        [HttpPost]
        public ActionResult EditMenu(RoleSetting model)
        {
            if (ModelState.IsValid)
            {
                UserInfo userInfo = (UserInfo)Session["UserProfile"];
                model.PermittedBy= userInfo.UserName;
                model.AssignDate = DateTime.Today;
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("MenuEntry");
            }
            return View(model);
        }
        public ActionResult MenuPermission()
        {
            //ViewBag.MenuList = db.MenuDeclare.ToList();
            ViewBag.UseList = new SelectList(db.UserInfo.ToList(), "Id", "UserName");
            return View();
        }
        public JsonResult GetAllMenuList(int UserID)
        {
            List<UserAuthonticationVM> item = new List<UserAuthonticationVM>();
            if (UserID !=0)
            {
                item = db.Database.SqlQuery<UserAuthonticationVM>("select isnull(MD.Id,0)ID,ISNULL(MD.MenuName,'')MenuName,ISNULL(MD.SubMenuName,'')SubMenuName,ISNULL(MD.Path,'')Path ,ISNULL(Details.Authorizedby,'')Authorizedby, ISNULL(Details.UserName,'')UserName from MenuDeclares MD left join(select Old.Id,Old.MenuId,Old.UserName,Author.UserName as Authorizedby from UserInfoes Author,(select UA.Id,UA.MenuId,UA.Authorizedby,Uinfo.UserName as UserName from UserAuthontications UA,UserInfoes Uinfo where UA.UserId=Uinfo.Id and UA.UserId="+ UserID + ") Old where Author.Id=Old.Authorizedby)as Details on MD.Id=Details.MenuId").ToList();
            }
            return new JsonResult { Data = item, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public JsonResult GiveMenuPermission(int MenuId, int UserId)
        {
            UserInfo userInfo = (UserInfo)Session["UserProfile"];
            string Message = "";
            var order = db.UserAuthontication.Where(x => x.MenuId == MenuId && x.UserId == UserId).FirstOrDefault();
            if (order != null)
            {                
                order.AuthorizedDate = DateTime.Now;
                db.Entry(order).State = EntityState.Modified;
                Message = "Update Successfully";
            }
            else
            {
                UserAuthontication UserPermission = new UserAuthontication();

                UserPermission.AuthorizedDate = DateTime.Now;
                UserPermission.MenuId = MenuId;
                UserPermission.UserId = UserId;
                UserPermission.Authorizedby = userInfo.Id;
                db.UserAuthontication.Add(UserPermission);
                Message = "Added Successfully";
            }
            db.SaveChanges();
            return Json(Message);
        }
        public ActionResult PasswordChange()
        {
            ViewBag.MenuLst = db.Database.SqlQuery<RoleSetting>("select * from RoleSettings where RoleCaption='Admin' order by MenuName").ToList();
            return View();
        }
        [HttpPost]
        public ActionResult PasswordChange(UserInfo Uinfo)
        {
            //UserInfo userInfo = (UserInfo)Session["UserProfile"];
            var User = db.UserInfo.Where(x => x.UserName == Uinfo.UserName).FirstOrDefault();
            if (User != null)
            {
                string pass = Uinfo.Password;
                for (int i = 1; i >= pass.Length; i++)
                {
                    pass += getEncode(Convert.ToChar(pass[i]));
                }
                try
                {
                    User.Password = pass;
                    User.Old_Password = User.Password;
                    db.Entry(User).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    ViewData["Message"] = "Error found during update password. Please try again.";
                    throw;
                }
                ViewData["Message"] = "Success";
            }
            else
            {
                ViewData["Message"] = "No User Found";
            }
            return View();
        }
        private static string getEncode(char iChr)
        {
            switch (iChr)
            {
                case 'a': return "coe";
                case 'b': return "wer";
                case 'c': return "ibq";
                case 'd': return "am7";
                case 'e': return "pm1";
                case 'f': return "mop";
                case 'g': return "9v4";
                case 'h': return "qu6";
                case 'i': return "zxc";
                case 'j': return "4mp";
                case 'k': return "f88";
                case 'l': return "qe2";
                case 'm': return "vbn";
                case 'n': return "qwt";
                case 'o': return "pl5";
                case 'p': return "13s";
                case 'q': return "c%l";
                case 'r': return "w$w";
                case 's': return "6a@";
                case 't': return "!2&";
                case 'u': return "(=c";
                case 'v': return "wvf";
                case 'w': return "dp0";
                case 'x': return "w$-";
                case 'y': return "vn&";
                case 'z': return "c*4";

                case '1': return "aq@";
                case '2': return "902";
                case '3': return "2.&";
                case '4': return "/w!";
                case '5': return "|pq";
                case '6': return "ml|";
                case '7': return "t`?";
                case '8': return ">^s";
                case '9': return "<s^";
                case '0': return ";&c";

                case 'A': return "$)c";
                case 'B': return "-gt";
                case 'C': return "|p*";
                case 'D': return "1" + ((char)34).ToString() + "r";
                case 'E': return "c>:";
                case 'F': return "@+x";
                case 'G': return "v^a";
                case 'H': return "]eE";
                case 'I': return "aP0";
                case 'J': return "{=1";
                case 'K': return "cWv";
                case 'L': return "cDc";
                case 'M': return "*,!";
                case 'N': return "fW" + ((char)34).ToString();
                case 'O': return ".?T";
                case 'P': return "%<8";
                case 'Q': return "@:a";
                case 'R': return "&c$";
                case 'S': return "WnY";
                case 'T': return "{Sh";
                case 'U': return "_%M";
                case 'V': return "}`$";
                case 'W': return "QlU";
                case 'X': return "Im^";
                case 'Y': return "l|P";
                case 'Z': return ".>#";

                case '!': return "\\" + ((char)34).ToString() + "]";
                case '@': return "cY,";
                case '#': return "x%B";
                case '$': return "a*v";
                case '%': return "`&T";
                case '^': return ";%R";
                case '&': return "eG_";
                case '*': return "Z/e";
                case '(': return "rG\\";
                case ')': return "]*F";
                case '_': return "@B*";
                case '-': return "+Hc";
                case '=': return "&|D";
                case '+': return "(:#";
                case '[': return "SlW";
                case ']': return "`QB";
                case '{': return "{D>";
                case '}': return "+c%";
                case ':': return "(s:";
                case ';': return "^a(";
                case '`': return "16.";
                case (char)34: return "s.*";
                case ',': return "&?W";
                case '.': return "GPQ";
                case '<': return "SK*";
                case '>': return "RL^";
                case '/': return "40C";
                case '?': return "?#9";
                case '\\': return "_?/";
                case '|': return "(_@";
                case ' ': return "=#B";
                default: return "000";
            }
        }
        public string PassDecode(string vText)
        {
            int i = 0;
            int lrLen;
            string lrChr = "";
            string lrFin = "";
            i = (i + 1);
            lrLen = vText.Length;
            while ((i <= lrLen))
            {
                lrChr = vText.Substring((i - 1), 3);
                switch (lrChr)
                {
                    case "coe":
                        lrChr = "a";
                        break;
                    case "wer":
                        lrChr = "b";
                        break;
                    case "ibq":
                        lrChr = "c";
                        break;
                    case "am7":
                        lrChr = "d";
                        break;
                    case "pm1":
                        lrChr = "e";
                        break;
                    case "mop":
                        lrChr = "f";
                        break;
                    case "9v4":
                        lrChr = "g";
                        break;
                    case "qu6":
                        lrChr = "h";
                        break;
                    case "zxc":
                        lrChr = "i";
                        break;
                    case "4mp":
                        lrChr = "j";
                        break;
                    case "f88":
                        lrChr = "k";
                        break;
                    case "qe2":
                        lrChr = "l";
                        break;
                    case "vbn":
                        lrChr = "m";
                        break;
                    case "qwt":
                        lrChr = "n";
                        break;
                    case "pl5":
                        lrChr = "o";
                        break;
                    case "13s":
                        lrChr = "p";
                        break;
                    case "c%l":
                        lrChr = "q";
                        break;
                    case "w$w":
                        lrChr = "r";
                        break;
                    case "6a@":
                        lrChr = "s";
                        break;
                    case "!2&":
                        lrChr = "t";
                        break;
                    case "(=c":
                        lrChr = "u";
                        break;
                    case "wvf":
                        lrChr = "v";
                        break;
                    case "dp0":
                        lrChr = "w";
                        break;
                    case "w$-":
                        lrChr = "x";
                        break;
                    case "vn&":
                        lrChr = "y";
                        break;
                    case "c*4":
                        lrChr = "z";
                        break;
                    case "aq@":
                        lrChr = "1";
                        break;
                    case "902":
                        lrChr = "2";
                        break;
                    case "2.&":
                        lrChr = "3";
                        break;
                    case "/w!":
                        lrChr = "4";
                        break;
                    case "|pq":
                        lrChr = "5";
                        break;
                    case "ml|":
                        lrChr = "6";
                        break;
                    case "t`?":
                        lrChr = "7";
                        break;
                    case ">^s":
                        lrChr = "8";
                        break;
                    case "<s^":
                        lrChr = "9";
                        break;
                    case ";&c":
                        lrChr = "0";
                        break;
                    case "$)c":
                        lrChr = "A";
                        break;
                    case "-gt":
                        lrChr = "B";
                        break;
                    case "|p*":
                        lrChr = "C";
                        break;
                    case ("1" + "\"" + "r"):
                        lrChr = "D";
                        break;
                    case "c>:":
                        lrChr = "E";
                        break;
                    case "@+x":
                        lrChr = "F";
                        break;
                    case "v^a":
                        lrChr = "G";
                        break;
                    case "]eE":
                        lrChr = "H";
                        break;
                    case "aP0":
                        lrChr = "I";
                        break;
                    case "{=1":
                        lrChr = "J";
                        break;
                    case "cWv":
                        lrChr = "K";
                        break;
                    case "cDc":
                        lrChr = "L";
                        break;
                    case "*,!":
                        lrChr = "M";
                        break;
                    case ("fW" + "\""):
                        lrChr = "N";
                        break;
                    case ".?T":
                        lrChr = "O";
                        break;
                    case "%<8":
                        lrChr = "P";
                        break;
                    case "@:a":
                        lrChr = "Q";
                        break;
                    case "&c$":
                        lrChr = "R";
                        break;
                    case "WnY":
                        lrChr = "S";
                        break;
                    case "{Sh":
                        lrChr = "T";
                        break;
                    case "_%M":
                        lrChr = "U";
                        break;
                    case "}`$":
                        lrChr = "V";
                        break;
                    case "QlU":
                        lrChr = "W";
                        break;
                    case "Im^":
                        lrChr = "X";
                        break;
                    case "l|P":
                        lrChr = "Y";
                        break;
                    case ".>#":
                        lrChr = "Z";
                        break;
                    case ("\\" + "\"" + "]"):
                        lrChr = "!";
                        break;
                    case "cY,":
                        lrChr = "@";
                        break;
                    case "x%B":
                        lrChr = "#";
                        break;
                    case "a*v":
                        lrChr = "$";
                        break;
                    case "`&T":
                        lrChr = "%";
                        break;
                    case ";%R":
                        lrChr = "^";
                        break;
                    case "eG_":
                        lrChr = "&";
                        break;
                    case "Z/e":
                        lrChr = "*";
                        break;
                    case "rG\\":
                        lrChr = "(";
                        break;
                    case "]*F":
                        lrChr = ")";
                        break;
                    case "@B*":
                        lrChr = "_";
                        break;
                    case "+Hc":
                        lrChr = "-";
                        break;
                    case "&|D":
                        lrChr = "=";
                        break;
                    case "(:#":
                        lrChr = "+";
                        break;
                    case "SlW":
                        lrChr = "[";
                        break;
                    case "`QB":
                        lrChr = "]";
                        break;
                    case "{D>":
                        lrChr = "{";
                        break;
                    case "+c%":
                        lrChr = "}";
                        break;
                    case "(s:":
                        lrChr = ":";
                        break;
                    case "^a(":
                        lrChr = ";";
                        break;
                    case "16.":
                        lrChr = "`";
                        break;
                    case "s.*":
                        lrChr = "\"";
                        break;
                    case "&?W":
                        lrChr = ",";
                        break;
                    case "GPQ":
                        lrChr = ".";
                        break;
                    case "SK*":
                        lrChr = "<";
                        break;
                    case "RL^":
                        lrChr = ">";
                        break;
                    case "40C":
                        lrChr = "/";
                        break;
                    case "?#9":
                        lrChr = "?";
                        break;
                    case "_?/":
                        lrChr = "\\";
                        break;
                    case "(_@":
                        lrChr = "|";
                        break;
                    case "=#B":
                        lrChr = " ";
                        break;
                }
                lrFin = (lrFin + lrChr);
                i = (i + 3);
            }
            return lrFin;
        }
        public ActionResult RolePermission()
        {
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
                        ViewBag.RoleList = new SelectList(db.RoleName.ToList(), "Id", "RoleCaption"); 
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
                //return Redirect("https://scsapps.xyz/Bypass");
                return this.RedirectToAction("SmartLogins", "Setings");
            }
        }
        public JsonResult GetRoleList(string RoleCaption)
        {
            List<RoleSetting> item = new List<RoleSetting>();   
            //item = db.Database.SqlQuery<RoleSetting>("select RSettings.Id,RSettings.MenuName,RSettings.PermittedBy,RSettings.AssignDate,RSettings.SubMenuName,RSettings.Path,RNames.RoleCaption from RoleSettings RSettings left join RoleNames RNames on RNames.RoleCaption=RSettings.RoleCaption and RSettings.RoleCaption='" + RoleCaption + "'").ToList();
            item = db.Database.SqlQuery<RoleSetting>("select DISTINCT RSettings.Path,RSettings.SubMenuName,RSettings.MenuName,Permitted.RoleCaption,1 as id, GETDATE() AssignDate, '1'as PermittedBy from RoleSettings RSettings  left join (select Path,SubMenuName,MenuName,RoleCaption from RoleSettings where RoleCaption='" + RoleCaption + "') Permitted on RSettings.Path=Permitted.Path and RSettings.SubMenuName=Permitted.SubMenuName and RSettings.MenuName=Permitted.MenuName").ToList();
            return new JsonResult { Data = item, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public JsonResult GiveRolePermission(string MenuName, string SubMenuName, string Path,string RoleName)
        {
            HttpCookie useridCookie = Request.Cookies["um_apps_userid"];
            string userId = useridCookie == null ? "nouser" : useridCookie.Value.ToString();

            string Message = "";
            var RoleList = db.RoleSetting.Where(x => x.MenuName==MenuName && x.SubMenuName== SubMenuName && x.Path==Path).FirstOrDefault();

            var RoleListExist = db.RoleSetting.Where(x => x.MenuName==RoleList.MenuName && x.SubMenuName==RoleList.SubMenuName && x.RoleCaption == RoleName).FirstOrDefault();
            var RoleListNull = db.RoleSetting.Where(x => x.MenuName == RoleList.MenuName && x.SubMenuName == RoleList.SubMenuName && x.RoleCaption == null).FirstOrDefault();

            if (RoleListExist == null)
            { 
                if(RoleListNull != null)
                {
                    RoleListNull.RoleCaption = RoleName;
                    RoleListNull.PermittedBy = userId;
                    RoleListNull.AssignDate = DateTime.Now;
                    db.Entry(RoleListNull).State = EntityState.Modified;
                    db.SaveChanges();
                    Message = "Update Successfully";
                }
                else
                {
                    RoleSetting RolePermission = new RoleSetting();
                    RolePermission.MenuName = RoleList.MenuName;
                    RolePermission.SubMenuName = RoleList.SubMenuName;
                    RolePermission.AssignDate = DateTime.Now;
                    RolePermission.Path = RoleList.Path;
                    RolePermission.PermittedBy = userId;
                    RolePermission.RoleCaption = RoleName;
                    db.RoleSetting.Add(RolePermission);
                    db.SaveChanges();
                    Message = "Added Successfully";
                }
            }
            else
            {
                RoleListExist.RoleCaption = null;
                RoleListExist.PermittedBy = userId;
                RoleListExist.AssignDate = DateTime.Now;
                db.Entry(RoleListExist).State = EntityState.Modified;
                db.SaveChanges();
                Message = "Update Successfully";
            }            
            return Json(Message);
        }
        public JsonResult GetMenuList(string RoleCaption)
        {
            List<RoleSetting> item = new List<RoleSetting>();
            item = db.Database.SqlQuery<RoleSetting>("select * from RoleSettings where RoleCaption='"+ RoleCaption + "' order by MenuName").ToList();
            return new JsonResult { Data = item, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public ActionResult ApprovalPermission()
        {
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
                        ViewBag.TypeList = new SelectList(db.ReqTypeName.ToList(), "id", "TypeName");
                        ViewBag.UserList = new SelectList(db.UserInfo, "Id", "UserName");
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
                return this.RedirectToAction("SmartLogins","Setings");
                //return Redirect("https://scsapps.xyz/Bypass");
            }
        }
        public JsonResult GetApprovalSeq(string TypeId, string UserId)
        {
            int types = Convert.ToInt32(TypeId);
            int User = Convert.ToInt32(UserId);
            List<ReqApprovSeqPermissionVM> item = new List<ReqApprovSeqPermissionVM>();

            item = db.Database.SqlQuery<ReqApprovSeqPermissionVM>("select ISNULL(ReqAppSeqPer.UserID, 0)UserID,ISNULL(ReqAppSeqPer.AuthorizedID, 0)AuthorizedID,ReqChkList.CheckName,ReqChkList.Types TypeId,ReqChkList.CheckId,ISNULL(ReqChkList.Input_Date, 0) AuthorizedDate from ReqApprovSeqPermissions ReqAppSeqPer right join (select * from ReqCheckLists where Types=" + types + ")ReqChkList on ReqChkList.Types = ReqAppSeqPer.TypeId and ReqAppSeqPer.UserID = "+ User ).ToList();
                     
            return new JsonResult { Data = item, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        //public JsonResult GetReqType(int TypeId)
        //{
        //    List<ReqCheckList> item = new List<ReqCheckList>();
        //    if (TypeId != 0)
        //    {                
        //        item = db.ReqCheckList.Where(x=>x.Types== TypeId).ToList();
        //    }
        //    return new JsonResult { Data = item, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        //}
        public JsonResult GiveReqApprovedPermission(int CheckId, int UserId, int TypeId)
        {
            UserInfo userInfo = (UserInfo)Session["UserProfile"];

            string Message = "";

            //var ApprovedByUser = db.ReqApprovSeqPermission.Where(x =>  x.UserID == UserId && x.TypeId == TypeId).FirstOrDefault();
            //if(ApprovedByUser!= null)
            //{
            //    var Approved = db.ReqApprovSeqPermission.Where(x => x.CheckId == CheckId && x.UserID == UserId && x.TypeId == TypeId).FirstOrDefault();
            //    if(Approved != null)
            //    {
            //        if (Approved.AuthorizedID == null)
            //        {
            //            Approved.AuthorizedID = userInfo.Id;
            //            Approved.CheckId = CheckId;
            //            Approved.UserID = UserId;
            //            Approved.TypeId = TypeId;
            //        }
            //        else
            //        {
            //            Approved.AuthorizedID = userInfo.Id;
            //            Approved.CheckId = CheckId;
            //            Approved.UserID = 0;
            //            Approved.TypeId = 0;
            //        }
            //        Approved.AuthorizedDate = DateTime.Now;
            //        db.Entry(Approved).State = EntityState.Modified;
            //        Message = "Update Successfully";
            //    }
            //    else
            //    {

            //    }
            //}
            //else
            //{
            //    ReqApprovSeqPermission ReqPermission = new ReqApprovSeqPermission();
            //    ReqPermission.AuthorizedDate = DateTime.Now;
            //    ReqPermission.AuthorizedID = userInfo.Id;
            //    ReqPermission.CheckId = CheckId;
            //    ReqPermission.UserID = UserId;
            //    ReqPermission.CheckId = CheckId;
            //    ReqPermission.TypeId = TypeId;
            //    db.ReqApprovSeqPermission.Add(ReqPermission);
            //    Message = "Added Successfully";
            //}

            var Approved = db.ReqApprovSeqPermission.Where(x => x.CheckId == CheckId && x.UserID == UserId && x.TypeId == TypeId).FirstOrDefault();

            if (Approved != null)
            {
                if (Approved.Status == 0)
                {
                    Approved.AuthorizedID = userInfo.Id;
                    Approved.Status = 1;
                }
                else
                {
                    Approved.AuthorizedID = userInfo.Id;
                    Approved.Status = 0;
                }
                Approved.AuthorizedDate = DateTime.Now;
                db.Entry(Approved).State = EntityState.Modified;
                Message = "Update Successfully";
            }
            else
            {
                ReqApprovSeqPermission ReqPermission = new ReqApprovSeqPermission();
                ReqPermission.AuthorizedDate = DateTime.Now;
                ReqPermission.AuthorizedID = userInfo.Id;
                ReqPermission.CheckId = CheckId;
                ReqPermission.UserID = UserId;
                ReqPermission.CheckId = CheckId;
                ReqPermission.TypeId = TypeId;
                db.ReqApprovSeqPermission.Add(ReqPermission);
                Message = "Added Successfully";
            }
            db.SaveChanges();
            return Json(Message);
        }
        public ActionResult RoleEntry()
        {
            ViewBag.RoleList = db.RoleName.ToList();            
            return View();
        }
        [HttpPost]
        public ActionResult AddRole(RoleName objRole)
        {
            using (DataContext db = new DataContext())
            {                
                objRole.SL = db.RoleName.Count() + 1;
                db.RoleName.Add(objRole);
            }
            return RedirectToAction("RoleEntry");
        }
        public ActionResult DeleteRole(int id)
        {
            RoleName role = db.RoleName.Find(id);
            db.RoleName.Remove(role);
            db.SaveChanges();
            return RedirectToAction("RoleEntry");
        }
        public ActionResult EditRole(int id)
        {
            var Role = db.RoleName.Single(p => p.Id == id);
            return View(Role);
        }
        [HttpPost]
        public ActionResult EditRole(RoleName model)
        {
            if (ModelState.IsValid)
            {                
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("RoleEntry");
            }
            return View(model);
        }
        public ActionResult Permission()
        {
            UserInfo userInfo = (UserInfo)Session["UserProfile"];
            if (userInfo != null)
            {
                HttpCookie useridCookie = Request.Cookies["um_apps_userid"];
                if (userInfo.UserName == useridCookie.Values.ToString())
                {
                    HttpCookie rolesCookie = Request.Cookies["um_apps_roles"];
                    string appsrole = rolesCookie == null ? "noroles" : rolesCookie.Value.ToString();
                    appsrole = appsrole.Replace("%20", " ");
                    string controller_action = this.ControllerContext.RouteData.Values["controller"].ToString() + "/" + this.ControllerContext.RouteData.Values["action"].ToString();
                    int MenuId = db.Database.SqlQuery<int>("select id from RoleSettings where Path='" + controller_action + "' and RoleCaption='" + appsrole + "'").FirstOrDefault();
                    if (MenuId != 0)
                    {
                        ViewBag.UserList = db.Database.SqlQuery<UserAuthorizationVM>("select UInfo.Id,UInfo.HRId,UInfo.Name,BInfo.BranchName Branch,DEntries.DepartmentName as Department,convert(varchar, UInfo.Create_Date, 103)Create_Date,UInfo.Role from(select Uinfo.Id, Uinfo.UserName as HRId, Uinfo.Name, Uinfo.Activation_Date Create_Date, isnull(RName.RoleCaption, '')Role, Uinfo.Branch_Id, Uinfo.Department_Id from UserInfoes Uinfo left join RoleNames RName on Uinfo.RoleId = RName.Id) UInfo, BranchInfoes BInfo, DepartmentEntries DEntries where UInfo.Branch_Id = BInfo.BranchID and UInfo.Department_Id = DEntries.DepartmentId").ToList(); ;
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
        public ActionResult EditPermission(int Id)
        {
            var Users = db.UserInfo.Single(p => p.Id == Id);
            ViewBag.UserData = db.Database.SqlQuery<UserAuthorizationVM>("select UInfo.UserName as HRId,UInfo.Name,BInfo.BranchName as Branch,DEntries.DepartmentName as Department, convert(varchar, UInfo.Activation_Date, 103) Create_Date, UInfo.RoleId from UserInfoes UInfo, BranchInfoes BInfo, DepartmentEntries DEntries where UInfo.Branch_Id = BInfo.BranchID and UInfo.Department_Id = DEntries.DepartmentId and UInfo.Id = " + Id + "") ;
            ViewBag.Rolse= new SelectList(db.RoleName.ToList(), "Id", "RoleCaption");
            return View(Users);
        }  
        public JsonResult EditPermis(int? RoleID, string HRId, int Branch_Id, int Department_Id)
        {            
                int TotalUpdate = 0;            
                var previous = db.UserInfo.SingleOrDefault(p => p.UserName ==HRId && p.Branch_Id == Branch_Id && p.Department_Id== Department_Id) ;
                previous.RoleId = RoleID;       
                db.Entry(previous).State = EntityState.Modified;
                TotalUpdate=db.SaveChanges();
                         
            return Json(TotalUpdate);
        }
        public ActionResult Previlize()
        {
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
                        //ViewBag.BranchList = new SelectList(db.Branch.ToList(), "BranchID", "BranchName");
                        ViewBag.ReqCheckLists = new SelectList(db.ReqCheckList.ToList(), "CheckId", "CheckName");
                        ViewBag.ReqCategoryList = new SelectList(db.ReqCategoryNames, "Id", "TypeName");
                        ViewBag.AllReqSeqList= db.Database.SqlQuery<BranchReqSeqAssignVM>("select RCN.TypeName CategoryName,BRSA.ReqSeqID,BRSA.UserName,FORMAT(BRSA.ActivationDate,'dd/MM/yyyy') as ActivationDate,t1.CheckName " +
                            "from BranchReqSeqAssigns BRSA,ReqCategoryNames RCN,(SELECT TOP 2147483647 ReqSeqID, CheckName = dbo.getallSequenceName(ReqSeqID) FROM dbo.BranchReqSeqAssigns  GROUP BY ReqSeqID  ORDER BY ReqSeqID) t1" +
                            " where RCN.Id=BRSA.CategoryID and BRSA.ReqSeqID=t1.ReqSeqID group by RCN.TypeName,BRSA.ReqSeqID,BRSA.UserName,BRSA.ActivationDate,t1.CheckName").ToList();
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
                return View();
            }
        }
        public JsonResult SaveBranchReqSeq(List<BranchReqSeqAssign> Sequence_Details)
        {
            int serial = 0;
            string Message = "";
            int intIdt = db.BranchReqSeqAssign.Select(u => u.ReqSeqID).DefaultIfEmpty(0).Max();
            intIdt = intIdt + 1;
            HttpCookie useridCookie = Request.Cookies["um_apps_userid"];
            //for (int i = 0; i < checks.Length; i++)
            //{
            //    ReqStatusSequence ReqSequence = new ReqStatusSequence();
            //    ReqSequence.AuthorizeDate = DateTime.Now;
            //    ReqSequence.typeId = typeid;
            //    ReqSequence.AuthorizedUserName = authorizedUserName;
            //    ReqSequence.ReqStatus = intIdt;
            //    ReqSequence.CheckId = checks[i];
            //    db.ReqStatusSequence.Add(ReqSequence);
            //    db.SaveChanges();
            //    Message = "Added Successfully!";
            //}

            foreach (BranchReqSeqAssign details in Sequence_Details)
            {
                serial = serial + 1;
                details.ReqSeqID = intIdt;
                details.AssigneDate = DateTime.Now;
                details.UserName = useridCookie.Values.ToString();
                details.Serial = serial;
                db.BranchReqSeqAssign.Add(details);
                db.SaveChanges();
                Message = "Added Successfully!";
            }
            return Json(Message);
        }
        public ActionResult BranchUserPermission()
        {
            ViewBag.BranchList = new SelectList(db.Branch.ToList(), "BranchID", "BranchName");
            ViewBag.Designation = new SelectList(db.Designations.ToList(), "Id", "DesignationName");
            return View();
        }
        public JsonResult GetApprovalSequence(int BranchID)
        {
            //List<ReqCheckList> item = new List<ReqCheckList>();
            var item = db.Database.SqlQuery<BranchReqSeqAssignVM>("select RCL.CheckId,RCL.CheckName from ReqCheckLists RCL,BranchReqSeqAssigns BRSA where BRSA.CheckID=RCL.CheckId and BRSA.BranchID=" + BranchID +" and BRSA.ReqSeqID=(select max(ReqSeqID) ReqSeqID from BranchReqSeqAssigns where BranchID="+ BranchID + ") order by BRSA.Serial" ).ToList();
            return new JsonResult { Data = item, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public JsonResult GetUserByBranchAndDesignation(int BranchID,int DesignationID)
        {
            List<UserInfo> item = new List<UserInfo>();
             item = db.Database.SqlQuery<UserInfo>("select * from UserInfoes where Branch_Id="+ BranchID + " and Designation=" + DesignationID ).ToList<UserInfo>();
            return new JsonResult { Data = item, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public JsonResult GiveBranchUserPermission(int BranchID,string UserId,int CheckId)
        {
            HttpCookie useridCookie = Request.Cookies["um_apps_userid"];
            string Message = "";
            UserWisePermission BranchUserPermission = new UserWisePermission();

            BranchUserPermission.PrevelizeDate = DateTime.Now;
            BranchUserPermission.UserName = UserId;
            BranchUserPermission.BranchId = BranchID;
            BranchUserPermission.CheckId = CheckId;
            BranchUserPermission.PrevelizeUserId = useridCookie.Values.ToString();

            db.UserWisePermissions.Add(BranchUserPermission);
            Message = "Added Successfully";

            db.SaveChanges();
            return Json(Message);
        }
        public ActionResult BranchUserApproval()
        {
            ViewBag.BranchList = new SelectList(db.Branch, "BranchID", "BranchName");
            ViewBag.ReqCategoryList = new SelectList(db.ReqCategoryNames, "Id", "TypeName");

            ViewBag.CBD = db.Database.SqlQuery<BranchReqSeqAssignVM>("select Category CategoryName,Binfo.BranchName,DEntry.DepartmentName from BranchUserApprovals BUA, BranchInfoes Binfo, DepartmentEntries DEntry where BUA.DepartmentId = DEntry.DepartmentId and BUA.BranchId = Binfo.BranchID and Category is not null group by Category, Binfo.BranchName, dentry.DepartmentName").ToList();
            ViewBag.CBDCU = db.Database.SqlQuery<BranchReqSeqAssignVM>("select Category CategoryName,Binfo.BranchName,DEntry.DepartmentName,RCL.CheckName,UserId UserName from BranchUserApprovals BUA,BranchInfoes Binfo, DepartmentEntries DEntry,ReqCheckLists RCL where BUA.DepartmentId=DEntry.DepartmentId and BUA.BranchId=Binfo.BranchID and Category is not null and BUA.CheckId=RCL.CheckId group by Category,Binfo.BranchName,dentry.DepartmentName,RCL.CheckName,UserId").ToList();


            //ViewBag.Designation = new SelectList(db.Designations.ToList(), "Id", "DesignationName");  
            return View();
        }
        public JsonResult GetDesignationNameByBranch(int BranchId)
        {
            var item = db.Database.SqlQuery<Designation>("select * from Designations where id in(select Designation from UserInfoes where Branch_id=" + BranchId + ")").ToList();
            return new JsonResult { Data = item, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public JsonResult GetReqSeqNameCategory(int CategoryID)
        {
            var item = db.Database.SqlQuery<ReqCheckList>("select RCL.* from BranchReqSeqAssigns BRS,ReqCheckLists RCL where BRS.ReqSeqID = (select MAX(ReqSeqID)ReqSeqID from BranchReqSeqAssigns where CategoryID = " + CategoryID + ") and BRS.CheckID=RCL.CheckId order by BRS.Serial").ToList();
            return new JsonResult { Data = item, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public JsonResult GetUserNameByDesignation(int DesignationId, int BranchId)
        {
            var item = db.Database.SqlQuery<UserInfo>("select * from UserInfoes where Designation="+ DesignationId + " and Branch_Id="+ BranchId).ToList();
            return new JsonResult { Data = item, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public JsonResult SaveBranchUserApproval(List<BranchUserApproval> BranchUserApproval)
        {
            int serial = 0;
            string Message = "";
            foreach (BranchUserApproval details in BranchUserApproval)
            {
                serial = serial + 1;                
                details.PrevelizeDate = DateTime.Now;                
                db.BranchUserApprovals.Add(details);
                //db.SaveChanges();
                //Message = "Added Successfully!";
            }
            int totalNumberOfRow=db.SaveChanges();
            return Json(totalNumberOfRow);
        }
        public JsonResult GetDepartmentNameByBranch(int BranchId)
        {
            var item = db.Database.SqlQuery<DepartmentEntry>("select DEntry.* from DepartmentInfoes DInfo,DepartmentEntries DEntry where DInfo.DepartmentId=DEntry.DepartmentId and BranchId=" + BranchId).ToList();
            return new JsonResult { Data = item, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

    }
}