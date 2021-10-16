using MySql.Data.MySqlClient;
using SchedulingApp.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;

namespace SchedulingApp.Utilities
{
    public static class DataAccess
    {
        private static MySqlConnection _connection;
        private static string _server = "127.0.0.1";
        private static string _database = "client_schedule";
        private static string _userId = "sqlUser";
        private static string _password = "Passw0rd!";

        static DataAccess() => _connection = new MySqlConnection($"SERVER={_server};DATABASE={_database};UID={_userId};PASSWORD={_password};"); // This lambda assigns a new MySqlConnection for using the database for the duration the app is running.  This expression saved a couple lines of code and increased readability for me

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

        #region User Queries
        public static void InsertUser(string username, string password)
        {
            if (!OpenConnection())
            {
                return;
            }

            var command = _connection.CreateCommand();
            command.CommandText = "INSERT INTO user(userName, password, active, createDate, createdBy, lastUpdate, lastUpdateBy) VALUES(@userName, @password, @active, @createDate, @createdBy, @lastUpdate, @lastUpdateBy)";
            command.Parameters.AddWithValue("@userName", username);
            command.Parameters.AddWithValue("@password", password);
            command.Parameters.AddWithValue("@active", 1);
            command.Parameters.AddWithValue("@createDate", $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}");
            command.Parameters.AddWithValue("@createdBy", username);
            command.Parameters.AddWithValue("@lastUpdate", $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}");
            command.Parameters.AddWithValue("@lastUpdateBy", username);

            command.ExecuteNonQuery();

            CloseConnection();
        }

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
            command.CommandText = "SELECT count(*) from user WHERE userName = @userName and password = @password";
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

        public static User SelectUser(string username)
        {
            if (!OpenConnection())
            {
                return null;
            }

            var command = _connection.CreateCommand();
            command.CommandText = "SELECT * FROM client_schedule.user WHERE userName = @username LIMIT 1";
            command.Parameters.AddWithValue("@username", username);
            MySqlDataReader reader = command.ExecuteReader();

            User result = null;
            while (reader.Read())
            {
                result = new User()
                {
                    UserId = reader.GetInt32(0),
                    UserName = reader.GetString(1)
                };
            }

            CloseConnection();

            return result;
        }

        public static User SelectUser(int userId)
        {
            if (!OpenConnection())
            {
                return null;
            }

            var command = _connection.CreateCommand();
            command.CommandText = "SELECT * FROM client_schedule.user WHERE userId = @userId";
            command.Parameters.AddWithValue("@userId", userId);
            MySqlDataReader reader = command.ExecuteReader();

            User result = null;
            while (reader.Read())
            {
                result = new User()
                {
                    UserId = reader.GetInt32(0),
                    UserName = reader.GetString(1)
                };
            }

            CloseConnection();

            return result;
        }

        public static List<User> SelectAllUsers()
        {
            if (!OpenConnection())
            {
                return null;
            }

            var command = _connection.CreateCommand();
            command.CommandText = "SELECT * FROM user";
            MySqlDataReader reader = command.ExecuteReader();

            var results = new List<User>();
            while (reader.Read())
            {
                var user = new User()
                {
                    UserId = reader.GetInt32(0),
                    UserName = reader.GetString(1),
                    CreateDate = reader.GetDateTime(4).ToLocalTime(),
                    CreatedBy = reader.GetString(5),
                    LastUpdate = reader.GetDateTime(6).ToLocalTime(),
                    LastUpdateBy = reader.GetString(7)
                };

                results.Add(user);
            }

            CloseConnection();

            return results;
        }
        #endregion

        #region Country Queries
        public static long InsertCountry(Country country)
        {
            if (!OpenConnection())
            {
                return 0;
            }

            var command = _connection.CreateCommand();
            command.CommandText = "INSERT INTO country (country, createDate, createdBy, lastUpdate, lastUpdateBy) VALUES (@country, @createDate, @createdBy, @lastUpdate, @lastUpdateBy)";
            command.Parameters.AddWithValue("@country", country.CountryName);
            command.Parameters.AddWithValue("@createDate", DateTime.UtcNow);
            command.Parameters.AddWithValue("@createdBy", NavigationService.MainVM.CurrentUser.UserName);
            command.Parameters.AddWithValue("@lastUpdate", DateTime.UtcNow);
            command.Parameters.AddWithValue("@lastUpdateBy", NavigationService.MainVM.CurrentUser.UserName);
            command.ExecuteNonQuery();
            long idFromInsert = command.LastInsertedId;
            CloseConnection();

            return idFromInsert;
        }

