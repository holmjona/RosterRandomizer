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
        public AddStudent() {
            InitializeComponent();
            tbEmailError.Visibility = Visibility.Hidden;
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
            Student newStudent = new Student();
            newStudent.FirstName = txtFirstName.Text;
            newStudent.LastName = txtLastName.Text;
            newStudent.Email = txtEmail.Text;

            DataStore.Students.AddUnique(newStudent.Email, newStudent);
            AddSuccessful = true;
            this.Close();
        }
    }
}
