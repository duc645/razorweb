using System.Runtime.Intrinsics.X86;
using System.Collections.Specialized;
using System.Threading.Tasks.Dataflow;
using System.Dynamic;
using System.Xml.Schema;
using System.ComponentModel.DataAnnotations.Schema;
using System.Resources;
using System.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using cs58.models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System.Security.Claims;


namespace cs58.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ExternalLoginModel : PageModel
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<ExternalLoginModel> _logger;

        public ExternalLoginModel(
            SignInManager<AppUser> signInManager,
            UserManager<AppUser> userManager,
            ILogger<ExternalLoginModel> logger,
            IEmailSender emailSender)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ProviderDisplayName { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        public IActionResult OnGetAsync()
        {
            return RedirectToPage("./Login");
        }

        public IActionResult OnPost(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.

            //trang chuyen huong den
            var redirectUrl = Url.Page("./ExternalLogin", pageHandler: "Callback", values: new { returnUrl });
            //properties : chua ten provider, provider ,clientId, secretId ,trang chuyen huong,...
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

            //phuong thuc nay ket noi den dich vu ngoai
            //tra ve challenResult, noi dung nay dc render tren trinh duyet
            //mo ra 1 cai Pop up de nguoi dung ket noi toi dich vu ngoai
            //cho phep ung dung cua minh truy cap vao thong tin cua ho
            //khi nguoi dung dong y truy cap
            //thi dich vu ngoai se gui ma token den ung dung cua minh
            //thong qua dia chi CakkbackPath = "dang-nhap-tu-google" 
            //da dc cau hinh trong file startup.
            ////thong tin dich vu ngoai gui den la ma token
            ////ung dung se tu dong truy cap thong tin ket noi toi tai khoan cua user
            ////khi da ket noi va lay dc thong tin cua user , thi trang "dang-nhap-tu...
            ////se tu dong chuyen den trang da dc thiet lap
            ////trong bien redirectUrl cua Handler OnPost cua file nay(ben tren).
            ////Thong tin ma no ket noi lay dc se dc luu vao section cua ung dung
            ////luc nay o cac trang khac chung ta co the doc dc nhung thong tin nay

            /// =>TÓM LẠI khi người dùng nhấn đăng nhập google 
            //thì handler OnPost đc gọi ,nó sẽ pop up ra cái cửa sổ
            //cửa sổ này là trang đăng nhập của google 
            //do phương thức ChallengeResult gọi ra
            //người dùng đồng ý thì ứng dụng sẽ lây được thông tin
            //chú ý :"ứng dụng" ở đây là ứng dụng mình tạo ra trên 
            //trang google api.
            //Sau khi người dùng đồng ý nó chuyển thông tin đến trang
            //dang-nhap-tu-google (đã thiết lập trong file startup-(Callbackpah))
            //lúc này đã có đc thông tin của người dùng lưu trong section.
            //Sau đó nó chuyển hướng ngay tới trang thiết lập trong
            //biến redirectUrl (ở bên trên) ( trang đó là trang này)
            //với handler là OnGetCallbackAsync(bên dưới)
            return new ChallengeResult(provider, properties);
        }

        public async Task<IActionResult> OnGetCallbackAsync(string returnUrl = null, string remoteError = null)
        {

            //return Content("Dung lai o day ");
            returnUrl = returnUrl ?? Url.Content("~/");

            // nếu remoteError khác null
            //tức là dịch vụ ngoài có gắn kèm trên URL callbackpath cái 
            //giá trị remoteError thì tức là có lỗi gì đó
            //(ví dụ như người dùng ko đồng ý) 
            //nếu vậy thì nó sẽ chuyển hướng về trang login
            if (remoteError != null)
            {
                ErrorMessage = $"Lỗi từ dịch vụ ngoài: {remoteError}";
                return RedirectToPage("./Login", new {ReturnUrl = returnUrl });
            }


            //TRƯỜNG HỢP KHÔNG BỊ LỖI
            //dùng phương thức này để lấy thông tin người dùng từ dịch vụ ngoài
            //phương thức này trả về đối tượng externalLoginInfo
            var info = await _signInManager.GetExternalLoginInfoAsync();

            //nếu info rỗng thì báo lỗi
            if (info == null)
            {
                //ErrorMessage là một thuộc tính của trang này
                //thuộc tính này có thiết lập atribute [TempData]
                //tức là giá trị của thuộc tính này sẽ lưu trong section của 
                //ừng dụng ,khi giá trị này lưu trong section của ứng dụng
                //thì trang /Login sẽ đọc được nó để hiển thị thông tin
                ErrorMessage = "Không lấy được thông tin từ dịch vụ ngoài.";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }
            

            
            // Sign in the user with this external login provider if the user already has a login.

            //bước tiếp theo là dùng thông tin lấy đc từ user để đăng nhập
            //dùng phương thức _signInManager.ExternalLoginSignInAsync
            //tham số là info.LoginProvider, info.ProviderKey
            //trả về biến result
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor : true);
            
            //nếu đăng nhập thành công
            //khi có cái account đã liễn kết với dịch vụ ngoài 
            //đăng nhập thành công thì nó sẽ về trang returnUrl
            //Chú ý , ở đầu tham số của phương thức OnGetCallbackAsync 
            //đã thiết lập returnUrl = NullableConverter
            //nhưng bên dưới có dòng 
            //returnUrl =returnUrl ?? Url.Content("~/")
            //tức là nếu returnUrl rỗng thì returnUrl là trang chủ ,
            // còn nếu không rỗng thì về trang returnUrl nếu có thiết lập
            if (result.Succeeded)
            {
                _logger.LogInformation("{Name} logged in with {LoginProvider} provider.", info.Principal.Identity.Name, info.LoginProvider);
                return LocalRedirect(returnUrl);
            }

            //thất bại do tài khoản bị khóa
            //thì về trang lockout
            if (result.IsLockedOut)
            {
                return RedirectToPage("./Lockout");
            }
            //các trường hợp lội khác
            else
            {
                // If the user does not have an account, then ask the user to create an account.

                //TH1: trên hệ thống có tài khoản nhưng chưa liên kết dịch vụ ngoài của google
                //=>Liên kết tài khoản với dịch vụ ngoài rồi tiến hành đăng nhập

                //TH2: chưa có tài khoản'
                //=> Tạo tài khoản , liên kết dịch vụ ngoài 
                //và tiến hành đăng nhập luôn


                
                ReturnUrl = returnUrl;

                //Để thực hiên 2 việc trên 
                //thiết lập  ProviderDisplayName để truyền sang view
                ProviderDisplayName = info.ProviderDisplayName;
                if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
                {
                    Input = new InputModel
                    {
                        //thiết lập email , lấy đc từ dịch vụ ngoài
                        Email = info.Principal.FindFirstValue(ClaimTypes.Email)
                    };
                }
                //sau đó render trang này
                return Page();
            }
        }

        public async Task<IActionResult> OnPostConfirmationAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            // Get the information about the user from the external login provider

            //lấy thông tin từ dich vụ ngoài lưu vào info
            //vd : lấy đc email của ng dùng , nhưng chưa liên kết đc
            //chưa tạo tài khoản trong ứng dụng web
            //thì chuyển đến handler này
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ErrorMessage = "Lỗi lấy thông tin từ dịch vụ ngoài";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            //Nếu lấy đc thông tin từ dịch ngoài và cái địa chỉ
            //email do người dùng submit phù hợp
            if (ModelState.IsValid)
            {
                //Sửa lại code, ko dùng theo mặc định.
                //Tìm trong ứng dụng xem có tài khoản nào đc đky với
                //email này chưa
                var registeredUser = await _userManager.FindByEmailAsync(Input.Email);

                //đặt biến lưu trữ email lấy từ info(dịch vụ ngoài)
                string externalEmail = null;

                AppUser externalEmailUser = null;

                //kiểm tra xem dịch vụ ngoại có cung cấp email này ko(biến info)
                //khái niệm về Claim nói ở video sau
                //Claim là đặc tính mô tả một đối tượng nào đó
                //ví dụ ngày sinh là một đặc tính của bạn ,
                //thì coi ngày sinh là một claim của user.
                //Nếu cái này chả về true, tức là trong info có 
                //cung cấp địa chỉ email
                //thì thực hiện đọc địa chỉ email đó
               if( info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
               {
                //lấy địa chỉ email gán vào externalEmail
                    externalEmail = info.Principal.FindFirstValue(ClaimTypes.Email);
               }

                if(externalEmail != null)
                {
                    externalEmailUser = await _userManager.FindByEmailAsync(externalEmail);
                }    

                //Nếu có user với đia chỉ email do người dùng submit đến
                //đã có trên hệ thống
                if((registeredUser !=null)&&(externalEmailUser != null))
                {
                    //hoac registeredUser.Email ==  externalEmailUser.Email
                    if(registeredUser.Id == externalEmailUser.Id)
                    {
                        //TRường hợp này là email lúc xác thưc trùng với email 
                        //lúc sau nhập
                        //trong trường hợp này thực hiện liên kết tài khoản 
                        //và đăng nhập luôn

                        //liên kết
                       var resultLink = await _userManager.AddLoginAsync(registeredUser, info);
                        //Nếu liên kết thành công thì đăng nhập luôn
                        //và chuyển hướng về trang chủ
                       if(resultLink.Succeeded)
                       {
                        await _signInManager.SignInAsync(registeredUser, isPersistent: false);
                        return LocalRedirect(returnUrl);
                       }
                    }
                    else {
                        // registeredUser.Id != externalEmailUser.Id
                        //VD : có 2 tài khoản trong hệ thống
                        //user1 với emai1@abc.com
                        //user2 với emai2@abc.com
                        //ứng dụng sẽ quyết định liên kết dịch vụ ngoài
                        //với user1 hay là user2 
                        //TRƯỜNG HỢP NÀY TỐT NHẤT KO LIÊN kết
                        //ví dụ bạn a dùng email của mình xác thực đăng nhập dịch vụ ngoài
                        //nhưng lúc liên kết (form nhập email) lại dùng email 
                        //của tài khoản bạn B 
                        //TỐT NHẤT KO LIÊN KẾT VÀ XUẤT HIỆN THÔNG BÁO LỖI
                        ModelState.AddModelError(string.Empty,"Không liên kết được"
                        +"tài khoản, hãy sử dụng email khác");
                        return Page();
                    }
                }
                //trường hợp : đăng nhập gg với email đã có tài khoản sử dụng trong hệ thống
                //nhưng khi nhập email liên kết lại chọn một email ko nằm trong bất cứ
                //tài khoản nào
                //Để an toàn thì ko liên kết , ko làm gì cả , chỉ thông báo lỗi.
                //Ở đây điều kiện registeredUser == null vì
                // var registeredUser = await _userManager.FindByEmailAsync(Input.Email);
                // dùng phương thức trên để kiểm tra email ở ô nhập có tài khoản
                //nào dùng chưa
                if((externalEmailUser != null)&&(registeredUser == null))
                {
                    ModelState.AddModelError(string.Empty,"Không hỗ trợ tạo tài khoản mới"
                    +"-có email khác với email từ dịch vụ ngoài");
                        return Page();
                }

                if((externalEmailUser == null)&&(externalEmail == Input.Email))
                {
                    //trường hợp này xảy ra nghĩa là chưa có tài khoản
                    //cách làm : => tạo account, liên kết , và đăng nhập luôn
                    var newUser = new AppUser() {
                        UserName = externalEmail,
                        Email = externalEmail
                    };
                    var resultNewUser = await _userManager.CreateAsync(newUser);
                    if (resultNewUser.Succeeded)
                    {
                        //Liên kết user mới và info
                       await _userManager.AddLoginAsync(newUser, info);
                       var code = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
                       await _userManager.ConfirmEmailAsync(newUser,code);
                       await _signInManager.SignInAsync(newUser,isPersistent : false);

                       return LocalRedirect(returnUrl);
                    }
                    else {
                            ModelState.AddModelError(string.Empty,"Không tạo được tài khoản mới");
                    
                             return Page();
                    }
                }
                
                //CÁC TRƯỜNG HỢP CÒN LẠI THÌ 
                //THỰC HIỆN THEO CODE MẶC ĐỊNH
                

                //tạo ra một user
                var user = new AppUser { UserName = Input.Email, Email = Input.Email };
                //dang ky tai khoan
                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    //Nếu tạo user thành công thì nó sẽ thực hiện liên kết
                    //tài khoản mới tạo ra đó
                    //với tài khoản từ dịch vụ ngoài
                    result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        _logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);

                        var userId = await _userManager.GetUserIdAsync(user);
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                        var callbackUrl = Url.Page(
                            "/Account/ConfirmEmail",
                            pageHandler: null,
                            values: new { area = "Identity", userId = userId, code = code },
                            protocol: Request.Scheme);

                        await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                            $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                        // If account confirmation is required, we need to show the link if we don't have a real email sender
                        if (_userManager.Options.SignIn.RequireConfirmedAccount)
                        {
                            return RedirectToPage("./RegisterConfirmation", new { Email = Input.Email });
                        }

                        await _signInManager.SignInAsync(user, isPersistent: false, info.LoginProvider);

                        //quay ve trang chu , dong dau tien cua handler
                        return LocalRedirect(returnUrl);

                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            //nếu validate dự liệu lỗi hoặc ,...
            // nó sẽ mở lại trang ExternalLogin
            //trang này (form nhập mail)
            ProviderDisplayName = info.ProviderDisplayName;
            ReturnUrl = returnUrl;
            return Page();
        }
    }
}