        public static Country SelectCountry(int id)
        {
            if (!OpenConnection())
            {
                return null;
            }

            var command = _connection.CreateCommand();
            command.CommandText = "SELECT * FROM country WHERE countryId = @countryId";
            command.Parameters.AddWithValue("@countryId", id);
            MySqlDataReader reader = command.ExecuteReader();

            Country result = null;
            while (reader.Read())
            {
                result = new Country()
                {
                    CountryId = reader.GetInt32(0),
                    CountryName = reader.GetString(1),
                    CreateDate = reader.GetDateTime(2).ToLocalTime(),
                    CreatedBy = reader.GetString(3),
                    LastUpdate = reader.GetDateTime(4).ToLocalTime(),
                    LastUpdateBy = reader.GetString(5)
                };
            }

            CloseConnection();

            return result;
        }

        public static Country SelectCountry(string country)
        {
            if (!OpenConnection())
            {
                return null;
            }

            var command = _connection.CreateCommand();
            command.CommandText = "SELECT * FROM country WHERE country = @country";
            command.Parameters.AddWithValue("@country", country);
            MySqlDataReader reader = command.ExecuteReader();

            Country result = null;
            while (reader.Read())
            {
                result = new Country()
                {
                    CountryId = reader.GetInt32(0),
                    CountryName = reader.GetString(1),
                    CreateDate = reader.GetDateTime(2).ToLocalTime(),
                    CreatedBy = reader.GetString(3),
                    LastUpdate = reader.GetDateTime(4).ToLocalTime(),
                    LastUpdateBy = reader.GetString(5)
                };
            }

            CloseConnection();

            return result;
        }

        public static int UpdateCountry(Country country)
        {
            if (!OpenConnection())
            {
                return 0;
            }

            var command = _connection.CreateCommand();
            command.CommandText = "UPDATE country SET country = @country, lastUpdate = @lastUpdate, lastUpdateBy = @lastUpdateBy WHERE countryId = @countryId";
            command.Parameters.AddWithValue("@countryId", country.CountryId);
            command.Parameters.AddWithValue("@country", country.CountryName);
            command.Parameters.AddWithValue("@lastUpdate", DateTime.UtcNow);
            command.Parameters.AddWithValue("@lastUpdateBy", NavigationService.MainVM.CurrentUser.UserName);
            var rowsAffected = command.ExecuteNonQuery();
            CloseConnection();

            return rowsAffected;
        }

        public static int RemoveCountry(int countryId)
        {
            if (!OpenConnection())
            {
                return 0;
            }

            var command = _connection.CreateCommand();
            command.CommandText = "DELETE FROM country WHERE countryId = @countryId";
            command.Parameters.AddWithValue("@countryId", countryId);
            var rowsAffected = command.ExecuteNonQuery();
            CloseConnection();

            return rowsAffected;
        }
        #endregion

        #region City Queries
        public static long InsertCity(City city)
        {
            if (!OpenConnection())
            {
                return 0;
            }

            var command = _connection.CreateCommand();
            command.CommandText = "INSERT INTO city (city, countryId, createDate, createdBy, lastUpdate, lastUpdateBy) VALUES (@city, @countryId, @createDate, @createdBy, @lastUpdate, @lastUpdateBy)";
            command.Parameters.AddWithValue("@city", city.CityName);
            command.Parameters.AddWithValue("@countryId", city.CountryId);
            command.Parameters.AddWithValue("@createDate", DateTime.UtcNow);
            command.Parameters.AddWithValue("@createdBy", NavigationService.MainVM.CurrentUser.UserName);
            command.Parameters.AddWithValue("@lastUpdate", DateTime.UtcNow);
            command.Parameters.AddWithValue("@lastUpdateBy", NavigationService.MainVM.CurrentUser.UserName);
            command.ExecuteNonQuery();
            long idFromInsert = command.LastInsertedId;
            CloseConnection();

            return idFromInsert;
        }

