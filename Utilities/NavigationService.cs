using SchedulingApp.Data;
using SchedulingApp.Models;
using SchedulingApp.ViewModels;
using System;
using System.Diagnostics;

namespace SchedulingApp.Utilities
{
    public static class NavigationService
    {
        public static MainViewModel MainViewModel { get; private set; }
        public static View PreviousView { get; private set; }
        public static View CurrentView { get; private set; }
        private static LoginViewModel _loginViewModel { get; set; }
        private static HomeViewModel _homeViewModel { get; set; }
        private static BookAppointmentViewModel _bookAppointmentViewModel { get; set; }
        private static UpdateAppointmentViewModel _updateAppointmentViewModel { get; set; }
        private static CustomersViewModel _customersViewModel { get; set; }
        private static AddCustomerViewModel _addCustomerViewModel { get; set; }
        private static UpdateCustomerViewModel _updateCustomerViewModel { get; set; }
        private static CalendarViewModel _calendarViewModel { get; set; }

        public static void AssignMainViewModel(MainViewModel mainViewModel)
        {
            MainViewModel = mainViewModel;
            _loginViewModel = new LoginViewModel();
            _homeViewModel = new HomeViewModel();
            _bookAppointmentViewModel = new BookAppointmentViewModel();
            _updateAppointmentViewModel = new UpdateAppointmentViewModel();
            _addCustomerViewModel = new AddCustomerViewModel();
            _updateCustomerViewModel = new UpdateCustomerViewModel();
            _calendarViewModel = new CalendarViewModel();
            _customersViewModel = new CustomersViewModel();

            NavigateTo(View.Login);
        }

        public static void NavigateTo(View newView)
        {
            PreviousView = CurrentView;
            CurrentView = newView;

            switch (newView)
            {
                case View.Login:
                    _homeViewModel.AlertUpcoming = true;
                    MainViewModel.CurrentView = _loginViewModel;
                    break;

                case View.Home:
                    _homeViewModel.UpdateProperties();
                    MainViewModel.CurrentView = _homeViewModel;
                    if(_homeViewModel.AlertUpcoming)
                    {
                        _homeViewModel.AlertUpcomingAppointments();
                    }
                    _homeViewModel.AlertUpcoming = false;
                    break;

                case View.BookAppointment:
                    _bookAppointmentViewModel.ResetProperties();
                    MainViewModel.CurrentView = _bookAppointmentViewModel;
                    break;

                case View.AddCustomer:
                    MainViewModel.CurrentView = _addCustomerViewModel;
                    break;

                case View.Calendar:
                    _calendarViewModel.SetProperties();
                    MainViewModel.CurrentView = _calendarViewModel;
                    break;

                case View.Customers:
                    _customersViewModel.SetProperties();
                    MainViewModel.CurrentView = _customersViewModel;
                    break;
            }
        }

        public static void NavigateTo(View newView, object obj)
        {
            PreviousView = CurrentView;
            CurrentView = newView;

            switch (newView)
            {
                case View.UpdateAppointment:
                    if(obj is Appointment)
                    {
                        _updateAppointmentViewModel.SetProperties((Appointment)obj);
                        MainViewModel.CurrentView = _updateAppointmentViewModel;
                    }
                    break;

                case View.UpdateCustomer:
                    if(obj is Customer)
                    {
                        var customer = obj as Customer;
                        _updateCustomerViewModel.SetProperties((Customer)obj);
                        MainViewModel.CurrentView = _updateCustomerViewModel;
                    }
                    else if (obj is int)
                    {
                        var customer = new DataAccess().SelectCustomer((int)obj);
                        if (customer is null)
                        {
                            Debug.WriteLine($"customer was null");
                            break;
                        }

                        _updateCustomerViewModel.SetProperties(customer);
                        MainViewModel.CurrentView = _updateCustomerViewModel;
                    }
                    break;
            }    
        }
    }
}
