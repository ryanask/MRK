using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace AskrindoMVC.Areas.Admin.Models.UserMgr
{
    public class UserModel
    {
        [Required(ErrorMessage = "Harus diisi")]
        [Display(Name = "Nama User")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Harus diisi")]
        [Display(Name = "Nama Lengkap")]
        public string FullName { get; set; }

        [Display(Name = "Jabatan")]
        [StringLength(200, ErrorMessage = "Maks 200 karakter")]
        public string JobTitle { get; set; }

        [Required(ErrorMessage = "Harus diisi")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Harus diisi")]
        [Display(Name = "Risk Contact Person?")]
        public bool IsRCP { get; set; }

        [Required(ErrorMessage = "Harus diisi")]
        [StringLength(100, ErrorMessage = "Min 3 karakter", MinimumLength = 3)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Konfirmasi Password")]
        [Compare("Password", ErrorMessage = "Password dan Konfirmasi Password tidak sama")]
        public string ConfirmPassword { get; set; }
    }

    public class EditUserModel
    {
        [Required(ErrorMessage = "Harus diisi")]
        [Display(Name = "Nama User")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Harus diisi")]
        [Display(Name = "Nama Lengkap")]
        public string FullName { get; set; }

        [Display(Name = "Jabatan")]
        [StringLength(200, ErrorMessage = "Maks 200 karakter")]
        public string JobTitle { get; set; }

        [Required(ErrorMessage = "Harus diisi")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Harus diisi")]
        [Display(Name = "Risk Contact Person?")]
        public bool IsRCP { get; set; }
    }

    public class ChangeUserPasswordModel
    {
        [Display(Name = "Nama User")]
        public string UserName { get; set; }

        [Display(Name = "Nama Lengkap")]
        public string FullName { get; set; }

        [Display(Name = "Jabatan")]
        public string JobTitle { get; set; }

        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Risk Contact Person?")]
        public bool IsRCP { get; set; }

        [Required(ErrorMessage = "Harus diisi")]
        [StringLength(100, ErrorMessage = "Min 3 karakter", MinimumLength = 3)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Konfirmasi Password")]
        [Compare("Password", ErrorMessage = "Password dan Konfirmasi Password tidak sama")]
        public string ConfirmPassword { get; set; }
    }

    public class DeleteUserModel
    {
        [Display(Name = "Nama User")]
        public string UserName { get; set; }

        [Display(Name = "Nama Lengkap")]
        public string FullName { get; set; }

        [Display(Name = "Jabatan")]
        public string JobTitle { get; set; }

        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Risk Contact Person?")]
        public bool IsRCP { get; set; }
    }

    public class LoginModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class EditUserModelNew
    {
        [Required(ErrorMessage = "Harus diisi")]
        [Display(Name = "Nama User")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Harus diisi")]
        [Display(Name = "Nama Lengkap")]
        public string FullName { get; set; }

        [Display(Name = "Jabatan")]
        [StringLength(200, ErrorMessage = "Maks 200 karakter")]
        public string JobTitle { get; set; }

        [Required(ErrorMessage = "Harus diisi")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Harus diisi")]
        [Display(Name = "Risk Contact Person?")]
        public bool IsRCP { get; set; }

        [Display(Name = "Group Menu")]
        public SelectList GroupMenu { get; set; }

        [Display(Name = "Group User")]
        public SelectList GroupUser { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Konfirmasi Password")]
        [Compare("Password", ErrorMessage = "Password dan Konfirmasi Password tidak sama")]
        public string KonfirmasiPassword { get; set; }

        [Display(Name = "Pusat/Cabang")]
        public SelectList Kantor { get; set; }

        [Display(Name = "Direktorat")]
        public SelectList Direktorat { get; set; }

        [Display(Name = "Bagian Dibawah Direktur")]
        public string Direktur { get; set; }

        [Display(Name = "Divisi")]
        public string Divisi { get; set; }

        [Display(Name = "Bagian")]
        public string Bagian { get; set; }

        [Display(Name = "Cabang")]
        public string Cabang { get; set; }

        [Display(Name = "Unit")]
        public string Unit { get; set; }

        public EditUserParam param { get; set; }
    }

    public class EditUserParam
    {
        public int? posID { get; set; }
        public int? DeptID { get; set; }
        public int? DivisionID { get; set; }
        public int? SubDivID { get; set; }
        public int? BranchID { get; set; }
        public int? SubBranchID { get; set; }
        public int? BizUnitID { get; set; }
        public int? UserGroupID { get; set; }

        public string UserName { get; set; }
        public string FullName { get; set; }
        public string JobTitle { get; set; }
        public string Email { get; set; }

        public bool IsRCP { get; set; }
        public bool isPusat { get; set; }

        public SelectList GroupMenu { get; set; }
        public SelectList GroupUser { get; set; }

        public string Password { get; set; }
        public string KonfirmasiPassword { get; set; }

        public SelectList Kantor { get; set; }
        public SelectList Direktorat { get; set; }
        public SelectList Direktur { get; set; }
        public SelectList Divisi { get; set; }
        public SelectList Bagian { get; set; }
        public SelectList Cabang { get; set; }
        public SelectList SubCabang { get; set; }
        public SelectList Unit { get; set; }
        public SelectList UserGroup { get; set; }
    }

    public class UserModel2
    {
        [Required(ErrorMessage = "Harus diisi")]
        [Display(Name = "Nama User")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Harus diisi")]
        [Display(Name = "Nama Lengkap")]
        public string FullName { get; set; }

        [Display(Name = "Jabatan")]
        [StringLength(200, ErrorMessage = "Maks 200 karakter")]
        public string JobTitle { get; set; }

        [Required(ErrorMessage = "Harus diisi")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Harus diisi")]
        [Display(Name = "Risk Contact Person?")]
        public bool IsRCP { get; set; }

        [Required(ErrorMessage = "Harus diisi")]
        [StringLength(100, ErrorMessage = "Min 3 karakter", MinimumLength = 3)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Konfirmasi Password")]
        [Compare("Password", ErrorMessage = "Password dan Konfirmasi Password tidak sama")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Pusat/Cabang")]
        public SelectList Kantor { get; set; }

        [Display(Name = "Direktorat")]
        public SelectList Direktorat { get; set; }

        [Display(Name = "Bagian Dibawah Direktur")]
        public string Direktur { get; set; }

        [Display(Name = "Divisi")]
        public string Divisi { get; set; }

        [Display(Name = "Bagian")]
        public string Bagian { get; set; }

        [Display(Name = "Cabang")]
        public string Cabang { get; set; }

        [Display(Name = "Unit")]
        public string Unit { get; set; }

        public EditUserParam param { get; set; }
    }
}