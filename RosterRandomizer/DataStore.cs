using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace RosterRandomizer {
    /// <summary>
    /// Keep data to be stored between forms.
    /// </summary>
    public static class DataStore {
        private static Dictionary<string, Student> _Students;

        public static Dictionary<string, Student> Students {
            get { 
                if (_Students == null) {
                    _Students = new Dictionary<string, Student>();
                }
                return _Students;
            }
        }

        public static void Clear() {
            _Students = null;
        }

        internal static Student GetStudent(string studNumber) {
            int intNumber = -1;
            if (!int.TryParse(studNumber, out intNumber)){
                intNumber = -1;
            }
            return GetStudent(intNumber);
        }
        internal static Student GetStudent(int studNumber) {
            if (studNumber < 0) return null;
            return _Students.Values.FirstOrDefault(s => s.ID == studNumber);
        }

        internal static string GetUnusedKey() {
                Random rnd = new Random();
                string key = "";
                if (_Students.Count == _Students.Values.Count(s => s.IsSelected || !s.InClass)) {
                    // no keys left.
                    key = "NONE";
                } else {
                    do {
                        int studIndex = rnd.Next(_Students.Count);
                        key = _Students.GetKeyAtIndex(studIndex);

                    } while (_Students[key].IsSelected == true || ! _Students[key].InClass);
                }
                return key;
        }
    }
}
