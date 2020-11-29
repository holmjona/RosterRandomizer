﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RosterRandomizer {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {


        private static List<CheckBox> _StudentCheckBoxes = new List<CheckBox>();
        private string _JSONFileFilter = "JSON File|*.json;*.js|All Files|*.*";
        private double _BoxSize = 100;

        public MainWindow() {
            InitializeComponent();
            tbShowingOnTop.Visibility = Visibility.Hidden;
            // fix for odd menu behavior is likely found here:
            //https://stackoverflow.com/questions/4630954/wpf-menu-displays-to-the-left-of-the-window
        }
        /// <summary>
        /// Load Roster from file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLoadRoster_Click(object sender, RoutedEventArgs e) {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = _JSONFileFilter;
            if (ofd.ShowDialog() == true) {
                String lines = File.ReadAllText(ofd.FileName);
                List<Student> students = DataStore.ParseStudents(lines);
                foreach (Student stud in students) {
                    DataStore.Students.AddUnique(stud.Email, stud);
                }
                FillWrapPanel();
            } else {
                ShowPopUp("Oops, problem getting file.");
            }
        }

        /// <summary>
        /// Fills the Wrap Panel with students.
        /// </summary>
        private void FillWrapPanel() {
            wpStudents.Children.Clear();
            int studentNumber = 0;
            foreach (Student stud in DataStore.Students.Values) {
                stud.ID = studentNumber;
                MakeStudentGrid(stud);
                studentNumber++;
            }
            foreach (Student stud in DataStore.Students.Values) {
                UpdateStudentGridStyle(stud);
            }

        }

        /// <summary>
        /// Make the student card.
        /// </summary>
        /// <param name="stud">Student to populate card about.</param>
        private void MakeStudentGrid(Student stud) {
            Grid grd = new Grid();
            grd.Width = _BoxSize;
            grd.Height = _BoxSize;
            grd.Name = "Grid_" + stud.ID;
            grd.MouseUp += StudentGrid_Checked;
            // Name row
            grd.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(8, GridUnitType.Star) });
            // Tags row
            grd.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(2, GridUnitType.Star) });
            //grd.Style = App.Current.Resources["styGridStudent"] as Style;
            // Name
            Viewbox vbName = new Viewbox();
            vbName.Style = App.Current.Resources["styViewStudent"] as Style;
            TextBlock tbName = new TextBlock();
            tbName.Style = App.Current.Resources["styTextStudent"] as Style;
            tbName.Inlines.Add(new Run(stud.FirstName));
            tbName.Inlines.Add(new LineBreak());
            tbName.Inlines.Add(new Run(stud.LastName));
            vbName.Child = tbName;
            grd.Children.Add(vbName);
            // checkbox
            Viewbox vbCheck = new Viewbox();
            CheckBox chk = new CheckBox();
            chk.Name = "Check_" + stud.ID;
            // set state based on existing student state.
            chk.IsChecked = stud.IsSelected;
            chk.Checked += Student_Checked;
            chk.Tag = grd;
            vbCheck.Child = chk;
            grd.Children.Add(vbCheck);
            Grid.SetRow(vbCheck, 1);
            _StudentCheckBoxes.Add(chk);
            wpStudents.Children.Add(grd);

            // Student Number 
            TextBlock tbNumber = new TextBlock();
            tbNumber.Text = stud.ID.ToString();
            tbNumber.Style = App.Current.Resources["styStudentNumber"] as Style;

            // Reset button
            Button btnReset = new Button();
            btnReset.Name = "Reset_" + stud.ID;
            btnReset.Click += btnResetMe_Click;
            btnReset.Style = App.Current.Resources["styResetStudentButton"] as Style;
            grd.Children.Add(btnReset);

            Button btnEdit = new Button();
            btnEdit.Name = "Edit_" + stud.ID;
            btnEdit.Click += BtnEdit_Click;
            btnEdit.Style = App.Current.Resources["styEditStudentButton"] as Style;
            grd.Children.Add(btnEdit);

            grd.Children.Add(tbNumber);

        }



        private void StudentGrid_Checked(object sender, MouseButtonEventArgs e) {
            Student_Checked(sender, null);
        }

        /// <summary>
        /// React to the student card being checked or unchecked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Student_Checked(object sender, RoutedEventArgs e) {
            CheckBox chk = null;
            Grid g = null;
            string studNumber = ((FrameworkElement)sender).Name.Split("_")[1];
            // check to see if the checkbox or grid was clicked.
            if (sender.GetType() == typeof(CheckBox)) {
                chk = (CheckBox)sender;
                g = (Grid)chk.Tag;
            } else if (sender.GetType() == typeof(Grid)) {
                g = (Grid)sender;
                //string gNum = g.Name.Split("_")[1];
                chk = (CheckBox)LogicalTreeHelper.FindLogicalNode(g, "Check_" + studNumber);
                chk.IsChecked = !chk.IsChecked;
            }
            Student found = DataStore.GetStudent(studNumber);
            found.IsSelected = chk.IsChecked == true;
            UpdateStudentGridStyle(found);
        }

        /// <summary>
        /// Update teh style for a given student.
        /// </summary>
        /// <param name="id">The unique ID for the student.</param>
        private void UpdateStudentGridStyle(int id) {
            Student found = DataStore.GetStudent(id);
            UpdateStudentGridStyle(found);
        }

        /// <summary>
        /// Update the style for a given student
        /// </summary>
        /// <param name="stud">Student associated with card.</param>
        private void UpdateStudentGridStyle(Student stud) {
            Grid studGrid = (Grid)LogicalTreeHelper.FindLogicalNode(wpStudents, "Grid_" + stud.ID);
            // check student attributes to decide which style to apply.
            if (stud.InClass) {
                if (stud.IsSelected) {
                    studGrid.Style = App.Current.Resources["styGridStudentChecked"] as Style;
                } else {
                    studGrid.Style = App.Current.Resources["styGridStudent"] as Style;
                }
            } else {
                studGrid.Style = App.Current.Resources["styGridStudentAbsent"] as Style;
            }
        }

        /// <summary>
        /// Pick a random student that has not already been picked or is absent.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPickRandom_Click(object sender, RoutedEventArgs e) {
            if (DataStore.Students.Count > 0) {
                string key = DataStore.GetUnusedKey();
                // react based on next key
                if (key == "NONE") { // No students left to pick from.
                    ShowPopUp("All students picked, resetting list.");
                    // reset all students (not absent)
                    foreach (Student currStudent in DataStore.Students.Values) {
                        CheckBox chk = (CheckBox)LogicalTreeHelper.FindLogicalNode(wpStudents, "Check_" + currStudent.ID);
                        chk.IsChecked = currStudent.IsSelected = false;
                        UpdateStudentGridStyle(currStudent);
                    }
                    key = DataStore.GetUnusedKey();
                }
                if (key != "NONE") { // Found an available student.
                    Student studFound = DataStore.Students[key];
                    ShowPopUp(studFound); // display student to user.
                    CheckBox chkFound = (CheckBox)LogicalTreeHelper.FindLogicalNode(wpStudents, "Check_" + studFound.ID);
                    chkFound.IsChecked = true;
                    Student_Checked(chkFound, null);
                    UpdateStudentGridStyle(studFound);
                    //if (!studFound.InClass) {
                    if (PopUp.PickAgain) {
                        // pick another student
                        PopUp.PickAgain = false;
                        btnPickRandom_Click(sender, e);
                    }
                } else {
                    // should not hit this.
                    ShowPopUp("No students available to be selected.");
                }
            } else {
                ShowPopUp("There are no students in the system.");
            }
        }

        /// <summary>
        /// Show popup for text box
        /// </summary>
        /// <param name="txt"></param>
        private void ShowPopUp(string txt) {
            ShowPopUp((object)txt);
        }

        private void ShowPopUp(object obj) {
            PopUp frm;
            if (obj.GetType() == typeof(Student)) {
                frm = new PopUp((Student)obj, this.Topmost);
            } else {
                // assume string if not student.
                frm = new PopUp(obj.ToString(), this.Topmost);
            }
            frm.Owner = this;
            frm.ShowDialog();
        }

        private void btnRemoveSelected_Click(object sender, RoutedEventArgs e) {
            List<string> keysToRemove = new List<string>();
            List<CheckBox> chkToRemove = new List<CheckBox>();
            foreach (CheckBox myCheck in _StudentCheckBoxes) {
                if (myCheck.IsChecked == true) {
                    string cNum = myCheck.Name.Split("_")[1];
                    Student found = DataStore.GetStudent(cNum);
                    // what to remove 
                    keysToRemove.Add(found.Email);
                    chkToRemove.Add(myCheck);

                    Grid myGrid = (Grid)LogicalTreeHelper.FindLogicalNode(wpStudents, "Grid_" + cNum);
                    wpStudents.Children.Remove(myGrid);
                }
            }
            // remove students
            foreach (string key in keysToRemove) {
                DataStore.Students.Remove(key);
            }
            // remove checkboxes
            foreach (CheckBox chk in chkToRemove) {
                _StudentCheckBoxes.Remove(chk);
            }
        }

        private void btnAddStudent_Click(object sender, RoutedEventArgs e) {
            AddStudent frm = new AddStudent();
            frm.ShowDialog();
            if (frm.AddSuccessful) {
                // added student
                Student lastStudentAdded = DataStore.Students.Values.Last();
                int newID = DataStore.Students.Count();
                lastStudentAdded.ID = newID;
                MakeStudentGrid(lastStudentAdded);
            }

        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e) {
            Button btn = (Button)sender;
            string[] idParts = btn.Name.Split("_");
            int studentID = int.Parse(idParts[1]);
            Student studToEdit = DataStore.Students.Values
                .FirstOrDefault(s => s.ID == studentID);
            if (studToEdit != null) {
                AddStudent frm = new AddStudent(studToEdit);
                frm.ShowDialog();
                if (frm.AddSuccessful) {
                    // Student email changed
                }
                FillWrapPanel();
            } else {
                ShowPopUp("Could not find the student to edit.");
            }

        }

        private void btnExportStudents_Click(object sender, RoutedEventArgs e) {
            if (DataStore.Students.Count > 0) {
                SaveFileDialog sfd = new SaveFileDialog();
                List<Student> studentsToExport = new List<Student>();
                foreach (Student st in DataStore.Students.Values) {
                    studentsToExport.Add(st);
                }
                sfd.Filter = _JSONFileFilter;
                if (sfd.ShowDialog() == true) {
                    string jsonString = DataStore.ConvertToJSON(studentsToExport);
                    File.WriteAllText(sfd.FileName, jsonString);
                } else {
                    ShowPopUp("Oops, problem getting file.");
                }
            } else {
                ShowPopUp("There are no students in the system. There is nothing to export.");
            }
        }

        private void btnResetSelected_Click(object sender, RoutedEventArgs e) {

        }

        private void btnResetMe_Click(object sender, RoutedEventArgs e) {
            Button btn = (Button)sender;
            string bNum = btn.Name.Split("_")[1];
            CheckBox chk = _StudentCheckBoxes.First(ch => ch.Name == "Check_" + bNum);

            Student found = DataStore.GetStudent(bNum);
            found.InClass = true;
            chk.IsChecked = found.IsSelected = false;

            UpdateStudentGridStyle(found);
        }

        private void miOnTop_Checked(object sender, RoutedEventArgs e) {
            chkOnTop.IsChecked = miOnTop.IsChecked;

        }

        private void chkOnTop_Checked(object sender, RoutedEventArgs e) {

            if (chkOnTop.IsChecked == true) {
                this.Topmost = true;
                tbShowingOnTop.Visibility = Visibility.Visible;
                this.Title = this.Title + " (on top)";
            } else {
                this.Topmost = false;
                tbShowingOnTop.Visibility = Visibility.Hidden;
                this.Title = this.Title.Remove(this.Title.IndexOf(" (on top)"));
            }
            // make sure all checks match condition being set. 
            miOnTop.IsChecked = chkOnTop.IsChecked == true;

        }

        private void miExit_Click(object sender, RoutedEventArgs e) {
            Application.Current.Shutdown();
        }

        private void miWhereGetRoster_Click(object sender, RoutedEventArgs e) {
            Help frm = new Help(Help.Tabs.WhereToGet);
            frm.ShowDialog();
        }

        private void miWhatFileFormat_Click(object sender, RoutedEventArgs e) {
            Help frm = new Help(Help.Tabs.WhatFormat);
            frm.ShowDialog();
        }

        private void sldBoxSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            _BoxSize = sldBoxSize.Value;
            FillWrapPanel();
        }

        private void miAttendanceReport_Click(object sender, RoutedEventArgs e) {
            Attendance frm = new Attendance();
            frm.ShowDialog();
        }
    }
}
