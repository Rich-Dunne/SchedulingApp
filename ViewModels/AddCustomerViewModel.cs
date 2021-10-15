using SchedulingApp.Models;
using SchedulingApp.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;

namespace SchedulingApp.ViewModels
{
    public class AddCustomerViewModel : ObservableObject, INotifyDataErrorInfo
    {
        private List<Customer> _customers = DataAccess.SelectAllCustomers();
        private List<User> _users = DataAccess.SelectAllUsers();

        #region Form Properties
        private string _firstName = "";
        public string FirstName
        {
            get => _firstName;
            set
            {
                _firstName = value;
                OnPropertyChanged();
                ValidateFirstName();
            }
        }

        private string _lastName = "";
        public string LastName
        {
            get => _lastName;
            set
            {
                _lastName = value;
                OnPropertyChanged();
                ValidateLastName();
            }
        }

        private string _address = "";
        public string Address
        {
            get => _address;
            set
            {
                _address = value;
                OnPropertyChanged();
                ValidateAddress();
            }
        }

        private string _address2 = "";
        public string Address2
        {
            get => _address2;
            set
            {
                _address2 = value;
                OnPropertyChanged();
            }
        }

        private string _city = "";
        public string City
        {
            get => _city;
            set
            {
                _city = value;
                OnPropertyChanged();
                ValidateCity();
            }
        }

        private string _country = "";
        public string Country
        {
            get => _country;
            set
            {
                _country = value;
                OnPropertyChanged();
                ValidateCountry();
            }
        }

        private string _postal = "";
        public string Postal
        {
            get => _postal;
            set
            {
                _postal = value;
                OnPropertyChanged();
                ValidatePostal();
            }
        }

        private string _phone = "";
        public string Phone
        {
            get => _phone;
            set
            {
                _phone = value;
                OnPropertyChanged();
                ValidatePhone();
            }
        }
        #endregion

        #region Error Properties
        private ErrorsViewModel _errorsViewModel;
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
        public bool HasErrors => _errorsViewModel.HasErrors;
        #endregion

        #region Commands
        public RelayCommand LoginViewCommand { get; set; }
        public RelayCommand HomeViewCommand { get; set; }
        public RelayCommand AddCustomerCommand { get; set; }
        #endregion

        public AddCustomerViewModel()
        {
            Debug.WriteLine($"AddCustomer VM initialized.");
            _errorsViewModel = new ErrorsViewModel();
            _errorsViewModel.ErrorsChanged += ErrorsViewModel_ErrorsChanged;
            LoginViewCommand = new RelayCommand(o => { NavigationService.NavigateTo<LoginViewModel>(); });
            HomeViewCommand = new RelayCommand(o => { NavigationService.NavigateTo<HomeViewModel>(); });
            AddCustomerCommand = new RelayCommand(o => { AddCustomer(); });

        }

        public void AddCustomer()
        {
            ValidateInput();
            if(HasErrors)
            {
                MessageBox.Show($"Please fix your form errors before continuing.", $"Customer Error", MessageBoxButton.OK);
                return;
            }

            Country country = DataAccess.SelectCountry(Country);
            if(country is null)
            {
                Debug.WriteLine($"Creating new record from null country");
                CreateNewCustomerRecord();
                return;
            }

            City city = DataAccess.SelectCity(City, country.CountryId);
            if(city is null)
            {
                Debug.WriteLine($"Creating new record from null city");
                CreateNewCustomerRecord(country);
                return;
            }

            Address address = DataAccess.SelectAddress(Address, Address2, Postal, city.CityId);
            if(address is null)
            {
                Debug.WriteLine($"Creating new record from null address");
                CreateNewCustomerRecord(country, city);
                return;
            }

            Customer customer = DataAccess.SelectCustomer($"{FirstName} {LastName}", address.AddressId);
            if(customer is null)
            {
                Debug.WriteLine($"Creating new record from null customer");
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
                    CreatedBy = NavigationService.MainVM.CurrentUser.UserName,
                    LastUpdate = DateTime.Now,
                    LastUpdateBy = NavigationService.MainVM.CurrentUser.UserName
                };
                country.CountryId = (int)DataAccess.InsertCountry(country);
            }

