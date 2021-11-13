using SchedulingApp.Data;
using SchedulingApp.Models;
using SchedulingApp.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Data;

namespace SchedulingApp.ViewModels
{
    public class CalendarViewModel : ObservableObject
    {
        private List<User> _users { get => new DataAccess().SelectAllUsers(); }

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
                }

                CollectionView.Refresh();
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


        private string[] _appointmentTypes = { "All", AppointmentType.Sales.ToString(), AppointmentType.Lunch.ToString(), AppointmentType.Presentation.ToString() };
        public string[] AppointmentTypes { get => _appointmentTypes; }

        private string _selectedTimeFrame;
        public string SelectedTimeFrame
        {
            get => _selectedTimeFrame;
            set
            {
                _selectedTimeFrame = value;
                OnPropertyChanged();
                CollectionView.Refresh();
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
                CollectionView.Refresh();
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

        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                CollectionView.Refresh();
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

        private int _filterResultsCount;
        public int FilterResultsCount
        {
            get => _filterResultsCount;
            set
            {
                _filterResultsCount = value;
                OnPropertyChanged();
            }
        }

        public ICollectionView CollectionView { get; set; }
        #endregion

        #region Commands
        public RelayCommand TestViewCommand { get; set; }
        public RelayCommand HomeViewCommand { get; set; }
        public RelayCommand LoginViewCommand { get; set; }
        public RelayCommand CalendarViewCommand { get; set; }
        public RelayCommand BookAppointmentCommand { get; set; }
        public RelayCommand CustomersViewCommand { get; set; }
        #endregion

        public CalendarViewModel()
        {
            HomeViewCommand = new RelayCommand(o => NavigationService.NavigateTo(View.Home));
            LoginViewCommand = new RelayCommand(o => NavigationService.NavigateTo(View.Login));
            CalendarViewCommand = new RelayCommand(o => NavigationService.NavigateTo(View.Calendar));
            BookAppointmentCommand = new RelayCommand(o => NavigationService.NavigateTo(View.BookAppointment));
            CustomersViewCommand = new RelayCommand(o => NavigationService.NavigateTo(View.Customers));

            CollectionView = CollectionViewSource.GetDefaultView(Appointments);
            CollectionView.Filter = AppointmentFilter;
            CollectionView.GroupDescriptions.Add(new PropertyGroupDescription("MonthYear"));
            CollectionView.CollectionChanged += CollectionView_CollectionChanged;
        }

        private void CollectionView_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            FilterResultsCount = CollectionView.Cast<Appointment>().Count();
        }

        public void SetProperties()
        {
            SelectedUser = NavigationService.MainViewModel.CurrentUser.UserName;
            UserNames.RemoveAll(x => x != "All");
            _users.ForEach(x => UserNames.Add(x.UserName));

            Appointments.Clear();
            var allAppointments = new DataAccess().SelectAllAppointments();
            allAppointments.ForEach(x => Appointments.Add(x));
            NoAppointmentsFound = Appointments.Count == 0 ? "Visible" : "Collapsed";
            CalendarVisibility = Appointments.Count == 0 ? "Collapsed" : "Visible";
        }

        private bool AppointmentFilter(object obj)
        {
            if(obj is Appointment appointment)
            {
                bool matchingUser = true;
                bool matchingDate = true;
                bool matchingType = true;
                bool matchingCustomer = true;

                if(SelectedUser != "All")
                {
                    matchingUser = appointment.UserId == User.UserId;
                }

                switch (SelectedTimeFrame)
                {
                    case "All":
                        break;
                    case "Today":
                        matchingDate = appointment.Start.Day == DateTime.Today.Day;
                        break;

                    case "This Week":
                        var firstDayOfWeek = DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek);
                        matchingDate = appointment.Start.Day >= firstDayOfWeek.Day && appointment.End.Day <= firstDayOfWeek.AddDays(6).Day;
                        break;

                    case "This Month":
                        matchingDate = appointment.Start.Month == DateTime.Today.Month;
                        break;

                    default:
                        matchingDate = appointment.Start.ToString("MMMM") == SelectedTimeFrame;
                        break;
                }

                if (SelectedAppointmentType != "All")
                {
                    matchingType = appointment.Type == SelectedAppointmentType;
                }

                if (!string.IsNullOrWhiteSpace(SearchText))
                {
                    matchingCustomer = appointment.Customer.CustomerName.ToLower().Contains(SearchText);
                }

                return matchingUser && matchingDate && matchingType && matchingCustomer;
            }

            return false;
        }
    }
}
