using System.IO;
using System.Text;
using System.Reflection;
using System.Runtime.Intrinsics.X86;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.AccessControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using cs58.models;
using Microsoft.AspNetCore.Authorization;
namespace App.Admin.Role
{
    [Authorize(Roles = "Admin")]  
    public class CreateModel : RolePageModel
    {

        public CreateModel(RoleManager<IdentityRole> roleManager, MyBlogContext myBlogContext) : base (roleManager,myBlogContext){

        }

        public class InputModel{
            [Display(Name="Tên Role")]
            [Required(ErrorMessage= "Phải nhập tên Role")]
            [StringLength(256,MinimumLength = 3 ,ErrorMessage="{0} phải từ {2} đến {1} ký tự")]
            public string Name {set;get;}
        }
        [BindProperty]
        public InputModel Input {set;get;}
        public void OnGet()
        {

        }


        public async Task<IActionResult> OnPostAsync() {

            if(!ModelState.IsValid)
            {
                return Page();
            }
            var newRole = new IdentityRole(Input.Name);
            var result = await _roleManager.CreateAsync(newRole);
            if(result.Succeeded)
            {
                StatusMessage = $"Bạn vừa tạo Role thành công : {Input.Name}";
                //cùng thư mục với trang hiện tại (create) nên có dấu chấm trc gạch
                return RedirectToPage("./Index");
            }
            else {

                //khi roleManager khởi tạo thì trong phương tạo của nó đc inject nhiều dịch vụ
                //trong đó có dịch vụ IdentityErrorDescriber với biến là errors
                //dịch vụ này để tùy biến các thông báo lỗi của Identity

                //IdentityErrorDescriber có nhiều method đc khai báo virtual , có thể nạp chồng

                //Cách để tùy biến , dịch sang tiếng việt các lỗi có trong result.Errors
                //Bây giờ ta tạo ra một lớp kế thừa lớp IdentityErrorDescriber
                //sau đó ta sẽ lấy lớp của chúng ta đăng ký thay cho IdentityErrorDescriber
                result.Errors.ToList().ForEach(error => {
                    ModelState.AddModelError(string.Empty, error.Description);
                });

            }
            // //tạo ra đối tượng kiểu IdentityRole để cho vào method CreateAsync
            // var newRole = new IdentityRole("Ten-cua-Role");
            
            // //tham số của phương thức là đối tượng IdentityRole
            // await _roleManager.CreateAsync(newRole);
            // //mặc định trang này sẽ return Page() nghĩa là render trang Create.cshtml
            return Page();
        }
    }
}
