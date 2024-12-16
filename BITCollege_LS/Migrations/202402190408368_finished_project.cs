namespace BITCollege_LS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class finished_project : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Courses", "CourseNumber", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Courses", "CourseNumber", c => c.String(nullable: false));
        }
    }
}
