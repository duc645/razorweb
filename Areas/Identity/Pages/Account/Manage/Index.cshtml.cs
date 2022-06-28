using System.ComponentModel;
using System.Data;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Buffers.Text;
using System.Reflection.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using cs58.models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;

namespace cs58.Areas.Identity.Pages.Account.Manage
{
    [Authorize]
    //using Microsoft.AspNetCore.Authorization;
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public IndexModel(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Phone(ErrorMessage = "Sai định dạng số điện thoại")]
            [Display(Name = "Số điện thoại")]
            public string PhoneNumber { get; set; }

            [Display(Name = "Địa chỉ nhà")]
            [StringLength(400)]
            public string HomeAdress { get; set; }

            //nếu muốn ngày sinh nằm từ năm này đến năm kia
            //thì ta phải xây dựng validate riêng
            [Display(Name = "Ngày sinh")]
            public DateTime? BirthDate { get; set; }



        }

        private async Task LoadAsync(AppUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                HomeAdress = user.HomeAdress,
                BirthDate = user.BirthDate

            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            //đầu tiên khi bấm vào tên user cạnh đăng xuất.
            //nó sẽ vào OnGetAsync
            //Đầu kiên lấy thông tin của User đang đăng nhập
            //nếu user(thông tin của tài khoản đăng nhập) = null thì 
            //trả ra lỗi .
            //Ví dụ như : mình đăng nhập, rồi vào profile , copy url 
            //rồi mình đăng xuất, rồi dán url vừa copy và chạy
            //thì lúc này nó sẽ không lấy đc thông tin của người đăng nhập
            //vì ko có ai đăng nhập
            //À Ở ĐÂY TA THIẾT LẬP THUỘC TÍNH [Authorrize]
            //phải đăng nhập mới cho vào trang này, nếu ko đăng nhập mà vào trang này thì
            //nó chuyển hướng về trang login

            //User vẫn chưa hiểu lấy ở đâu, hay là thông tin của người đang đăng nhập,
            //hay laf hệ thống nó đặt mặc định là User ???
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }


            //còn nếu có thông tin user đăng nhập
            //tiep tuc goi phuong thuc LoadAsync
            await LoadAsync(user);
            return Page();//goij trang hien tai
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            user.HomeAdress = Input.HomeAdress;
            user.BirthDate = Input.BirthDate;
            user.PhoneNumber = Input.PhoneNumber;

            //cap nhat len co so du lieu
            await _userManager.UpdateAsync(user);

            //code mawjc ddinh
            // var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            // if (Input.PhoneNumber != phoneNumber)
            // {
            //     var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
            //     if (!setPhoneResult.Succeeded)
            //     {
            //         StatusMessage = "Unexpected error when trying to set phone number.";
            //         return RedirectToPage();
            //     }
            // }
            
            //sau khi update len database
            //thuc hien dang nhap lai de no update thong tin moi
            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Hồ sơ đã được cập nhật";
            return RedirectToPage();
        }
    }
}
