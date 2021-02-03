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
    /// Interaction logic for Picked_Student.xaml
    /// </summary>
    public partial class PopUp : Window {
        public static bool PickAgain = false;
        Student _ThisStudent = null;
        //private bool _AltDown = false;

        public PopUp(string txtToShow, bool isTopMost) {
            this.Topmost = isTopMost;
            InitializeComponent();
            tbName.Text = txtToShow;
            cdNotHere.Width = new GridLength(0);
            cdPickAgain.Width = new GridLength(0);
            btnNotHere.Visibility = Visibility.Collapsed;
            pthPositive.Visibility = Visibility.Collapsed;
            btnOKAgain.Visibility = Visibility.Collapsed;
            SetBindings();
        }

        public PopUp(Student stud, bool isTopMost, bool showFullNames) {
            this.Topmost = isTopMost;
            InitializeComponent();
            _ThisStudent = stud;
            tbName.Text = stud.FirstName + " ";
            if (showFullNames) {
                tbName.Text += stud.LastName;
            } else {
                tbName.Text += stud.LastName[0].ToString();
            }
           // tbName.Text = stud.FullName;
            SetBindings();
        }

        public void SetBindings() {

            //this.KeyDown += PopUp_KeyDown;
            this.KeyUp += PopUp_KeyUp;
        }

        private void PopUp_KeyUp(object sender, KeyEventArgs e) {
           // if (_AltDown) {
             //   if (e.Key == Key.LeftAlt || e.Key == Key.RightAlt || e.Key == Key.System) {
                  //  _AltDown = false;
                //} else
        if (e.Key == Key.A) {
                    btnOKAgain_Click(null, null);
                } else if (e.Key == Key.O) {
                    btnOK_Click(null, null);
                } else if (e.Key == Key.N) {
                    btnNotHere_Click(null, null);
                }
            //}
         //   MessageBox.Show(e.Key.ToString());
           // ShowUnderlines();
        }

        //private void PopUp_KeyDown(object sender, KeyEventArgs e) {
        //    if (e.Key == Key.LeftAlt || e.Key == Key.RightAlt || e.Key == Key.System) {
        //        _AltDown = true;
        //    }

        //    ShowUnderlines();
        //}

        //private void ShowUnderlines() {
        //    if (_AltDown) {
        //        runAnother.TextDecorations.Add(new TextDecoration());
        //        runNotHere.TextDecorations.Add(new TextDecoration());
        //        runOK.TextDecorations.Add(new TextDecoration());
        //    } else {
        //        runAnother.TextDecorations.Clear();
        //        runNotHere.TextDecorations.Clear();
        //        runOK.TextDecorations.Clear();
        //    }
        //    UpdateLayout();
        //}

        private void btnOK_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }
        private void btnOKAgain_Click(object sender, RoutedEventArgs e) {
            PopUp.PickAgain = true;
            this.Close();
        }

        private void btnNotHere_Click(object sender, RoutedEventArgs e) {
            if (_ThisStudent != null) {
                _ThisStudent.InClass = false;
            }
            PopUp.PickAgain = true;
            this.Close();

        }
    }
}
