using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace BITCollegeService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ICollegeRegistration" in both code and config file together.
    [ServiceContract]
    public interface ICollegeRegistration
    {
        [OperationContract]
        void DoWork();

        /// <summary>
        /// Drops a course registration based on the provided registration ID.
        /// </summary>
        /// <param name="registrationId"></param>
        /// <returns></returns>
        [OperationContract]
        bool DropCourse(int registrationId);

        /// <summary>
        /// Registers a course for a student with the given student ID and course ID, along with 
        /// optional notes. Returns an integer indicating the result of the registration attempt.
        /// </summary>
        /// <param name="studentId"></param>
        /// <param name="courseId"></param>
        /// <param name="notes"></param>
        /// <returns></returns>
        [OperationContract]
        int RegisterCourse(int studentId, int courseId, String notes);

        /// <summary>
        /// Updates the grade for a course registration and returns the calculated grade point 
        /// average if successful.
        /// </summary>
        /// <param name="grade"></param>
        /// <param name="registrationId"></param>
        /// <param name="notes"></param>
        /// <returns></returns>
        [OperationContract]
        double? UpdateGrade(double grade, int registrationId, String notes);
    }
}
