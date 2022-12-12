using SCS_Inventory.Models;
using scs_Project.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace SCS_Inventory.Controllers
{
    public class ItemController : Controller
    {
        DataContext db = new DataContext();
        //
        // GET: /Item/
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ProductListIndex()
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
                        return View(db.ProductList.ToList());
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
        public ActionResult AddProduct(ProductList Products)
        {
            int intIdt = db.ProductList.Select(u => u.ProductId).DefaultIfEmpty(0).Max();
            
            Products.ProductId = intIdt + 1;
            Products.Input_Date  = DateTime.Now;
            Products.Input_User =1;
            Products.Row_Status = true;
            db.ProductList.Add(Products);
            db.SaveChanges();
            Int64 id = Products.id;
            return RedirectToAction("ProductListIndex");
        }
        public ActionResult EditProducts(int id)
        {
            if (id < 0)
            {
                id = 2;
            }
            var exCategory = db.ProductList.Where(c => c.id == id).FirstOrDefault();
            return View(exCategory);
        }
        [HttpPost]
        public ActionResult EditProducts(ProductList productList)
        {
            productList.Edit_Date = DateTime.Now;
            productList.Edit_User = 1;

            db.Entry(productList).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("ProductListIndex");
        }
        public ActionResult CategoryIndex()
        {            
            return View(db.CategoryInfo.ToList());
        }
        [HttpPost]
        public ActionResult AddCategory(CategoryInfo model)
        {
            model.date = DateTime.Now;
            model.time = DateTime.Now;
            model.Row_Status = true;
            db.CategoryInfo.Add(model);
            db.SaveChanges();
            Int64 id = model.id;
            return RedirectToAction("CategoryIndex"); 
        }
        public ActionResult EditCategory(int id)
        {
            if (id < 0)
            {
                id = 2;
            }
            var exCategory = db.CategoryInfo.Where(c => c.id == id).FirstOrDefault();
            return PartialView("EditCategory", exCategory);
        }
        [HttpPost]
        public ActionResult EditCategory(CategoryInfo category)
        {
            category.date = DateTime.Now;
            category.time = DateTime.Now;

            db.Entry(category).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("CategoryIndex");
        }
        public ActionResult SubProductIndex()
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
                        ViewBag.SubProductRecord = from p in db.ProductList
                                                   join s in db.SubProductList on p.ProductId equals s.ProductId
                                                   select new ProductVM { P_List = p, S_P_List = s };
                        ViewBag.ProductList = new SelectList(db.ProductList, "ProductId", "ProductName");
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
        [HttpPost]
        public ActionResult AddSubProducts(SubProductList model)
        {
            int intIdt = db.SubProductList.Select(u => u.SubProductId).DefaultIfEmpty(0).Max();

            UserInfo userInfo = (UserInfo)Session["UserProfile"];
            model.Input_User = userInfo.Id;
            model.SubProductId = intIdt + 1;
            model.Input_Date = DateTime.Now;            
            db.SubProductList.Add(model);
            db.SaveChanges();
            return RedirectToAction("SubProductIndex");
        }
        public ActionResult EditSubProduct(int id)
        {
            if (id < 0)
            {
                id = 2;
            }
            ViewBag.ParentCategory = new SelectList(db.ProductList, "ProductId", "ProductName");
            var exSubProduct = db.SubProductList.Single(c => c.SubProductId == id);
            return PartialView("EditSubProduct", exSubProduct);
        }
        [HttpPost]
        public ActionResult EditSubProduct(SubProductList SubProducts)
        {
            if (ModelState.IsValid)
            {
                SubProducts.Edit_Date = DateTime.Now;
                SubProducts.Edit_User = 1;
                db.Entry(SubProducts).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("SubProductIndex");
            }
            return View(SubProducts);
        }
        public ActionResult SubCategoryIndex()
        {
            ViewBag.SubCategoryRecord = from c in db.CategoryInfo
                                        join s in db.SubCategoryInfo on c.id equals s.CategoryInfoid
                                        select new ItemInfoVM { C_Info = c, S_C_Info = s };
            ViewBag.ParentCategory = new SelectList(db.CategoryInfo, "id", "CategoryName");
            return View(db.SubCategoryInfo.ToList());
        }
        [HttpPost]
        public ActionResult AddSubCategory(SubCategoryInfo model)
        {
            model.date = DateTime.Now;
            model.time = DateTime.Now;
            model.Row_Status = true;
            db.SubCategoryInfo.Add(model);
            db.SaveChanges();
            return RedirectToAction("SubCategoryIndex");
        }

        public ActionResult EditSubCategory(int id)
        {
            if (id < 0)
            {
                id = 2;
            }
            ViewBag.ParentCategory = new SelectList(db.CategoryInfo, "id", "CategoryName");
            var exCategory = db.SubCategoryInfo.Where(c => c.id == id).FirstOrDefault();
            return PartialView("EditSubCategory", exCategory);
        }
        [HttpPost]
        public ActionResult EditSubCategory(SubCategoryInfo SubCategory)
        {
            SubCategory.date = DateTime.Now;
            SubCategory.time = DateTime.Now;

            db.Entry(SubCategory).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("SubCategoryIndex");
        }
        public ActionResult ModelIndex()
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
                        ViewBag.ModelRecord = from c in db.CategoryInfo
                                              join m in db.ModelInfo on c.id equals m.CategoryInfoid
                                              join s in db.SubCategoryInfo on m.SubCategoryInfoid equals s.id
                                              select new ItemInfoVM { C_Info = c, S_C_Info = s, M_Info = m };
                        ViewBag.Category = new SelectList(db.CategoryInfo, "id", "CategoryName");
                        ViewBag.SubCategory = new SelectList(db.SubCategoryInfo, "id", "SubCategoryName");
                        return View(db.ModelInfo.ToList());
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
        public ActionResult AddModel(ModelInfo model)
        {
            model.date = DateTime.Now;
            model.time = DateTime.Now;
            model.Row_Status = true;
            db.ModelInfo.Add(model);
            db.SaveChanges();
            return RedirectToAction("ModelIndex");
        }

        public ActionResult EditModel(int id)
        {
            if (id < 0)
            {
                id = 2;
            }
            ViewBag.Category = new SelectList(db.CategoryInfo, "id", "CategoryName");
            ViewBag.SubCategory = new SelectList(db.SubCategoryInfo, "id", "SubCategoryName");

            var exModel = db.ModelInfo.Where(c => c.id == id).FirstOrDefault();
            return PartialView("EditModel", exModel);
        }
        [HttpPost]
        public ActionResult EditModel(ModelInfo model)
        {
            model.date = DateTime.Now;
            model.time = DateTime.Now;

            db.Entry(model).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("ModelIndex");
        }

        public ActionResult ItemIndex()
        {
            //ViewBag.ItemRecord = from c in db.CategoryInfo
            //                      join m in db.ModelInfo on c.id equals m.CategoryInfoid
            //                      join s in db.SubCategoryInfo on m.SubCategoryInfoid equals s.id
            //                      join i in db.ItemInfo on m.SubCategoryInfoid equals i.SubCategoryInfoid
            //                      select new ItemInfoVM { C_Info = c, S_C_Info = s, M_Info = m, I_Info = i };
            ViewBag.ItemRecord=from i in db.ItemInfo
                                  join m in db.ModelInfo on i.ModelInfoid equals m.id
                                  join s in db.SubCategoryInfo on i.SubCategoryInfoid equals s.id
                                  join c in db.CategoryInfo on i.CategoryInfoid equals c.id
                               select new ItemInfoNV2
                               {
                                   CategoryName = c.CategoryName,
                                   SubCategoryName = s.SubCategoryName,
                                   ModelName = m.ModelName,
                                            Id = i.Id,
                                            ItemName = i.ItemName,
                                            Opening_Qty = i.Opening_Qty,
                                            Unit = i.Unit,
                                            Barcode=i.Barcode
                               };
                               

            ViewBag.Category = new SelectList(db.CategoryInfo, "id", "CategoryName");
            ViewBag.SubCategory = new SelectList(db.SubCategoryInfo, "id", "SubCategoryName");
            ViewBag.Model = new SelectList(db.ModelInfo, "id", "ModelName");
            return View(db.ItemInfo.ToList());
        }
        [HttpPost]
        public ActionResult AddItem(ItemInfo model)
        {
            model.date = DateTime.Now;
            model.time = DateTime.Now;
            model.Row_Status = true;
            db.ItemInfo.Add(model);
            db.SaveChanges();
            return RedirectToAction("ItemIndex");
        }

        public ActionResult EditItem(int id)
        {
            //int id = Int32.Parse(Decrypt(encripted));
            if (id < 0)
            {
                id = 2;
            }
            ViewBag.Category = new SelectList(db.CategoryInfo, "id", "CategoryName");
            ViewBag.SubCategory = new SelectList(db.SubCategoryInfo, "id", "SubCategoryName");
            ViewBag.Model = new SelectList(db.ModelInfo, "id", "ModelName");

            var exItem = db.ItemInfo.Where(c => c.Id == id).FirstOrDefault();
            return PartialView("EditItem", exItem);
        }
        [HttpPost]
        public ActionResult EditItem(ItemInfo model)
        {
            model.date = DateTime.Now;
            model.time = DateTime.Now;

            db.Entry(model).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("ItemIndex");
        }

        public string Decrypt(string cipherText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }

	}
}