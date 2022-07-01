using System.Runtime.InteropServices;
using System.IO;
using Microsoft.AspNetCore.Identity;
namespace App.Services
{
    public class AppIdentityErrorDescriber : IdentityErrorDescriber {
        //lỗi trùng tên Role
        public override IdentityError DuplicateRoleName(string role){
         var er =  base.DuplicateRoleName(role);
         return new IdentityError(){
            Code = er.Code,
            Description = $"Role có tên {role} bị trùng"
         };
        }
        //Lỗi trùng tên User
        public override IdentityError DuplicateUserName(string userName){
         var er =  base.DuplicateUserName(userName);
         return new IdentityError(){
            Code = er.Code,
            Description = $"User có tên {userName} bị trùng"
         };
        }
    }
}