using Microsoft.Win32;
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
        public MainWindow() {
            InitializeComponent();
        }

        private void btnLoadRoster_Click(object sender, RoutedEventArgs e) {
            OpenFileDialog ofd = new OpenFileDialog();

            if (ofd.ShowDialog() == true) {
                String lines = File.ReadAllText(ofd.FileName);
                // Moodles puts the list in a list. Weird I know.
                List<List<Student>> students = JsonSerializer.Deserialize<List<List<Student>>>(lines);
                foreach (Student stud in students[0]) {
                    DataStore.Students.AddUnique(stud.Email, stud);
                }
                FillWrapPanel();
            } else {
                ShowPopUp("Oops, problem getting file.");
            }
        }

        private void FillWrapPanel() {
            wpStudents.Children.Clear();
            int studentNumber = 0;
            foreach (Student stud in DataStore.Students.Values) {
                MakeStudentGrid(studentNumber, stud);
                studentNumber++;
            }
        }

        private void MakeStudentGrid(int studentNumber, Student stud) {
            Grid grd = new Grid();
            grd.Name = "Grid_" + studentNumber;
            grd.MouseUp += StudentGrid_Checked;
            // Name row
            grd.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(8, GridUnitType.Star) });
            // Tags row
            grd.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(2, GridUnitType.Star) });
            grd.Style = App.Current.Resources["styGridStudent"] as Style;
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
            chk.Name = "Check_" + studentNumber;
            chk.Checked += Student_Checked;
            chk.Tag = grd;
            vbCheck.Child = chk;
            grd.Children.Add(vbCheck);
            Grid.SetRow(vbCheck, 1);
            DataStore.StudentCheckBoxes.AddUnique(stud.Email, chk);
            wpStudents.Children.Add(grd);

            // Student Number 
            TextBlock tbNumber = new TextBlock();
            tbNumber.Text = studentNumber.ToString();
            tbNumber.Style = App.Current.Resources["styStudentNumber"] as Style;

            grd.Children.Add(tbNumber);

        }

        private void StudentGrid_Checked(object sender, MouseButtonEventArgs e) {
            Student_Checked(sender, null);
        }

        private void Student_Checked(object sender, RoutedEventArgs e) {
            CheckBox chk = null;
            Grid g = null;
            if (sender.GetType() == typeof(CheckBox)) {
                chk = (CheckBox)sender;
                g = (Grid)chk.Tag;
            } else if (sender.GetType() == typeof(Grid)) {
                g = (Grid)sender;
                string gNum = g.Name.Split("_")[1];
                chk = (CheckBox)LogicalTreeHelper.FindLogicalNode(g, "Check_" + gNum);
                chk.IsChecked = !chk.IsChecked;
            }

            if (chk.IsChecked == true) {
                g.Style = App.Current.Resources["styGridStudentChecked"] as Style;
            } else {
                g.Style = App.Current.Resources["styGridStudent"] as Style;
            }
        }

        private void btnPickRandom_Click(object sender, RoutedEventArgs e) {
            if (DataStore.Students.Count > 0) {
                string key = GetUnusedKey(DataStore.Students, DataStore.StudentCheckBoxes);
                if (key == "NONE") {
                    ShowPopUp("All students picked, resetting list.");
                    foreach (CheckBox chk in DataStore.StudentCheckBoxes.Values) {
                        chk.IsChecked = false;
                        string cNum = chk.Name.Split("_")[1];
                        Grid parentGrid = (Grid)LogicalTreeHelper.FindLogicalNode(wpStudents, "Grid_" + cNum);
                        parentGrid.Style = App.Current.Resources["styGridStudent"] as Style;
                    }
                    key = GetUnusedKey(DataStore.Students, DataStore.StudentCheckBoxes);
                }
                Student studFound = DataStore.Students[key];
                ShowPopUp(studFound.FullName);
                CheckBox chkFound = DataStore.StudentCheckBoxes[key];
                chkFound.IsChecked = true;
                Student_Checked(chkFound, null);
            } else {
                ShowPopUp("There are no students in the system.");
            }
        }

        private void ShowPopUp(String text) {
            PopUp frm = new PopUp(text);
            frm.Owner = this;
            frm.ShowDialog();
        }

        private string GetUnusedKey(Dictionary<string, Student> students, Dictionary<string, CheckBox> studentsChecked) {
            Random rnd = new Random();
            string key = "";
            if (students.Count == studentsChecked.Values.Count(c => c.IsChecked == true)) {
                // no keys left.
                key = "NONE";
            } else {
                do {
                    int studIndex = rnd.Next(students.Count);
                    key = students.GetKeyAtIndex(studIndex);

                } while (studentsChecked[key].IsChecked == true);
            }
            return key;
        }

        private void btnRemoveSelected_Click(object sender, RoutedEventArgs e) {
            List<string> keysToRemove = new List<string>();
            foreach (KeyValuePair<string, CheckBox> kvp in DataStore.StudentCheckBoxes) {
                CheckBox myCheck = kvp.Value;
                if (myCheck.IsChecked == true) {
                    string cNum = myCheck.Name.Split("_")[1];
                    keysToRemove.Add(kvp.Key);
                    Grid myGrid = (Grid)LogicalTreeHelper.FindLogicalNode(wpStudents, "Grid_" + cNum);
                    wpStudents.Children.Remove(myGrid);
                }
            }
            foreach (string key in keysToRemove) {
                DataStore.Students.Remove(key);
                DataStore.StudentCheckBoxes.Remove(key);
            }
        }

        private void btnAddStudent_Click(object sender, RoutedEventArgs e) {
            AddStudent frm = new AddStudent();
            frm.ShowDialog();
            if (frm.AddSuccessful) {
                // added student
                Student lastStudentAdded = DataStore.Students.Values.Last();
                int newID = DataStore.Students.Count();
                MakeStudentGrid(newID, lastStudentAdded);
            }

        }
    }
}
