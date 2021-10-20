using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace RosterRandomizer {
    /// <summary>
    /// Interaction logic for Attendance.xaml
    /// </summary>
    public partial class Attendance : Window {
        public Attendance() {
            InitializeComponent();
        }

        Dictionary<string, List<Student>> _DictResults;
        private void btnPickFolder_Click(object sender, RoutedEventArgs e) {
            _DictResults = new Dictionary<string, List<Student>>();
            CommonOpenFileDialog ofd = new CommonOpenFileDialog();
            ofd.IsFolderPicker = true;
            ofd.ShowDialog();
            string[] fNames = Directory.GetFiles(ofd.FileName);
            foreach (string fName in fNames) {
                string ext = Path.GetExtension(fName);
                if (ext == ".json") {
                    try {
                        String lines = File.ReadAllText(fName);
                        List<Student> students = DataStore.ParseStudents(lines);
                        _DictResults.Add(fName, students);
                    } catch {

                    }
                    //TextBlock tb = new TextBlock();
                    //tb.Text = fName + " " + _DictResults[fName].Count.ToString();
                    //stkOutput.Children.Add(tb);
                }

            }
            
            ShowResults(_DictResults);

        }

        private void ShowResults(Dictionary<string, List<Student>> dictResults) {
            SortedDictionary<String, List<Student>> attend = new SortedDictionary<string, List<Student>>();
            // Populate way to get every student for every day of class.
            foreach (KeyValuePair<String, List<Student>> kvp in dictResults) {
                foreach (Student stud in kvp.Value) {
                    if (!attend.ContainsKey(stud.Email)) {
                        attend.Add(stud.Email, new List<Student>());
                    }
                    attend[stud.Email].Add(stud);
                }
            }

            SortedDictionary<string, string> sortDict = new SortedDictionary<string, string>();
            foreach (KeyValuePair<String, List<Student>> kvp in attend) {
                String stuName = kvp.Value[0].FullName;
                int inClass = 0;
                double totClass = (double)kvp.Value.Count;
                foreach(Student stuAtt in kvp.Value) {
                    if (stuAtt.InClass) {
                        inClass++;
                    }
                }
                double avg = inClass / totClass;
                sortDict.Add(kvp.Value[0].LastName.PadRight(40) + kvp.Value[0].FirstName, 
                    String.Format("{0} - {1}", stuName , avg.ToString("P2")));
            }

            foreach (KeyValuePair<string, string> kvp in sortDict) {
                TextBlock tb = new TextBlock();
                tb.Text = kvp.Value;
                stkOutput.Children.Add(tb);
            }

        }
    }
}
