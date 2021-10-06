using SchedulingApp.Models;
using SchedulingApp.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SchedulingApp.ViewModels
{
    public class HomeViewModel : ObservableObject
    {
        private object _currentView;
        public object CurrentView
        {
            get { return _currentView; }
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }

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
        private MainViewModel _MAIN_VIEW_MODEL;
        #endregion

        #region Commands
        public RelayCommand TestViewCommand { get; set; }
        public RelayCommand HomeViewCommand { get; set; }
        public RelayCommand LoginViewCommand { get; set; }
        public RelayCommand CancelUpcomingCommand { get; set; }
        #endregion

        public HomeViewModel(MainViewModel mainViewModel)
        {
            _MAIN_VIEW_MODEL = mainViewModel;
            CurrentUser = _MAIN_VIEW_MODEL.CurrentUser;
            TestViewCommand = new RelayCommand(o => { NavigateTestView((MainViewModel)o); });
            HomeViewCommand = new RelayCommand(o => { NavigateHomeView((MainViewModel)o); });
            LoginViewCommand = new RelayCommand(o => { NavigateLoginView((MainViewModel)o); });
            CancelUpcomingCommand = new RelayCommand(o => CancelUpcomingAppointment());

            GetTodaysAppointments();
            NoneTodayVisibility = TodaysAppointments.Count == 0 ? "Visible" : "Collapsed";
            CurrentWeek = GetCurrentWeek();
            AppointmentsThisWeek = GetAppointmentsThisWeek();
            SetUpcomingProperties();
            
            //TestDataInsertion();
            //TestDataDeletion();
            //TestDataSelection();
        }

        private void NavigateTestView(MainViewModel mainViewModel) => mainViewModel.CurrentView = mainViewModel.TestViewModel;

        private void NavigateHomeView(MainViewModel mainViewModel) => mainViewModel.CurrentView = mainViewModel.HomeViewModel;

        private void NavigateLoginView(MainViewModel mainViewModel)
        {
            mainViewModel.CurrentUser = null;
            mainViewModel.CurrentView = mainViewModel.LoginViewModel;
        }

        private void GetTodaysAppointments()
        {
            TodaysAppointments.Clear();
            var todaysAppointments = DataAccess.SelectAppointmentsInDateRange(DateTime.UtcNow.AddMinutes(-30), DateTime.UtcNow.AddMinutes(30));
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
            if (UpcomingAppointment == null)
            {
                UpcomingDateTime = "";
                HasUpcomingAppointment = false;
                return;
            }

            var upcomingDay = UpcomingAppointment.Start.Day;
            if (upcomingDay == DateTime.Now.Day)
            {
                UpcomingDateTime += "Today @ ";
            }
            else if (upcomingDay == DateTime.Now.AddDays(1).Day)
            {
                UpcomingDateTime += "Tomorrow @ ";
            }
            else
            {
                UpcomingDateTime = $"{UpcomingAppointment.Start.ToString("MMM dd")} @ ";
            }

            UpcomingDateTime += UpcomingAppointment.Start.ToShortTimeString();

            UpcomingCustomer = UpcomingAppointment.CustomerName;
            UpcomingDetails = $"{UpcomingAppointment.Type} with {UpcomingCustomer}";
            HasUpcomingAppointment = true;
        }

        private void TestDataInsertion()
        {
            //DataAccess.InsertCountry(new Country() { CountryName = "Test Country" }, "test");
            //DataAccess.InsertCity(new City() { CityName = "Test City", CountryId = 1 }, "test");
            //DataAccess.InsertAddress(new Address() { Address1 = "420 Test St.", Address2 = "Apartment 69", CityId = 1, PostalCode = 90210, Phone = 8675309 }, "test");
            //DataAccess.InsertCustomer(new Customer() { CustomerName = "Test Customer", AddressId = 1, Active = true }, "test");
            //DataAccess.InsertAppointment(new Appointment() { CustomerId = 1, UserId = 1, Type = "Test appointment", Start = DateTime.Today.AddDays(1), End = DateTime.Today.AddDays(2)}, "test");
        }

        private void TestDataDeletion()
        {
            //DataAccess.RemoveCountry(5);
            //DataAccess.RemoveCity(8);
            //DataAccess.RemoveAddress(5);
            //DataAccess.RemoveCustomer(9);
            //DataAccess.RemoveAppointment(5);
        }

        private void TestDataSelection()
        {
            //var appointments = DataAccess.SelectAllAppointments();
            //Debug.WriteLine($"Appointments found: {appointments.Count}");

            //var customers = DataAccess.SelectAllCustomers();
            //Debug.WriteLine($"Customers found: {customers.Count}");

            //var appointmentsInRange = DataAccess.SelectAppointmentsInDateRange(firstDay, lastDay);
            //Debug.WriteLine($"Appointments found: {appointmentsInRange.Count}");
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
                SetUpcomingProperties();
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
