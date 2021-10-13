using SchedulingApp.Models;
using SchedulingApp.ViewModels;

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

        public static void AssignMainViewModel(MainViewModel mainViewModel)
        {
            MainVM = mainViewModel;
            _loginVM = new LoginViewModel();
            _homeVM = new HomeViewModel();
            _bookAppointmentVM = new BookAppointmentViewModel();
            _updateAppointmentVM = new UpdateAppointmentViewModel();
            _addCustomerVM = new AddCustomerViewModel();

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
        }

        public static void NavigateTo<T>(object obj)
        {
            if (typeof(T) == typeof(UpdateAppointmentViewModel))
            {
                _updateAppointmentVM.SetProperties((Appointment)obj);
                MainVM.CurrentView = _updateAppointmentVM;
                return;
            }
        }
    }
}