        public static City SelectCity(int id)
        {
            if (!OpenConnection())
            {
                return null;
            }

            var command = _connection.CreateCommand();
            command.CommandText = "SELECT * FROM city WHERE cityId = @cityId";
            command.Parameters.AddWithValue("@cityId", id);
            MySqlDataReader reader = command.ExecuteReader();

            City result = null;
            while (reader.Read())
            {
                result = new City()
                {
                    CityId = reader.GetInt32(0),
                    CityName = reader.GetString(1),
                    CountryId = reader.GetInt32(2),
                    CreateDate = reader.GetDateTime(3).ToLocalTime(),
                    CreatedBy = reader.GetString(4),
                    LastUpdate = reader.GetDateTime(5).ToLocalTime(),
                    LastUpdateBy = reader.GetString(6)
                };
            }

            CloseConnection();

            return result;
        }

        public static City SelectCity(string city, int countryId)
        {
            if (!OpenConnection())
            {
                return null;
            }

            var command = _connection.CreateCommand();
            command.CommandText = "SELECT * FROM city WHERE city = @city AND countryId = @countryId";
            command.Parameters.AddWithValue("@city", city);
            command.Parameters.AddWithValue("@countryId", countryId);
            MySqlDataReader reader = command.ExecuteReader();

            City result = null;
            while (reader.Read())
            {
                result = new City()
                {
                    CityId = reader.GetInt32(0),
                    CityName = reader.GetString(1),
                    CountryId = reader.GetInt32(2),
                    CreateDate = reader.GetDateTime(3).ToLocalTime(),
                    CreatedBy = reader.GetString(4),
                    LastUpdate = reader.GetDateTime(5).ToLocalTime(),
                    LastUpdateBy = reader.GetString(6)
                };
            }

            CloseConnection();

            return result;
        }

        public static int UpdateCity(City city)
        {
            if (!OpenConnection())
            {
                return 0;
            }

            var command = _connection.CreateCommand();
            command.CommandText = "UPDATE city SET city = @city, countryId = @countryId, lastUpdate = @lastUpdate, lastUpdateBy = @lastUpdateBy WHERE cityId = @cityId";
            command.Parameters.AddWithValue("@cityId", city.CityId);
            command.Parameters.AddWithValue("@city", city.CityName);
            command.Parameters.AddWithValue("@countryId", city.CountryId);
            command.Parameters.AddWithValue("@lastUpdate", DateTime.UtcNow);
            command.Parameters.AddWithValue("@lastUpdateBy", NavigationService.MainVM.CurrentUser.UserName);
            var rowsAffected = command.ExecuteNonQuery();
            CloseConnection();

            return rowsAffected;
        }

        public static int RemoveCity(int cityId)
        {
            if (!OpenConnection())
            {
                return 0;
            }

            var command = _connection.CreateCommand();
            command.CommandText = "DELETE FROM city WHERE cityId = @cityId";
            command.Parameters.AddWithValue("@cityId", cityId);
            var rowsAffected = command.ExecuteNonQuery();
            CloseConnection();

            return rowsAffected;
        }
        #endregion

        #region Address Queries
        public static long InsertAddress(Address address)
        {
            if (!OpenConnection())
            {
                return 0;
            }

            var command = _connection.CreateCommand();
            command.CommandText = "INSERT INTO address (address, address2, cityId, postalCode, phone, createDate, createdBy, lastUpdate, lastUpdateBy) VALUES (@address, @address2, @cityId, @postalCode, @phone, @createDate, @createdBy, @lastUpdate, @lastUpdateBy)";
            command.Parameters.AddWithValue("@address", address.Address1);
            command.Parameters.AddWithValue("@address2", address.Address2);
            command.Parameters.AddWithValue("@cityId", address.CityId);
            command.Parameters.AddWithValue("@postalCode", address.PostalCode);
            command.Parameters.AddWithValue("@phone", address.Phone);
            command.Parameters.AddWithValue("@createDate", DateTime.UtcNow);
            command.Parameters.AddWithValue("@createdBy", NavigationService.MainVM.CurrentUser.UserName);
            command.Parameters.AddWithValue("@lastUpdate", DateTime.UtcNow);
            command.Parameters.AddWithValue("@lastUpdateBy", NavigationService.MainVM.CurrentUser.UserName);
            command.ExecuteNonQuery();
            long idFromInsert = command.LastInsertedId;
            CloseConnection();

            return idFromInsert;
        }

