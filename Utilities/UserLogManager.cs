using System;
using System.IO;

namespace SchedulingApp.Utilities
{
    public static class UserLogManager
    {
        private static string _currentDirectory = Directory.GetCurrentDirectory();
        private const string _filename = "userlog.txt";

        public static void LogUserSignIn(string username) =>  File.AppendAllText($"{_currentDirectory}\\{_filename}", $"[{DateTime.Now}]: User '{username}' signed in.\n");

    }
}
