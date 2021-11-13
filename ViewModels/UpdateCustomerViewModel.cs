using SchedulingApp.Data;
using SchedulingApp.Models;
using SchedulingApp.Utilities;
using SchedulingApp.Validation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;

namespace SchedulingApp.ViewModels
{
    public class UpdateCustomerViewModel : ObservableObject, INotifyDataErrorInfo
    {
        private Customer _customer { get; set; }

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
        public RelayCommand UpdateCustomerCommand { get; set; }
        public RelayCommand DeleteCommand { get; set; }
        public RelayCommand CalendarViewCommand { get; set; }
        public RelayCommand CustomersViewCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }
        #endregion

        public UpdateCustomerViewModel()
        {
            Debug.WriteLine($"UpdateCustomer VM initialized.");
            _errorsViewModel = new ErrorsViewModel();
            _errorsViewModel.ErrorsChanged += ErrorsViewModel_ErrorsChanged;
            LoginViewCommand = new RelayCommand(o => NavigationService.NavigateTo(View.Login));
            HomeViewCommand = new RelayCommand(o => NavigationService.NavigateTo(View.Home));
            CalendarViewCommand = new RelayCommand(o => NavigationService.NavigateTo(View.Calendar));
            CustomersViewCommand = new RelayCommand(o => NavigationService.NavigateTo(View.Customers));
            UpdateCustomerCommand = new RelayCommand(o => Update());
            CancelCommand = new RelayCommand(o => NavigationService.NavigateTo(NavigationService.PreviousView));
            DeleteCommand = new RelayCommand(o => _customer.Delete(true));
        }

        public void Update()
        {
            ValidateInput();
            if (HasErrors)
            {
                Debug.WriteLine($"Form has errors.");
                MessageBox.Show($"Please fix your form errors before continuing.", $"Customer Error", MessageBoxButton.OK);
                return;
            }

            if(DataIsUnchanged())
            {
                Debug.WriteLine($"No changes made.");
                NavigationService.NavigateTo(NavigationService.PreviousView);
                return;
            }

            _customer.FirstName = FirstName;
            _customer.LastName = LastName;
            _customer.Address.Address1 = Address;
            _customer.Address.Address2 = Address2;
            _customer.Address.PostalCode = Postal;
            _customer.Address.Phone = Phone;
            _customer.Address.City.Country.CountryName = Country;
            _customer.Address.City.CityName = City;

            new DataAccess().Update(_customer);

            NavigationService.NavigateTo(NavigationService.PreviousView);
        }

        private bool DataIsUnchanged()
        {
            bool customerChanged = _customer.CustomerName != $"{FirstName} {LastName}";
            bool addressChanged = _customer.Address.Address1 != Address || _customer.Address.Address2 != Address2 || _customer.Address.PostalCode != Postal || _customer.Address.Phone != Phone;
            bool cityChanged = _customer.Address.City.CityName != City;
            bool countryChanged = _customer.Address.City.Country.CountryName != Country;

            if(!customerChanged && !addressChanged && !cityChanged && !countryChanged)
            {
                return true;
            }
            return false;
        }

        public void SetProperties(Customer customer)
        {
            _customer = customer;
            FirstName = _customer.FirstName;
            LastName = _customer.LastName;
            Address = _customer.Address.Address1;
            Address2 = _customer.Address.Address2;
            Postal = _customer.Address.PostalCode;
            Phone = _customer.Address.Phone;
            City = _customer.Address.City.CityName;
            Country = _customer.Address.City.Country.CountryName;
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
