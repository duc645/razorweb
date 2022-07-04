using System.Text;
using System.Reflection.Metadata;
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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authorization;
namespace App.Admin.User
{

    //TH1: viết nhiều dòng Authorize, thì phải thỏa mãn cả 3 dòng authorize mới đc truy cập
    //nghĩa là user có cả 3 vai trò này thì đc truy cập
    // [Authorize(Roles = "Admin")]
     // [Authorize(Roles = "VipMember")]
     // [Authorize(Roles = "Editor")]

    // [Authorize(Roles = "Admin")]  
    //TH2: [Authorize(Roles = "role1,role2,role3")]
    // => user nào có 1 TRONG 3 cái vai trò trên sẽ đc truy cập => giống với phép toán HOẶC
    // [Authorize(Roles = "Admin,VipMember,Editor")]
    //nếu user ko có quyền mà truy cập nó sẽ chuyển hướng về trang ko đc truy cập
    //tên là khongduoctruycap.html (file này tương ứng với trang AccessDenied,
    // file nằm ở Identity/Pages/Accouunt)
    //được cấu hình trong file startup
    /*
                services.ConfigureApplicationCookie(options => {
                options.LoginPath = "/login/";
                options.LogoutPath = "/logout/";
                options.AccessDeniedPath = "/khongduoctruycap.html";
            });
    */
    public class IndexModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        public IndexModel(UserManager<AppUser> userManager){
           _userManager = userManager;
        }
        [TempData]
        public string StatusMessage {get;set;}
        
        //Do class AppUser ko có trường Role nên ko đưa đc ra trang index
        //tạo 1 lớp khác kế thừa lớp AppUser và cho thêm trường Role
        public class UserAndRole : AppUser{
          public string RoleNames {set;get;}
        }

        public List<UserAndRole> users {set;get;}
        //cchuyen OnGet thanh phuong thuc bat dong bo => public void OnGet()
        
        public const int ITEMS_PER_PAGE = 10; //số phần tử hiển thị mỗi trang
        [BindProperty(SupportsGet = true, Name = "p")]
        public int currentPage {get;set;}//trang hiện tại đc Binding đến từ tham số p
        public int countPages {get;set;}//tổng số trang


        public int totalUsers {set;get;}

        public async Task  OnGet()
        {
          //giam dan : OrderByDescending
          //tang dan : OrderBy
          //users =  await  _userManager.Users.OrderBy(u => u.UserName).ToListAsync();
          var qr = _userManager.Users.OrderBy(u => u.UserName);

          //code xử lý phân trang
          totalUsers = await qr.CountAsync();
          countPages = (int)Math.Ceiling((double)totalUsers / ITEMS_PER_PAGE);
          if(currentPage < 1)
           currentPage = 1;
          if(currentPage > countPages ) 
            currentPage = countPages;

          var qr1 = qr.Skip((currentPage - 1 ) * ITEMS_PER_PAGE)
              .Take(ITEMS_PER_PAGE)
              .Select(u => new UserAndRole(){
                Id = u.Id,
                UserName = u.UserName,
              } );
          //Select ở trên nghĩa là mỗi phần tử trong qr1(kí hiệu là u)
          //thì sẽ trả về một đối tượng mới new UserAndRole()
          //và thiết lập nó có các thuộc tính ở trên(các thuộc tính mà chúng ta sd ở view)
          users = await qr1.ToListAsync();
          foreach (var user in users)
          {
            var roles = await _userManager.GetRolesAsync(user);
            user.RoleNames = string.Join(" | ",roles);
          }
        }

        //nếu truy cập bằng http Post thì dùng  RedirectToPage() để chuyển hướng về phương thức get
        public void OnPost() => RedirectToPage();
    }
}
