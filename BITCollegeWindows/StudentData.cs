using BITCollege_LS.Controllers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using BITCollege_LS.Models;
using BITCollege_LS.Data;

namespace BITCollegeWindows
{
    public partial class StudentData : Form
    {
        // Data Context Object
        BITCollege_LSContext db = new BITCollege_LSContext();

        ///Given: Student and Registration data will be retrieved
        ///in this form and passed throughout application
        ///These variables will be used to store the current
        ///Student and selected Registration
        ConstructorData constructorData = new ConstructorData();

        /// <summary>
        /// This constructor will be used when this form is opened from
        /// the MDI Frame.
        /// </summary>
        public StudentData()
        {
            InitializeComponent();
        }

        /// <summary>
        /// given:  This constructor will be used when returning to StudentData
        /// from another form.  This constructor will pass back
        /// specific information about the student and registration
        /// based on activites taking place in another form.
        /// </summary>
        /// <param name="constructorData">constructorData object containing
        /// specific student and registration data.</param>
        public StudentData (ConstructorData constructor)
        {
            InitializeComponent();
            //Further code to be added.
            this.constructorData = constructor;
            this.studentNumberMaskedTextBox.Text = constructor.StudentData.StudentNumber.ToString().Trim();
            studentNumberMaskedTextBox_Leave(null, null);

            if (constructorData.RegistrationData != null)
                this.registrationNumberComboBox.Text = constructorData.RegistrationData.RegistrationNumber.ToString().Trim();
        }

        /// <summary>
        /// given: Open grading form passing constructor data.
        /// </summary>
        private void lnkUpdateGrade_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            PopulateConstructorData();
            Grading grading = new Grading(constructorData);
            grading.MdiParent = this.MdiParent;
            grading.Show();
            this.Close();
        }


        /// <summary>
        /// given: Open history form passing constructor data.
        /// </summary>
        private void lnkViewDetails_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            PopulateConstructorData();
            History history = new History(constructorData);
            history.MdiParent = this.MdiParent;
            history.Show();
            this.Close();
        }

        /// <summary>
        /// given:  Opens the form in top right corner of the frame.
        /// </summary>
        private void StudentData_Load(object sender, EventArgs e)
        {
            //keeps location of form static when opened and closed
            this.Location = new Point(0, 0);
        }

        /// <summary>
        /// Retrieves the information about student on leaving the student number mask text box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void studentNumberMaskedTextBox_Leave(object sender, EventArgs e)
        {
            if (!this.studentNumberMaskedTextBox.MaskCompleted)
            {
                // Setting the focus to Masked text box
                this.studentNumberMaskedTextBox.Focus();
            }
            else
            {
                // Parsing the studentNumberMaskedTextBox.Text into long to convert it (as it is string)
                long.TryParse(this.studentNumberMaskedTextBox.Text, out long studentNumber);
                constructorData.StudentData = db.Students.Where(x => x.StudentNumber == studentNumber).SingleOrDefault();

                if (constructorData.StudentData == null)
                {
                    // Disabling the links
                    this.lnkUpdateGrade.Enabled = false;
                    this.lnkViewDetails.Enabled = false;

                    this.studentNumberMaskedTextBox.Focus();
                    studentBindingSource.DataSource = typeof(Student);
                    registrationBindingSource.DataSource = typeof(Registration);
                    MessageBox.Show($"Student {studentNumber} does not exist.", "Invalid Student Number", MessageBoxButtons.OK);
                }
                else
                {
                    studentBindingSource.DataSource = constructorData.StudentData;
                    IQueryable<Registration> registrations = db.Registrations.Where(x => x.StudentId == constructorData.StudentData.StudentId);

                    if (registrations == null)
                    {
                        this.lnkUpdateGrade.Enabled = false;
                        this.lnkViewDetails.Enabled = false;
                        registrationBindingSource.DataSource = typeof(Registration);
                    }
                    else
                    {
                        PopulateConstructorData();
                        registrationBindingSource.DataSource = registrations.ToList();
                        this.lnkUpdateGrade.Enabled = true;
                        this.lnkViewDetails.Enabled = true;
                    }
                }
            }
        }

       /// <summary>
       /// Populates the constructor data using the binding sources.
       /// </summary>
       private void PopulateConstructorData()
       {
            if (registrationBindingSource.Current != null)
                this.constructorData.RegistrationData = (Registration)registrationBindingSource.Current;
       }
    }
}
