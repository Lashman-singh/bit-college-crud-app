
using BITCollege_LS.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BITCollegeWindows
{
    public partial class History : Form
    {
        // Data Context Object
        BITCollege_LSContext db = new BITCollege_LSContext();

        ///given:  student and registration data will passed throughout 
        ///application. This object will be used to store the current
        ///student and selected registration
        ConstructorData constructorData;

        /// <summary>
        /// given:  This constructor will be used when called from the
        /// Student form.  This constructor will receive 
        /// specific information about the student and registration
        /// further code required:  
        /// </summary>
        /// <param name="constructorData">constructorData object containing
        /// specific student and registration data.</param>
        public History(ConstructorData constructorData)
        {
            InitializeComponent();

            //gets the data from student data using binding source💡
            this.constructorData = constructorData;
            studentBindingSource.DataSource = constructorData.StudentData;
            registrationBindingSource.DataSource = constructorData.RegistrationData;
        }


        /// <summary>
        /// given: This code will navigate back to the Student form with
        /// the specific student and registration data that launched
        /// this form.
        /// </summary>
        private void lnkReturn_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //return to student with the data selected for this form
            StudentData student = new StudentData(constructorData);
            student.MdiParent = this.MdiParent;
            student.Show();
            this.Close();
        }

        /// <summary>
        /// given:  Open this form in top right corner of the frame.
        /// further code required:
        /// </summary>
        private void History_Load(object sender, EventArgs e)
        {
            try
            {
                registrationBindingSource.DataSource = (from registration in db.Registrations
                                                       join course in db.Courses
                                                       on registration.CourseId equals course.CourseId
                                                       where registration.StudentId == constructorData.StudentData.StudentId
                                                       select new
                                                       {
                                                           registration.RegistrationNumber,
                                                           registration.RegistrationDate,
                                                           registration.Grade,
                                                           registration.Notes,
                                                           course.Title
                                                       }).ToList();
                registrationDataGridView.DataSource = registrationBindingSource;
            }
            catch (Exception ex) 
            {
                MessageBox.Show($"{ex.Message}","An error occurred", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
            this.Location = new Point(0, 0);
        }
    }
}
