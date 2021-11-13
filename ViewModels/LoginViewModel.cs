using SchedulingApp.Data;
using SchedulingApp.Models;
using SchedulingApp.Utilities;
using SchedulingApp.Validation;
using System;
using System.Collections.Generic;
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

                var loginError = LoginValidator.ValidateUsername(value);
                if(loginError is not null)
                {
                    HasErrors = true;
                    UsernameErrorMessage = loginError.ErrorMessage;
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
                _password = value.ToSHA256();
                OnPropertyChanged();

                var passwordError = LoginValidator.ValidatePassword(value);
                if (passwordError is not null)
                {
                    HasErrors = true;
                    PasswordErrorMessage = passwordError.ErrorMessage;
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
            SignInCommand = new RelayCommand(o => { SignIn(); });
            LoginValidator.UseForeignLanguage = UserIsOutsideUSA();
            if(LoginValidator.UseForeignLanguage)
            {
                UpdateLoginLanguage();
            }
        }

        private bool UserIsOutsideUSA()
        {
            var languageName = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
            return languageName == "es";
        }

        private void UpdateLoginLanguage()
        {
            LoginHeaderText = "Scheduling App Acceso";
            UsernameLabel = "Nombre de usuario";
            PasswordLabel = "Contraseña";
            ButtonText = "Iniciar sesión";
        }

        private void SignIn()
        {
            var newUser = new User(Username, Password);
            var loginErrors = LoginValidator.ValidateLogin(newUser);
            if (loginErrors.Count > 0)
            {
                Debug.WriteLine($"Login information is not valid.");
                HasErrors = true;

                GetErrorMessages(loginErrors);
                return;
            }

            newUser = new DataAccess().SelectUser(newUser);
            UserLogManager.LogValidSignIn(newUser.UserName);

            NavigationService.MainViewModel.CurrentUser = newUser;
            ResetProperties();

            NavigationService.NavigateTo(View.Home);
            HasErrors = false;
        }

        private void GetErrorMessages(List<LoginError> loginErrors)
        {
            var usernameError = loginErrors.FirstOrDefault(x => x.GetType() == typeof(UsernameLoginError));
            if (!string.IsNullOrWhiteSpace(usernameError?.ErrorMessage))
            {
                UsernameErrorMessage = usernameError.ErrorMessage;
                UsernameErrorVisibility = "Visible";
            }

            var passwordError = loginErrors.FirstOrDefault(x => x.GetType() == typeof(PasswordLoginError));
            if (!string.IsNullOrWhiteSpace(passwordError?.ErrorMessage))
            {
                PasswordErrorMessage = passwordError.ErrorMessage;
                PasswordErrorVisibility = "Visible";
            }

            string errors = "";
            if (UsernameErrorMessage != "")
            {
                errors += UsernameErrorMessage;
            }
            if (errors != "" && PasswordErrorMessage != "")
            {
                errors += $", {PasswordErrorMessage}";
            }
            else if (PasswordErrorMessage != "")
            {
                errors += PasswordErrorMessage;
            }
            UserLogManager.LogInvalidSignIn(Username, errors);
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
