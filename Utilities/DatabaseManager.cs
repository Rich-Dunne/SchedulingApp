using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text;

namespace SchedulingApp.Utilities
{
    public static class DatabaseManager
    {
        private static MySqlConnection _connection;
        private static string _server = "127.0.0.1";
        private static string _database = "client_schedule";
        private static string _userId = "sqlUser";
        private static string _password = "Passw0rd!";

        static DatabaseManager()
        {
            Initialize();
        }

        private static void Initialize()
        {
            var connectionString = $"SERVER={_server};DATABASE={_database};UID={_userId};PASSWORD={_password};";
            _connection = new MySqlConnection(connectionString);
        }

        private static bool OpenConnection()
        {
            try
            {
                if (_connection.State == ConnectionState.Closed)
                {
                    _connection.Open();
                    return true;
                }
            }
            catch (MySqlException exception)
            {
                switch (exception.Number)
                {
                    case 0:
                        Debug.WriteLine("Cannot connect to server.");
                        break;

                    case 1045:
                        Debug.WriteLine("Invalid username/password.");
                        break;
                }
                return false;
            }
            return false;
        }

        private static bool CloseConnection()
        {
            try
            {
                _connection.Close();
                return true;
            }
            catch (MySqlException exception)
            {
                Debug.WriteLine(exception.Message);
                return false;
            }
        }

        //public static void Ping()
        //{
        //    if (!OpenConnection())
        //    {
        //        return;
        //    }

        //    Debug.WriteLine($"Databse ping successful: {_connection.Ping()}");

        //    CloseConnection();
        //}

        //public static void InsertUser(string username, string password)
        //{
        //    if (!OpenConnection())
        //    {
        //        return;
        //    }

        //    var command = _connection.CreateCommand();
        //    command.CommandText = "INSERT INTO user(userName, password, active, createDate, createdBy, lastUpdate, lastUpdateBy) VALUES(@userName, @password, @active, @createDate, @createdBy, @lastUpdate, @lastUpdateBy)";
        //    command.Parameters.AddWithValue("@userName", username);
        //    command.Parameters.AddWithValue("@password", password);
        //    command.Parameters.AddWithValue("@active", 1);
        //    command.Parameters.AddWithValue("@createDate", $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}");
        //    command.Parameters.AddWithValue("@createdBy", username);
        //    command.Parameters.AddWithValue("@lastUpdate", $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}");
        //    command.Parameters.AddWithValue("@lastUpdateBy", username);

        //    command.ExecuteNonQuery();

        //    CloseConnection();
        //}

        public static bool FindUsername(string username)
        {
            if (!OpenConnection())
            {
                return false;
            }

            var command = _connection.CreateCommand();
            command.CommandText = "SELECT count(*) from user WHERE userName = @userName";
            command.Parameters.AddWithValue("@username", username);
            var count = command.ExecuteScalar();
            CloseConnection();

            if (count == null)
            {
                return false;
            }

            int matches = Convert.ToInt32(count);
            //Console.WriteLine($"Number of matching usernames: {matches}");

            return matches > 0;
        }

        public static bool FindValidUser(string username, string password)
        {
            if (!OpenConnection())
            {
                return false;
            }

            var command = _connection.CreateCommand();
            command.CommandText = "SELECT count(*) from user WHERE userName = @userName and password=@password";
            command.Parameters.AddWithValue("@username", username);
            command.Parameters.AddWithValue("@password", password);
            var count = command.ExecuteScalar();
            CloseConnection();

            if (count == null)
            {
                return false;
            }

            int matches = Convert.ToInt32(count);
            //Console.WriteLine($"Number of matching accounts: {matches}");

            return matches > 0;
        }

        public static void Update(string query)
        {
            if (!OpenConnection())
            {
                return;
            }

            var command = new MySqlCommand(query, _connection);
            command.ExecuteNonQuery();

            CloseConnection();
        }

        public static void Delete(string query)
        {
            if (!OpenConnection())
            {
                return;
            }

            MySqlCommand command = new MySqlCommand(query, _connection);
            command.ExecuteNonQuery();

            CloseConnection();
        }

        public static List<string> Select()
        {
            return new List<string>();
        }

        //Count statement
        public static int Count()
        {
            return 0;
        }

        //Backup
        public static void Backup()
        {
        }

        //Restore
        public static void Restore()
        {
        }
    }
}
