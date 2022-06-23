using System.Reflection.Metadata;
using System.Runtime.Intrinsics.X86;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using cs58.models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace cs58.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(SignInManager<AppUser> signInManager, 
            ILogger<LoginModel> logger,
            UserManager<AppUser> userManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage="Phải nhập UserName hoặc Email!")]
            [Display(Name="Tên tài khoản hoặc địa chỉ email")]
            // [EmailAddress]
            public string UserNameOrEmail { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Display(Name="Mật khẩu")]
            public string Password { get; set; }

            [Display(Name = "Nhớ thông tin đăng nhập?")]
            public bool RememberMe { get; set; }
            //nếu RememberMe= true thì nó sẽ lưu thông tin đăng nhập 
            //vào cookie của trình duyệt
            //lần sau người dùng có thể truy cập đc ngay
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        
            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                //_signInManager.SignInAsync => tham so la ca user(AppUser)
                var result = await _signInManager.PasswordSignInAsync(Input.UserNameOrEmail, Input.Password, Input.RememberMe, lockoutOnFailure: true);
                //tim userName theo email
                if(!result.Succeeded)
                {
                    //nguoi dung nhap dia chi email chu ko phai UserName
                   var user = await _userManager.FindByEmailAsync(Input.UserNameOrEmail);
                   if(user !=null)
                   {
                    //tham so lockoutOnFailure: true la sau mot so lan dang nhap that bai 
                    //no se khoa khong cho dang nhap nua
                     result = await _signInManager.PasswordSignInAsync(user.UserName, Input.Password, Input.RememberMe, lockoutOnFailure: true);
                   }
                }
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    return LocalRedirect(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("Tài khoản bị khóa!");
                    return RedirectToPage("./Lockout");// chuyen huong den trang lockout
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Đăng nhập thất bại, Tài khoản không tồn tại "
                    + "hoặc tài khoản và mật khẩu không chính xác");
                    return Page();//van o trang dang nhap va thong bao loi tren
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
