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
        public PopUp(string txtToShow,bool isTopMost) {
            this.Topmost = isTopMost;
            InitializeComponent();
            tbName.Text = txtToShow;
            cdNotHere.Width = new GridLength(0);
            cdPickAgain.Width = new GridLength(0);
            btnNotHere.Visibility = Visibility.Collapsed;
            pthPositive.Visibility = Visibility.Collapsed;
            btnOKAgain.Visibility = Visibility.Collapsed;
        }

        public PopUp(Student stud, bool isTopMost) {
            this.Topmost = isTopMost;
            InitializeComponent();
            _ThisStudent = stud;
            tbName.Text = stud.FullName;
        }

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
