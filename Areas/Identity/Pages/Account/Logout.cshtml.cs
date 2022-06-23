using System.Resources;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using cs58.models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace cs58.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ILogger<LogoutModel> _logger;

        public LogoutModel(SignInManager<AppUser> signInManager, ILogger<LogoutModel> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        public void OnGet()
        {
            // _signInManager.IsSignedIn(User); //=> de kiểm tra xem user có đăng nhập ko
            //User.Identity.IsAuthenticated -> cách khác để kt xem có user đăng nhập ko
            
        }

        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            //returnUrl la tham so truyen vao(o tren url vd int id => /login/3 ...) , mac dinh la ko co gi
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");

                //neu url co tham so thi chuyen den trang co tham so 
                //neu ko thi quay lai trang hien tai
            if (returnUrl != null)
            {
                //neu url co tham so thi chuyen den trang co tham so 
                //neu ko thi quay lai trang hien tai
                return LocalRedirect(returnUrl);
            }
            else
            {
                returnUrl = Url.Content("~/");
                // return RedirectToPage();//trang hien tai
                return LocalRedirect(returnUrl);
            }
        }
    }
}
