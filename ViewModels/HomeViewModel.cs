using SchedulingApp.Models;
using SchedulingApp.Utilities;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace SchedulingApp.ViewModels
{
    public class HomeViewModel : ObservableObject
    {
        #region View Properties
        public string TodayDate { get; } = DateTime.Now.ToString("MMM dd, yyyy");

        private ObservableCollection<Appointment> _todaysAppointments = new ObservableCollection<Appointment>();
        public ObservableCollection<Appointment> TodaysAppointments
        {
            get => _todaysAppointments;
            set
            {
                _todaysAppointments = value;
                OnPropertyChanged();
            }
        }
        
        private string _currentWeek;
        public string CurrentWeek 
        { 
            get => _currentWeek;
            set
            {
                _currentWeek = value;
                OnPropertyChanged();
            }
        }

        private int _appointmentsThisWeek;
        public int AppointmentsThisWeek
        {
            get => _appointmentsThisWeek;
            set
            {
                _appointmentsThisWeek = value;
                OnPropertyChanged();
            }
        }

        private int _customers;
        public int Customers
        {
            get => _customers;
            set
            {
                _customers = value;
                OnPropertyChanged();
            }
        }

        private User _currentUser;
        public User CurrentUser
        {
            get => _currentUser;
            set
            {
                _currentUser = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Upcoming Appointment Properties
        private Appointment _upcomingAppointment;
        public Appointment UpcomingAppointment
        {
            get => _upcomingAppointment;
            set
            {
                _upcomingAppointment = value;
                OnPropertyChanged();
            }
        }

        private string _upcomingDateTime;
        public string UpcomingDateTime
        {
            get => _upcomingDateTime;
            set
            {
                _upcomingDateTime = value;
                OnPropertyChanged();
            }
        }

        private string _upcomingDetails;
        public string UpcomingDetails
        {
            get => _upcomingDetails;
            set
            {
                _upcomingDetails = value;
                OnPropertyChanged();
            }
        }

        private string _upcomingCustomer = "";
        public string UpcomingCustomer
        {
            get => _upcomingCustomer;
            set
            {
                _upcomingCustomer = value;
                OnPropertyChanged();
            }
        }

        private bool _hasUpcomingAppointment = false;
        public bool HasUpcomingAppointment
        {
            get => _hasUpcomingAppointment;
            set
            {
                _hasUpcomingAppointment = value;
                OnPropertyChanged();

                if(_hasUpcomingAppointment)
                {
                    UpcomingVisibility = "Visible";
                    NoneUpcomingVisibility = "Collapsed";
                }
                else
                {
                    UpcomingVisibility = "Collapsed";
                    NoneUpcomingVisibility = "Visible";
                }
            }
        }

        private string _upcomingVisibility = "Collapsed";
        public string UpcomingVisibility
        {
            get => _upcomingVisibility;
            set
            {
                _upcomingVisibility = value;
                OnPropertyChanged();
            }
        }

        private string _noneUpcomingVisibility = "Collapsed";
        public string NoneUpcomingVisibility
        {
            get => _noneUpcomingVisibility;
            set
            {
                _noneUpcomingVisibility = value;
                OnPropertyChanged();
            }
        }

        private string _noneTodayVisibility = "Collapsed";
        public string NoneTodayVisibility
        {
            get => _noneTodayVisibility;
            set
            {
                _noneTodayVisibility = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Constants
        private DateTime _FIRST_DAY_OF_WEEK = DateTime.Now.AddDays(-(int) DateTime.Now.DayOfWeek);
        #endregion

        #region Commands
        public RelayCommand TestViewCommand { get; set; }
        public RelayCommand HomeViewCommand { get; set; }
        public RelayCommand LoginViewCommand { get; set; }
        public RelayCommand CalendarViewCommand { get; set; }
        public RelayCommand CancelUpcomingCommand { get; set; }
        public RelayCommand BookAppointmentCommand { get; set; }
        public RelayCommand UpdateAppointmentCommand { get; set; }
        public RelayCommand AddClientCommand { get; set; }
        public RelayCommand UpdateCustomerCommand { get; set; }
        #endregion

        public HomeViewModel()
        {
            HomeViewCommand = new RelayCommand(o => NavigationService.NavigateTo<HomeViewModel>());
            LoginViewCommand = new RelayCommand(o => NavigationService.NavigateTo<LoginViewModel>());
            CalendarViewCommand = new RelayCommand(o => NavigationService.NavigateTo<CalendarViewModel>());
            CancelUpcomingCommand = new RelayCommand(o => CancelUpcomingAppointment());
            BookAppointmentCommand = new RelayCommand(o => NavigationService.NavigateTo<BookAppointmentViewModel>());
            UpdateAppointmentCommand = new RelayCommand(o => NavigationService.NavigateTo<UpdateAppointmentViewModel>(UpcomingAppointment));
            
            // To be removed
            AddClientCommand = new RelayCommand(o => NavigationService.NavigateTo<AddCustomerViewModel>());
            UpdateCustomerCommand = new RelayCommand(o => NavigationService.NavigateTo<UpdateCustomerViewModel>(1));

            UpdateProperties();
        }

        public void UpdateProperties()
        {
            GetTodaysAppointments();
            NoneTodayVisibility = TodaysAppointments.Count == 0 ? "Visible" : "Collapsed";
            CurrentWeek = GetCurrentWeek();
            AppointmentsThisWeek = GetAppointmentsThisWeek();
            Customers = DataAccess.CountAllCustomers();
            SetUpcomingProperties();
        }

        private void GetTodaysAppointments()
        {
            TodaysAppointments.Clear();
            var todaysAppointments = DataAccess.SelectAppointmentsInDateRange(DateTime.Today, DateTime.Today.AddHours(23).AddMinutes(59).AddSeconds(59));
            foreach(Appointment appointment in todaysAppointments)
            {
                if (appointment.Start.Day == DateTime.Today.Day)
                {
                    TodaysAppointments.Add(appointment);
                }
            }
        }

        private void SetUpcomingProperties()
        {
            UpcomingAppointment = DataAccess.SelectNextAppointment();
            UpcomingDateTime = "";
            if (UpcomingAppointment == null)
            {
                HasUpcomingAppointment = false;
                return;
            }

            var upcomingDay = UpcomingAppointment.Start.Day;
            if (upcomingDay == DateTime.Now.ToLocalTime().Day)
            {
                UpcomingDateTime += "Today @ ";
            }
            else if (upcomingDay == DateTime.Now.ToLocalTime().AddDays(1).Day)
            {
                UpcomingDateTime += "Tomorrow @ ";
            }
            else
            {
                UpcomingDateTime = $"{UpcomingAppointment.Start:MMM dd} @ ";
            }

            UpcomingDateTime += UpcomingAppointment.Start.ToLocalTime().ToShortTimeString();

            UpcomingCustomer = UpcomingAppointment.CustomerName;
            UpcomingDetails = $"{UpcomingAppointment.Type} with {UpcomingCustomer}";
            HasUpcomingAppointment = true;
        }

        private string GetCurrentWeek()
        {
            DateTime lastDay = _FIRST_DAY_OF_WEEK.AddDays(6);
            return $"{_FIRST_DAY_OF_WEEK.ToString("MMM dd")} - {lastDay.ToString("MMM dd")}";
        }

        private int GetAppointmentsThisWeek()
        {
            DateTime lastDay = _FIRST_DAY_OF_WEEK.AddDays(6);
            return DataAccess.SelectAppointmentsInDateRange(_FIRST_DAY_OF_WEEK, lastDay).Count;
        }

        private void CancelUpcomingAppointment()
        {
            var cancelPrompt = MessageBox.Show($"Are you sure you want to cancel your upcoming appointment with {UpcomingAppointment.CustomerName}?", "Cancel appointment?", MessageBoxButton.YesNo);
            if(cancelPrompt == MessageBoxResult.Yes)
            {
                DataAccess.RemoveAppointment(UpcomingAppointment.AppointmentId);
                UpdateProperties();
            }
        }

        public void AlertUpcomingAppointments()
        {
            var appointments = TodaysAppointments.Where(x => x.Start.ToLocalTime().Subtract(DateTime.Now.ToLocalTime()).TotalMinutes <= 15 && x.Start.ToLocalTime().Subtract(DateTime.Now.ToLocalTime()).TotalMinutes >= 0).OrderBy(y => y.TimeTo);
            if(appointments.Count() == 0)
            {
                return;
            }

            string appointmentDetails = "";
            foreach(Appointment appointment in appointments)
            {
                appointmentDetails += $"{appointment.Type} with {appointment.CustomerName} at {appointment.Start.ToLocalTime().ToShortTimeString()}\n";
            }

            MessageBox.Show($"The following appointments are coming up soon:\n\n" +
                            $"{appointmentDetails}", "Upcoming Appointment", MessageBoxButton.OK);
        }
    }
}
