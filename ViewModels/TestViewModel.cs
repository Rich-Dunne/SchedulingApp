using SchedulingApp.Models;
using SchedulingApp.Utilities;

namespace SchedulingApp.ViewModels
{
    public class TestViewModel : ObservableObject
    {
        #region View Properties

        #endregion

        #region Commands
        public RelayCommand TestViewCommand { get; set; }
        public RelayCommand HomeViewCommand { get; set; }
        public RelayCommand LoginViewCommand { get; set; }
        public RelayCommand CalendarViewCommand { get; set; }
        #endregion

        public TestViewModel()
        {
            HomeViewCommand = new RelayCommand(o => NavigationService.NavigateTo<HomeViewModel>());
            LoginViewCommand = new RelayCommand(o => NavigationService.NavigateTo<LoginViewModel>());
            CalendarViewCommand = new RelayCommand(o => NavigationService.NavigateTo<CalendarViewModel>());
        }
    }
}
