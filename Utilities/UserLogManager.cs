using System;
using System.IO;

namespace SchedulingApp.Utilities
{
    public static class UserLogManager
    {
        private static string _currentDirectory = Directory.GetCurrentDirectory();
        private const string _filename = "userlog.txt";

        public static void LogValidSignIn(string username) =>  File.AppendAllText($"{_currentDirectory}\\{_filename}", $"[{DateTime.UtcNow} UTC]: User '{username}' signed in successfully.\n"); // This lambda calls the single File.AppendAllText function in order log the user sign-in to a text file.  This expression saved a couple lines of code and increased readability for me
    
        public static void LogInvalidSignIn(string username, string errors) => File.AppendAllText($"{_currentDirectory}\\{_filename}", $"[{DateTime.UtcNow} UTC]: Failed login attempt with username '{username}'.  Errors: {errors}\n");
    }
}
