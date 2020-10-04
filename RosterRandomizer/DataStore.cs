using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Windows.Controls;

namespace RosterRandomizer {
    /// <summary>
    /// Keep data to be stored between forms.
    /// </summary>
    public static class DataStore {
        private static Dictionary<string, Student> _Students;
        public static readonly Dictionary<string, SemaphoreSlim> Semaphores = new Dictionary<string, SemaphoreSlim>();

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
            if (!int.TryParse(studNumber, out intNumber)) {
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

                } while (_Students[key].IsSelected == true || !_Students[key].InClass);
            }
            return key;
        }

        /// <summary>
        /// Count the number of nested arrays.
        /// Mostly needed if this file is from Moodle.
        /// </summary>
        /// <param name="startOfFile">start of the file string</param>
        /// <returns>count of opening array bracke</returns>
        public static int CountArrays(string startOfFile) {
            int countOfOpens = 0;
            foreach (char c in startOfFile) {
                if (c == '[') countOfOpens++;
            }
            return countOfOpens;
        }


        public static List<Student> ParseStudents(string textToParse) {
            List<Student> students = null;
            bool isMoodleFile = false;
            string startOfFile = textToParse.Substring(0, textToParse.IndexOf('{') + 1);
            int countOfLists = CountArrays(startOfFile);
            if (countOfLists == 2) {
                // must be Moodle
                // Moodles puts the list in a list. Weird I know.
                List<List<Student>> temp = JsonSerializer.Deserialize<List<List<Student>>>(textToParse);
                students = temp[0];
            } else { // assume one list
                students = JsonSerializer.Deserialize<List<Student>>(textToParse);
            }
            return students;
        }

        public static string ConvertToJSON(List<Student> students) {
            return JsonSerializer.Serialize<List<Student>>(students);
        }
    }
}
