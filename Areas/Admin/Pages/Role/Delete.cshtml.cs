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
    public class DeleteModel : RolePageModel
    {

        public DeleteModel(RoleManager<IdentityRole> roleManager, MyBlogContext myBlogContext) : base (roleManager,myBlogContext){

        }



        public IdentityRole role{set;get;}

        public async Task<IActionResult> OnGet(string roleid)
        {
            if(roleid == null)
            {
                return NotFound("Không tìm thấy role");
            }
            //phương thức này trả về đối tượng IdentityRole
            //NullReferenceException: Object reference not set to an instance of an object.
            //bên trên khai báo mà vẫn khai báo var ở đây nên lỗi
             role = await _roleManager.FindByIdAsync(roleid);
             if(role ==null)
             {
                 return NotFound("Không tìm thấy role");
             }

            return Page();
        }


        public async Task<IActionResult> OnPostAsync(string roleid) {
            if(roleid == null)
            {
                return NotFound("Không tìm thấy role");
            }

            role = await _roleManager.FindByIdAsync(roleid);

            if(role == null) {
                return NotFound("Không tìm thấy role"); 
            } 

            var result = await _roleManager.DeleteAsync(role);
            if(result.Succeeded)
            {
                StatusMessage = "Xóa thành công";

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
