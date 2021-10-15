using SchedulingApp.Models;
using SchedulingApp.ViewModels;
using System.Diagnostics;

namespace SchedulingApp.Utilities
{
    public static class NavigationService
    {
        public static MainViewModel MainVM { get; private set; }
        private static LoginViewModel _loginVM;
        private static HomeViewModel _homeVM;
        private static BookAppointmentViewModel _bookAppointmentVM;
        private static UpdateAppointmentViewModel _updateAppointmentVM;
        private static AddCustomerViewModel _addCustomerVM;
        private static UpdateCustomerViewModel _updateCustomerVM;

        public static void AssignMainViewModel(MainViewModel mainViewModel)
        {
            MainVM = mainViewModel;
            _loginVM = new LoginViewModel();
            _homeVM = new HomeViewModel();
            _bookAppointmentVM = new BookAppointmentViewModel();
            _updateAppointmentVM = new UpdateAppointmentViewModel();
            _addCustomerVM = new AddCustomerViewModel();
            _updateCustomerVM = new UpdateCustomerViewModel();

            NavigateTo<LoginViewModel>();
        }

        public static void NavigateTo<T>(bool firstTime = false)
        {
            if (typeof(T) == typeof(LoginViewModel))
            {
                MainVM.CurrentView = _loginVM;
                return;
            }

            if (typeof(T) == typeof(HomeViewModel))
            {
                if (firstTime)
                {
                    _homeVM.CurrentUser = MainVM.CurrentUser;
                    _homeVM.AlertUpcomingAppointments();
                }

                _homeVM.UpdateProperties();
                MainVM.CurrentView = _homeVM;
                return;
            }

            if (typeof(T) == typeof(BookAppointmentViewModel))
            {
                _bookAppointmentVM.ResetProperties();
                MainVM.CurrentView = _bookAppointmentVM;
                return;
            }

            if (typeof(T) == typeof(AddCustomerViewModel))
            {
                MainVM.CurrentView = _addCustomerVM;
                return;
            }

            if (typeof(T) == typeof(UpdateCustomerViewModel))
            {
                MainVM.CurrentView = _updateCustomerVM;
                return;
            }
        }

        public static void NavigateTo<T>(object obj)
        {
            if (typeof(T) == typeof(UpdateAppointmentViewModel) && obj.GetType() == typeof(Appointment))
            {
                _updateAppointmentVM.SetProperties((Appointment)obj);
                MainVM.CurrentView = _updateAppointmentVM;
                return;
            }

            if (typeof(T) == typeof(UpdateCustomerViewModel) && obj.GetType() == typeof(Customer))
            {
                _updateCustomerVM.SetProperties((Customer)obj);
                MainVM.CurrentView = _updateCustomerVM;
                return;
            }

            if (typeof(T) == typeof(UpdateCustomerViewModel) && obj.GetType() == typeof(int))
            {
                var customer = DataAccess.SelectCustomer((int)obj);
                if(customer is null)
                {
                    Debug.WriteLine($"customer was null");
                    return;
                }

                _updateCustomerVM.SetProperties(customer);
                MainVM.CurrentView = _updateCustomerVM;
                return;
            }
        }
    }
}
