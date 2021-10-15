using SchedulingApp.Models;
using SchedulingApp.Utilities;
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

        private string _selectedUser;
        public string SelectedUser
        {
            get => _selectedUser;
            set
            {
                _selectedUser = value;
                OnPropertyChanged();

                User = _users.First(x => x.UserName == _selectedUser);
                Appointments.Clear();
                var appointments = DataAccess.SelectAppointmentsForUser(User);
                appointments.ForEach(x => Appointments.Add(x));
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
