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
        public PopUp(string txtToShow) {
            InitializeComponent();
            tbName.Text = txtToShow;
        }

        private void btnOK_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }
    }
}
