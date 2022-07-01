using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using cs58.models;
using System.Security.AccessControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace App.Admin.Role { 
    public class RolePageModel : PageModel {
        //phạm vi truy cập đặt là protected để lớp kế thừa có thể truy cập đc
        protected readonly  RoleManager<IdentityRole> _roleManager;
        protected readonly MyBlogContext _context;
        [TempData]
        public string StatusMessage {get;set;}

        public RolePageModel(RoleManager<IdentityRole> roleManager, MyBlogContext myBlogContext){
            _roleManager = roleManager;
            _context = myBlogContext;
        }
    }
}