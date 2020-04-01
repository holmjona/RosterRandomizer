using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace RosterRandomizer {
    /// <summary>
    /// Keep data to be stored between forms.
    /// </summary>
    public static class DataStore {
        private static Dictionary<string, Student> _Students;
        private static Dictionary<string, CheckBox> _StudentsCheckBoxes;

        public static Dictionary<string, Student> Students {
            get { 
                if (_Students == null) {
                    _Students = new Dictionary<string, Student>();
                }
                return _Students;
            }
        }

        public static Dictionary<string, CheckBox> StudentCheckBoxes {
            get {
                if (_StudentsCheckBoxes == null) {
                    _StudentsCheckBoxes = new Dictionary<string, CheckBox>();
                }
                return _StudentsCheckBoxes;
            }
        }

        public static void Clear() {
            _Students = null;
            _StudentsCheckBoxes = null;
        }
    }
}
