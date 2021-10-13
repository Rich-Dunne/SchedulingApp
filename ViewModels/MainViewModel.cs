using SchedulingApp.Models;
using SchedulingApp.Utilities;
using System.Diagnostics;

namespace SchedulingApp.ViewModels
{
    public class MainViewModel : ObservableObject
    {
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
            NavigationService.AssignMainViewModel(this);
        }
    }
}
