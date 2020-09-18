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
    public partial class AddStudent : Window {
        public bool AddSuccessful = false;
        Student studentToEdit = null;
        public AddStudent() {
            InitializeComponent();
            tbEmailError.Visibility = Visibility.Hidden;
        }

        public AddStudent(Student stud):this(){
            studentToEdit = stud;
            lblTitle.Content = "Edit " + stud.FullName;
            btnAddStudent.Content = "Save Changes";
            txtFirstName.Text = studentToEdit.FirstName;
            txtLastName.Text = studentToEdit.LastName;
            txtEmail.Text = studentToEdit.Email;
        }

        private void txtEmail_KeyUp(object sender, KeyEventArgs e) {
            string txtToCheck = ((TextBox)sender).Text.ToLower();
            bool isValid = true;
            foreach(string key in DataStore.Students.Keys) {
                if (key == txtToCheck) {
                    isValid = false;
                    break;
                }
            }
            if (!isValid) {
                txtEmail.Style = App.Current.Resources["styTextBoxError"] as Style;
                tbEmailError.Visibility = Visibility.Visible;
                btnAddStudent.IsEnabled = false;
            } else {
                txtEmail.Style = null;
                tbEmailError.Visibility = Visibility.Hidden;
                btnAddStudent.IsEnabled = true;
            }

        }

        private void btnAddStudent_Click(object sender, RoutedEventArgs e) {
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
            thisStudent.Email = txtEmail.Text;
            
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
