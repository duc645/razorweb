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

            //Dang ky Identity
            services.AddIdentity<AppUser, IdentityRole>()
                    .AddEntityFrameworkStores<MyBlogContext>()
                    .AddDefaultTokenProviders();

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
    -Authentication : Xác định danh tính -> Login , Logout, ...
    -Authorization : xác thực quyền truy cập ,...
    - Quản lý user : Sign up , user , Role ,....
*/