using SchedulingApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulingApp.ViewModels
{
    public class TestViewModel : ObservableObject
    {
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

        #region Commands
        public RelayCommand TestViewCommand { get; set; }
        public RelayCommand HomeViewCommand { get; set; }
        #endregion

        public TestViewModel()
        {
            TestViewCommand = new RelayCommand(o => { NavigateTestView((MainViewModel)o); });
            HomeViewCommand = new RelayCommand(o => { NavigateHomeView((MainViewModel)o); });
        }
        private void NavigateTestView(MainViewModel mainViewModel)
        {
            mainViewModel.CurrentView = mainViewModel.TestViewModel;
        }

        private void NavigateHomeView(MainViewModel mainViewModel)
        {
            mainViewModel.CurrentView = mainViewModel.HomeViewModel;
        }
    }
}
