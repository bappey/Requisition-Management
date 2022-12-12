using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SCS_Inventory.Models
{
    public class Setings
    {
    }
    public class UserInfo
    {
        public int Id { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Name { get; set; }
        public int Branch_Id { get; set; }
        public int Department_Id { get; set; }
        [Required]
        public string Password { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public DateTime Activation_Date { get; set; }
        public string Old_Password { get; set; }
        public string Phone { get; set; }
        public string BranchCode { get; set; }
        public int? RoleId { get; set; }
        public int? Authorizedby { get; set; }
        public DateTime? AuthorizedDate { get; set; }
        public int Designation { get; set; }
    }
    public class SettingVM
    {
        public Models.ReqCheckPermission Req_Check_P { get; set; }
        public UserInfo U_info { get; set; }
        public Models.ReqCheckList Req_Check_L {get; set;}
    }
    public class ReqCheckPermissionVM
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public int CheckId { get; set; }
        public string CheckName { get; set; }
        public string AuthorizedUserName { get; set; }
        public DateTime  AuthorizeDate { get; set; }
        public bool Status { get; set; }
    }  
    public class ReqStatusSequence
    {
        public int Id { get; set; }
        public int typeId { get; set; }
        public int CheckId { get; set; }       
        public string AuthorizedUserName { get; set; }
        public DateTime AuthorizeDate { get; set; }
        public int ReqStatus { get; set; }
        public int Placement { get; set; }
        public DateTime GrantDate { get; set; }
    }
    public class ReqStatusSequenceVM
    {
        public string TypeName { get; set; }
        public string CheckName { get; set; }
        public string AuthorizedUserName { get; set; }
        public string AuthorizeDate { get; set; }
    }
    public class LoginViewModel
    {
        public int id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int BrunchId { get; set; }
        public int DepartmentId { get; set; }
        public bool RememberMe { get; set; }
        public bool ShouldLockOut { get; set; }
    }   
    public class UserAuthontication
    {
        public int Id { get; set; }
        public int MenuId { get; set; }
        public int UserId { get; set; }
        public int Authorizedby { get; set; }
        public DateTime AuthorizedDate { get; set; }
    }
    public class UserAuthonticationVM
    {
        public int Id { get; set; }        
        public string MenuName { get; set; }
        public string SubMenuName { get; set; }
        public string Path { get; set; }
        public string Authorizedby { get; set; }
        public string UserName { get; set; }
    }                                                                                                             
    public partial class RoleName
    {
        public int Id { get; set; }
        public decimal SL { get; set; }
        public string RoleCaption { get; set; }
        public Nullable<bool> Status { get; set; }
    }
    public partial class RoleSetting
    {
        public int Id { get; set; }
        public string MenuName { get; set; }
        public string Path { get; set; }
        public string SubMenuName { get; set; }
        public string RoleCaption { get; set; }
        public DateTime? AssignDate { get; set; }
        public string PermittedBy { get; set; }
    }
    public class ReqApprovSeqPermission
    {
        public int Id { get; set; }
        public int UserID { get; set; }
        public int TypeId { get; set; }
        public int? AuthorizedID { get; set; }
        public DateTime AuthorizedDate { get; set; }
        public int CheckId { get; set; }
        public int Status { get; set; }
    }
    public class ReqApprovSeqPermissionVM
    {        
        public int UserID { get; set; }
        public int TypeId { get; set; }
        public int AuthorizedID { get; set; }
        public DateTime AuthorizedDate { get; set; }
        public string CheckName{get; set;}
        public int CheckId { get; set; }
    }
    public class UserAuthorizationVM
    {
        public int Id { get; set; }
        public String HRId { get; set; }
        public String Name { get; set; }
        public string Department { get; set; }
        public string Branch { get; set; }
        public String Role { get; set; }        
        public String Create_Date { get; set; }
    }
    public class BranchReqSeqAssign
    {
        public int Id { get; set; }  
        public int Serial { get; set; }
        //public int BranchID { get; set; }        
        public int CheckID { get; set; }
        public DateTime ActivationDate { get; set; }
        public int ReqSeqID { get; set; }        
        public string UserName { get; set; }
        public DateTime AssigneDate { get; set; }
        public int CategoryID { get; set; }
    }
    public class BranchReqSeqAssignVM
    {
        public int CheckID { get; set; }
        public string CheckName { get; set; }
        public string CategoryName { get; set; }
        public string ActivationDate { get; set; }
        public string BranchName { get; set; }
        public string PrevelizeBy { get; set; }
        public int ReqSeqID { get; set; }
        public int Serial { get; set; }
        public string UserName { get; set; }
        public string DepartmentName { get; set; }
    }
    public class Designation
    {
        public int Id { get; set; }        
        public string DesignationName { get; set; }
       
    }
    public class UserWisePermission
    {
        public int Id { get; set; }       
        public int BranchId { get; set; }
        public int CheckId { get; set; }
        public string UserName { get; set; }
        public string PrevelizeUserId { get; set; }
        public string DesignationName { get; set; }
        public DateTime PrevelizeDate { get; set; }
    }
    public class BranchUserApproval
    {
        public int Id { get; set; }
        public int DesignationId { get; set; }
        public int BranchId { get; set; }
        public string UserId { get; set; }
        public int CheckId { get; set; }        
        public string PrevelizeUserId { get; set; }
        public string Category { get; set; }
        public DateTime PrevelizeDate { get; set; }
        public int DepartmentId { get; set; }
    }
}