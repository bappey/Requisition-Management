using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SCS_Inventory.Models;
using scs_Project.Models;

namespace scs_Project.Controllers
{
    public class VendorController : Controller
    {
        DataContext db = new DataContext();
        // GET: Vendor
        [RequireHttps]
        public ActionResult Index()
        {
            //Hashtable userInfo = (Hashtable)Session["UserProfile"];
            
            UserInfo userInfo = (UserInfo)Session["UserProfile"];
            if (userInfo != null)
            {
                HttpCookie useridCookie = Request.Cookies["um_apps_userid"];
                if (userInfo.UserName== useridCookie.Value.ToString())
                {
                    HttpCookie rolesCookie = Request.Cookies["um_apps_roles"];
                    string appsrole = rolesCookie == null ? "noroles" :  rolesCookie.Value.ToString();
                    appsrole = appsrole.Replace("%20", " ");
                    string controller_action = this.ControllerContext.RouteData.Values["controller"].ToString() + "/" + this.ControllerContext.RouteData.Values["action"].ToString();
                    int MenuId = db.Database.SqlQuery<int>("select id from RoleSettings where Path='" + controller_action + "' and RoleCaption='" + appsrole + "'").FirstOrDefault();
                    if (MenuId != 0)
                    {
                        List<VendorInfo> vendors = db.Vendor.ToList();
                        return View(vendors);
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

        //public ActionResult AddVendor()
        //{
        //    return View();
        //}
        [HttpPost]
        public ActionResult AddVendor(VendorInfo objVendor)
        {
            using (DataContext db = new DataContext())
            {
                objVendor.date = DateTime.Now;
                objVendor.time = DateTime.Now;
                objVendor.Row_Status = true;

                db.Vendor.Add(objVendor);
                db.SaveChanges();
                Int64 id = objVendor.id;
            }
            return RedirectToAction("Index");
        }
        public ActionResult DeleteVendor(int id)
       {
            
            VendorInfo city = db.Vendor.Find(id);
            db.Vendor.Remove(city);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult EditVendor(int id)
        {
            var vendor = db.Vendor.Single(p => p.id == id);
            return View(vendor);
        }
        [HttpPost]
        public ActionResult EditVendor(VendorInfo model)
        {
            if (ModelState.IsValid)
            {
                model.date = DateTime.Now;
                model.time = DateTime.Now;
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);  
        }
    }
}