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
            HomeViewCommand = new RelayCommand(o => NavigationService.NavigateTo(View.Home));
            LoginViewCommand = new RelayCommand(o => NavigationService.NavigateTo(View.Login));
            CalendarViewCommand = new RelayCommand(o => NavigationService.NavigateTo(View.Calendar));
        }
    }
}