        public static Address SelectAddress(int id)
        {
            if (!OpenConnection())
            {
                return null;
            }

            var command = _connection.CreateCommand();
            command.CommandText = "SELECT * FROM address WHERE addressId = @addressId";
            command.Parameters.AddWithValue("@addressId", id);
            MySqlDataReader reader = command.ExecuteReader();

            Address result = null;
            while (reader.Read())
            {
                result = new Address()
                {
                    AddressId = reader.GetInt32(0),
                    Address1 = reader.GetString(1),
                    Address2 = reader.GetString(2),
                    CityId = reader.GetInt32(3),
                    PostalCode = reader.GetInt32(4).ToString(),
                    Phone = reader.GetString(5),
                    CreateDate = reader.GetDateTime(6).ToLocalTime(),
                    CreatedBy = reader.GetString(7),
                    LastUpdate = reader.GetDateTime(8).ToLocalTime(),
                    LastUpdateBy = reader.GetString(9)
                };
            }

            CloseConnection();

            return result;
        }

        public static Address SelectAddress(string address, string address2, string postalCode, int cityId)
        {
            if (!OpenConnection())
            {
                return null;
            }

            var command = _connection.CreateCommand();
            command.CommandText = "SELECT * FROM address WHERE address = @address AND address2 = @address2 AND postalCode = @postalCode AND cityId = @cityId";
            command.Parameters.AddWithValue("@address", address);
            command.Parameters.AddWithValue("@address2", address2);
            command.Parameters.AddWithValue("@postalCode", postalCode);
            command.Parameters.AddWithValue("@cityId", cityId);
            MySqlDataReader reader = command.ExecuteReader();

            Address result = null;
            while (reader.Read())
            {
                result = new Address()
                {
                    AddressId = reader.GetInt32(0),
                    Address1 = reader.GetString(1),
                    Address2 = reader.GetString(2),
                    CityId = reader.GetInt32(3),
                    PostalCode = reader.GetInt32(4).ToString(),
                    Phone = reader.GetString(5),
                    CreateDate = reader.GetDateTime(6).ToLocalTime(),
                    CreatedBy = reader.GetString(7),
                    LastUpdate = reader.GetDateTime(8).ToLocalTime(),
                    LastUpdateBy = reader.GetString(9)
                };
            }

            CloseConnection();

            return result;
        }

        public static int UpdateAddress(Address address)
        {
            if (!OpenConnection())
            {
                return 0;
            }

            var command = _connection.CreateCommand();
            command.CommandText = "UPDATE address SET address = @address, address2 = @address2, cityId = @cityId, postalCode = @postalCode, phone = @phone, lastUpdate = @lastUpdate, lastUpdateBy = @lastUpdateBy WHERE addressId = @addressId";
            command.Parameters.AddWithValue("@addressId", address.AddressId);
            command.Parameters.AddWithValue("@address", address.Address1);
            command.Parameters.AddWithValue("@address2", address.Address2);
            command.Parameters.AddWithValue("@cityId", address.CityId);
            command.Parameters.AddWithValue("@postalCode", address.PostalCode);
            command.Parameters.AddWithValue("@phone", address.Phone);
            command.Parameters.AddWithValue("@lastUpdate", DateTime.UtcNow);
            command.Parameters.AddWithValue("@lastUpdateBy", NavigationService.MainVM.CurrentUser.UserName);
            var rowsAffected = command.ExecuteNonQuery();
            CloseConnection();

            return rowsAffected;
        }

        public static int RemoveAddress(int addressId)
        {
            if (!OpenConnection())
            {
                return 0;
            }

            var command = _connection.CreateCommand();
            command.CommandText = "DELETE FROM address WHERE addressId = @addressId";
            command.Parameters.AddWithValue("@addressId", addressId);
            var rowsAffected = command.ExecuteNonQuery();
            CloseConnection();

            return rowsAffected;
        }
        #endregion

        #region Customer Queries
        public static long InsertCustomer(Customer customer)
        {
            if (!OpenConnection())
            {
                return 0;
            }

            var command = _connection.CreateCommand();
            command.CommandText = "INSERT INTO customer (customerName, addressId, active, createDate, createdBy, lastUpdate, lastUpdateBy) VALUES (@customerName, @addressId, @active, @createDate, @createdBy, @lastUpdate, @lastUpdateBy)";
            command.Parameters.AddWithValue("@customerName", customer.CustomerName);
            command.Parameters.AddWithValue("@addressId", customer.AddressId);
            command.Parameters.AddWithValue("@active", Convert.ToInt32(customer.Active));
            command.Parameters.AddWithValue("@createDate", DateTime.UtcNow);
            command.Parameters.AddWithValue("@createdBy", NavigationService.MainVM.CurrentUser.UserName);
            command.Parameters.AddWithValue("@lastUpdate", DateTime.UtcNow);
            command.Parameters.AddWithValue("@lastUpdateBy", NavigationService.MainVM.CurrentUser.UserName);
            command.ExecuteNonQuery();
            long idFromInsert = command.LastInsertedId;
            CloseConnection();

            return idFromInsert;
        }

