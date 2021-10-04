using SchedulingApp.Models;
using SchedulingApp.Utilities;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

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

        private string _username = "";
        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged();

                HasErrors = !LoginValidator.UsernameIsValid(value, out string usernameErrorMessage);
                if(!string.IsNullOrWhiteSpace(usernameErrorMessage))
                {
                    UsernameErrorMessage = usernameErrorMessage;
                    UsernameErrorVisibility = "Visible";
                }
                else
                {
                    UsernameErrorMessage = "";
                    UsernameErrorVisibility = "Collapsed";
                }
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

                HasErrors = !LoginValidator.PasswordIsValid(value, out string passwordErrorMessage);
                if (!string.IsNullOrWhiteSpace(passwordErrorMessage))
                {
                    PasswordErrorMessage = passwordErrorMessage;
                    PasswordErrorVisibility = "Visible";
                }
                else
                {
                    PasswordErrorMessage = "";
                    PasswordErrorVisibility = "Collapsed";
                }
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

        #region Validation Properties
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
        #endregion

        #region Commands
        public RelayCommand SignInCommand { get; set; }
        #endregion

        public LoginViewModel()
        {
            SignInCommand = new RelayCommand(o => { SignIn((MainViewModel)o); });
            LoginValidator.UseForeignLanguage = UserIsOutsideUSA();
            if(UserIsOutsideUSA())
            {
                UpdateLoginLanguage();
            }
        }

        private bool UserIsOutsideUSA()
        {
            var languageName = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
            var regKeyGeoId = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Control Panel\International\Geo");
            var geoID = (string)regKeyGeoId.GetValue("Nation");
            var allRegions = CultureInfo.GetCultures(CultureTypes.SpecificCultures).Select(x => new RegionInfo(x.ToString()));
            var regionInfo = allRegions.FirstOrDefault(r => r.GeoId == int.Parse(geoID));


            return regionInfo.DisplayName != "United States" || languageName != "en";
        }

        private void UpdateLoginLanguage()
        {
            LoginHeaderText = "Scheduling App Acceso";
            UsernameLabel = "Nombre de usuario";
            PasswordLabel = "Contraseña";
            ButtonText = "Iniciar sesión";
        }

        private void SignIn(MainViewModel mainViewModel)
        {
            HasErrors = !LoginValidator.ValidateLogin(Username, Password, out string usernameErrorMessage, out string passwordErrorMessage);
            if (HasErrors)
            {
                if(!string.IsNullOrWhiteSpace(usernameErrorMessage))
                {
                    UsernameErrorMessage = usernameErrorMessage;
                    UsernameErrorVisibility = "Visible";
                }

                if (!string.IsNullOrWhiteSpace(passwordErrorMessage))
                {
                    PasswordErrorMessage = passwordErrorMessage;
                    PasswordErrorVisibility = "Visible";
                }
                Debug.WriteLine($"Login information is not valid.");

                string errors = "";
                if(UsernameErrorMessage != "")
                {
                    errors += UsernameErrorMessage;
                }
                if(errors != "" && PasswordErrorMessage != "")
                {
                    errors += $", {PasswordErrorMessage}";
                }
                else if (PasswordErrorMessage != "")
                {
                    errors += PasswordErrorMessage;
                }
                UserLogManager.LogInvalidSignIn(Username, errors);
                return;
            }

            UserLogManager.LogValidSignIn(Username);

            mainViewModel.CurrentUser = DataAccess.SelectUser(Username);
            mainViewModel.CurrentView = mainViewModel.HomeViewModel;
            ResetProperties();
        }

        private void ResetProperties()
        {
            Username = "";
            Password = "";
            UsernameErrorVisibility = "Collapsed";
            PasswordErrorVisibility = "Collapsed";
        }
    }
 }
