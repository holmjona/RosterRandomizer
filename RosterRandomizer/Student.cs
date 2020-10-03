using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Controls;

namespace RosterRandomizer {
    public class Student {
        private int _ID;
        private string _FirstName;
        private string _LastName;
        private string _Email;
        // null flag allows us to know if the list has been exported yet.
        // null means first import, true or false means has been exported and reloaded.
        private bool? _InClass = null;
        private bool _IsSelected = false;
        private List<string> _Tags;

        public Student() { }

        /// <summary>
        /// ID to uniquely identify student in the system.
        /// </summary>
        public int ID {
            get { return _ID; }
            set { _ID = value; }
        }


        [JsonPropertyName("firstname")]
        public string FirstName {
            get { return _FirstName; }
            set { _FirstName = value; }
        }

        [JsonPropertyName("lastname")]
        public string LastName {
            get { return _LastName; }
            set { _LastName = value; }
        }

        [JsonPropertyName("email")]
        public string Email {
            get { return _Email; }
            set { _Email = value; }
        }

        [JsonPropertyName("tags")]
        public List<string> Tags {
            get { return _Tags; }
            set { _Tags = value; }
        }

        [JsonPropertyName("inclass")]
        public bool InClass {
            get { return _InClass != false; }
            set { _InClass = value; }
        }

        public bool IsFromExported {
            get { return _InClass != null; }

        }

        public bool IsSelected {
            get { return _IsSelected; }
            set { _IsSelected = value; }
        }


        public string FullName {
            get {
                return FirstName + " " + LastName;
            }
        }

        public override string ToString() {
            return FullName;
        }
    }
}
