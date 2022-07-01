using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using cs58.models;
using Microsoft.AspNetCore.Authorization;

namespace App.Admin.Role
{
    [Authorize(Roles = "Admin")]  
    public class IndexModel : RolePageModel
    {
        public IndexModel(RoleManager<IdentityRole> roleManager, MyBlogContext myBlogContext) : base (roleManager,myBlogContext){

        }
        
        public List<IdentityRole> roles {set;get;}
        //cchuyen OnGet thanh phuong thuc bat dong bo => public void OnGet()

        public async Task  OnGet()
        {
          //giam dan : OrderByDescending
          //tang dan : OrderBy
          roles =  await  _roleManager.Roles.OrderBy(r => r.Name).ToListAsync();
        }

        //nếu truy cập bằng http Post thì dùng  RedirectToPage() để chuyển hướng về phương thức get
        public void OnPost() => RedirectToPage();
    }
}
