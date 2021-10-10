using SchedulingApp.Models;
using SchedulingApp.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace SchedulingApp.ViewModels
{
    public class AddCustomerViewModel : ObservableObject
    {
        private MainViewModel _MAIN_VIEW_MODEL;
        private List<Customer> _customers = DataAccess.SelectAllCustomers();
        private List<User> _users = DataAccess.SelectAllUsers();

        #region Form Properties
        private string _firstName;
        public string FirstName
        {
            get => _firstName;
            set
            {
                _firstName = value;
                OnPropertyChanged();
            }
        }

        private string _lastName;
        public string LastName
        {
            get => _lastName;
            set
            {
                _lastName = value;
                OnPropertyChanged();
            }
        }

        private string _address;
        public string Address
        {
            get => _address;
            set
            {
                _address = value;
                OnPropertyChanged();
            }
        }

        private string _address2;
        public string Address2
        {
            get => _address2;
            set
            {
                _address2 = value;
                OnPropertyChanged();
            }
        }

        private string _city;
        public string City
        {
            get => _city;
            set
            {
                _city = value;
                OnPropertyChanged();
            }
        }

        private string _country;
        public string Country
        {
            get => _country;
            set
            {
                _country = value;
                OnPropertyChanged();
            }
        }

        private string _postal;
        public string Postal
        {
            get => _postal;
            set
            {
                _postal = value;
                OnPropertyChanged();
            }
        }

        private string _phone;
        public string Phone
        {
            get => _phone;
            set
            {
                _phone = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Commands
        public RelayCommand LoginViewCommand { get; set; }
        public RelayCommand HomeViewCommand { get; set; }
        public RelayCommand AddCustomerCommand { get; set; }
        #endregion

        public AddCustomerViewModel(MainViewModel mainViewModel)
        {
            Debug.WriteLine($"AddCustomer VM initialized.");
            _MAIN_VIEW_MODEL = mainViewModel;
            LoginViewCommand = new RelayCommand(o => { NavigateLoginView((MainViewModel)o); });
            HomeViewCommand = new RelayCommand(o => { NavigateHomeView(); });
            AddCustomerCommand = new RelayCommand(o => { AddCustomer(); });
        }

        private void NavigateLoginView(MainViewModel mainViewModel)
        {
            mainViewModel.CurrentUser = null;
            mainViewModel.CurrentView = mainViewModel.LoginViewModel;
        }

        private void NavigateHomeView() => _MAIN_VIEW_MODEL.CurrentView = _MAIN_VIEW_MODEL.HomeViewModel;

        public void AddCustomer()
        {
            Country country = DataAccess.SelectCountry(Country);
            if(country is null)
            {
                CreateNewCustomerRecord();
                return;
            }

            City city = DataAccess.SelectCity(City, country.CountryId);
            if(city is null)
            {
                CreateNewCustomerRecord(country);
                return;
            }

            Address address = DataAccess.SelectAddress(Address, Address2, int.Parse(Postal), city.CityId);
            if(address is null)
            {
                CreateNewCustomerRecord(country, city);
                return;
            }

            Customer customer = DataAccess.SelectCustomer($"{FirstName} {LastName}", address.AddressId);
            if(customer is null)
            {
                CreateNewCustomerRecord(country, city, address);
                return;
            }

            Debug.WriteLine($"A customer with that information already exists.");
            MessageBox.Show($"A customer with that information already exists.", $"Customer Already Exists", MessageBoxButton.OK);
        }

        private void CreateNewCustomerRecord(Country country = null, City city = null, Address address = null)
        {
            if (country is null)
            {
                country = new Country()
                {
                    CountryName = Country,
                    CreateDate = DateTime.Now,
                    CreatedBy = _MAIN_VIEW_MODEL.CurrentUser.UserName,
                    LastUpdate = DateTime.Now,
                    LastUpdateBy = _MAIN_VIEW_MODEL.CurrentUser.UserName
                };
                country.CountryId = (int)DataAccess.InsertCountry(country, _MAIN_VIEW_MODEL.CurrentUser.UserName);
            }

            if (city is null)
            {
                city = new City()
                {
                    CityName = City,
                    CountryId = country.CountryId,
                    CreateDate = DateTime.Now,
                    CreatedBy = _MAIN_VIEW_MODEL.CurrentUser.UserName,
                    LastUpdate = DateTime.Now,
                    LastUpdateBy = _MAIN_VIEW_MODEL.CurrentUser.UserName
                };
                city.CityId = (int)DataAccess.InsertCity(city, _MAIN_VIEW_MODEL.CurrentUser.UserName);
            }

            if (address is null)
            {
                address = new Address()
                {
                    Address1 = Address,
                    Address2 = Address2,
                    CityId = city.CityId,
                    PostalCode = int.Parse(Postal),
                    Phone = Phone,
                    CreateDate = DateTime.Now,
                    CreatedBy = _MAIN_VIEW_MODEL.CurrentUser.UserName,
                    LastUpdate = DateTime.Now,
                    LastUpdateBy = _MAIN_VIEW_MODEL.CurrentUser.UserName
                };
                address.AddressId = (int)DataAccess.InsertAddress(address, _MAIN_VIEW_MODEL.CurrentUser.UserName);
            }

            var customer = new Customer()
            {
                CustomerName = $"{FirstName} {LastName}",
                AddressId = address.AddressId,
                CreateDate = DateTime.Now,
                CreatedBy = _MAIN_VIEW_MODEL.CurrentUser.UserName,
                LastUpdate = DateTime.Now,
                LastUpdateBy = _MAIN_VIEW_MODEL.CurrentUser.UserName
            };
            DataAccess.InsertCustomer(customer, _MAIN_VIEW_MODEL.CurrentUser.UserName);

            ResetProperties();
            _MAIN_VIEW_MODEL.HomeViewModel.UpdateProperties();
            _MAIN_VIEW_MODEL.CurrentView = _MAIN_VIEW_MODEL.HomeViewModel;
        }

        public void ResetProperties()
        {
            FirstName = "";
            LastName = "";
            Address = "";
            Address2 = "";
            City = "";
            Postal = "";
            Country = "";
            Phone = "";
        }
    }
}
