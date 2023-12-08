using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WePeEf.Model
{
    public class Person : INotifyPropertyChanged
    {
        
        private string _fisrtname;
        public string FirstName
        {
            get
            {
                return _fisrtname;
            }
            set
            {
                _fisrtname = value;
                OnPropertyRaised("FirstName");
                OnPropertyRaised("FullName");
            }
        }
        private string _lastname;
        public string LastName
        {
            get
            {
                return _lastname;
            }
            set
            {
                _lastname = value;
                OnPropertyRaised("LastName");
                OnPropertyRaised("FullName");
            }
        }
        private string _fullname;
        public string FullName
        {
            get
            {
                return _fisrtname + " " + _lastname; ;
            }
            set
            {
                _fullname = value;
                OnPropertyRaised("FullName");
            }
        }
        public Person()
        {
            _fisrtname = "Nirav";
            _lastname = "Daraniya";
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyRaised(string propertyname)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
            }
        }
    }
}
