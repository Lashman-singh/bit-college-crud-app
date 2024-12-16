using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Utility;
using System.Runtime.Remoting.Messaging;
using BITCollege_LS.Data;
using System.Security.Cryptography.X509Certificates;
using System.Runtime.InteropServices;
using System.Data.Entity.Infrastructure;

namespace BITCollege_LS.Models
{
    /// <summary>
    ///  Student Model - to represent Student table in database.
    /// </summary>
    public class Student
    {
        BITCollege_LSContext db = new BITCollege_LSContext();
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int StudentId { get; set; }

        [Required]
        [ForeignKey("GradePointState")]
        public int GradePointStateId { get; set; }

        [ForeignKey("AcademicProgram")]
        public int? AcademicProgramId { get; set; }

        [Display(Name = "Student\nNumber")]
        public long StudentNumber { get; set; }

        [Required]
        [Display(Name = "First\nName")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last\nName")]
        public string LastName { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        [RegularExpression("^(N[BLSTU]|[AMN]B|[BQ]C|ON|PE|SK|YT)", ErrorMessage = "Invalid Canadian province code")]
        public string Province { get; set; }

        [Required]
        [Display(Name = "Date")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime DateCreated { get; set; }

        [Range(0, 4.5)]
        [DisplayFormat(DataFormatString = "{0:F2}")]
        [Display(Name = "Grade\nPoint\nAverage")]
        public double? GradePointAverage { get; set; }

        [Required]
        [Display(Name = "Fees")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public double OutstandingFees { get; set; }

        public string Notes { get; set; }

        [Display(Name = "Name")]
        public string FullName
        {
            get
            {
                return String.Format("{0} {1}", FirstName, LastName);
            }
        }

        [Display(Name = "Address")]
        public string FullAddress
        {
            get
            {
                return String.Format("{0} {1}, {2}", Address, City, Province);
            }
        }

        //Navigation properties
        public virtual GradePointState GradePointState { get; set; }

        public virtual AcademicProgram AcademicProgram { get; set; }

        public virtual ICollection<Registration> Registration { get; set; }

        /// <summary>
        /// Method checks the current state and update it based on the GradePointState
        /// </summary>
        public void ChangeState()
        {   
            GradePointState currentState = db.GradePointStates.Find(this.GradePointStateId);

            int nextState = 0;

            while(nextState != currentState.GradePointStateId)
            {
                currentState.StateChangeCheck(this);
                nextState = currentState.GradePointStateId;
                currentState = db.GradePointStates.Find(GradePointStateId);
            }
        }

        /// <summary>
        /// Updates the number to next available number
        /// </summary>
        public void SetNextStudentNumber()
        {
          this.StudentNumber = (long)StoredProcedure.NextNumber(string.Format("Next{0}", this.GetType().Name));
        }
    }

    /// <summary>
    /// AcademicProgram Model - to represent the AcademicProgrm table in the database.
    /// </summary>
    public class AcademicProgram
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AcademicProgramId { get; set; }

        [Required]
        [Display(Name = "Program")]
        public string ProgramAcronym { get; set; }

        [Required]
        [Display(Name = "Program\nName")]
        public string Description { get; set; }

        //Navigation properties
        public virtual ICollection<Student> Student { get; set; }

        public virtual ICollection<Course> Course { get; set; }
    }

    /// <summary>
    /// GradePointState Model - to represent the GradePointState table in the database.
    /// </summary>
    public abstract class GradePointState
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int GradePointStateId { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        [Display(Name = "Lower\nLimit")]
        public double LowerLimit { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        [Display(Name = "Upper\nLimit")]
        public double UpperLimit { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        [Display(Name = "Tuition\nRate\nFactor")]
        public double TuitionRateFactor { get; set; }
       
        [Display(Name = "State")]
        public string Description
        {
            get
            {
                return BusinessRules.ParseString(GetType().Name, "State");
            }
        }

        //Navigation property
        public virtual ICollection<Student> Student { get; set; }

        public abstract double TuitionRateAdjustment(Student student);

        public abstract void StateChangeCheck(Student student);

        protected static BITCollege_LSContext db = new BITCollege_LSContext();

        }

    /// <summary>
    /// SuspendedState Model - to represent the course table in the database.
    /// </summary>
    public class SuspendedState : GradePointState
    {
        private static SuspendedState suspendedState;

        private const double Lower_Limit = 0.00;
        private const double Upper_Limit = 1.00;
        private const double Tuition_Rate_Factor = 1.1;

        /// <summary>
        /// Private constructor to enforce singleton pattern and initialize constants.
        /// </summary>
        private SuspendedState()
        {
            this.LowerLimit = Lower_Limit;
            this.UpperLimit = Upper_Limit;
            this.TuitionRateFactor = Tuition_Rate_Factor;
        }

        /// <summary>
        /// GetInstance() method to implement singleton pattern for 
        /// SuspendedState to retrieve or create instance.
        /// </summary>
        /// <returns>suspendedState</returns>
        public static SuspendedState GetInstance()
        {
            if (suspendedState == null)
            {
                suspendedState = db.SuspendedStates.SingleOrDefault();

                if (suspendedState == null)
                {
                    // If not, use the private constructor to populate the static variable
                    suspendedState = new SuspendedState();

                    // Use the populated static variable to persist this record to the database
                    db.SuspendedStates.Add(suspendedState);
                    db.SaveChanges();
                }
            }
            return suspendedState;
        }

        /// <summary>
        /// Updates the tuition rate based on the GradePointAverage, course taken,
        /// and time. Depends on current state.
        /// </summary>
        /// <param name="student">student object</param>
        /// <returns>charges "updated tuition rate</returns>
        public override double TuitionRateAdjustment(Student student)
        {
            double charges = Tuition_Rate_Factor;

            if (student.GradePointAverage < 0.75 && student.GradePointAverage > 0.5)
            {
                charges += 0.02;
            }
            else if (student.GradePointAverage < 0.5)
            {
                charges += 0.05;
            }
            return charges;
        }

        /// <summary>
        /// Updates the state based on GPA and limit
        /// </summary>
        /// <param name="student"></param>
        public override void StateChangeCheck(Student student)
        {
            if(student.GradePointAverage > UpperLimit)
            {
                student.GradePointStateId = ProbationState.GetInstance().GradePointStateId;
            }
        }
    }

    /// <summary>
    /// ProbationState Model - to represent the ProbationState table in the database.
    /// </summary>
    public class ProbationState : GradePointState
    {
        private const double Lower_Limit = 1.00;
        private const double Upper_Limit = 2.00;
        private const double Tuition_Rate_Factor = 1.075;

        private static ProbationState probationState;

        /// <summary>
        /// Private constructor to enforce singleton pattern and initialize constants.
        /// </summary>
        private ProbationState()
        {
            this.LowerLimit = Lower_Limit;
            this.UpperLimit = Upper_Limit;
            this.TuitionRateFactor = Tuition_Rate_Factor;
        }

        /// <summary>
        /// GetInstance() method to implement singleton pattern for 
        /// ProbationState to retrieve or create instance.
        /// </summary>
        /// <returns>probationState</returns>
        public static ProbationState GetInstance()
        {
            if(probationState == null)
            {
                probationState = db.ProbationStates.SingleOrDefault();

                if (probationState == null)
                {
                    // If not, use the private constructor to populate the static variable
                    probationState = new ProbationState();
                    db.ProbationStates.Add(probationState);

                    // Use the populated static variable to persist this record to the database
                    db.SaveChanges();
                }
            }
            return probationState;
        }

        /// <summary>
        /// Updates the tuition rate based on the GradePointAverage, course taken,
        /// and time. Depends on current state.
        /// </summary>
        /// <param name="student">student object</param>
        /// <returns>charges "updated tuition rate</returns>
        public override double TuitionRateAdjustment(Student student)
        {
            double charges = TuitionRateFactor;

            // Check if the student has completed 5 or more courses
            if (student.Registration.Count(r => r.Grade != null) >= 5)
            {
                charges += 0.035; 
            }

            return charges;
        }

        /// <summary>
        /// Updates the state based on GPA and limit
        /// </summary>
        /// <param name="student"></param>
        public override void StateChangeCheck(Student student)
        {
            if(student.GradePointAverage > UpperLimit)
            {
                student.GradePointStateId = RegularState.GetInstance().GradePointStateId;
            }
            else if(student.GradePointAverage  < LowerLimit)
            { 
                student.GradePointStateId = SuspendedState.GetInstance().GradePointStateId;
            }
            db.SaveChanges();
        }
    }

    /// <summary>
    /// RegularState Model - to represent the RegularState table in the database.
    /// </summary>
    public class RegularState : GradePointState
    {
        private static RegularState regularState;

        public const double Lower_Limit = 2.00;
        public const double Upper_Limit = 3.70;
        public const double Tuition_Rate_Factor = 1.0;

        /// <summary>
        /// Private constructor to enforce singleton pattern and initialize constants.
        /// </summary>
        private RegularState()
        {
            this.LowerLimit = Lower_Limit;
            this.UpperLimit = Upper_Limit;
            this.TuitionRateFactor = Tuition_Rate_Factor;
        }

        /// <summary>
        /// GetInstance() method to implement singleton pattern for 
        /// RegularState to retrieve or create instance.
        /// </summary>
        /// <returns>regularState</returns>
        public static RegularState GetInstance()
        {
            if (regularState == null)
            {
                regularState = db.RegularStates.SingleOrDefault();

                if (regularState == null)
                {
                    // If not, use the private constructor to populate the static variable
                    regularState = new RegularState();

                    // Use the populated static variable to persist this record to the database
                    db.RegularStates.Add(regularState);
                    db.SaveChanges();
                }
            }
            return regularState;
        }

        /// <summary>
        /// Updates the tuition rate based on the GradePointAverage, course taken,
        /// and time. Depends on current state.
        /// </summary>
        /// <param name="student">student object</param>
        /// <returns>Tuition_Rate_Factor "updated tuition rate</returns>
        public override double TuitionRateAdjustment(Student student)
        {
            return Tuition_Rate_Factor;
        }

        /// <summary>
        /// Updates the state based on GPA and limit
        /// </summary>
        /// <param name="student"></param>
        public override void StateChangeCheck(Student student)
        {
            if(student.GradePointAverage > UpperLimit)
            {
                student.GradePointStateId = HonoursState.GetInstance().GradePointStateId;
            }
            else if(student.GradePointAverage < LowerLimit)
            {
                student.GradePointStateId = ProbationState.GetInstance().GradePointStateId;

            }
            db.SaveChanges();
        }
    }

    /// <summary>
    /// HonoursState Model - to represent the HonoursState table in the database.
    /// </summary>
    public class HonoursState : GradePointState
    {
        private static HonoursState honoursState;

        public const double Lower_Limit = 3.70;
        public const double Upper_Limit = 4.50;
        public const double Tuition_Rate_Factor = 0.9;

        /// <summary>
        /// Private constructor to enforce singleton pattern and initialize constants.
        /// </summary>
        private HonoursState()
        {
            this.LowerLimit = Lower_Limit;
            this.UpperLimit = Upper_Limit;
            this.TuitionRateFactor = Tuition_Rate_Factor;
        }

        /// <summary>
        /// GetInstance() method to implement singleton pattern for 
        /// HonoursState to retrieve or create instance.
        /// </summary>
        /// <returns>honoursState</returns>
        public static HonoursState GetInstance()
        {
            if (honoursState == null)
            {
                honoursState = db.HonoursStates.SingleOrDefault();

                if (honoursState == null)
                {
                    honoursState = new HonoursState();

                    db.HonoursStates.Add(honoursState);
                    db.SaveChanges();
                }
            }
            return honoursState;
        }

        /// <summary>
        /// Updates the tuition rate based on the GradePointAverage, course taken,
        /// and time. Depends on current state.
        /// </summary>
        /// <param name="student">student object</param>
        /// <returns>charges "updated tuition rate</returns>
        public override double TuitionRateAdjustment(Student student)
        {
            double charges = Tuition_Rate_Factor;

            if (student.Registration.Count(r => r.Grade != null) >= 5)
            {
                charges -= 0.05; 
            }
            
            if (student.GradePointAverage > 4.25)
            {
                charges -= 0.02;
            }

            return charges;
        }

        /// <summary>
        /// Updates the state based on GPA and limit
        /// </summary>
        /// <param name="student"></param>
        public override void StateChangeCheck(Student student)
        {
            if(student.GradePointAverage < LowerLimit)
            {
                student.GradePointStateId = RegularState.GetInstance().GradePointStateId;
                db.SaveChanges();
            }
        }
    }

    /// <summary>
    /// Course Model - to represent the Course table in the database.
    /// </summary>
    public abstract class Course
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int CourseId { get; set; }

        [ForeignKey("AcademicProgram")]
        public int? AcademicProgramId { get; set; }

        [Display(Name = "Course\nNumber")]
        public string CourseNumber { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Display(Name = "Credit\nHours")]
        public double CreditHours { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        [Display(Name = "Tuition")]
        public double TuitionAmount { get; set; }

        [Display(Name = "Course\nType")]
        public string CourseType
        {
            get
            {
                return BusinessRules.ParseString(GetType().Name, "Course");
            }
        }

        public string Notes { get; set; }

        //Navigation properties
        public virtual ICollection<Registration> Registrations { get; set; }

        public virtual AcademicProgram AcademicProgram { get; set; }

        /// <summary>
        /// Abstract method used in the sub classes of course to update number.
        /// </summary>
        public abstract void SetNextCourseNumber();
    }

    /// <summary>
    /// GradedCourse Model - inherits Course class 
    /// </summary>
    public class GradedCourse : Course
    {
        [Required]
        [Display(Name = "Assignments")]
        [DisplayFormat(DataFormatString = "{0:p}")]
        public double AssignmentWeight { get; set; }

        [Required]
        [Display(Name = "Exams")]
        [DisplayFormat(DataFormatString = "{0:p}")]
        public double ExamWeight { get; set; }

        /// <summary>
        /// Override abstract method, sets next course number.
        /// </summary>
        public override void SetNextCourseNumber()
        {
            this.CourseNumber = "G-" + (long)StoredProcedure.NextNumber(string.Format("Next{0}", this.GetType().Name));
        }
    }

    /// <summary>
    /// MasteryCourse Model - inherits Course class 
    /// </summary>
    public class MasteryCourse : Course
    {
        [Required]
        [Display(Name = "Maximum\nAttempts")]
        public int MaximumAttempts { get; set; }
        
        /// <summary>
        /// Override abstract method, sets next course number.
        /// </summary>
        public override void SetNextCourseNumber()
        {
            this.CourseNumber = "M-" + (long)StoredProcedure.NextNumber(string.Format("Next{0}", this.GetType().Name));
        }
    }

    /// <summary>
    /// AuditCourse Model - inherits Course class 
    /// </summary>
    /// 
    public class AuditCourse : Course
    {
        /// <summary>
        /// Override abstract method, sets next course number.
        /// </summary>
        public override void SetNextCourseNumber()
        {
            this.CourseNumber = "A-" + (long)StoredProcedure.NextNumber(string.Format("Next{0}", this.GetType().Name));
        }
    }

    /// <summary>   
    /// Registration Model - to represent the course table in the database.
    /// </summary>
    public class Registration
    {
        BITCollege_LSContext db = new BITCollege_LSContext();
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int RegistrationId { get; set; }

        [Required]
        [ForeignKey("Student")]
        public int StudentId { get; set; }

        [Required]
        [ForeignKey("Course")]
        public int CourseId { get; set; }

        [Display(Name = "Registration\nNumber")]
        public long RegistrationNumber { get; set; }

        [Required]
        [Display(Name = "Date")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime RegistrationDate { get; set; }

        [DisplayFormat(NullDisplayText = "Ungraded")]
        [Range(0, 1)]
        public double? Grade { get; set; }

        public string Notes { get; set; }

        //Navigation properties
        public virtual Student Student { get; set; }

        public virtual Course Course { get; set; }

        /// <summary>
        /// Updates next available number for the registration.
        /// </summary>
        public void SetNextRegistrationNumber()
        {
            this.RegistrationNumber = (long)StoredProcedure.NextNumber(string.Format("Next{0}", this.GetType().Name));
        }
    }

    /// <summary>
    /// NextUniqueNumber Model - abstract class NextUniqueNumber
    /// </summary>
    public abstract class NextUniqueNumber
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int NextUniqueNumberId { get; set; }

        [Required]
        public long NextAvailableNumber { get; set; }

        protected static BITCollege_LSContext db = new BITCollege_LSContext(); 
    }

    /// <summary>
    /// NextStuent Model - inherits NextUinqueNumber class
    /// </summary>
    public class NextStudent : NextUniqueNumber
    {
        private static NextStudent nextStudent;

        /// <summary>
        /// Private constructor to enforce the singleton pattern and 
        /// initialize the next available number to 20000000.
        /// </summary>
        private NextStudent() 
        {
            NextAvailableNumber = 20000000;
        }

        //// <summary>
        /// GetInstance() method to implement singleton pattern for 
        /// NextStudent to retrieve or create instance.
        /// </summary>
        /// <returns>nextStudent</returns>
        public static NextStudent GetInstance()
        {
            if (nextStudent == null)
            {
                nextStudent = db.NextStudents.SingleOrDefault();

                if (nextStudent == null)
                {
                    nextStudent = new NextStudent();
                    db.NextStudents.Add(nextStudent);
                    db.SaveChanges();
                }
            }
            return nextStudent;
        }
    }

    /// <summary>
    /// NextRegistration Model - inherits NextUinqueNumber class
    /// </summary>
    public class NextRegistration : NextUniqueNumber
    {
        private static NextRegistration nextRegistration;

        /// <summary>
        /// Private constructor to enforce the singleton pattern and 
        /// initialize the next available number to 700.
        /// </summary>
        private NextRegistration()
        {
            NextAvailableNumber = 700;
        }

        /// <summary>
        /// Gets an instance of the NextRegistration class, ensuring a singleton pattern.
        /// If an instance doesn't exist in the database, creates and persists a new instance with a next available number of 700.
        /// </summary>
        /// <returns>An instance of the NextRegistration class.</returns>
        public static NextRegistration GetInstance()
        {
            if(nextRegistration == null)
            {
                nextRegistration = db.NextRegistrations.SingleOrDefault();

                if(nextRegistration == null)
                {
                    nextRegistration = new NextRegistration();
                    db.NextRegistrations.Add(nextRegistration);
                    db.SaveChanges();
                }
            }
            return nextRegistration;
        }
    }

    /// <summary>
    /// NextGradedCourse Model - inherits NextUinqueNumber class
    /// </summary>
    public class NextGradedCourse : NextUniqueNumber
    {
        private static NextGradedCourse nextGradedCourse;

        /// <summary>
        /// Private constructor to enforce the singleton pattern and 
        /// initialize the next available number to 200000.
        /// </summary>
        private NextGradedCourse()
        {
            NextAvailableNumber = 200000;
        }

        /// <summary>
        /// Gets the instance of the NextGradedCourse singleton class. If an instance doesn't exist, 
        /// it creates one and initializes the next available number.
        /// </summary>
        /// <returns>The singleton instance of NextGradedCourse.</returns>
        public static NextGradedCourse GetInstance()
        {
            if(nextGradedCourse == null)
            {
                nextGradedCourse = db.NextGradedCourses.SingleOrDefault();

                if(nextGradedCourse == null)
                {
                    nextGradedCourse = new NextGradedCourse();
                    db.NextGradedCourses.Add(nextGradedCourse);
                    db.SaveChanges();
                }
            }
            return nextGradedCourse;
        }
    }

    /// <summary>
    /// NextAuditCourse Model - inherits NextUinqueNumber class
    /// </summary>
    public class NextAuditCourse : NextUniqueNumber
    {
        private static NextAuditCourse nextAuditCourse;

        /// <summary>
        /// Private constructor to enforce the singleton pattern and 
        /// initialize the next available number to 2000.
        /// </summary>
        private NextAuditCourse()
        {
            NextAvailableNumber = 2000;
        }

        /// <summary>
        /// Retrieves the singleton instance of NextAuditCourse, 
        /// creating a new instance if it doesn't exist.
        /// </summary>
        /// <returns>The NextAuditCourse singleton instance.</returns>
        public static NextAuditCourse GetInstance()
        {
            if(nextAuditCourse == null)
            {
                nextAuditCourse = db.NextAuditCourses.SingleOrDefault();

                if(nextAuditCourse == null)
                {
                    nextAuditCourse = new NextAuditCourse();
                    db.NextAuditCourses.Add(nextAuditCourse);
                    db.SaveChanges();
                }
            }
            return nextAuditCourse;
        }
    }

    /// <summary>
    /// NextMasteryCourse Model - inherits NextUinqueNumber class
    /// </summary>
    public class NextMasteryCourse : NextUniqueNumber
    {
        private static NextMasteryCourse nextMasteryCourse;

        /// <summary>
        /// Private constructor to enforce the singleton pattern and 
        /// initialize the next available number to 20000.
        /// </summary>
        private NextMasteryCourse()
        {
            NextAvailableNumber = 20000;
        }

        /// <summary>
        /// Retrieves the singleton instance of NextMasteryCourse, 
        /// creating a new instance if it doesn't exist.
        /// </summary>
        /// <returns>The NextMasteryCourse singleton instance.</returns>
        public static NextMasteryCourse GetInstance()
        {
            if(nextMasteryCourse == null)
            {
                nextMasteryCourse = db.NextMasteryCourses.SingleOrDefault();

                if(nextMasteryCourse == null)
                {
                    nextMasteryCourse = new NextMasteryCourse();
                    db.NextMasteryCourses.Add(nextMasteryCourse);
                    db.SaveChanges();
                }
            }
            return nextMasteryCourse;
        }
    }
}
    