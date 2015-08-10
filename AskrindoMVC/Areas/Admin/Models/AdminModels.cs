using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace AskrindoMVC.Areas.Admin.Models
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
}