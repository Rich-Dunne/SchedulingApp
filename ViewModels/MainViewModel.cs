﻿using SchedulingApp.Models;
using System.Diagnostics;

namespace SchedulingApp.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        public HomeViewModel HomeViewModel { get; set; }
        public LoginViewModel LoginViewModel { get; set; }
        public BookAppointmentViewModel BookAppointmentViewModel { get; set; }
        public TestViewModel TestViewModel { get; set; }
        public RelayCommand SignInCommand { get; set; }
        public RelayCommand TestViewCommand { get; set; }
        public RelayCommand HomeViewCommand { get; set; }

        private User _currentUser;
        public User CurrentUser
        {
            get => _currentUser;
            set
            {
                _currentUser = value;
                OnPropertyChanged();
                HomeViewModel = new HomeViewModel(this);
                BookAppointmentViewModel = new BookAppointmentViewModel(this);
            }
        }


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

        public MainViewModel()
        {
            Debug.WriteLine($"Initialized MainViewModel");
            LoginViewModel = new LoginViewModel();
            CurrentView = LoginViewModel;
        }
    }
}
