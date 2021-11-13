using System.Text.RegularExpressions;

namespace SchedulingApp.Validation
{
    public static class CustomerValidator
    {
        #region RegEx
        private static readonly string _SPACED_NAME_REGEX = "^[a-zA-Z- ]*$";
        private static readonly string _PHONE_NUMBER_REGEX = "^(\\+\\d{1,2}\\s)?\\(?\\d{3}\\)?[\\s.-]?\\d{3}[\\s.-]?\\d{4}$";
        #endregion

        #region Error Messages
        private static readonly string _requiredError = "Required";
        private static readonly string _formatError = "Invalid format";
        private static readonly string _phoneFormatError = "Invalid format (area code required)";
        #endregion

        public static bool ValidateName(string name, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (string.IsNullOrWhiteSpace(name))
            {
                errorMessage = _requiredError;
                return false;
            }
            
            if (!Regex.IsMatch(name, _SPACED_NAME_REGEX))
            {
                errorMessage = _formatError;
                return false;
            }

            return true;
        }

        public static bool ValidateRequired(string address, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (string.IsNullOrWhiteSpace(address))
            {
                errorMessage = _requiredError;
                return false;
            }

            return true;
        }

        public static bool ValidatePhone(string phone, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (string.IsNullOrWhiteSpace(phone))
            {
                errorMessage = _requiredError;
                return false;
            }
            
            if (!Regex.IsMatch(phone, _PHONE_NUMBER_REGEX))
            {
                errorMessage = _phoneFormatError;
                return false;
            }

            return true;
        }
    }
}
