using System.Xml;
using System.ComponentModel;
using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Bogus;
using cs58.models;

namespace cs58.Migrations
{
    public partial class initdb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "articles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Content = table.Column<string>(type: "ntext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_articles", x => x.Id);
                });

            //Insert data
            //Fake data : dung thu vien Bogus, co o tren Nuget

            // Randomizer.Seed = new Random(8675309);
            // var fakerArticle = new Faker<Article>();
            // fakerArticle.RuleFor(a => a.Title,f => f.Lorem.Sentence(5, 5));
            // fakerArticle.RuleFor(a => a.Created, f => f.Date.Between(new DateTime(2021,1,1), new DateTime(2021,7,30)));
            // fakerArticle.RuleFor(a => a.Content,f => f.Lorem.Paragraphs(1, 4));

            // for(int i=0 ;i <150;i++){
            //     Article article = fakerArticle.Generate();//tap ra doi tuong co cac gia tri thuoc tinh tren
            //     migrationBuilder.InsertData(
            //         table: "articles",
            //         columns: new[] {"Title", "Created", "Content"},
            //         values: new object[] {
            //             article.Title ,
            //             article.Created,
            //             article.Content,
            //         }
            //     );
            // }

            
        //     migrationBuilder.InsertData(
        //         table: "articles",
        //         columns: new[] {"Title", "Created", "Content"},
        //         values: new object[] {
        //             "Bai viet 2",
        //             new DateTime(2021, 8, 21),
        //             "Noi dung 2"
        //         }
        //     );

        //     migrationBuilder.InsertData(
        //         table: "articles",
        //         columns: new[] {"Title", "Created", "Content"},
        //         values: new object[] {
        //             "Bai viet 3",
        //             new DateTime(2021, 8, 22),
        //             "Noi dung 3"
        //         }
        //     );
            
        //     migrationBuilder.InsertData(
        //         table: "articles",
        //         columns: new[] {"Title", "Created", "Content"},
        //         values: new object[] {
        //             "Bai viet 4",
        //             new DateTime(2021, 8, 23),
        //             "Noi dung 4"
        //         }
        //     );
         }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "articles");
        }
    }
}
