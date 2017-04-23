namespace TextPlagiarismWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Courses",
                c => new
                    {
                        courseId = c.String(nullable: false, maxLength: 128),
                        courseName = c.String(),
                        description = c.String(),
                        Hours = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.courseId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Courses");
        }
    }
}
