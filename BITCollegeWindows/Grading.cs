
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
using Utility;

namespace BITCollegeWindows
{
    public partial class Grading : Form
    {
        //Db context for project

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
        public Grading(ConstructorData constructor)
        {
            InitializeComponent();
            this.constructorData = constructor;
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
        /// given:  Always open in this form in the top right corner of the frame.
        /// further code required:
        /// </summary>
        private void Grading_Load(object sender, EventArgs e)
        { 
            if (gradeTextBox.Text.Trim().Length > 0)
            {
                this.gradeTextBox.Enabled = false;
                this.lblExisting.Visible = true;
                this.lnkUpdate.Enabled = false;
            }
            else
            {
                this.gradeTextBox.Enabled = true;
                this.lblExisting.Visible = false;
                this.lnkUpdate.Enabled = true;
            }

            this.Location = new Point(0, 0);
        }

        /// <summary>
        /// Handles the logic for updating a student grade
        /// </summary>
        private void lnkUpdate_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string gradeValue = Numeric.ClearFormatting(this.gradeTextBox.Text, "%");
            if (!Numeric.IsNumeric(gradeValue, System.Globalization.NumberStyles.Number))
            {
                MessageBox.Show( "Enter the valid input : Numbers",
                                "Inappropriate Input for Grades", MessageBoxButtons.OK, 
                                MessageBoxIcon.Error);
            }
            else
            {
                
                double gradeNumeric;

                if(Double.TryParse(gradeValue, out gradeNumeric))
                {
                    double grade = gradeNumeric /= 100.0;
                    if (grade >= 0 && grade <= 1)
                    {
                        CollegeService.CollegeRegistrationClient service = new CollegeService.CollegeRegistrationClient();
                        string notes = "";
                        service.UpdateGrade(gradeNumeric, constructorData.RegistrationData.RegistrationId, notes);
                        this.gradeTextBox.Enabled = false;
                    }
                    else
                    {
                        MessageBox.Show("Enter a grade ranging from 0 to 100",
                                        "Invalid Grade", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Please remove any special characters such as '/', '%', or '\"'.",
                                     "Input Contains Special Characters", 
                                     MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
