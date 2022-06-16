using Microsoft.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore.DbContext;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
namespace cs58.models{
   public class MyBlogContext : IdentityDbContext<AppUser>
   {
      
       public MyBlogContext(DbContextOptions<MyBlogContext> options) : base(options)
       {

       }

       protected override void OnConfiguring(DbContextOptionsBuilder builder){
           base.OnConfiguring(builder);
       }
       
       protected override void OnModelCreating(ModelBuilder modelBuilder){
           base.OnModelCreating(modelBuilder);
       }


        public DbSet<Article> articles{get;set;} 
       //Dbset la mot tap hop chua cac phan tu kieu Article
       //khai bao nhu nay thi trong csdl se co bang articles, co cac dong theo kieu du lieu Article



    //    migration : o trang thai "pending" : chua duoc su dung de tao ra csdl , chua dc cap nhat len sql server
   }
}