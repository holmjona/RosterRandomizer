using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Windows;

namespace RosterRandomizer {
   public class Student:UIElement {
        private string _FirstName;
        private string _LastName;
        private string _Email;
        private List<string> _Tags;

        public Student() { }

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
