using BITCollege_LS.Data;
using BITCollege_LS.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Utility;

namespace BITCollegeService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "CollegeRegistration" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select CollegeRegistration.svc or CollegeRegistration.svc.cs at the Solution Explorer and start debugging.
    public class CollegeRegistration : ICollegeRegistration
    {
        /// <summary>
        /// DB context instance
        /// </summary>
        BITCollege_LSContext db = new BITCollege_LSContext();

        public void DoWork()
        {
        }

        /// <summary>
        /// Removes a course registration from the database based on the provided registration ID.
        /// </summary>
        /// <param name="registrationId">The unique identifier of the registration to be dropped.</param>
        /// <returns>True if the course registration was successfully dropped, otherwise false.</returns>
        public bool DropCourse(int registrationId)
        {
            try
            {
                Registration query = db.Registrations.Find(registrationId);

                if (query != null)
                {
                    db.Registrations.Remove(query);
                    db.SaveChanges();
                    return true;
                }

                return false;

            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Registers a course for a student with the given student ID and course ID, along with optional notes.
        /// </summary>
        /// <param name="studentId">The unique identifier of the student.</param>
        /// <param name="courseId">The unique identifier of the course.</param>
        /// <param name="notes">Optional notes associated with the registration.</param>
        /// <returns>
        /// An integer indicating the result of the registration
        /// </returns>
        public int RegisterCourse(int studentId, int courseId, string notes)
        {
            try
            {
                IQueryable<Registration> allRegistrationQuery = db.Registrations
                    .Where(x => x.StudentId == studentId && x.CourseId == courseId);
                 
                IEnumerable<Registration> incompleteRegistrations = allRegistrationQuery
                    .Where(x => x.Grade == null);

                if (incompleteRegistrations.Any())
                    return -100;

                Student studentQuery = db.Students.Find(studentId);
                Course course = db.Courses.Find(courseId);

                if (course.CourseType == "Mastery")
                {
                    MasteryCourse masteryCourseQuery = db.Courses.OfType<MasteryCourse>()
                        .SingleOrDefault(x => x.CourseId == courseId);

                    if (masteryCourseQuery != null && allRegistrationQuery.Count() >= masteryCourseQuery.MaximumAttempts)
                        return -200;
                }

                Registration registration = new Registration
                {
                    StudentId = studentId,
                    CourseId = courseId,
                    Notes = notes,
                    RegistrationDate = DateTime.Now
                };

                registration.SetNextRegistrationNumber();
                db.Registrations.Add(registration);

                studentQuery.OutstandingFees += studentQuery.GradePointState.TuitionRateAdjustment(studentQuery) * course.TuitionAmount;

                db.SaveChanges();

                return 0;
            }
            catch (Exception)
            {
                return -300;
            }
        }

        /// <summary>
        /// Updates the grade and notes for a given registration ID.
        /// </summary>
        /// <param name="grade">The new grade to be assigned.</param>
        /// <param name="registrationId">The unique identifier of the registration.</param>
        /// <param name="notes">Optional notes associated with the grade update.</param>
        /// <returns>
        /// The calculated grade point average if the grade update was successful, otherwise null.
        /// </returns>
        public double? UpdateGrade(double grade, int registrationId, string notes)
        {
            try
            {
                Registration registration = db.Registrations.Find(registrationId);

                if (registration != null)
                {
                    registration.Grade = grade;
                    registration.Notes = notes;

                    db.SaveChanges();

                    double? calculatedGradePointAverage = CalculateGradePointAverage(registration.StudentId);

                    if (calculatedGradePointAverage == null)
                    {
                        Console.WriteLine("Calculated Grade Point Average is null after updating grade.");
                    }
                    else
                    {
                        Console.WriteLine($"Calculated Grade Point Average after updating grade: {calculatedGradePointAverage}");
                    }

                    return calculatedGradePointAverage;
                }
                Console.WriteLine("Registration not found.");
                return null;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Calculates the grade point average for a student based on completed registrations.
        /// </summary>
        /// <param name="studentId">The unique identifier of the student.</param>
        /// <returns>
        /// The calculated grade point average if successful, otherwise null.
        /// </returns>
        private double? CalculateGradePointAverage(int studentId)
        {
            try
            {
                IQueryable<Registration> registrations = db.Registrations
                    .Where(r => r.StudentId == studentId && r.Grade != null);

                double totalGradePointValue = 0;
                double totalCreditHours = 0;

                foreach (Registration registration in registrations.ToList())
                {
                    double grade = registration.Grade.Value;
                    CourseType courseType = Utility.BusinessRules.CourseTypeLookup(registration.Course.CourseType);

                    if (courseType != CourseType.AUDIT)
                    {
                        double gradePointValue = Utility.BusinessRules.GradeLookup(grade, courseType);

                        double courseCreditHours = registration.Course.CreditHours;
                        double registrationGradePointValue = gradePointValue * courseCreditHours;

                        totalGradePointValue += registrationGradePointValue;
                        totalCreditHours += courseCreditHours;
                    }
                }

                double? calculatedGradePointAverage = null;

                if (totalCreditHours != 0)
                {
                    calculatedGradePointAverage = totalGradePointValue / totalCreditHours;
                }

                Student student = db.Students.Find(studentId);
                student.GradePointAverage = calculatedGradePointAverage;

                db.SaveChanges();

                return calculatedGradePointAverage;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
