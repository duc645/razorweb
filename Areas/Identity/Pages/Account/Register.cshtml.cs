using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using cs58.models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace cs58.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterModel(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage="Mời bạn nhập {0}")]
            [EmailAddress(ErrorMessage="Sai định dạng email")]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required(ErrorMessage="Mời bạn nhập {0}")]
            [StringLength(100, ErrorMessage = "{0} phải từ {2} đến {1} ký tự", MinimumLength = 2)]
            [DataType(DataType.Password)]
            [Display(Name = "Mật khẩu")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Nhập lại mật khẩu")]
            [Compare("Password", ErrorMessage = "Mật khẩu lặp lại không chính xác")]
            public string ConfirmPassword { get; set; }

            [DataType(DataType.Text)]
            [Display(Name = "Tên tài khoản")]
            [Required(ErrorMessage="Mời bạn nhập {0}")]
            [StringLength(100,ErrorMessage="{0} phải dài từ {2} đến {1} ký tự",MinimumLength=3)]
            public string UserName {set;get;}
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = new AppUser { UserName = Input.UserName, Email = Input.Email };
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("Đã tạo User mới");


                    //phát sinh token để xác nhận email 
                    //sau này người dùng mở email và gủi lại mã token này lên ứng dụng
                    //thì ứng dụng sẽ biết cái email của người dùng đky đó là có thật
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    //mã token đc encode để có thể đính kèm nó trên địa chỉ URL
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                    //phát sinh địa chỉ URL gọi tới trang confirmemail 
                    //để phát sinh url gọi tới action của controller, hay các trang razor , sử dụng thuộc tính Url và phương thức Page
                    //đây là url phát sinh : 
                    // khi ConfirmEmail k thiết lập đường dẫn=>https://localhost:5001/Identity/Account/ConfirmEmail?userId=abc&code=xyz&returnUrl=
                    // còn ConfirmEmail thiết lập đường dẫn =>https://localhost:5001/confirm-email?userId=abc&code=xyz&returnUrl=
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);
                    
                    //url được tạo trên sẽ được gửi đến email của ng dùng
                    //gửi mail dùng dịch vụ _emailSender(đã đk vào hệ thống và đã inject vào model này)
                    //để gửi mail dùng pt SendEmailAsync

                    await _emailSender.SendEmailAsync(Input.Email, "Xac nhan dia chi email",
                        $"Bạn đã đăng ký tài khoản trên razorweb , hãy  <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>bấm vào đây để kích hoạt tài khoản</a>.");


                    //Sau khi gui mail no kiem tra cau hinh RequireConfirmedAccount trong file startup trong phan configure
                    //neu registerConfirmAccount = false thi se dang nhap ngay cho user do
                    //chi chuyen sang trang RegisterConfirmation khi tạo tài khoản mới hoặc
                    //thiết lập registerConfirmAccount = true trong file startup
                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {   
                        //inPersistent :thiet lap cookie de lan sau tu dang nhap , ko can dang nhap lai
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }

                //thong bao loi neu ko tao dc tai khoan , thong bao hien thi tren form
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
