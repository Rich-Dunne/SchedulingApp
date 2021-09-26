using SchedulingApp.Models;
using System.Diagnostics;

namespace SchedulingApp.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        public HomeViewModel HomeViewModel { get; set; }
        public LoginViewModel LoginViewModel { get; set; }
        public TestViewModel TestViewModel { get; set; }
        public RelayCommand SignInCommand { get; set; }
        public RelayCommand TestViewCommand { get; set; }
        public RelayCommand HomeViewCommand { get; set; }


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
            HomeViewModel = new HomeViewModel();
            LoginViewModel = new LoginViewModel();
            TestViewModel = new TestViewModel();
            CurrentView = LoginViewModel;

            HomeViewCommand = new RelayCommand(o => { CurrentView = HomeViewModel; });
            TestViewCommand = new RelayCommand(o => { CurrentView = TestViewModel; });
        }
    }
}
