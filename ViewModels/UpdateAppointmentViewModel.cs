using SchedulingApp.Models;
using SchedulingApp.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;

namespace SchedulingApp.ViewModels
{
    public class UpdateAppointmentViewModel : ObservableObject
    {
        private List<Customer> _customers = DataAccess.SelectAllCustomers();
        private List<User> _users = DataAccess.SelectAllUsers();
        private Appointment _appointment;

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

        public List<string> AppointmentTypes { get; } = new List<string>() { AppointmentType.Scrum.ToString(), AppointmentType.Sales.ToString(), AppointmentType.Lunch.ToString(), AppointmentType.Presentation.ToString() };

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

                //_selectedDateTime = SelectedDate.Date.Add(DateTime.Parse(SelectedTime).TimeOfDay);
                //Debug.WriteLine($"Selected DateTime is {_selectedDateTime}");
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

                //_selectedDateTime = SelectedDate.Date.Add(DateTime.Parse(SelectedTime).TimeOfDay);
                //Debug.WriteLine($"Selected DateTime is {_selectedDateTime}");
            }
        }

        //private DateTime _selectedDateTime;

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
        public RelayCommand UpdateAppointmentCommand { get; set; }
        #endregion

        public UpdateAppointmentViewModel()
        {
            Debug.WriteLine($"UpdateAppointment VM initialized.");
            LoginViewCommand = new RelayCommand(o => { NavigationService.NavigateTo<LoginViewModel>(); });
            HomeViewCommand = new RelayCommand(o => { NavigationService.NavigateTo<HomeViewModel>(); });
            UpdateAppointmentCommand = new RelayCommand(o => { UpdateAppointment(); });
        }

        public void UpdateAppointment()
        {
            if(string.IsNullOrEmpty(SelectedTime))
            {
                MessageBox.Show($"Please choose an appointment time.", $"Invalid Appointment Time", MessageBoxButton.OK);
                return;
            }

            var canParse = int.TryParse(Regex.Match(SelectedDuration, @"\d+").Value, out int duration);
            var startTime = SelectedDate.Date.Add(DateTime.Parse(SelectedTime).TimeOfDay);
            var endTime = startTime.AddMinutes(duration);

            bool overlappingAppointment = DataAccess.FindOverlappingAppointments(_appointment, startTime, endTime);
            if(overlappingAppointment)
            {
                Debug.WriteLine($"Appointment times overlap.");
                MessageBox.Show($"An appointment is already scheduled during this time.  Please choose another time.", $"Appointment Time Unavailable", MessageBoxButton.OK);
                return;
            }

            var user = DataAccess.SelectUser(SelectedUser);
            var customer = DataAccess.SelectCustomer(SelectedCustomer);

            _appointment.CustomerId = customer.CustomerId;
            _appointment.UserId = user.UserId;
            _appointment.Type = SelectedAppointmentType;
            _appointment.Start = startTime;
            _appointment.End = endTime;

            DataAccess.UpdateAppointment(_appointment);
            Debug.WriteLine($"Appointment updated");
            ResetProperties();

            NavigationService.NavigateTo<HomeViewModel>();
        }

        public void SetProperties(Appointment appointment)
        {
            _appointment = appointment;
            UserNames = new List<string>();
            _users.ForEach(x => UserNames.Add(x.UserName));

            _customers = DataAccess.SelectAllCustomers();
            CustomerNames = new List<string>();
            _customers.ForEach(x => CustomerNames.Add(x.CustomerName));

            SelectedUser = _users.First(x => x.UserId == appointment.UserId).UserName;
            SelectedAppointmentType = appointment.Type;
            SelectedCustomer = CustomerNames.First(x => x == appointment.CustomerName);
            SelectedDate = appointment.Start.Date;
            SelectedTime = Times.FirstOrDefault(x => x == appointment.Start.ToShortTimeString());
            SelectedDuration = Durations.First(x => x.Contains(appointment.End.Subtract(appointment.Start).TotalMinutes.ToString()));
        }

        public void ResetProperties()
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
