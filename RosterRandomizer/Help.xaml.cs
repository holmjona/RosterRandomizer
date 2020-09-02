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
    /// Interaction logic for Help.xaml
    /// </summary>
    public partial class Help : Window {
        public enum Tabs {
            WhereToGet,
            WhatFormat
        }
        public Help(Tabs tb) {
            InitializeComponent();
            tbcHelp.SelectedIndex = (int)tb;
        }
    }
}
