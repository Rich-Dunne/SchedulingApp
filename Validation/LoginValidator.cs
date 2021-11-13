using SchedulingApp.Data;
using SchedulingApp.Models;
using SchedulingApp.Utilities;
using System.Collections.Generic;
using System.Diagnostics;

namespace SchedulingApp.Validation
{
    public static class LoginValidator
    {
        public static bool UseForeignLanguage { get; set; } = false;

        public static LoginError ValidateUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return new UsernameLoginError(ErrorMessageType.Required, UseForeignLanguage);
            }

            return null;
        }

        public static LoginError ValidatePassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                return new PasswordLoginError(ErrorMessageType.Required, UseForeignLanguage);
            }

            return null;
        }

        public static List<LoginError> ValidateLogin(User user)
        {
            List<LoginError> errors = new List<LoginError>();

            var usernameError = ValidateUsername(user.UserName);
            if(usernameError is not null)
            {
                errors.Add(usernameError);
            }

            var passwordError = ValidatePassword(user.Password);
            if(passwordError is not null)
            {
                errors.Add(passwordError);
            }

            if(errors.Count > 0)
            {
                return errors;
            }

            var newUser = new DataAccess().SelectUser(user.UserName);
            if(newUser is null)
            {
                errors.Add(new UsernameLoginError(ErrorMessageType.Incorrect, UseForeignLanguage));
                return errors;
            }

            if(user.Password != newUser.Password)
            {
                errors.Add(new PasswordLoginError(ErrorMessageType.Incorrect, UseForeignLanguage));
                return errors;
            }

            return errors;
        }
    }
}
