using SchedulingApp.Models;
using SchedulingApp.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace SchedulingApp.ViewModels
{
    public class AddCustomerViewModel : ObservableObject, INotifyDataErrorInfo
    {
        private ErrorsViewModel _errorsViewModel;
        private MainViewModel _MAIN_VIEW_MODEL;
        private List<Customer> _customers = DataAccess.SelectAllCustomers();
        private List<User> _users = DataAccess.SelectAllUsers();
        private string _NAME_REGEX = "^[a-zA-Z]*$";
        private string _SPACED_NAME_REGEX = "^[a-zA-Z ]*$";
        private string _PHONE_NUMBER_REGEX = "^(\\+\\d{1,2}\\s)?\\(?\\d{3}\\)?[\\s.-]?\\d{3}[\\s.-]?\\d{4}$";

        #region Form Properties
        private string _firstName = "";
        public string FirstName
        {
            get => _firstName;
            set
            {
                _firstName = value;
                OnPropertyChanged();
                ValidateInput();
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
                ValidateInput();
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
                ValidateInput();
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
                ValidateInput();
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
                ValidateInput();
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
                ValidateInput();
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
                ValidateInput();
            }
        }
        #endregion

        #region Validation
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public bool HasErrors => _errorsViewModel.HasErrors;
        private string _requiredError { get; } = "Required";
        private string _formatError { get; } = "Invalid format";
        private string _phoneFormatError { get; } = "Invalid format (area code required)";
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
            _errorsViewModel = new ErrorsViewModel();
            _errorsViewModel.ErrorsChanged += ErrorsViewModel_ErrorsChanged;
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

            Address address = DataAccess.SelectAddress(Address, Address2, int.Parse(Postal), city.CityId);
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

        public IEnumerable GetErrors(string propertyName) => _errorsViewModel.GetErrors(propertyName);

        private void ErrorsViewModel_ErrorsChanged(object sender, DataErrorsChangedEventArgs e)
        {
            ErrorsChanged?.Invoke(this, e);
        }

        private void ValidateInput()
        {
            _errorsViewModel.ClearErrors(nameof(FirstName));
            _errorsViewModel.ClearErrors(nameof(LastName));
            _errorsViewModel.ClearErrors(nameof(Address));
            _errorsViewModel.ClearErrors(nameof(City));
            _errorsViewModel.ClearErrors(nameof(Postal));
            _errorsViewModel.ClearErrors(nameof(Country));
            _errorsViewModel.ClearErrors(nameof(Phone));

            // FirstName validation
            if (string.IsNullOrWhiteSpace(FirstName))
            {
                _errorsViewModel.AddError(nameof(FirstName), _requiredError);
            }
            else if (!Regex.IsMatch(FirstName, _NAME_REGEX))
            {
                _errorsViewModel.AddError(nameof(FirstName), _formatError);
            }

            // LastName validation
            if (string.IsNullOrWhiteSpace(LastName))
            {
                _errorsViewModel.AddError(nameof(LastName), _requiredError);
            }
            else if (!Regex.IsMatch(LastName, _NAME_REGEX))
            {
                _errorsViewModel.AddError(nameof(LastName), _formatError);
            }

            // Address validation
            if (string.IsNullOrWhiteSpace(Address))
            {
                _errorsViewModel.AddError(nameof(Address), _requiredError);
            }

            // City validation
            if (string.IsNullOrWhiteSpace(City))
            {
                _errorsViewModel.AddError(nameof(City), _requiredError);
            }
            else if (!Regex.IsMatch(City, _SPACED_NAME_REGEX))
            {
                _errorsViewModel.AddError(nameof(City), _formatError);
            }

            // Postal validation
            if (string.IsNullOrWhiteSpace(Postal))
            {
                _errorsViewModel.AddError(nameof(Postal), _requiredError);
            }

            // Country validation
            if (string.IsNullOrWhiteSpace(Country))
            {
                _errorsViewModel.AddError(nameof(Country), _requiredError);
            }
            else if (!Regex.IsMatch(Country, _SPACED_NAME_REGEX))
            {
                _errorsViewModel.AddError(nameof(Country), _formatError);
            }

            // Phone validation
            if (string.IsNullOrWhiteSpace(Phone))
            {
                _errorsViewModel.AddError(nameof(Phone), _requiredError);
            }
            else if (!Regex.IsMatch(Phone, _PHONE_NUMBER_REGEX))
            {
                _errorsViewModel.AddError(nameof(Phone), _phoneFormatError);
            }
        }
    }
}
