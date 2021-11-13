using SchedulingApp.Utilities;
using System;

namespace SchedulingApp.Validation
{
    public class PasswordLoginError : LoginError
    {
        private static string _requiredErrorMessage = "Password required";
        private static string _translatedRequiredErrorMessage = "Contraseña required";
        private static string _wrongErrorMessage = "Incorrect password";
        private static string _translatedWrongErrorMessage = "Contraseña incorrecta";

        public PasswordLoginError(ErrorMessageType messageType, bool translated)
        {
            switch (messageType)
            {
                case ErrorMessageType.Required:
                    ErrorMessage = translated ? _translatedRequiredErrorMessage : _requiredErrorMessage;
                    break;

                case ErrorMessageType.Incorrect:
                    ErrorMessage = translated ? _translatedWrongErrorMessage : _wrongErrorMessage;
                    break;
            }
        }
    }
}
