using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace cs58.Migrations
{
    public partial class SeedUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            for (int i=1 ;i< 150 ;i++){
                migrationBuilder.InsertData(
                    "Users",
                    columns: new[] {
                        "Id",
                        "UserName",
                        "Email",
                        "SecurityStamp",
                        "EmailConfirmed",
                        "PhoneNumberConfirmed",
                        "TwoFactorEnabled",
                        "LockoutEnabled",
                        "AccessFailedCount",
                        "HomeAdress",
                    },
                    values : new object[] {
                        Guid.NewGuid().ToString(),//phát sinh dữ liệu ngẫu nhiên cho côt Id
                        "User-"+i.ToString("D3"),//D3 tức là i =1 thì UserName sẽ là 001
                        $"email{i.ToString("D3")}@example.com",
                        Guid.NewGuid().ToString(),
                        true,
                        false,
                        false,
                        false,
                        0,
                        "...@#%..."
                    }
                );
            }
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
