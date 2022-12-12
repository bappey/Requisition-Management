using SCS_Inventory.Models;
using scs_Project.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace scs_Project.Controllers
{
    public class DepartmentController : Controller
    {
        DataContext db = new DataContext();
        // GET: Department
        public ActionResult Index()
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
                        ViewBag.BranchName = new SelectList(db.Branch.ToList(), "BranchID", "BranchName");
                        ViewBag.DepartmentName = new SelectList(db.DepartmentEntry.ToList(), "DepartmentId", "DepartmentName");
                        ViewBag.departmentsRecord = from d in db.Department
                                                    join b in db.Branch on d.BranchId equals b.BranchID
                                                    join e in db.DepartmentEntry on d.DepartmentId equals e.DepartmentId
                                                    select new DeptListViewModel { DeptInfo = d, BInfo = b, D_Entry = e };
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
        //public ActionResult AddDepartment()
        //{
        //    ViewBag.BranchName = new SelectList(db.Branch.ToList(), "BranchCode", "BranchName");
        //    return View();
        //}
        [HttpPost]
        public ActionResult AddDepartment(DepartmentInfo objDepartment)
        {
            using (DataContext db = new DataContext())
            {
                HttpCookie useridCookie = Request.Cookies["um_apps_userid"];
                string userid = useridCookie == null ? "nouser" : useridCookie.Value.ToString();

                objDepartment.Input_Date = DateTime.Now;
                objDepartment.Input_User = userid;
                db.Department.Add(objDepartment);
                db.SaveChanges();
                Int64 id = objDepartment.id;
            }
            return RedirectToAction("Index");
        }
        public ActionResult DeleteDepartment(int id)
        {
            DepartmentInfo dept = db.Department.Find(id);
            db.Department.Remove(dept);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult EditDepartment(int id)
        {
            var department = db.Department.Single(p => p.id == id);
            ViewBag.BranchName = new SelectList(db.Branch.ToList(), "ID", "BranchName");
            ViewBag.DepartmentName = new SelectList(db.DepartmentEntry.ToList(), "Department_Id", "DepartmentName");
            return View(department);
        }
        [HttpPost]
        public ActionResult EditDepartment(DepartmentInfo model)
        {
            if (ModelState.IsValid)
            {
                HttpCookie useridCookie = Request.Cookies["um_apps_userid"];
                string userid = useridCookie == null ? "nouser" : useridCookie.Value.ToString();

                model.Edit_Date = DateTime.Now;
                model.Edit_User = userid;

                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }        
        public ActionResult DepartmentIndex()
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
                        List<DepartmentEntry> DepartmentEntry = db.DepartmentEntry.ToList();
                        return View(DepartmentEntry);
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
        [HttpPost]
        public ActionResult EntryDepartment(DepartmentEntry objDepartment)
        {
            HttpCookie useridCookie = Request.Cookies["um_apps_userid"];
            string userid = useridCookie == null ? "nouser" : useridCookie.Value.ToString();
            int intIdt = db.DepartmentEntry.Select(u => u.DepartmentId).DefaultIfEmpty(0).Max();
            //int intIdt = db.DepartmentEntry.Select(u => u.DepartmentId).Max();
            objDepartment.DepartmentId = intIdt + 1;
            objDepartment.Input_Date = DateTime.Now;
            objDepartment.Input_User = userid;

            db.DepartmentEntry.Add(objDepartment);
            db.SaveChanges();
            return RedirectToAction("DepartmentIndex");
        }
        public ActionResult DepartmentDelete(int id)
        {
            DepartmentEntry department = db.DepartmentEntry.Find(id);
            db.DepartmentEntry.Remove(department);
            db.SaveChanges();
            return RedirectToAction("DepartmentIndex");
        }
        public ActionResult DepartmentEdit(int id)
        {
            var departments = db.DepartmentEntry.Single(p => p.id == id);
            return View(departments);
        }
        [HttpPost]
        public ActionResult DepartmentEdit(DepartmentEntry model)
        {
            if (ModelState.IsValid)
            {
                HttpCookie useridCookie = Request.Cookies["um_apps_userid"];
                string userid = useridCookie == null ? "nouser" : useridCookie.Value.ToString();

                model.Edit_User = userid;
                model.Edit_Date = DateTime.Now;
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("DepartmentIndex");
            }
            return View(model);
        }
    }
}