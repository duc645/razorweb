using System.IO;
using System.Collections.Specialized;
using System.ComponentModel.Design.Serialization;
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
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace App.Admin.User
{
    public class AddRoleModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AddRoleModel(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        [TempData]
        public string StatusMessage { get; set; }
        

        
        public AppUser user{get;set;}

        //Một mảng chuỗi chứa danh sách các Role đc gán của 1 user
        [BindProperty]
        [Display(Name="Các Role gán cho user")]
        public string[] RoleNames{get;set;}

        public SelectList allRoles {set;get;}


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
            //phương thức lấy ra các role của user, phương thức này
            //trả về list các role đc gán cho user
            //string ở cuối là chỉ rõ convert sang mảng kiểu string
            //CHÚ Ý QUAN TRỌNG , CÓ DÒNG NÀY LÀ VÌ
            //LÚC ẤN SANG TRANG THÊM ROLE , NÓ SẼ HIGHT LIGHT lên nhưng Role mà user
            //này có
            RoleNames = (await _userManager.GetRolesAsync(user)).ToArray<string>();

            //Lấy ra danh sách role
            //r => r.Name: với mỗi role trong Roles ta lấy ra tên của mỗi role
            List<string> roleNames = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
            allRoles = new SelectList(roleNames);


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
            //bây giờ sẽ so sánh RoleNames (do mình chọn ở form) với OldRoleNames
            var OldRoleNames = (await _userManager.GetRolesAsync(user)).ToArray();

            //lấy ra các cái role ở trong OldRoleNames với điều kiện là :
            // cái role đó không nằm trong RoleNames
            //RoleNames.Contains(r) : các role nằm trong RoleNames
            // !RoleNames.Contains(r) : các role ko nằm trong RoleNames

            var deleteRoles= OldRoleNames.Where(r => !RoleNames.Contains(r));

            var addRoles = RoleNames.Where(r => !OldRoleNames.Contains(r));

            List<string> roleNames = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
            allRoles = new SelectList(roleNames);

            //xoa các roles của user với id ...
            var resultDelete = await _userManager.RemoveFromRolesAsync(user,deleteRoles);
            
            //Nếu có lỗi sẽ đẩy lỗi vào ModelState vả return Page();
           if(!resultDelete.Succeeded)
            {
                //resultDelete duyệt qua các errors
                resultDelete.Errors.ToList().ForEach(error => {
                    ModelState.AddModelError(string.Empty, error.Description);
                });
                return Page();
            }


            var resultAdd = await _userManager.AddToRolesAsync(user,addRoles);
            if(!resultAdd.Succeeded)
            {
                //resultAdd duyệt qua các lỗi và đẩy vào ModelState
                resultDelete.Errors.ToList().ForEach(error => {
                    ModelState.AddModelError(string.Empty, error.Description);
                });
                return Page();
            }


            // Ở ĐÂY Ko cần kiểm tra dữ liệu nhập vào có thỏa mãn ko
            //vì dữ liệu nhập vào là lấy ở trong hệ thống
            //ví dụ như ở đây là các Role ở trong hệ thống
            // if (!ModelState.IsValid)
            // {
            //     return Page();
            // }


            StatusMessage = $" Đã cập nhật Role thành công cho User : {user.UserName}";

            return RedirectToPage("./Index");
        }
    }
}