        public static List<Customer> SelectAllCustomers()
        {
            if (!OpenConnection())
            {
                return null;
            }

            var command = _connection.CreateCommand();
            command.CommandText = "SELECT * FROM customer";
            MySqlDataReader reader = command.ExecuteReader();
            //Debug.WriteLine($"{reader.GetName(0)}\t {reader.GetName(1)}\t\t {reader.GetName(2)}");

            var results = new List<Customer>();
            while (reader.Read())
            {
                //Debug.WriteLine($"{reader.GetInt32(0)}\t\t\t {reader.GetString(1)}\t\t {reader.GetString(2)}");
                var customer = new Customer()
                {
                    CustomerId = reader.GetInt32(0),
                    CustomerName = reader.GetString(1),
                    AddressId = reader.GetInt32(2),
                    CreateDate = reader.GetDateTime(4),
                    CreatedBy = reader.GetString(5),
                    LastUpdate = reader.GetDateTime(6),
                    LastUpdateBy = reader.GetString(7)
                };

                results.Add(customer);
            }

            CloseConnection();

            return results;
        }

        public static Customer SelectCustomer(int id)
        {
            if (!OpenConnection())
            {
                return null;
            }

            var command = _connection.CreateCommand();
            command.CommandText = "SELECT * FROM customer WHERE customerId = @customerId";
            command.Parameters.AddWithValue("@customerId", id);
            MySqlDataReader reader = command.ExecuteReader();

            Customer result = null;
            while (reader.Read())
            {
                result = new Customer()
                {
                    CustomerId = reader.GetInt32(0),
                    CustomerName = reader.GetString(1),
                    AddressId = reader.GetInt32(2),
                    CreateDate = reader.GetDateTime(4).ToLocalTime(),
                    CreatedBy = reader.GetString(5),
                    LastUpdate = reader.GetDateTime(6).ToLocalTime(),
                    LastUpdateBy = reader.GetString(7)
                };
            }

            CloseConnection();
            return result;
        }

        public static Customer SelectCustomer(string customerName)
        {
            if (!OpenConnection())
            {
                return null;
            }

            var command = _connection.CreateCommand();
            command.CommandText = "SELECT * FROM customer WHERE customerName = @customerName LIMIT 1";
            command.Parameters.AddWithValue("@customerName", customerName);
            MySqlDataReader reader = command.ExecuteReader();

            Customer result = null;
            while (reader.Read())
            {
                result = new Customer()
                {
                    CustomerId = reader.GetInt32(0),
                    CustomerName = reader.GetString(1),
                    AddressId = reader.GetInt32(2),
                    CreateDate = reader.GetDateTime(4).ToLocalTime(),
                    CreatedBy = reader.GetString(5),
                    LastUpdate = reader.GetDateTime(6).ToLocalTime(),
                    LastUpdateBy = reader.GetString(7)
                };
            }

            CloseConnection();

            return result;
        }

        public static Customer SelectCustomer(string customerName, int addressId)
        {
            if (!OpenConnection())
            {
                return null;
            }

            var command = _connection.CreateCommand();
            command.CommandText = "SELECT * FROM customer WHERE customerName = @customerName AND addressId = @addressId";
            command.Parameters.AddWithValue("@customerName", customerName);
            command.Parameters.AddWithValue("@addressId", addressId);
            MySqlDataReader reader = command.ExecuteReader();

            Customer result = null;
            while (reader.Read())
            {
                result = new Customer()
                {
                    CustomerId = reader.GetInt32(0),
                    CustomerName = reader.GetString(1),
                    AddressId = reader.GetInt32(2),
                    CreateDate = reader.GetDateTime(4).ToLocalTime(),
                    CreatedBy = reader.GetString(5),
                    LastUpdate = reader.GetDateTime(6).ToLocalTime(),
                    LastUpdateBy = reader.GetString(7)
                };
            }

            CloseConnection();

            return result;
        }

