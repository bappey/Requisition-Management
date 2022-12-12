using scs_Project.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;


namespace SCS_Inventory.Models
{
    public class RequestionFileModel
    {
        public int id { get; set; }
        public string FileName { get; set; }        
        public int RequestionNumber { get; set; }
        public string Extention { get; set; }
        public int FileSize { get; set; }
        public DateTime DateTime { get; set; }
        public string UploadBy { get; set; }
        //public HttpPostedFileWrapper AttachFile { get; set; }
    }
    public class Requestion
    {
    }
    public class RequisitionMaster
    {        
        public int id { get; set; }
        public int ReqId { get; set; }
        public DateTime? Requisition_Date { get; set; }
        public int BranchID { get; set; }
        public string Requested_by { get; set; }
        public int Req_Status { get; set; }
        public DateTime? CMDC_Authorization_Date_Time { get; set; }
        public DateTime? Procurement_Authorization_Date_Time { get; set; }
        public int Requsition_Type { get; set; }
        public int DepartmentId { get; set; }
        public int CMDC_Authorized_User { get; set; }
        public int Procurement_Authorized_User { get; set; }
        public int Input_User { get; set; }
        public DateTime? Input_Date { get; set; }
        public int Edit_User { get; set; }
        public DateTime? Edit_Date { get; set; }
        public int Row_Status { get; set; }
    }
    //public class RequisitionMaster
    //{
    //    public int id { get; set; }
    //    public int ReqId { get; set; }
    //    public DateTime Requisition_Date { get; set; }
    //    public int Branch_Id { get; set; }
    //    public int Requested_by { get; set; }
    //    //1=Quotation_Generated, 2=Decline
    //    public int Requisition_Status { get; set; }
    //    public DateTime POthorize_Date_TIme { get; set; }        
    //    public DateTime COthorize_Date_Time { get; set; }
    //    public string Requsition_Type { get; set; }
    //    public int Row_Status { get; set; }
    //    public int Department_Id { get; set; }
    //    public int CMDC_Authorized_User { get; set; }
    //    public int Procurement_Authorized_User { get; set; }
    //    public int Input_User { get; set; }
    //    public DateTime? Input_Date { get; set; }
    //    public int Edit_User { get; set; }
    //    public DateTime? Edit_Date { get; set; }
    //}
    public class RequisitionDetails
    {
        public int id { get; set; }
        public int Req_Id { get; set; }
        public int SubProductId { get; set; }
        public string Unit { get; set; }
        public string Description { get; set; }
        public int ApprovedQty { get; set; }
        public string UserCommend { get; set; }
        public int Qty { get; set; }
        public string Input_User { get; set; }
        public DateTime? Input_Date { get; set; }
        public int Row_Status { get; set; }
        public string EntryType { get; set; }
        public int AmendmendId { get; set; }
        public int Req_Status { get; set; }
    }
    public class OtherRequistionDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int App_Req_ID { get; set; }
        public string RequestName { get; set; }
        public string Description { get; set; }
        public string Description1 { get; set; }
        public string CaptionStatus { get; set; }
        public int BranchID { get; set; }
        public string Requested_by { get; set; }
        public int Requsition_Type { get; set; }
        public int DepartmentId { get; set; }
        public string ItContactPerson { get; set; }
        public int AmendmentID { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string CompleteBy { get; set; }
        public DateTime? CompleteDate { get; set; }
        public DateTime? RequestedDate { get; set; }
        public string BranchApprovedBy { get; set; }
        public DateTime? BranchApprovedDate { get; set; }

    }
    public class OtherRequistionDetailsNew
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public bool IsCancel { get; set; }
        public int App_Req_ID { get; set; }
        public string RequestName { get; set; }
        public string Description { get; set; }
        public string Description1 { get; set; }
        public int Status { get; set; }       
        public string Requested_by { get; set; }
        public int BranchID { get; set; }
        public int DepartmentId { get; set; }
        public string ItContactPerson { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string CompleteBy { get; set; }
        public DateTime? CompleteDate { get; set; }
        public DateTime? RequestedDate { get; set; }
        public string Remarks { get; set; }
        public int Requsition_Type { get; set; }
        public int AmendmentId { get; set; }
        public int BranchReqSeqAssignsId { get; set; }

    }
    public class OtherRequistionDetailsNewVM
    {         
        public int App_Req_ID { get; set; }
        public int BranchID { get; set; }
        public string BranchName { get; set; }
        public string Requested_by { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public int Status { get; set; }
        public int Serial { get; set; }
        public string CurrentStatus { get; set; }        
        public string NextStatus { get; set; }
        public string CompleteDate { get; set; }
        public string CompleteBy { get; set; }

    }
    //public class RequestionDetails
    //{
    //    [Key]
    //    public int DetailsId { get; set; }
    //    public int SubProductId { get; set; }
    //    public string Unit { get; set; }
    //    public string Description { get; set; }
    //    public int Qty { get; set; }
    //    public int Approved_Qty { get; set; }
    //    public string Users_Commend { get; set; }
    //    public int ReqId { get; set; }
    //    public int Row_Status { get; set; }
    //    public int Input_User { get; set; }
    //    public DateTime? Input_Date { get; set; }

