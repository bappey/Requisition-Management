using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using scs_Project.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using SCS_Inventory.Models;

namespace scs_Project.Models
{
    public class DataContext: DbContext
    {
        public DataContext() : base("SCSInventoryEntities")
        {
            this.Database.SqlQuery<ReqStatusSequenceVM>("tempData");
        }
        public DbSet<Division> Division { get; set; }
        public DbSet<BranchType> BranchType { get; set; }
        public DbSet<SubProductList> SubProductList { get; set; }
        public DbSet<ProductList> ProductList { get; set; }
        public DbSet<VendorInfo> Vendor { get; set; }
        public DbSet<BranchInfo> Branch { get; set; }
        public DbSet<DepartmentInfo> Department { get; set; }
        public DbSet<DepartmentEntry> DepartmentEntry { get; set; }
        public DbSet<CategoryInfo> CategoryInfo { get; set; }
        public DbSet<SubCategoryInfo> SubCategoryInfo { get; set; }
        public DbSet<ModelInfo> ModelInfo { get; set; }
        public DbSet<ItemInfo> ItemInfo { get; set; }
        public DbSet<RequisitionMaster> RequisitionMaster { get; set; }
        public DbSet<RequisitionDetails> RequisitionDetail { get; set; }
        public DbSet<UserInfo> UserInfo { get; set; }
        public DbSet<PriceQuotation> PriceQuotations { get; set; }
        public DbSet<ReqTypeName> ReqTypeName { get; set; }
        public DbSet<ReqCheckList> ReqCheckList { get; set; }
        public DbSet<ReqCheckPermission> ReqCheckPermission { get; set; }
        public DbSet<ReqStatusSequence> ReqStatusSequence { get; set; }   
        public DbSet<ReceiptDetails> ReceiptDetails { get; set; }
        public DbSet<ReceiptMaster> ReceiptMaster { get; set; }
        public DbSet<OtherRequistionDetails> OtherRequisitionDetail { get; set; }
        public DbSet<UserAuthontication> UserAuthontication { get; set; }
        public DbSet<RoleName> RoleName { get; set; }
        public DbSet<RoleSetting> RoleSetting { get; set; }
        public DbSet<ReqApprovSeqPermission> ReqApprovSeqPermission { get; set; }
        public DbSet<BranchReqSeqAssign> BranchReqSeqAssign { get; set; }
        public DbSet<Designation> Designations { get; set; }
        public DbSet<UserWisePermission> UserWisePermissions { get; set; }
        public DbSet<BranchUserApproval> BranchUserApprovals { get; set; }
        public DbSet<OtherRequistionDetailsNew> OtherRequistionDetailsNew { get; set; }
        public DbSet<ReqCategoryName> ReqCategoryNames { get; set; }
        public DbSet<RequestionFileModel> RequestionFileModels { get; set; }
        //public System.Data.Entity.DbSet<SCS_Inventory.Models.ReceiptDetailsVM> ReceiptDetailsVMs { get; set; }
    }
}