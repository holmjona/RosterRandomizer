using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RosterRandomizer {
    /// <summary>
    /// Interaction logic for AddStudent.xaml
    /// </summary>
    public partial class StudentEntryForm : Window {
        public bool AddSuccessful = false;
        Student studentToEdit = null;
        public StudentEntryForm() {
            InitializeComponent();
            tbIdentifierError.Visibility = Visibility.Hidden;
            
            SetTabOrder(txtFirstName, txtLastName, txtIdentifier, btnCommit);

        }

        public StudentEntryForm(Student stud) : this() {
            studentToEdit = stud;
            lblTitle.Content = "Edit " + stud.FullName;
            btnCommit.Content = "Save Changes";
            txtFirstName.Text = studentToEdit.FirstName;
            txtLastName.Text = studentToEdit.LastName;
            txtIdentifier.Text = studentToEdit.Email;
        }

        private void SetTabOrder(params Control[] elements) {
            int tabOrder = 0;
            foreach(Control ele in elements) {
                ele.TabIndex = tabOrder++;
            }
            elements[0].Focus();
        }

        private void txtIdentifier_KeyUp(object sender, KeyEventArgs e) {
            string txtToCheck = ((TextBox)sender).Text.ToLower();
            bool isValid = true;
            foreach (string key in DataStore.Students.Keys) {
                if (key == txtToCheck) {
                    isValid = false;
                    break;
                }
            }
            if (!isValid) {
                txtIdentifier.Style = App.Current.Resources["styTextBoxError"] as Style;
                tbIdentifierError.Visibility = Visibility.Visible;
                btnCommit.IsEnabled = false;
            } else {
                txtIdentifier.Style = null;
                tbIdentifierError.Visibility = Visibility.Hidden;
                btnCommit.IsEnabled = true;
            }

        }

        private void btnCommit_Click(object sender, RoutedEventArgs e) {
            Student thisStudent;
            bool editingExisting = studentToEdit != null;
            string oldEmail = "";
            bool addStudentToList = true;
            if (editingExisting) {
                thisStudent = studentToEdit;
                oldEmail = thisStudent.Email;
            } else {
                thisStudent = new Student();
            }
            thisStudent.FirstName = txtFirstName.Text;
            thisStudent.LastName = txtLastName.Text;
            thisStudent.Email = txtIdentifier.Text;

            if (editingExisting) {
                bool emailChanged = oldEmail != thisStudent.Email;
                if (emailChanged) {
                    DataStore.Students.Remove(oldEmail);
                    addStudentToList = true;
                }
            }
            if (addStudentToList) {
                DataStore.Students.AddUnique(thisStudent.Email, thisStudent);
                AddSuccessful = true;
            }

            this.Close();
        }
    }
}