        public static int UpdateCustomer(Customer customer)
        {
            if (!OpenConnection())
            {
                return 0;
            }

            var command = _connection.CreateCommand();
            command.CommandText = "UPDATE customer SET customerName = @customerName, addressId = @addressId, lastUpdate = @lastUpdate, lastUpdateBy = @lastUpdateBy WHERE customerId = @customerId";
            command.Parameters.AddWithValue("@customerId", customer.CustomerId);
            command.Parameters.AddWithValue("@customerName", customer.CustomerName);
            command.Parameters.AddWithValue("@addressId", customer.AddressId);
            command.Parameters.AddWithValue("@lastUpdate", DateTime.UtcNow);
            command.Parameters.AddWithValue("@lastUpdateBy", NavigationService.MainVM.CurrentUser.UserName);
            var rowsAffected = command.ExecuteNonQuery();
            CloseConnection();

            return rowsAffected;
        }

        public static int CountAllCustomers()
        {
            if (!OpenConnection())
            {
                return 0;
            }

            var command = _connection.CreateCommand();
            command.CommandText = "Select count(*) FROM customer";
            var count = command.ExecuteScalar();
            CloseConnection();

            if (count == null)
            {
                return 0;
            }

            return Convert.ToInt32(count);
        }

        public static int RemoveCustomer(Customer customer)
        {
            if (!OpenConnection())
            {
                return 0;
            }

            var command = _connection.CreateCommand();
            command.CommandText = "DELETE FROM customer WHERE customerId = @customerId";
            command.Parameters.AddWithValue("@customerId", customer.CustomerId);
            var rowsAffected = command.ExecuteNonQuery();

            CloseConnection();

            RemoveAddress(customer.AddressId);
            return rowsAffected;
        }
        #endregion

        #region Appointment Queries
        public static long InsertAppointment(Appointment appointment)
        {
            if (!OpenConnection())
            {
                return 0;
            }

            var command = _connection.CreateCommand();
            command.CommandText = "INSERT INTO appointment (customerId, userId, title, description, location, contact, type, url, start, end, createDate, createdBy, lastUpdate, lastUpdateBy) VALUES (@customerId, @userId, @title, @description, @location, @contact, @type, @url, @start, @end, @createDate, @createdBy, @lastUpdate, @lastUpdateBy)";
            command.Parameters.AddWithValue("@customerId", appointment.CustomerId);
            command.Parameters.AddWithValue("@userId", appointment.UserId);
            command.Parameters.AddWithValue("@title", appointment.Title);
            command.Parameters.AddWithValue("@description", appointment.Description);
            command.Parameters.AddWithValue("@location", appointment.Location);
            command.Parameters.AddWithValue("@contact", appointment.Contact);
            command.Parameters.AddWithValue("@type", appointment.Type);
            command.Parameters.AddWithValue("@url", appointment.URL);
            command.Parameters.AddWithValue("@start", appointment.Start.ToUniversalTime());
            command.Parameters.AddWithValue("@end", appointment.End.ToUniversalTime());
            command.Parameters.AddWithValue("@createDate", DateTime.UtcNow);
            command.Parameters.AddWithValue("@createdBy", NavigationService.MainVM.CurrentUser.UserName);
            command.Parameters.AddWithValue("@lastUpdate", DateTime.UtcNow);
            command.Parameters.AddWithValue("@lastUpdateBy", NavigationService.MainVM.CurrentUser.UserName);
            command.ExecuteNonQuery();
            long idFromInsert = command.LastInsertedId;
            CloseConnection();

            return idFromInsert;
        }

        public static List<Appointment> SelectAllAppointments()
        {
            if (!OpenConnection())
            {
                return null;
            }

            var command = _connection.CreateCommand();
            command.CommandText = "SELECT * FROM appointment";
            MySqlDataReader reader = command.ExecuteReader();

            var results = new List<Appointment>();
            while (reader.Read())
            {
                var appointment = new Appointment()
                { 
                    AppointmentId = reader.GetInt32(0),
                    CustomerId = reader.GetInt32(1),
                    UserId = reader.GetInt32(2),
                    Type = reader.GetString(7),
                    Start = reader.GetDateTime(9).ToLocalTime(),
                    End = reader.GetDateTime(10).ToLocalTime(),
                    CreateDate = reader.GetDateTime(11).ToLocalTime(),
                    CreatedBy = reader.GetString(12),
                    LastUpdate = reader.GetDateTime(13).ToLocalTime(),
                    LastUpdateBy = reader.GetString(14)
                };

                results.Add(appointment);
            }

            CloseConnection();

            return results;
        }

