using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cs58.models{
    public class AppUser : IdentityUser{
        [Column(TypeName = "nvarchar")]
        [StringLength(400)]
        public string HomeAdress {set;get;}
    }
}