using System;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace cs58.models{
    public class AppUser : IdentityUser{
        [Column(TypeName = "nvarchar")]
        [StringLength(400)]
        public string HomeAdress {set;get;}
        //muoons nhap nhieu dong thi thay the input bang textarea

        //ngay StringLength
        //Khong co truong Required => co the nhan gtri null
        //Them vao tu bai cs64 , 12:13.
        //Khi theem 1 truong du lieu moi phai migration.
        //Các lệnh :  dotnet ef migrations list : xem ds cac migrations đc thoại
        //dotnet ef migrations add UpdateUser2 : tạo ra một cái migrations mới(trạng thái Pending)
        //dotnet ef database update : Cập nhật lại database (có lệnh này mới ko lỗi)

        //Chú ý do theo video , sau khi migration như trên thì 
        //thầy có thay đổi là cho thêm ? là có thể null 
        //do đố cần phải sửa đổi , thực hiện như sau : 
        // dotnet ef migrations list : xem ds các migration
        //dotnet ef database update UpdateUser : quay trở về migration cũ
        //Tiếp : dùng, dotnet ef migrations remove : để xóa đi migration UpdateUser2
        //Bây giờ thì UpdateUser2 đã bị xóa.
        //Dùng : dotnet ef migrations add UpdateUser2 : tạo ra một cái migrations mới (Pending)
        //dotnet ef database update : Cập nhật lại database (có lệnh này mới ko lỗi)
        [DataType(DataType.Date)]
        public DateTime? BirthDate {set;get;}//muon cho gtri null thi cho dau ?
        //nếu muốn ngày sinh nằm từ năm này đến năm kia
            //thì ta phải xây dựng validate riêng
    }
}