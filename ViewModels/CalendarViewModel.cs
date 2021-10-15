using SchedulingApp.Models;
using SchedulingApp.Utilities;
using System.Collections.Generic;

namespace SchedulingApp.ViewModels
{
    public class CalendarViewModel : ObservableObject
    {
        private List<User> _users = DataAccess.SelectAllUsers();

        #region View Properties
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
        #endregion

        #region Commands
        public RelayCommand TestViewCommand { get; set; }
        public RelayCommand HomeViewCommand { get; set; }
        public RelayCommand LoginViewCommand { get; set; }
        public RelayCommand CalendarViewCommand { get; set; }
        #endregion

        public CalendarViewModel()
        {
            HomeViewCommand = new RelayCommand(o => NavigationService.NavigateTo<HomeViewModel>());
            LoginViewCommand = new RelayCommand(o => NavigationService.NavigateTo<LoginViewModel>());
            CalendarViewCommand = new RelayCommand(o => NavigationService.NavigateTo<CalendarViewModel>());
        }

        public void SetProperties()
        {
            UserNames = new List<string>();
            _users.ForEach(x => UserNames.Add(x.UserName));
        }
    }
}
