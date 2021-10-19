using SchedulingApp.Models;
using SchedulingApp.Utilities;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Data;

namespace SchedulingApp.ViewModels
{
    public class CustomersViewModel : ObservableObject
    {
        #region View Properties

        private ObservableCollection<Customer> _customers = new ObservableCollection<Customer>();
        public ObservableCollection<Customer> Customers
        {
            get => _customers;
            set
            {
                _customers = value;
                OnPropertyChanged();
            }
        }

        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                CollectionView.Refresh();
            }
        }

        private string _noCustomersFound = "Collapsed";
        public string NoCustomersFound
        {
            get => _noCustomersFound;
            set
            {
                _noCustomersFound = value;
                OnPropertyChanged();
            }
        }

        private string _listViewVisibility = "Collapsed";
        public string ListViewVisibility
        {
            get => _listViewVisibility;
            set
            {
                _listViewVisibility = value;
                OnPropertyChanged();
            }
        }

        private int _filterResultsCount;
        public int FilterResultsCount
        {
            get => _filterResultsCount;
            set
            {
                _filterResultsCount = value;
                OnPropertyChanged();
            }
        }

        public ICollectionView CollectionView { get; set; }
        #endregion

        #region Commands
        public RelayCommand TestViewCommand { get; set; }
        public RelayCommand HomeViewCommand { get; set; }
        public RelayCommand LoginViewCommand { get; set; }
        public RelayCommand CalendarViewCommand { get; set; }
        public RelayCommand DeleteCommand { get; set; }
        public RelayCommand AddCustomerCommand { get; set; }
        #endregion

        public CustomersViewModel()
        {
            HomeViewCommand = new RelayCommand(o => NavigationService.NavigateTo(View.Home));
            LoginViewCommand = new RelayCommand(o => NavigationService.NavigateTo(View.Login));
            CalendarViewCommand = new RelayCommand(o => NavigationService.NavigateTo(View.Calendar));
            AddCustomerCommand = new RelayCommand(o => NavigationService.NavigateTo(View.AddCustomer));

            CollectionView = CollectionViewSource.GetDefaultView(Customers);
            CollectionView.Filter = CustomerFilter;
            CollectionView.GroupDescriptions.Add(new PropertyGroupDescription("FirstLetter"));
            CollectionView.SortDescriptions.Add(new SortDescription("CustomerName", ListSortDirection.Ascending));
            CollectionView.CollectionChanged += CollectionView_CollectionChanged;
        }

        private void CollectionView_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            FilterResultsCount = CollectionView.Cast<Customer>().Count();
        }

        public void SetProperties()
        {
            Customers.Clear();
            var allCustomers = DataAccess.SelectAllCustomers();
            allCustomers.ForEach(x => Customers.Add(x));
            NoCustomersFound = Customers.Count == 0 ? "Visible" : "Collapsed";
            ListViewVisibility = Customers.Count == 0 ? "Collapsed" : "Visible";
        }

        private bool CustomerFilter(object obj)
        {
            if(obj is Customer customer)
            {
                return customer.CustomerName.ToLower().Contains(SearchText);
            }

            return false;
        }
    }
}
