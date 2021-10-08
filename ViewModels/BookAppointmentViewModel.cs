using SchedulingApp.Models;
using SchedulingApp.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace SchedulingApp.ViewModels
{
    public class BookAppointmentViewModel : ObservableObject
    {
        //private object _currentView;
        //public object CurrentView
        //{
        //    get { return _currentView; }
        //    set
        //    {
        //        _currentView = value;
        //        OnPropertyChanged();
        //    }
        //}

        private MainViewModel _MAIN_VIEW_MODEL;
        private List<Customer> _customers = DataAccess.SelectAllCustomers();
        private List<User> _users = DataAccess.SelectAllUsers();

        #region Form Properties
        private List<string> _userNames;
        public List<string> UserNames
        {
            get => _userNames;
            set
            {
                _userNames = value;
                OnPropertyChanged();
            }
        }

        private List<string> _customerNames;
        public List<string> CustomerNames
        {
            get => _customerNames;
            set
            {
                _customerNames = value;
                OnPropertyChanged();
            }
        }

        public List<string> AppointmentTypes { get; } = new List<string>() { AppointmentType.Scrum.ToString(), AppointmentType.Sales.ToString(), AppointmentType.Lunch.ToString() };

        private string _selectedCustomer;
        public string SelectedCustomer
        {
            get => _selectedCustomer;
            set
            {
                _selectedCustomer = value;
                OnPropertyChanged();
            }
        }

        private string _selectedAppointmentType;
        public string SelectedAppointmentType
        {
            get => _selectedAppointmentType;
            set
            {
                _selectedAppointmentType = value;
                OnPropertyChanged();
            }
        }

        private string _selectedUser;
        public string SelectedUser
        {
            get => _selectedUser;
            set
            {
                _selectedUser = value;
                OnPropertyChanged();
            }
        }

        private DateTime _selectedDate = DateTime.Today;
        public DateTime SelectedDate
        {
            get => _selectedDate;
            set
            {
                _selectedDate = value;
                OnPropertyChanged();

                _selectedDateTime = SelectedDate.Date.Add(DateTime.Parse(SelectedTime).TimeOfDay);
                Debug.WriteLine($"Selected DateTime is {_selectedDateTime}");
            }
        }

        public string[] Times { get; } = new string[]
        {
            "9:00 AM", "9:15 AM", "9:30 AM", "9:45 AM",
            "10:00 AM", "10:15 AM", "10:30 AM", "10:45 AM",
            "11:00 AM", "11:15 AM", "11:30 AM", "11:45 AM",
            "12:00 PM", "12:15 PM", "12:30 PM", "12:45 PM",
            "1:00 PM", "1:15 PM", "1:30 PM", "1:45 PM",
            "2:00 PM", "2:15 PM", "2:30 PM", "2:45 PM",
            "3:00 PM", "3:15 PM", "3:30 PM", "3:45 PM",
            "4:00 PM", "4:15 PM", "4:30 PM", "4:45 PM",
        };

        private string _selectedTime;
        public string SelectedTime
        {
            get => _selectedTime;
            set
            {
                _selectedTime = value;
                OnPropertyChanged();

                _selectedDateTime = SelectedDate.Date.Add(DateTime.Parse(SelectedTime).TimeOfDay);
                Debug.WriteLine($"Selected DateTime is {_selectedDateTime}");
            }
        }

        private DateTime _selectedDateTime;

        public string[] Durations { get; } = new string[] { "15 minutes", "30 minutes", "45 minutes", "60 minutes" };

        private string _selectedDuration;
        public string SelectedDuration
        {
            get => _selectedDuration;
            set
            {
                _selectedDuration = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Commands
        public RelayCommand LoginViewCommand { get; set; }
        public RelayCommand HomeViewCommand { get; set; }
        public RelayCommand BookAppointmentCommand { get; set; }
        #endregion

        public BookAppointmentViewModel(MainViewModel mainViewModel)
        {
            Debug.WriteLine($"BookAppointment VM initialized.");
            _MAIN_VIEW_MODEL = mainViewModel;
            LoginViewCommand = new RelayCommand(o => { NavigateLoginView((MainViewModel)o); });
            HomeViewCommand = new RelayCommand(o => { NavigateHomeView(); });
            BookAppointmentCommand = new RelayCommand(o => { BookAppointment(); });
        }

        private void NavigateLoginView(MainViewModel mainViewModel)
        {
            mainViewModel.CurrentUser = null;
            mainViewModel.CurrentView = mainViewModel.LoginViewModel;
        }

        private void NavigateHomeView()
        {
            _MAIN_VIEW_MODEL.CurrentView = _MAIN_VIEW_MODEL.HomeViewModel;
        }

        public void BookAppointment()
        {
            var user = DataAccess.SelectUser(SelectedUser);
            Debug.WriteLine($"User: {user.UserName}");
            var customer = DataAccess.SelectCustomer(SelectedCustomer);
            Debug.WriteLine($"Customer: {customer.CustomerName}");
            var canParse = int.TryParse(Regex.Match(SelectedDuration, @"\d+").Value, out int duration);
            Debug.WriteLine($"Duration: {duration}");

            var endTime = _selectedDateTime.AddMinutes(duration);
            Debug.WriteLine($"Start:  {_selectedDateTime}, End: {endTime}");

            bool overlappingAppointment = DataAccess.FindOverlappingAppointments(_selectedDateTime.ToUniversalTime(), endTime.ToUniversalTime());
            if(overlappingAppointment)
            {
                Debug.WriteLine($"Appointment times overlap.");
                MessageBox.Show($"An appointment is already scheduled during this time.  Please choose another time.", $"Appointment Time Unavailable", MessageBoxButton.OK);
                return;
            }

            var appointment = new Appointment()
            {
                CustomerId = customer.CustomerId,
                UserId = user.UserId,
                Type = SelectedAppointmentType,
                Start = _selectedDateTime,
                End = _selectedDateTime.AddMinutes(duration)
            };

            DataAccess.InsertAppointment(appointment, _MAIN_VIEW_MODEL.CurrentUser.UserName);
            UpdateProperties();
            _MAIN_VIEW_MODEL.HomeViewModel.UpdateProperties();
            _MAIN_VIEW_MODEL.CurrentView = _MAIN_VIEW_MODEL.HomeViewModel;
        }

        public void UpdateProperties()
        {
            UserNames = new List<string>();
            _users.ForEach(x => UserNames.Add(x.UserName));

            _customers = DataAccess.SelectAllCustomers();
            CustomerNames = new List<string>();
            _customers.ForEach(x => CustomerNames.Add(x.CustomerName));

            SelectedUser = "";
            SelectedAppointmentType = "";
            SelectedCustomer = "";
            SelectedDuration = "";
        }
    }
}
