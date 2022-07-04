using System.Security.Principal;
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
using App.Services;
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
            //khi lấy ra dịch vụ  IdentityErrorDescriber , thì nó sẽ lấy ra một đối tượng
            //lớp  AppIdentityErrorDescriber
            services.AddSingleton<IdentityErrorDescriber,AppIdentityErrorDescriber>();

            services.AddAuthorization(options =>{
                options.AddPolicy("AllowEditRole", policyBuilder => {
                    //điều kiện của policy
                    //phai dang nhap , phai co vai tro admin va editor
                    policyBuilder.RequireAuthenticatedUser();
                    policyBuilder.RequireRole("Admin");
                    policyBuilder.RequireRole("Editor");
                    policyBuilder.RequireClaim("ten claim")

                });
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
        xác thực quyền dựa vào vai trò :  Role-based authorization
        +thông tin về zole đc lưu trữ trong bảng role
        +trên ứng dụng tạo ra nhiều role , user sẽ đc gán 1 hoặc nhiều zole
        +vd : Role : (Admin, Edit , Role ,Manager,Member ....)
        +Trong identity có 1 dịch vụ để quản lý các role
        =>RoleManager<IdentityRole>, đối tượng nó quản lý có kiểu IdentityRole
        =>với các phương thức tạo ra role mới, xóa role , lấy role theo id hoặc tên, CreateAsync,FindById,RoleExistsAsync(Kiểm tra sự tồn tại của IdentityRole theo tên của nó),...
        +Việc đầu tiên là phải tạo ra các trang quản lý Role : index, Create,Delete Edit
        +dùng lệnh để tạo nhanh các page : dotnet new page -n Index -o Areas/Admin/Pages/Role -na App.Admin.Role
        dotnet new page -n Create -o Areas/Admin/Pages/Role -na App.Admin.Role

        +giải thích : -n là tên trang ,-o là lưu ở đâu ,-na là tên namespace
    - Quản lý user : Sign up , user 

-Cac trang :
    /Identity/Account/Login : dang nhap
    /Identity/Account/Manage : quan ly tai khoan

    cac file razor identity dc mac dinh chay trong : Areas/Identity/Pages 
    => dat file _ViewStart(de thay doi layout cho cac trang identity
    (cac trang identity co layout mac dinh rieng)) o trong duong dan thu muc nay



-phat sinh code cua cac trang Identity
=>dotnet aspnet-codegenerator identity -dc cs58.models.MyBlogContext
*/

//chuyển lỗi thành tiếng việt 23:57 video cs65 , tạo lớp kế thừa IdentityErrorDescriber
//inject trong file startup và ghi đè lại phương thức hiển thị lỗi mà muốn 
//chuyển sang tiếng việt


//  Muốn thêm nhiều User phải migrations . cs65 40:00
//dotnet ef migrations list : danh sách các migrations
//dotnet ef migrations add SeedUsers
//rồi vào file migrations vừa tạo , vào phương thức up, thêm code :
/*
            for (int i=1 ;i< 150 ;i++){
                migrationBuilder.InsertData(
                    "Users",
                    columns: new[] {},
                    values : new object[] {}
                )
            }
*/
// sau khi viết code tự động sinh ra các user xong , thực hiện lệnh
//dotnet ef database update



//phân trang copy lại code , xem 43:45 bài cs65

//phần hiển thị các role của User ra ngoài trang index ,khó hiểu
//vì AppUser hay trong bang User ko có trường Role 
//nên tạo ra một lớp mới kế thừa lớp AppUser
//xem cs65 1:01:00



//Bây giờ đến bước sử dụng role đc gán cho user để xác thực quyền truy cập cho user
//ta sử dụng atribute [Authorize] - có thể thiết lập cho Controller , tới từng 
//action của controller. Nhưng trong PageModel nó chỉ thiết lập cho PageModel(cấp độ class) đó thôi
//ko thiết lập đc cho từng handler-> mặc định , muốn sử dụng đc [Authorize] thì user 
//phải đăng nhập


//cs66 : xác thực quyền truy cập theo :
// -Policy-based authorization :xác thực theo chính sách
// -Claims-based authorization :xác thực theo tính chất,đặc tính của user

//Policy : Tạo ra các chính sách(policy) 
//bên trong policy là các đoạn code ,tại đó nó kiểm tra user thỏa mã điều kiện đặt ra
//Để tạo ra các policy thì vào file startup , trong phương thức ConfigureServices
/*
            services.AddAuthorization(options =>{
                options.AddPolicy("TenPolicy",policyBuilder=> {
                    //user do phai dang nhap
                    policyBuilder.RequireAuthenticatedUser 
                    //user do phai co nhung claim
                    policyBuilder.RequireClaim 
                    //phai co role
                    policyBuilder.Role()
                    policyBuilder.UserName()
                    policyBuilder.RequireClaim("ten claim","giá trị mà tên claim có thể nhận","giatri2","giatri3")
                    hoặc :
                    policyBuilder.RequireClaim("ten claim", new string[]{
                        "giatri1",
                        "giatri2",
                    })

                });
                //tạo ra nhiều policy nữa ở đây
                options.AddPolicy("TENPOLICY2",policyBuilder=> {
                    //điều kiện của policy
                });
            });
*/
// Claims la mot dac tinh , tinh chat cua doi tuong(user)
//ví dụ có tấm bằng lái B2(được coi là vai trò- Role)
//cứ có tấm bằng này thì đc lái xe 4 chỗ
//tren bang lai co :
// +ngay sinh -> dc coi la claim
// +Noi sinh -> dc coi la claim (dac tinh mo ta ve nguoi)

//vd khac : mua rượu (> 18 tuổi)
// -Việc kiểm tra ngày sinh : gọi là xác thực quyền theo claim, theo tính chất của đối tượng
// Claims-based authorization

//trong Identity căn cứ theo bảng csdl , dbcontext bảng RoleClaim là đối tượng có kiểu
//IdentityRoleClaim<string> claim1; :là cái model tương ứng với những phần tử của bảng RoleClaims
//tương tự như vậy cái model tương ứng với những phần tử của bảng UserClaims,
//thì đó là các đối tượng kiểu IdentityUserClaim<string> claim2;
//TRONG IdentityRoleClaim<string> claim1,IdentityUserClaim<string> claim2
//có các trường dự liệu ClaimType , ClaimValue ,... chính là các cột của 1 một bảng
//
// Khi sử dụng các dịch vụ của Identty để truy vấn lấy ra những claim của một Roles ,
//hay là lấy ra claim của một Users thì nó ko trả về đối tượng IdentityUserClaim<string> claim2
//hay IdentityRoleClaim<string> claim1 mà nó thường trả về đối tượng 
//Claim claim3 : cái này nó khác với hai cái trên là ko có trường dữ liệu id
//các trường khác nó vẫn có