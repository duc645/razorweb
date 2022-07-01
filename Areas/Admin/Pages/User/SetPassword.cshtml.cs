using System.Reflection.Metadata;
using System.Text;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using cs58.models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using cs58.models;

namespace App.Admin.User
{
    public class SetPasswordModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public SetPasswordModel(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        [TempData]
        public string StatusMessage { get; set; }
        
        public class InputModel
        {
            [Required(ErrorMessage="Phải nhập {0}")]
            [StringLength(100, ErrorMessage = " {0} phải dài từ {2} đến {1} ký tự", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Mật khẩu mới")]
            public string NewPassword { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Xác nhận mật khẩu")]
            [Compare("NewPassword", ErrorMessage = "Lặp lại mật khẩu không chính xác")]
            public string ConfirmPassword { get; set; }
        }
        
        public AppUser user{get;set;}

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound($"Không tìm thấy User");
            }
            user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound($"Không thấy user với Id = {id}");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {

             if (string.IsNullOrEmpty(id))
            {
                return NotFound($"Không tìm thấy User");
            }
            user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound($"Không thấy user với Id = {id}");
            }


            if (!ModelState.IsValid)
            {
                return Page();
            }

            //ở đây lưu ý ,phương thức AddPasswordAsync đc thực thi khi user chưa có mật khẩu
            //cho nên ta dùng RemovePasswordAsync(user) để loại bỏ mật khẩu trước
            await _userManager.RemovePasswordAsync(user);


            //đặt mật khẩu
            var addPasswordResult = await _userManager.AddPasswordAsync(user, Input.NewPassword);
            if (!addPasswordResult.Succeeded)
            {
                foreach (var error in addPasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }

            StatusMessage = $"Mật khẩu đã được cập nhật cho User : {user.UserName}";

            return RedirectToPage("./Index");
        }
    }
}