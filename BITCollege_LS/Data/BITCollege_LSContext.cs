using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace BITCollege_LS.Data
{
    public class BITCollege_LSContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx
    
        public BITCollege_LSContext() : base("name=BITCollege_LSContext")
        {
        }

        public System.Data.Entity.DbSet<BITCollege_LS.Models.Student> Students { get; set; }

        public System.Data.Entity.DbSet<BITCollege_LS.Models.AcademicProgram> AcademicPrograms { get; set; }

        public System.Data.Entity.DbSet<BITCollege_LS.Models.GradePointState> GradePointStates { get; set; }

        public System.Data.Entity.DbSet<BITCollege_LS.Models.Course> Courses { get; set; }

        public System.Data.Entity.DbSet<BITCollege_LS.Models.Registration> Registrations { get; set; }

        public System.Data.Entity.DbSet<BITCollege_LS.Models.AuditCourse> AuditCourses{ get; set; }

        public System.Data.Entity.DbSet<BITCollege_LS.Models.MasteryCourse> MasteryCourses { get; set; }

        public System.Data.Entity.DbSet<BITCollege_LS.Models.GradedCourse> GradedCourses { get; set; }

        public System.Data.Entity.DbSet<BITCollege_LS.Models.SuspendedState> SuspendedStates { get; set; }

        public System.Data.Entity.DbSet<BITCollege_LS.Models.ProbationState> ProbationStates { get; set; }

        public System.Data.Entity.DbSet<BITCollege_LS.Models.RegularState> RegularStates { get; set; }

        public System.Data.Entity.DbSet<BITCollege_LS.Models.HonoursState> HonoursStates { get; set; }

        public System.Data.Entity.DbSet<BITCollege_LS.Models.NextAuditCourse> NextAuditCourses { get; set; }

        public System.Data.Entity.DbSet<BITCollege_LS.Models.NextGradedCourse> NextGradedCourses { get; set; }

        public System.Data.Entity.DbSet<BITCollege_LS.Models.NextMasteryCourse> NextMasteryCourses { get; set; }

        public System.Data.Entity.DbSet<BITCollege_LS.Models.NextRegistration> NextRegistrations { get; set; }

        public System.Data.Entity.DbSet<BITCollege_LS.Models.NextStudent> NextStudents { get; set; }

        public System.Data.Entity.DbSet<BITCollege_LS.Models.NextUniqueNumber> NextUniqueNumbers { get; set; }
    }
}