            if (city is null)
            {
                city = new City()
                {
                    CityName = City,
                    CountryId = country.CountryId,
                    CreateDate = DateTime.Now,
                    CreatedBy = NavigationService.MainVM.CurrentUser.UserName,
                    LastUpdate = DateTime.Now,
                    LastUpdateBy = NavigationService.MainVM.CurrentUser.UserName
                };
                city.CityId = (int)DataAccess.InsertCity(city);
            }

            if (address is null)
            {
                address = new Address()
                {
                    Address1 = Address,
                    Address2 = Address2,
                    CityId = city.CityId,
                    PostalCode = Postal,
                    Phone = Phone,
                    CreateDate = DateTime.Now,
                    CreatedBy = NavigationService.MainVM.CurrentUser.UserName,
                    LastUpdate = DateTime.Now,
                    LastUpdateBy = NavigationService.MainVM.CurrentUser.UserName
                };
                address.AddressId = (int)DataAccess.InsertAddress(address);
            }

            var customer = new Customer()
            {
                CustomerName = $"{FirstName} {LastName}",
                AddressId = address.AddressId,
                CreateDate = DateTime.Now,
                CreatedBy = NavigationService.MainVM.CurrentUser.UserName,
                LastUpdate = DateTime.Now,
                LastUpdateBy = NavigationService.MainVM.CurrentUser.UserName
            };
            DataAccess.InsertCustomer(customer);

            ResetProperties();

            NavigationService.NavigateTo<HomeViewModel>();
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

        public IEnumerable GetErrors(string propertyName) => _errorsViewModel.GetErrors(propertyName);

        private void ErrorsViewModel_ErrorsChanged(object sender, DataErrorsChangedEventArgs e)
        {
            ErrorsChanged?.Invoke(this, e);
        }

        private void ValidateInput()
        {
            ValidateFirstName();
            ValidateLastName();
            ValidateAddress();
            ValidateCity();
            ValidateCountry();
            ValidatePostal();
            ValidatePhone();
        }

        private void ValidateFirstName()
        {
            _errorsViewModel.ClearErrors(nameof(FirstName));
            if (!CustomerValidator.ValidateName(FirstName, out string errorMessage))
            {
                _errorsViewModel.AddError(nameof(FirstName), errorMessage);
            }
        }

        private void ValidateLastName()
        {
            _errorsViewModel.ClearErrors(nameof(LastName));
            if (!CustomerValidator.ValidateName(LastName, out string errorMessage))
            {
                _errorsViewModel.AddError(nameof(LastName), errorMessage);
            }
        }

        private void ValidateAddress()
        {
            _errorsViewModel.ClearErrors(nameof(Address));
            if (!CustomerValidator.ValidateRequired(Address, out string errorMessage))
            {
                _errorsViewModel.AddError(nameof(Address), errorMessage);
            }
        }

        private void ValidateCity()
        {
            _errorsViewModel.ClearErrors(nameof(City));
            if (!CustomerValidator.ValidateName(City, out string errorMessage))
            {
                _errorsViewModel.AddError(nameof(City), errorMessage);
            }
        }

        private void ValidateCountry()
        {
            _errorsViewModel.ClearErrors(nameof(Country));
            if (!CustomerValidator.ValidateName(Country, out string errorMessage))
            {
                _errorsViewModel.AddError(nameof(Country), errorMessage);
            }
        }

        private void ValidatePostal()
        {
            _errorsViewModel.ClearErrors(nameof(Postal));
            if (!CustomerValidator.ValidateRequired(Postal, out string errorMessage))
            {
                _errorsViewModel.AddError(nameof(Postal), errorMessage);
            }
        }

        private void ValidatePhone()
        {
            _errorsViewModel.ClearErrors(nameof(Phone));
            if (!CustomerValidator.ValidatePhone(Phone, out string errorMessage))
            {
                _errorsViewModel.AddError(nameof(Phone), errorMessage);
            }
        }
    }
}
