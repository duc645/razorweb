using System.Text.RegularExpressions;
using System.IO.Pipes;
using System.Xml.Linq;
using System.IO;
using System.Transactions;
using System.Net.Mime;
using System.Net;
using System.Security.Authentication;
using System.Net.NetworkInformation;
using System.Buffers;
using System.Reflection;
using System.Runtime.Serialization;
using System.Net.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Data.SqlClient;
// using cs58.data;
using Microsoft.EntityFrameworkCore;
using cs58.models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Routing;
namespace cs58
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddDbContext<MyBlogContext>(options =>{
                string connectstring = Configuration.GetConnectionString("MyBlogContext");
                options.UseSqlServer(connectstring);
            });


            services.AddOptions();
            var mailsetting = Configuration.GetSection("MailSettings");
            services.Configure<MailSettings>(mailsetting);
            // mailsetting chua de lieu cua MailSettings trong appsettings.json
            //  dang ky MailSetting vao ung dung , lop nay lay du lieu tu mailsetting
            // MailSetting se duoc inject vao lop SendMailService khi dich vu nay dc tao ra
            services.AddSingleton<IEmailSender, SendMailService>();
            //dang ky dich vu IEmailSender dc tao ra tu SendMailService

            //Dang ky Identity
            services.AddIdentity<AppUser, IdentityRole>()
                    .AddEntityFrameworkStores<MyBlogContext>()
                    .AddDefaultTokenProviders();
            

            //su dung dang nhap, dk,dang xuat voi giao dien mac dinh ,default
            // services.AddDefaultIdentity<AppUser>()
            //         .AddEntityFrameworkStores<MyBlogContext>()
            //         .AddDefaultTokenProviders();
            
            // Truy cập IdentityOptions
            services.Configure<IdentityOptions> (options => {
                // Thiết lập về Password
                options.Password.RequireDigit = false; // Không bắt phải có số
                options.Password.RequireLowercase = false; // Không bắt phải có chữ thường
                options.Password.RequireNonAlphanumeric = false; // Không bắt ký tự đặc biệt
                options.Password.RequireUppercase = false; // Không bắt buộc chữ in
                options.Password.RequiredLength = 3; // Số ký tự tối thiểu của password
                options.Password.RequiredUniqueChars = 1; // Số ký tự riêng biệt

                // Cấu hình Lockout - khóa user
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes (5); // Khóa 5 phút// thoi gian khóa đăn nhập là 5'
                options.Lockout.MaxFailedAccessAttempts = 3; // Thất bại 5 lầ thì khóa
                options.Lockout.AllowedForNewUsers = true;

                // Cấu hình về User.
                options.User.AllowedUserNameCharacters = // các ký tự đặt tên user
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;  // Email là duy nhất

                // Cấu hình đăng nhập.
                options.SignIn.RequireConfirmedEmail = true;            // Cấu hình xác thực địa chỉ email (email phải tồn tại)
                options.SignIn.RequireConfirmedPhoneNumber = false;     // Xác thực số điện thoại
                
                //NG DUNG PHAI XAC NHAN EMAIL MOI CHO DANG NHAP
                //neu thiet lap la true se khong cho dang nhap ma chuyen huong toi 
                //registerConfirmAccount
                options.SignIn.RequireConfirmedAccount = true;
            });
            
            services.ConfigureApplicationCookie(options => {
                options.LoginPath = "/login/";
                options.LogoutPath = "/logout/";
                options.AccessDeniedPath = "/khongduoctruycap.html";
            });


            services.AddAuthentication()
                    .AddGoogle(options => {
                         var gconfig = Configuration.GetSection("Authentication:Google");
                        options.ClientId = gconfig["ClientId"];
                        options.ClientSecret = gconfig["ClientSecret"];
                        //https://localhost:5001/signin-google
                        options.CallbackPath = "/dang-nhap-tu-google";

                    });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });

            // IdentityUser user;
            // IdentityDbContext context;
        }
    }
}


/*
CREATE, READ ,UPDATE , DELETE (CRUD)

lenh (tao ra cac CRUD cho 1 bang nao do) : dotnet aspnet-codegenerator razorpage -m cs58.models.Article -dc cs58.models.MyBlogContext -outDir Pages/Blog -udl --referenceScriptLibraries
*/ 


/*
Identity : 
cung cấp các chức năng :
    -Authentication : Xác định danh tính -> Login , Logout, ... => hết bài 64 cơ bản hoàn thành
    -Authorization : xác thực quyền truy cập ,có quyền truy cập những trang nào, có thể làm những gì...
    - Quản lý user : Sign up , user , Role ,....

-Cac trang :
    /Identity/Account/Login : dang nhap
    /Identity/Account/Manage : quan ly tai khoan

    cac file razor identity dc mac dinh chay trong : Areas/Identity/Pages 
    => dat file _ViewStart(de thay doi layout cho cac trang identity
    (cac trang identity co layout mac dinh rieng)) o trong duong dan thu muc nay



-phat sinh code cua cac trang Identity
=>dotnet aspnet-codegenerator identity -dc cs58.models.MyBlogContext
*/