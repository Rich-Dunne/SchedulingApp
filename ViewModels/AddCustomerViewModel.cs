using SchedulingApp.Data;
using SchedulingApp.Models;
using SchedulingApp.Utilities;
using SchedulingApp.Validation;
using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;

namespace SchedulingApp.ViewModels
{
    public class AddCustomerViewModel : ObservableObject, INotifyDataErrorInfo
    {

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
        public RelayCommand CustomersViewCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }
        #endregion

        public AddCustomerViewModel()
        {
            Debug.WriteLine($"AddCustomer VM initialized.");
            _errorsViewModel = new ErrorsViewModel();
            _errorsViewModel.ErrorsChanged += ErrorsViewModel_ErrorsChanged;
            LoginViewCommand = new RelayCommand(o => { NavigationService.NavigateTo(View.Login); });
            HomeViewCommand = new RelayCommand(o => { NavigationService.NavigateTo(View.Home); });
            AddCustomerCommand = new RelayCommand(o => { AddCustomer(); });
            CustomersViewCommand = new RelayCommand(o => NavigationService.NavigateTo(View.Customers));
            CancelCommand = new RelayCommand(o => NavigationService.NavigateTo(NavigationService.PreviousView));
        }

        public void AddCustomer()
        {
            ValidateInput();
            if(HasErrors)
            {
                MessageBox.Show($"Please fix your form errors before continuing.", $"Customer Error", MessageBoxButton.OK);
                return;
            }

            var dataAccess = new DataAccess();

            var country = new Country()
            {
                CountryName = Country,
                CreateDate = DateTime.Now,
                CreatedBy = NavigationService.MainViewModel.CurrentUser.UserName,
                LastUpdate = DateTime.Now,
                LastUpdateBy = NavigationService.MainViewModel.CurrentUser.UserName
            };
            country.CountryId = dataAccess.Insert(country);

            var city = new City()
            {
                CityName = City,
                CountryId = country.CountryId,
                CreateDate = DateTime.Now,
                CreatedBy = NavigationService.MainViewModel.CurrentUser.UserName,
                LastUpdate = DateTime.Now,
                LastUpdateBy = NavigationService.MainViewModel.CurrentUser.UserName
            };
            city.CityId = dataAccess.Insert(city);

            var address = new Address()
            {
                Address1 = Address,
                Address2 = Address2,
                CityId = city.CityId,
                PostalCode = Postal,
                Phone = Phone,
                CreateDate = DateTime.Now,
                CreatedBy = NavigationService.MainViewModel.CurrentUser.UserName,
                LastUpdate = DateTime.Now,
                LastUpdateBy = NavigationService.MainViewModel.CurrentUser.UserName
            };
            address.AddressId = dataAccess.Insert(address);

            var customer = new Customer()
            {
                FirstName = FirstName,
                LastName = LastName,
                AddressId = address.AddressId,
                CreateDate = DateTime.Now,
                CreatedBy = NavigationService.MainViewModel.CurrentUser.UserName,
                LastUpdate = DateTime.Now,
                LastUpdateBy = NavigationService.MainViewModel.CurrentUser.UserName
            };
            dataAccess.Insert(customer);

            ResetProperties();

            NavigationService.NavigateTo(NavigationService.PreviousView);
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
            _errorsViewModel.ClearErrors(nameof(FirstName));
            _errorsViewModel.ClearErrors(nameof(LastName));
            _errorsViewModel.ClearErrors(nameof(Address));
            _errorsViewModel.ClearErrors(nameof(Address2));
            _errorsViewModel.ClearErrors(nameof(City));
            _errorsViewModel.ClearErrors(nameof(Postal));
            _errorsViewModel.ClearErrors(nameof(Country));
            _errorsViewModel.ClearErrors(nameof(Phone));
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
