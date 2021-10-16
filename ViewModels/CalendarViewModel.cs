using SchedulingApp.Models;
using SchedulingApp.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Data;

namespace SchedulingApp.ViewModels
{
    public class CalendarViewModel : ObservableObject
    {
        private List<User> _users = DataAccess.SelectAllUsers();

        #region View Properties
        private List<string> _userNames = new List<string>() { "All" };
        public List<string> UserNames
        {
            get => _userNames;
            set
            {
                _userNames = value;
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

                User = null;
                if (_selectedUser != "All")
                {
                    User = _users.First(x => x.UserName == _selectedUser);
                    Debug.WriteLine($"SelectedUser: {SelectedUser}, User: {User.UserName}");
                }

                FilterAppointments();
            }
        }

        private User _user;
        public User User
        {
            get => _user;
            set
            {
                _user = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<Appointment> _appointments = new ObservableCollection<Appointment>();
        public ObservableCollection<Appointment> Appointments
        {
            get => _appointments;
            set
            {
                _appointments = value;
                OnPropertyChanged();
            }
        }

        private string[] _timeFrames =
        {
            "All", "Today", "This Week", "This Month", "January",
            "February", "March", "April", "May", "June", "July",
            "August", "September", "October", "November", "December"
        };
        public string[] TimeFrames { get => _timeFrames; }


        private string[] _appointmentTypes = { "All", AppointmentType.Scrum.ToString(), AppointmentType.Sales.ToString(), AppointmentType.Lunch.ToString(), AppointmentType.Presentation.ToString() };
        public string[] AppointmentTypes { get => _appointmentTypes; }

        private string _selectedTimeFrame;
        public string SelectedTimeFrame
        {
            get => _selectedTimeFrame;
            set
            {
                _selectedTimeFrame = value;
                OnPropertyChanged();
                Debug.WriteLine($"Selected time frame: {SelectedTimeFrame}");

                FilterAppointments();
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
                Debug.WriteLine($"Selected appointment type: {SelectedAppointmentType}");

                FilterAppointments();
            }
        }

        private Appointment _selectedAppointment;
        public Appointment SelectedAppointment
        {
            get => _selectedAppointment;
            set
            {
                _selectedAppointment = value;
                OnPropertyChanged();
            }
        }

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();

                FilterAppointments();
                Debug.WriteLine($"Search: \"{SearchText}\"");
            }
        }

        private string _noAppointmentsFound = "Collapsed";
        public string NoAppointmentsFound
        {
            get => _noAppointmentsFound;
            set
            {
                _noAppointmentsFound = value;
                OnPropertyChanged();
            }
        }

        private string _calendarVisibility = "Collapsed";
        public string CalendarVisibility
        {
            get => _calendarVisibility;
            set
            {
                _calendarVisibility = value;
                OnPropertyChanged();
            }
        }

        public CollectionViewSource CollectionView { get; set; }
        #endregion

        #region Commands
        public RelayCommand TestViewCommand { get; set; }
        public RelayCommand HomeViewCommand { get; set; }
        public RelayCommand LoginViewCommand { get; set; }
        public RelayCommand CalendarViewCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }
        public RelayCommand BookAppointmentCommand { get; set; }
        #endregion

        public CalendarViewModel()
        {
            HomeViewCommand = new RelayCommand(o => NavigationService.NavigateTo<HomeViewModel>());
            LoginViewCommand = new RelayCommand(o => NavigationService.NavigateTo<LoginViewModel>());
            CalendarViewCommand = new RelayCommand(o => NavigationService.NavigateTo<CalendarViewModel>());
            BookAppointmentCommand = new RelayCommand(o => NavigationService.NavigateTo<BookAppointmentViewModel>());
        }

        public void SetProperties()
        {
            SelectedUser = NavigationService.MainVM.CurrentUser.UserName;
            UserNames.RemoveAll(x => x != "All");
            _users.ForEach(x => UserNames.Add(x.UserName));
        }

        private void FilterAppointments()
        {
            Appointments.Clear();
            var appointments = DataAccess.SelectAllAppointments();
            if (_selectedUser != "All")
            {
                appointments.RemoveAll(x => x.UserId != User.UserId);
            }

            switch(SelectedTimeFrame)
            {
                case "All":
                    break;
                case "Today":
                    appointments.RemoveAll(x => x.Start.Day != DateTime.Today.Day);
                    break;

                case "This Week":
                    var firstDayOfWeek = DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek);
                    appointments.RemoveAll(x => x.Start.Day < firstDayOfWeek.Day || x.End.Day > firstDayOfWeek.AddDays(6).Day);
                    break;

                case "This Month":
                    appointments.RemoveAll(x => x.Start.Month != DateTime.Today.Month);
                    break;

                default:
                    appointments.RemoveAll(x => x.Start.ToString("MMMM") != SelectedTimeFrame);
                    break;
            }

            if (SelectedAppointmentType != "All")
            {
                appointments.RemoveAll(x => x.Type != SelectedAppointmentType);
            }

            if(!string.IsNullOrWhiteSpace(SearchText))
            {
                appointments.RemoveAll(x => !x.CustomerName.ToLower().Contains(SearchText));
            }

            Appointments.Clear();
            appointments.ForEach(x => Appointments.Add(x));
            NoAppointmentsFound = appointments.Count == 0 ? "Visible" : "Collapsed";
            CalendarVisibility = appointments.Count == 0 ? "Collapsed" : "Visible";

            var view = new CollectionViewSource();
            view.GroupDescriptions.Add(new PropertyGroupDescription("MonthYear"));
            // view.Filter ???
            view.Source = Appointments;
            CollectionView = view;
        }
    }
}
