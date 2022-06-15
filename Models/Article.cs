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

        [StringLength(255, MinimumLength = 5, ErrorMessage = "Tiêu đề phải từ {2} đến {1} ký tự")]
        [Required(ErrorMessage = "{0} khong duoc de trong")]
        [Column(TypeName="nvarchar")]
        [Display(Name="Tiêu đề")]
        public string Title {get;set;}

        [DataType(DataType.Date)]
        [Required(ErrorMessage = "{0} khong duoc de trong")]
        [Display(Name="Ngày đăng")]
        public DateTime Created {set;get;}
        
        [Column(TypeName="ntext")]
        [Display(Name="Nội dung")]
        public string Content {get;set;}

        //tiep theo la tao ra dbcontext : bieu dien cau truc csdl
    }
}