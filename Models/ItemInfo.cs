using scs_Project.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace scs_Project.Models
{
    public class ItemInfo
    {
        public int Id { get; set; }
        public string ItemName { get; set; }
        public int Opening_Qty { get; set; }
        public string Unit { get; set; }
        public string Barcode { get; set; }
        public Boolean Row_Status { get; set; }
        public DateTime date { get; set; }
        public DateTime time { get; set; }
        public string User_id { get; set; }
        public string Edit_id { get; set; }
        public int CategoryInfoid { get; set; }
        public int SubCategoryInfoid { get; set; }
        public int ModelInfoid { get; set; }
    }
    public class CategoryInfo
    {
        public int id { get; set; }
        public string CategoryName { get; set; }
        public Boolean Row_Status { get; set; }
        public DateTime date { get; set; }
        public DateTime time { get; set; }
        public string User_id { get; set; }
        public string Edit_id { get; set; }
    }
    public class ProductList
    {
        public int id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public Boolean Row_Status { get; set; }              
        public int Input_User { get; set; }
        public DateTime? Input_Date { get; set; }
        public int Edit_User { get; set; }
        public DateTime? Edit_Date { get; set; }
    }
    public class SubProductList
    {
        public int id { get; set; }
        public int SubProductId { get; set; }
        public string SubProductName { get; set; }
        public int ProductId { get; set; }
        public string ProductModel { get; set; }
        public bool Generate_SL { get; set; }
        public Boolean Row_Status { get; set; }
        public int Input_User { get; set; }
        public DateTime? Input_Date { get; set; }
        public int Edit_User { get; set; }
        public DateTime? Edit_Date { get; set; }   
        public string Unit { get; set; }
    }
    public class ProductVM
    {
        public ProductList P_List { get; set; }
        public SubProductList S_P_List { get; set; }
    }
    public class SubCategoryInfo
    {
        public int id { get; set; }
        //public int CategoryId { get; set; }
        public string SubCategoryName { get; set; }
        public Boolean Row_Status { get; set; }
        public DateTime date { get; set; }
        public DateTime time { get; set; }
        public string User_id { get; set; }
        public string Edit_id { get; set; }
        public int CategoryInfoid { get; set; }
    }
    public class ModelInfo
    {
        public int id { get; set; }
        public int CategoryInfoid { get; set; }
        public int SubCategoryInfoid { get; set; }
        public string ModelName { get; set; }
        public Boolean Row_Status { get; set; }
        public DateTime date { get; set; }
        public DateTime time { get; set; }
        public string User_id { get; set; }
        public string Edit_id { get; set; }
    }
    public class ItemInfoVM
    {
        public CategoryInfo C_Info { get; set; }
        public SubCategoryInfo S_C_Info { get; set; }
        public ModelInfo M_Info { get; set; }
        public ItemInfo I_Info { get; set; }
    }
    public class ItemInfoNV2
    {
        public string CategoryName { get; set; }
        public string SubCategoryName { get; set; }
        public string ModelName { get; set; }
        public int Id { get; set; }
        public string ItemName { get; set; }
        public int Opening_Qty { get; set; }
        public string Unit { get; set; }
        public string Barcode { get; set; }
    }
    
}