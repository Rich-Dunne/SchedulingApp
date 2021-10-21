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
    public class BookAppointmentViewModel : ObservableObject
    {
        private List<Customer> _customers { get => DataAccess.SelectAllCustomers(); }
        private List<User> _users { get => DataAccess.SelectAllUsers(); }

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

                _user = _users.FirstOrDefault(x => x.UserName == SelectedUser);
            }
        }

        private User _user;

        private DateTime _selectedDate = DateTime.Today;
        public DateTime SelectedDate
        {
            get => _selectedDate;
            set
            {
                _selectedDate = value;
                OnPropertyChanged();
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
            }
        }

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
        public RelayCommand CalendarViewCommand { get; set; }
        public RelayCommand CustomersViewCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }
        #endregion

        public BookAppointmentViewModel()
        {
            Debug.WriteLine($"BookAppointment VM initialized.");
            LoginViewCommand = new RelayCommand(o => { NavigationService.NavigateTo(View.Login); });
            HomeViewCommand = new RelayCommand(o => { NavigationService.NavigateTo(View.Home); });
            CalendarViewCommand = new RelayCommand(o => NavigationService.NavigateTo(View.Calendar));
            CustomersViewCommand = new RelayCommand(o => NavigationService.NavigateTo(View.Customers));
            BookAppointmentCommand = new RelayCommand(o => { BookAppointment(); });
            CancelCommand = new RelayCommand(o => NavigationService.NavigateTo(NavigationService.PreviousView));
        }

        public void BookAppointment()
        {
            var startTime = SelectedDate.Date.Add(DateTime.Parse(SelectedTime).TimeOfDay);
            var canParse = int.TryParse(Regex.Match(SelectedDuration, @"\d+").Value, out int duration);
            Debug.WriteLine($"Duration: {duration}");
            var endTime = startTime.AddMinutes(duration);
            Debug.WriteLine($"Start:  {startTime}, End: {endTime}");

            if (endTime.Hour >= 17 && endTime.Minute > 0)
            {
                Debug.WriteLine($"Appointment extends past end of business day.");
                MessageBox.Show($"Please choose a time and meeting duration that ends before 5:00 PM.", $"Appointment Time Unavailable", MessageBoxButton.OK);
                return;
            }

            bool overlappingAppointment = DataAccess.FindOverlappingAppointments(_user, startTime, endTime);
            if(overlappingAppointment)
            {
                Debug.WriteLine($"Appointment times overlap.");
                MessageBox.Show($"An appointment is already scheduled during this time for {SelectedUser}.  Please choose another time.", $"An appointment is already scheduled during this time for  {SelectedUser} .  Please choose a different time.", MessageBoxButton.OK);
                return;
            }

            var user = DataAccess.SelectUser(SelectedUser);
            Debug.WriteLine($"User: {user.UserName}");
            var customer = DataAccess.SelectCustomer(SelectedCustomer);
            Debug.WriteLine($"Customer: {customer.CustomerName}");

            var appointment = new Appointment()
            {
                CustomerId = customer.CustomerId,
                UserId = user.UserId,
                Type = SelectedAppointmentType,
                Start = startTime,
                End = endTime
            };

            DataAccess.InsertAppointment(appointment);
            ResetProperties();

            NavigationService.NavigateTo(View.Home);
        }

        public void ResetProperties()
        {
            UserNames = new List<string>();
            _users.ForEach(x => UserNames.Add(x.UserName));

            CustomerNames = new List<string>();
            _customers.ForEach(x => CustomerNames.Add(x.CustomerName));

            SelectedUser = "";
            SelectedAppointmentType = "";
            SelectedCustomer = "";
            SelectedDate = DateTime.Today;
            SelectedDuration = "";
        }

        public void SetDate(DateTime dateTime)
        {
            SelectedDate = dateTime;
        }
    }
}
