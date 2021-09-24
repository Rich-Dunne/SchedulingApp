using SchedulingApp.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SchedulingApp.ViewModels
{
    public class LoginViewModel : ObservableObject
    {
        #region Form Properties
        private string _usernameLabel = "Username";
        public string UsernameLabel
        {
            get => _usernameLabel;
            set
            {
                _usernameLabel = value;
                OnPropertyChanged();
            }
        }

        private string _usernameTextbox = "";
        public string UsernameTextbox
        {
            get => _usernameTextbox;
            set
            {
                _usernameTextbox = value;
                OnPropertyChanged();
                UsernameIsValid(_usernameTextbox);
            }
        }

        private string _password = "";
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
                PasswordIsValid(_password);
            }
        }

        private string _passwordLabel = "Password";
        public string PasswordLabel
        {
            get => _passwordLabel;
            set
            {
                _passwordLabel = value;
                OnPropertyChanged();
            }
        }

        private string _buttonText = "Sign In";
        public string ButtonText
        {
            get => _buttonText;
            set
            {
                _buttonText = value;
                OnPropertyChanged();
            }
        }

        private string _loginHeaderText = "Scheduling App Login";
        public string LoginHeaderText
        {
            get => _loginHeaderText;
            set
            {
                _loginHeaderText = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Validation Properties
        public string USERNAME { get; } = "test";
        public string PASSWORD { get; } = "test";

        private bool _hasErrors = false;
        public bool HasErrors
        {
            get => _hasErrors;
            set
            {
                _hasErrors = value;
                OnPropertyChanged();
            }
        }

        private string _usernameErrorMessage = "";
        public string UsernameErrorMessage
        {
            get => _usernameErrorMessage;
            set
            {
                _usernameErrorMessage = value;
                OnPropertyChanged();
            }
        }

        private string _passwordErrorMessage = "";
        public string PasswordErrorMessage
        {
            get => _passwordErrorMessage;
            set
            {
                _passwordErrorMessage = value;
                OnPropertyChanged();
            }
        }

        private string _requiredErrorMessage = "Required";
        public string RequiredErrorMessage
        {
            get => _requiredErrorMessage;
            set
            {
                _requiredErrorMessage = value;
                OnPropertyChanged();
            }
        }

        private string _wrongUsernameErrorMessage = "Username not found";
        public string WrongUsernameErrorMessage
        {
            get => _wrongUsernameErrorMessage;
            set
            {
                _wrongUsernameErrorMessage = value;
                OnPropertyChanged();
            }
        }

        private string _wrongPasswordErrorMessage = "Incorrect password";
        public string WrongPasswordErrorMessage
        {
            get => _wrongPasswordErrorMessage;
            set
            {
                _wrongPasswordErrorMessage = value;
                OnPropertyChanged();
            }
        }

        private string _usernameErrorVisibility = "Collapsed";
        public string UsernameErrorVisibility
        {
            get => _usernameErrorVisibility;
            set
            {
                _usernameErrorVisibility = value;
                OnPropertyChanged();
            }
        }

        private string _passwordErrorVisibility = "Collapsed";
        public string PasswordErrorVisibility
        {
            get => _passwordErrorVisibility;
            set
            {
                _passwordErrorVisibility = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Commands
        public RelayCommand SignInCommand { get; set; }
        #endregion

        public LoginViewModel()
        {
            Debug.WriteLine($"Login view initialized.");

            SignInCommand = new RelayCommand(o => { SignIn((MainViewModel)o); });
            if (UserIsOutsideUSA())
            {
                UpdateLoginLanguage();
            }
        }

        private bool UserIsOutsideUSA()
        {
            var regKeyGeoId = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Control Panel\International\Geo");
            var geoID = (string)regKeyGeoId.GetValue("Nation");
            var allRegions = CultureInfo.GetCultures(CultureTypes.SpecificCultures).Select(x => new RegionInfo(x.ToString()));
            var regionInfo = allRegions.FirstOrDefault(r => r.GeoId == int.Parse(geoID));

            return regionInfo.DisplayName != "United States";
        }

        private void UpdateLoginLanguage()
        {
            LoginHeaderText = "Scheduling App Acceso";
            UsernameLabel = "Nombre de usuario";
            PasswordLabel = "Contraseña";
            ButtonText = "Iniciar sesión";
            RequiredErrorMessage = "Requerido";
            WrongUsernameErrorMessage = "Usuario no encontrado";
            WrongPasswordErrorMessage = "Contraseña incorrecta";
        }

        private void SignIn(MainViewModel mainViewModel)
        {
            ValidateLogin(UsernameTextbox, Password);
            if (HasErrors)
            {
                Debug.WriteLine($"Login information is not valid.");
                return;
            }
            mainViewModel.CurrentView = mainViewModel.HomeViewModel;
        }

        public bool UsernameIsValid(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                UsernameErrorMessage = RequiredErrorMessage;
                UsernameErrorVisibility = "Visible";
                return false;
            }

            UsernameErrorVisibility = "Collapsed";
            return true;
        }

        public bool PasswordIsValid(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                PasswordErrorMessage = RequiredErrorMessage;
                PasswordErrorVisibility = "Visible";
                return false;
            }

            PasswordErrorVisibility = "Collapsed";
            return true;
        }

        public void ValidateLogin(string username, string password)
        {
            HasErrors = false;
            if (!UsernameIsValid(username))
            {
                HasErrors = true;
            }

            if (!PasswordIsValid(password))
            {
                HasErrors = true;
            }

            if(username != USERNAME)
            {
                UsernameErrorMessage = WrongUsernameErrorMessage;
                UsernameErrorVisibility = "Visible";
                HasErrors = true;
            }

            if (username == USERNAME && password != PASSWORD)
            {
                PasswordErrorMessage = WrongPasswordErrorMessage;
                PasswordErrorVisibility = "Visible";
                HasErrors = true;
            }
        }
    }
 }
