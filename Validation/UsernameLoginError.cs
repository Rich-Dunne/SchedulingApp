using SchedulingApp.Utilities;
using System;

namespace SchedulingApp.Validation
{
    public class UsernameLoginError : LoginError
    {
        private static string _requiredErrorMessage = "Username required.";
        private static string _translatedRequiredErrorMessage = "Usuario requerido";
        private static string _wrongErrorMessage = "Username not found";
        private static string _translatedWrongErrorMessage = "Usuario no encontrado";

        public UsernameLoginError(ErrorMessageType messageType, bool translated)
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
