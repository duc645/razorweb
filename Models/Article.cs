using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
namespace cs58.models
{
    // [Table("posts")]
    public class Article 
    {   
        [Key]
        public int Id {set;get;}

        [StringLength(255)]
        [Required]
        [Column(TypeName="nvarchar")]
        public string Title {get;set;}

        [DataType(DataType.Date)]
        [Required]
        public DateTime Created {set;get;}
        
        [Column(TypeName="ntext")]
        public string Content {get;set;}

        //tiep theo la tao ra dbcontext : bieu dien cau truc csdl
    }
}