    //}
    public class Common
    {
        public List<RequisitionMaster> R_Master { get; set; }
        public List<RequisitionDetails> R_Details { get; set; }
        public List<PriceQuotation>PQ_Details { get; set; }
        public List<ReceiptMaster> ReceiptMaster { get; set; }
        public List<ReceiptDetails> ReceiptDetails { get; set; }
        public List<OtherRequistionDetails> O_R_Details { get; set; }
        public List<RequestionFileModel> RFM { get; set; }
    }
    public class RequesitionVM
    {
        public ItemInfo I_info { get; set; }
        public RequisitionMaster R_master { get; set; }
        public BranchInfo B_info { get; set; }
        public UserInfo U_Info { get; set; }
        public RequisitionDetails R_Details { get; set; }
        public VendorInfo V_Info { get; set; }
        public PriceQuotation PQ_Info { get; set; }
        public SubProductList SP_List { get; set; }
        public ReqTypeName Req_Type { get; set; }
    }
    public class PriceQuotation
    {        
        public int Id { get; set; }
        public int ItemID { get; set; }
        public string Barcode { get; set; }
        public string Unit { get; set; }
        public string Description { get; set; }
        public int Qty { get; set; }
        public float price { get; set; }
        public int vendorID { get; set; }
        public int MasterID { get; set; }
        public DateTime? SubmissionDate { get; set; }
        public int CS_Status { get; set; }
        public int CMDC_Status { get; set; }
        public int Proqurement_Status { get; set; }
        public string Warienty { get; set; }
        public string MadeBy { get; set; }
        public string CS_Command { get; set; }
        public string CMDC_Command { get; set; }    
        public int E_Status { get; set; }
    }
    public class PriceListVM
    {
        public int MasterID { get; set; }
        public float price { get; set; }
        public int vendorID { get; set; }
        public string Vendor_Name { get; set; }
        public DateTime Requisition_Date { get; set; }
        public string Requsitor_Name { get; set; }
        public string BranchName { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public string Unit { get; set; }
        public int Qty { get; set; }
        public string Description { get; set; }
        public string Warienty { get; set; }
        public string MadeBy { get; set; }
        public int CS_Status { get; set; }
        public int CMDC_Status { get; set; }
        public int Proqurement_Status { get; set; }
        public string CS_Command { get; set; }
        public string CMDC_Command { get; set; }
    }
    public class RequesitionList
    {
        public int ReqId { get; set; }
        public string BranchName { get; set; }
        public DateTime? Requisition_Date { get; set; }
        public DateTime? Edit_Date { get; set; }
        public string Requsition_Type { get; set; }
        public string CMDC_Othorize_Status { get; set; }
        public string Procurement_department_Status { get; set; }
        public string Name { get; set; }
        public string Req_Status { get; set; }
        public string Description { get; set; }
    }
    public class ReqTypeName
    {
        public int id { get; set; }
        public string TypeName { get; set; } 
        public string Category { get; set; }
        public string Destination { get; set; }
        public string Designation { get; set; }
        public string Address { get; set; }
    }
    public class ReqCheckList
    {
        public int id { get; set; }
        public int CheckId { get; set; }
        public string CheckName { get; set; }
        public int Input_User { get; set; }
        public DateTime? Input_Date { get; set; }
        public int Edit_User { get; set; }
        public DateTime? Edit_Date { get; set; }
        public Boolean Row_Status { get; set; }
        public string ShowName { get; set; }
        public int Types { get; set; }
    }
    public class ReqCheckPermission
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public int CheckListId { get; set; }
        public string AuthorizedUserName { get; set; }
        public DateTime AuthorizeDate { get; set; }
        public bool Status { get; set; }
    }
    public class ReceiptMaster
    {
        public int id { get; set; }
        public int RequeisitionID { get; set; }
        public DateTime? ReceiveDate { get; set; }
        public int VendorID { get; set; }        
        public int Input_User { get; set; }
        public DateTime? Input_Date { get; set; }
        public int Edit_User { get; set; }
        public DateTime? Edit_Date { get; set; }
        public int Row_Status { get; set; }
        public int Qty { get; set; }
        public int ProductID { get; set; }
        public float TotalPrice { get; set; }
        public DateTime PurchageDate { get; set; }

    }
    public class ReceiptDetails
    {
        public int Id { get; set; }
        public int SubProductID { get; set; }
        public int ProductID { get; set; }
        public int ProductSerial { get; set; }
        public string Unit { get; set; }
        public string Remarks { get; set; }        
        public float price { get; set; }
        public int vendorID { get; set; }
        public int RequeisitionID { get; set; }        
        public string Warranty { get; set; }
        public string MadeBy { get; set; }
        public int ReqId { get; set; }
        public DateTime ? ExpireDate { get; set; }
        public DateTime?  PurchageDate { get; set; }
        public bool ReceiveStatus { get; set; }
        public DateTime InputDate { get; set; }
        public int InputUser { get; set; }
        public int EditUser { get; set; }
        public DateTime? ReceiveDate { get; set; }

    }
    public class ReceiptVM
    {
        public string Vendor_Name { get; set; }
        public string Name { get; set; }
        public DateTime ReceiveDate { get; set; }
        public int RequeisitionID { get; set; }
        public int Qty { get; set; }
        public double TotalPrice { get; set; }
        public DateTime PurchageDate { get; set; }
        public string Status { get; set; }
    }
    public class ReceiptDetailsVM
    {
        public int Id { get; set; }
        public int SubProductID { get; set; }
        public string ProductModel { get; set; }
        public int ProductID { get; set; }
        public int ProductSerial { get; set; }
        public string Unit { get; set; }
        public string Remarks { get; set; }
        public float price { get; set; }
        public int vendorID { get; set; }
        public int RequeisitionID { get; set; }
        public string Warranty { get; set; }
        public string MadeBy { get; set; }
        public int ReqId  { get; set; }
        public DateTime ExpireDate { get; set; }
        public DateTime PurchageDate { get; set; }
        public bool ReceiveStatus { get; set; }
        public DateTime InputDate { get; set; }
        public int InputUser { get; set; }
        public int EditUser { get; set; }
        public DateTime ReceiveDate { get; set; }
        public string Vendor_Name { get; set; }
        public int Qty { get; set; }
        public string Description { get; set; }
        public decimal Total_Price { get; set; }
        public string SubProductName { get; set; }
    }
    public class OtherRequistionDetailsVM
    {        
        public int id { get; set; }
        public string RequestName { get; set; }
        public string Description { get; set; }
        public string BranchName { get; set; }
        public string UserName { get; set; }
        public string DepartmentName { get; set; }
        public string TypeName { get; set; }
        public string Destination { get; set; }
        public string Designation { get; set; }
        public string Address { get; set; }
        public string ItContactPerson { get; set; }
        public int App_Req_ID { get; set; }
        public string CaptionStatus { get; set; }

        public string Description1 { get; set; }       
        public int AmendmentID { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string CompleteBy { get; set; }
        public DateTime? CompleteDate { get; set; }
        public string RequestedDate { get; set; }
        public string Requested_by { get; set; }
        public string BranchApprovedBy { get; set; }
        public DateTime? BranchApprovedDate { get; set; }
    }
    public class OtherRequistionDetailsNewApprovalVM
    { 
        public int App_Req_ID { get; set; }
        public int BranchID { get; set; }
        public string BranchName { get; set; }
        public string Requested_by { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public int Status { get; set; }
        public int Serial { get; set; }
        public string Current_Status { get; set; }
        public string Next_Status { get; set; }
        public string Description { get; set; }
        public string Description1 { get; set; }
        public string Remarks { get; set; }        
        public string RequestName { get; set; }

    }
    public class ReqCategoryName
    {
        public int Id { get; set; }
        public string TypeName { get; set; }
    }

}