        public static List<Appointment> SelectAppointmentsInDateRange(DateTime startDate, DateTime endDate)
        {
            if (!OpenConnection())
            {
                return null;
            }

            var command = _connection.CreateCommand();
            command.CommandText = "SELECT * FROM appointment WHERE start >= @startDate AND end <= @endDate";
            command.Parameters.AddWithValue("@startDate", startDate);
            command.Parameters.AddWithValue("@endDate", endDate);
            MySqlDataReader reader = command.ExecuteReader();

            var results = new List<Appointment>();
            while (reader.Read())
            {
                var appointment = new Appointment()
                {
                    AppointmentId = reader.GetInt32(0),
                    CustomerId = reader.GetInt32(1),
                    UserId = reader.GetInt32(2),
                    Type = reader.GetString(7),
                    Start = reader.GetDateTime(9).ToLocalTime(),
                    End = reader.GetDateTime(10).ToLocalTime(),
                    CreateDate = reader.GetDateTime(11).ToLocalTime(),
                    CreatedBy = reader.GetString(12),
                    LastUpdate = reader.GetDateTime(13).ToLocalTime(),
                    LastUpdateBy = reader.GetString(14)
                };

                results.Add(appointment);
            }

            CloseConnection();

            return results;
        }

        public static Appointment SelectNextAppointment(int userId)
        {
            if (!OpenConnection())
            {
                return null;
            }

            var command = _connection.CreateCommand();
            command.CommandText = "SELECT * FROM appointment WHERE userId = @userId AND start >= @startDate LIMIT 1";
            command.Parameters.AddWithValue("@userId", userId);
            command.Parameters.AddWithValue("@startDate", DateTime.UtcNow);
            MySqlDataReader reader = command.ExecuteReader();

            Appointment result = null;
            while (reader.Read())
            {
                result = new Appointment()
                {
                    AppointmentId = reader.GetInt32(0),
                    CustomerId = reader.GetInt32(1),
                    UserId = reader.GetInt32(2),
                    Type = reader.GetString(7),
                    Start = reader.GetDateTime(9).ToLocalTime(),
                    End = reader.GetDateTime(10).ToLocalTime(),
                    CreateDate = reader.GetDateTime(11).ToLocalTime(),
                    CreatedBy = reader.GetString(12),
                    LastUpdate = reader.GetDateTime(13).ToLocalTime(),
                    LastUpdateBy = reader.GetString(14)
                };
            }

            CloseConnection();

            return result;
        }

        public static List<Appointment> SelectAppointmentsForCustomer(Customer customer)
        {
            if (!OpenConnection())
            {
                return null;
            }

            var command = _connection.CreateCommand();
            command.CommandText = "SELECT * FROM appointment WHERE customerId = @customerId";
            command.Parameters.AddWithValue("@customerId", customer.CustomerId);
            MySqlDataReader reader = command.ExecuteReader();

            var results = new List<Appointment>();
            while (reader.Read())
            {
                var appointment = new Appointment()
                {
                    AppointmentId = reader.GetInt32(0),
                    CustomerId = reader.GetInt32(1),
                    UserId = reader.GetInt32(2),
                    Type = reader.GetString(7),
                    Start = reader.GetDateTime(9).ToLocalTime(),
                    End = reader.GetDateTime(10).ToLocalTime(),
                    CreateDate = reader.GetDateTime(11).ToLocalTime(),
                    CreatedBy = reader.GetString(12),
                    LastUpdate = reader.GetDateTime(13).ToLocalTime(),
                    LastUpdateBy = reader.GetString(14)
                };

                results.Add(appointment);
            }

            CloseConnection();

            return results;
        }

        public static List<Appointment> SelectAppointmentsForUser(User user)
        {
            if (!OpenConnection())
            {
                return null;
            }

            var command = _connection.CreateCommand();
            command.CommandText = "SELECT * FROM appointment WHERE userId = @userId";
            command.Parameters.AddWithValue("@userId", user.UserId);
            MySqlDataReader reader = command.ExecuteReader();

            var results = new List<Appointment>();
            while (reader.Read())
            {
                var appointment = new Appointment()
                {
                    AppointmentId = reader.GetInt32(0),
                    CustomerId = reader.GetInt32(1),
                    UserId = reader.GetInt32(2),
                    Type = reader.GetString(7),
                    Start = reader.GetDateTime(9).ToLocalTime(),
                    End = reader.GetDateTime(10).ToLocalTime(),
                    CreateDate = reader.GetDateTime(11).ToLocalTime(),
                    CreatedBy = reader.GetString(12),
                    LastUpdate = reader.GetDateTime(13).ToLocalTime(),
                    LastUpdateBy = reader.GetString(14)
                };

                results.Add(appointment);
            }

            CloseConnection();

            return results;
        }

