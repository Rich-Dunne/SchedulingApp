
namespace SchedulingApp.Utilities
{
    public static class LoginValidator
    {
        public const string USERNAME  = "test";
        public const string PASSWORD = "test";

        public static bool UseForeignLanguage { get; set; } = false;
        private static string _usernameRequiredErrorMessage { get; } = "Username required";
        private static string _altUsernameRequiredErrorMessage { get; } = "Usuario requerido";

        private static string _passwordRequiredErrorMessage { get; } = "Password required";
        private static string _altPasswordRequiredErrorMessage { get; } = "Contraseña required";

        private static string _wrongUsernameErrorMessage { get; } = "Username not found";
        private static string _altWrongUsernameErrorMessage { get; } = "Usuario no encontrado";

        private static string _wrongPasswordErrorMessage { get; } = "Incorrect password";
        private static string _altWrongPasswordErrorMessage { get; } = "Contraseña incorrecta";

        public static bool UsernameIsValid(string username, out string usernameErrorMessage)
        {
            usernameErrorMessage = "";
            if (string.IsNullOrWhiteSpace(username))
            {
                usernameErrorMessage = UseForeignLanguage ? _altUsernameRequiredErrorMessage : _usernameRequiredErrorMessage;
                return false;
            }

            return true;
        }

        public static bool PasswordIsValid(string password, out string passwordErrorMessage)
        {
            passwordErrorMessage = "";
            if (string.IsNullOrWhiteSpace(password))
            {
                passwordErrorMessage = UseForeignLanguage ? _altPasswordRequiredErrorMessage : _passwordRequiredErrorMessage;
                return false;
            }

            return true;
        }

        public static bool ValidateLogin(string username, string password, out string usernameErrorMessage, out string passwordErrorMessage)
        {
            usernameErrorMessage = "";
            passwordErrorMessage = "";

            var usernameIsValid = UsernameIsValid(username, out string usrErrorMessage);
            if(!usernameIsValid)
            {
                usernameErrorMessage = usrErrorMessage;
            }

            var passwordIsValid = PasswordIsValid(password, out string pwdErrorMessage);
            if(!passwordIsValid)
            {
                passwordErrorMessage = pwdErrorMessage;
            }

            if(!usernameIsValid || !passwordIsValid)
            {
                return false;
            }

            if(username != USERNAME)
            {
                usernameErrorMessage = UseForeignLanguage ? _altWrongUsernameErrorMessage : _wrongUsernameErrorMessage;
                return false;
            }

            if(username == USERNAME && password != PASSWORD)
            {
                passwordErrorMessage = UseForeignLanguage ? _altWrongPasswordErrorMessage : _wrongPasswordErrorMessage;
                return false;
            }

            return true;
        }
    }
}
