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
    public class BranchController : Controller
    {
        DataContext db = new DataContext();
        // GET: Branch
        public ActionResult Index()
        {
            UserInfo userInfo = (UserInfo)Session["UserProfile"];
            if (userInfo != null)
            {
                HttpCookie useridCookie = Request.Cookies["um_apps_userid"];
                if(userInfo.UserName== useridCookie.Values.ToString())
                {
                    HttpCookie rolesCookie = Request.Cookies["um_apps_roles"];
                    string appsrole = rolesCookie == null ? "noroles" : rolesCookie.Value.ToString();
                    appsrole = appsrole.Replace("%20", " ");
                    string controller_action = this.ControllerContext.RouteData.Values["controller"].ToString() + "/" + this.ControllerContext.RouteData.Values["action"].ToString();
                    //int MenuId = db.Database.SqlQuery<int>("select isnull(UA.MenuId,0)MenuId from MenuDeclares MD,UserAuthontications UA where MD.Id=UA.MenuId and UA.UserId=" + userInfo.Id + " and MD.Path='" + controller_action + "'").FirstOrDefault();
                    int MenuId = db.Database.SqlQuery<int>("select id from RoleSettings where Path='" + controller_action + "' and RoleCaption='" + appsrole + "'").FirstOrDefault();
                    if (MenuId != 0)
                    {
                        List<BranchInfo> Branch = db.Branch.ToList();
                        ViewBag.DivisionList = new SelectList(db.Division, "id", "DivisionName");
                        ViewBag.TypeList = new SelectList(db.BranchType, "id", "TypeName");
                        return View(Branch);
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
            //return View();            
        }
        [HttpPost]
        public ActionResult AddBranch(BranchInfo objBranch)
        {
            int intIdt = db.Branch.Select(u => u.BranchID).DefaultIfEmpty(0).Max();
            
            if (intIdt == 0)
            {
                intIdt = 100;
            }

            objBranch.BranchID = intIdt + 1;

            objBranch.Input_Date = DateTime.Now;
            objBranch.Input_User =1;
            //objBranch.Row_Status = true;
            db.Branch.Add(objBranch);
            db.SaveChanges();
            Int64 id = objBranch.id;
            return RedirectToAction("Index");
        }
        public ActionResult DeleteBranch(int id)
        {
            BranchInfo branch = db.Branch.Find(id);
            db.Branch.Remove(branch);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult EditBranch(int id)
        {
            ViewBag.DivisionList = new SelectList(db.Division, "id", "DivisionName");
            ViewBag.TypeList = new SelectList(db.BranchType, "id", "TypeName");

            var branch = db.Branch.Single(p => p.id == id);
            return View(branch);
        }
        [HttpPost]
        public ActionResult EditBranch(BranchInfo model)
        {
            if (ModelState.IsValid)
            {
                model.Edit_Date = DateTime.Now;
                model.Edit_User = 1; 

                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }

    }
} 