        public static int UpdateAppointment(Appointment appointment)
        {
            if (!OpenConnection())
            {
                return 0;
            }

            var command = _connection.CreateCommand();
            command.CommandText = "UPDATE appointment SET customerId = @customerId, userId = @userId, title = @title, description = @description, location = @location, contact = @contact, type = @type, url = @url, start = @start, end = @end, lastUpdate = @lastUpdate, lastUpdateBy = @lastUpdateBy WHERE appointmentId = @appointmentId";
            command.Parameters.AddWithValue("@appointmentId", appointment.AppointmentId);
            command.Parameters.AddWithValue("@customerId", appointment.CustomerId);
            command.Parameters.AddWithValue("@userId", appointment.UserId);
            command.Parameters.AddWithValue("@title", appointment.Title);
            command.Parameters.AddWithValue("@description", appointment.Description);
            command.Parameters.AddWithValue("@location", appointment.Location);
            command.Parameters.AddWithValue("@contact", appointment.Contact);
            command.Parameters.AddWithValue("@type", appointment.Type);
            command.Parameters.AddWithValue("@url", appointment.URL);
            command.Parameters.AddWithValue("@start", appointment.Start.ToUniversalTime());
            command.Parameters.AddWithValue("@end", appointment.End.ToUniversalTime());
            command.Parameters.AddWithValue("@lastUpdate", DateTime.UtcNow);
            command.Parameters.AddWithValue("@lastUpdateBy", NavigationService.MainVM.CurrentUser.UserName);
            var rowsAffected = command.ExecuteNonQuery();
            CloseConnection();

            return rowsAffected;
        }

        public static bool FindOverlappingAppointments(Appointment appointment, DateTime startTime, DateTime endTime)
        {
            if (!OpenConnection())
            {
                return false;
            }

            var command = _connection.CreateCommand();
            command.CommandText = "Select count(*) FROM appointment WHERE userId = @userId AND appointmentId != @appointmentId AND ((start >= @startTime AND start <= @endTime) OR (@endTime >= start AND @endTime <= end) OR (start <= @startTime AND end >= @endTime))";
            command.Parameters.AddWithValue("@userId", appointment.UserId);
            command.Parameters.AddWithValue("@appointmentId", appointment.AppointmentId);
            command.Parameters.AddWithValue("@startTime", startTime.ToUniversalTime());
            command.Parameters.AddWithValue("@endTime", endTime.ToUniversalTime());
            var count = command.ExecuteScalar();
            CloseConnection();

            if (count == null)
            {
                return false;
            }

            int matches = Convert.ToInt32(count);

            return matches > 0;
        }

        public static bool FindOverlappingAppointments(User user, DateTime startTime, DateTime endTime)
        {
            if (!OpenConnection())
            {
                return false;
            }

            var command = _connection.CreateCommand();
            command.CommandText = "Select count(*) FROM appointment WHERE userId = @userId AND ((start >= @startTime AND start <= @endTime) OR (@endTime >= start AND @endTime <= end) OR (start <= @startTime AND end >= @endTime))";
            command.Parameters.AddWithValue("@userId", user.UserId);
            command.Parameters.AddWithValue("@startTime", startTime.ToUniversalTime());
            command.Parameters.AddWithValue("@endTime", endTime.ToUniversalTime());
            var count = command.ExecuteScalar();
            CloseConnection();

            if (count == null)
            {
                return false;
            }

            int matches = Convert.ToInt32(count);

            return matches > 0;
        }

        public static int RemoveAppointment(int appointmentId)
        {
            if (!OpenConnection())
            {
                return 0;
            }

            var command = _connection.CreateCommand();
            command.CommandText = "DELETE FROM appointment WHERE appointmentId = @appointmentId";
            command.Parameters.AddWithValue("@appointmentId", appointmentId);
            var rowsAffected = command.ExecuteNonQuery();
            CloseConnection();

            return rowsAffected;
        }
        #endregion
    }
}
