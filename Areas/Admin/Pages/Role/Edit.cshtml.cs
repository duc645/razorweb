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
    // [Authorize(Roles = "Admin")]  
    [Authorize(Policy = "AllowEditRole")]  

    public class EditModel : RolePageModel
    {

        public EditModel(RoleManager<IdentityRole> roleManager, MyBlogContext myBlogContext) : base (roleManager,myBlogContext){

        }

        public class InputModel{
            [Display(Name="Tên Role")]
            [Required(ErrorMessage= "Phải nhập tên Role")]
            [StringLength(256,MinimumLength = 3 ,ErrorMessage="{0} phải từ {2} đến {1} ký tự")]
            public string Name {set;get;}
        }

        [BindProperty]
        public InputModel Input {set;get;}

        public IdentityRole role{set;get;}

        public async Task<IActionResult> OnGetAsync(string roleid)
        {
            if(roleid == null)
            {
                return NotFound("Không tìm thấy role");
            }
            //phương thức này trả về đối tượng IdentityRole
             var role = await _roleManager.FindByIdAsync(roleid);

            if(role == null) {
                return NotFound("Không tìm thấy role"); 
            } 
            Input = new InputModel(){
                    Name = role.Name
                };
            return Page();
        }


        public async Task<IActionResult> OnPostAsync(string roleid) {
            if(roleid == null)
            {
                return NotFound("Không tìm thấy role");
            }

            var role = await _roleManager.FindByIdAsync(roleid);

            if(role == null) {
                return NotFound("Không tìm thấy role"); 
            } 

            //nếu dữ liệu submit ko phù hợp
            if(!ModelState.IsValid)
            {
                return Page();
            }

            //nếu dữ liệu phù hợp
            role.Name = Input.Name;
            var result =  await _roleManager.UpdateAsync(role);

            //Nếu cập nhật thành công
            if(result.Succeeded)
            {
                StatusMessage = $"Role : {Input.Name} đã được cập nhật";

                return RedirectToPage("./Index");
            }
            else {


                result.Errors.ToList().ForEach(error => {
                    ModelState.AddModelError(string.Empty, error.Description);
                });

            }

            return Page();
        }
    }
}
