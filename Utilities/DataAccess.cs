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
            command.Parameters.AddWithValue("@createdBy", NavigationService.MainViewModel.CurrentUser.UserName);
            command.Parameters.AddWithValue("@lastUpdate", DateTime.UtcNow);
            command.Parameters.AddWithValue("@lastUpdateBy", NavigationService.MainViewModel.CurrentUser.UserName);
            command.ExecuteNonQuery();
            long idFromInsert = command.LastInsertedId;
            CloseConnection();

            return idFromInsert;
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
            command.Parameters.AddWithValue("@createdBy", NavigationService.MainViewModel.CurrentUser.UserName);
            command.Parameters.AddWithValue("@lastUpdate", DateTime.UtcNow);
            command.Parameters.AddWithValue("@lastUpdateBy", NavigationService.MainViewModel.CurrentUser.UserName);
            command.ExecuteNonQuery();
            long idFromInsert = command.LastInsertedId;
            CloseConnection();

            return idFromInsert;
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
            command.Parameters.AddWithValue("@createdBy", NavigationService.MainViewModel.CurrentUser.UserName);
            command.Parameters.AddWithValue("@lastUpdate", DateTime.UtcNow);
            command.Parameters.AddWithValue("@lastUpdateBy", NavigationService.MainViewModel.CurrentUser.UserName);
            command.ExecuteNonQuery();
            long idFromInsert = command.LastInsertedId;
            CloseConnection();

            return idFromInsert;
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
            command.Parameters.AddWithValue("@createdBy", NavigationService.MainViewModel.CurrentUser.UserName);
            command.Parameters.AddWithValue("@lastUpdate", DateTime.UtcNow);
            command.Parameters.AddWithValue("@lastUpdateBy", NavigationService.MainViewModel.CurrentUser.UserName);
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
            command.CommandText = "SELECT * FROM customer " +
                                    " INNER JOIN address ON customer.addressId = address.addressId " +
                                    " INNER JOIN city ON address.cityId = city.cityId " +
                                    " INNER JOIN country ON city.countryId = country.countryId";
            MySqlDataReader reader = command.ExecuteReader();

            var results = new List<Customer>();
            while (reader.Read())
            {
                results.Add(ReadCustomer(reader));
                //var customer = new Customer()
                //{
                //    CustomerId = reader.GetInt32(0),
                //    CustomerName = reader.GetString(1),
                //    AddressId = reader.GetInt32(2),
                //    CreateDate = reader.GetDateTime(4),
                //    CreatedBy = reader.GetString(5),
                //    LastUpdate = reader.GetDateTime(6),
                //    LastUpdateBy = reader.GetString(7),

                //    Address = new Address()
                //    {
                //        AddressId = reader.GetInt32(8),
                //        Address1 = reader.GetString(9),
                //        Address2 = reader.GetString(10),
                //        CityId = reader.GetInt32(11),
                //        PostalCode = reader.GetString(12),
                //        Phone = reader.GetString(13),
                //        CreateDate = reader.GetDateTime(14).ToLocalTime(),
                //        CreatedBy = reader.GetString(15),
                //        LastUpdate = reader.GetDateTime(16).ToLocalTime(),
                //        LastUpdateBy = reader.GetString(17)

                //    },

                //    City = new City()
                //    {
                //        CityId = reader.GetInt32(18),
                //        CityName = reader.GetString(19),
                //        CountryId = reader.GetInt32(20),
                //        CreateDate = reader.GetDateTime(21).ToLocalTime(),
                //        CreatedBy = reader.GetString(22),
                //        LastUpdate = reader.GetDateTime(23).ToLocalTime(),
                //        LastUpdateBy = reader.GetString(24)
                //    },

                //    Country = new Country()
                //    {
                //        CountryId = reader.GetInt32(25),
                //        CountryName = reader.GetString(26),
                //        CreateDate = reader.GetDateTime(27).ToLocalTime(),
                //        CreatedBy = reader.GetString(28),
                //        LastUpdate = reader.GetDateTime(29).ToLocalTime(),
                //        LastUpdateBy = reader.GetString(30)
                //    }
                //};

                //results.Add(customer);
            }

            CloseConnection();

            return results;
        }

        public static Customer SelectCustomer(int customerId)
        {
            if (!OpenConnection())
            {
                return null;
            }

            var command = _connection.CreateCommand();
            command.CommandText = "SELECT * FROM customer" +
                                    " INNER JOIN address ON customer.addressId = address.addressId " +
                                    " INNER JOIN city ON address.cityId = city.cityId " +
                                    " INNER JOIN country ON city.countryId = country.countryId " +
                                    " WHERE customerId = @customerId ";
            command.Parameters.AddWithValue("@customerId", customerId);
            MySqlDataReader reader = command.ExecuteReader();

            Customer result = null;
            while (reader.Read())
            {
                result = ReadCustomer(reader);
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
            command.CommandText = "SELECT * FROM customer" +
                                    " INNER JOIN address ON customer.addressId = address.addressId " +
                                    " INNER JOIN city ON address.cityId = city.cityId " +
                                    " INNER JOIN country ON city.countryId = country.countryId " +
                                    " WHERE customerName = @customerName ";
            command.Parameters.AddWithValue("@customerName", customerName);
            MySqlDataReader reader = command.ExecuteReader();

            Customer result = null;
            while (reader.Read())
            {
                result = ReadCustomer(reader);
            }

            CloseConnection();

            return result;
        }

        private static Customer ReadCustomer(MySqlDataReader reader)
        {
            var customer = new Customer()
            {
                CustomerId = reader.GetInt32(0),
                CustomerName = reader.GetString(1),
                AddressId = reader.GetInt32(2),
                CreateDate = reader.GetDateTime(4),
                CreatedBy = reader.GetString(5),
                LastUpdate = reader.GetDateTime(6),
                LastUpdateBy = reader.GetString(7),

                Address = new Address()
                {
                    AddressId = reader.GetInt32(8),
                    Address1 = reader.GetString(9),
                    Address2 = reader.GetString(10),
                    CityId = reader.GetInt32(11),
                    PostalCode = reader.GetString(12),
                    Phone = reader.GetString(13),
                    CreateDate = reader.GetDateTime(14).ToLocalTime(),
                    CreatedBy = reader.GetString(15),
                    LastUpdate = reader.GetDateTime(16).ToLocalTime(),
                    LastUpdateBy = reader.GetString(17),

                    City = new City()
                    {
                        CityId = reader.GetInt32(18),
                        CityName = reader.GetString(19),
                        CountryId = reader.GetInt32(20),
                        CreateDate = reader.GetDateTime(21).ToLocalTime(),
                        CreatedBy = reader.GetString(22),
                        LastUpdate = reader.GetDateTime(23).ToLocalTime(),
                        LastUpdateBy = reader.GetString(24),

                        Country = new Country()
                        {
                            CountryId = reader.GetInt32(25),
                            CountryName = reader.GetString(26),
                            CreateDate = reader.GetDateTime(27).ToLocalTime(),
                            CreatedBy = reader.GetString(28),
                            LastUpdate = reader.GetDateTime(29).ToLocalTime(),
                            LastUpdateBy = reader.GetString(30)
                        }
                    },
                },
            };

            return customer;
        }

        public static int UpdateCustomer(Customer customer)
        {
            if (!OpenConnection())
            {
                return 0;
            }

            var truncatedDateTime = DateTime.UtcNow.AddTicks(-(DateTime.UtcNow.Ticks % TimeSpan.TicksPerSecond));
            var command = _connection.CreateCommand();
            command.CommandText = "UPDATE customer " +
                                    "INNER JOIN address ON address.addressId = customer.addressId " +
                                    "INNER JOIN city ON address.cityId = city.cityId " +
                                    "INNER JOIN country ON city.countryId = country.countryId " +
                                    "SET customer.customerName = @customerName, " +
                                        "address.address = @address, " +
                                        "address.address2 = @address2, " +
                                        "address.postalCode = @postalCode, " +
                                        "address.phone = @phone, " +
                                        "address.lastUpdate = @lastUpdate, " +
                                        "address.lastUpdateBy = @lastUpdateBy, " +
                                        "city.city = @city, " +
                                        "city.lastUpdate = @lastUpdate, " +
                                        "city.lastUpdateBy = @lastUpdateBy, " +
                                        "country.country = @country, " +
                                        "country.lastUpdate = @lastUpdate, " +
                                        "country.lastUpdateBy = @lastUpdateBy " +
                                    "WHERE customer.customerId = @customerId";
            command.Parameters.AddWithValue("@customerId", customer.CustomerId);
            command.Parameters.AddWithValue("@customerName", customer.CustomerName);
            command.Parameters.AddWithValue("@address", customer.Address.Address1);
            command.Parameters.AddWithValue("@address2", customer.Address.Address2);
            command.Parameters.AddWithValue("@postalCode", customer.Address.PostalCode);
            command.Parameters.AddWithValue("@phone", customer.Address.Phone);
            command.Parameters.AddWithValue("@city", customer.Address.City.CityName);
            command.Parameters.AddWithValue("@country", customer.Address.City.Country.CountryName);
            command.Parameters.AddWithValue("@lastUpdate", truncatedDateTime);
            command.Parameters.AddWithValue("@lastUpdateBy", NavigationService.MainViewModel.CurrentUser.UserName);
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
            command.CommandText = "DELETE customer, address, city, country FROM customer  " +
                                    "INNER JOIN address on customer.addressId = address.addressId " +
                                    "INNER JOIN city ON address.cityId = city.cityId " +
                                    "INNER JOIN country on city.countryId = country.countryId " +
                                    "WHERE customerId = @customerId";
            command.Parameters.AddWithValue("@customerId", customer.CustomerId);
            var rowsAffected = command.ExecuteNonQuery();

            CloseConnection();

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
            command.Parameters.AddWithValue("@createdBy", NavigationService.MainViewModel.CurrentUser.UserName);
            command.Parameters.AddWithValue("@lastUpdate", DateTime.UtcNow);
            command.Parameters.AddWithValue("@lastUpdateBy", NavigationService.MainViewModel.CurrentUser.UserName);
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
            command.CommandText = "SELECT * FROM appointment " +
                                    "INNER JOIN customer ON appointment.customerId = customer.customerId " +
                                    "INNER JOIN address ON customer.addressId = address.addressId " +
                                    "INNER JOIN city on address.cityID = city.cityId " +
                                    "INNER JOIN country on city.countryID = country.countryId ";
            MySqlDataReader reader = command.ExecuteReader();

            var results = new List<Appointment>();
            while (reader.Read())
            {
                results.Add(ReadAppointment(reader));
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
            command.CommandText = "SELECT * FROM appointment " +
                                    "INNER JOIN customer ON appointment.customerId = customer.customerId " +
                                    "INNER JOIN address ON customer.addressId = address.addressId " +
                                    "INNER JOIN city on address.cityID = city.cityId " +
                                    "INNER JOIN country on city.countryID = country.countryId " +
                                    "WHERE start >= @startDate AND end <= @endDate";
            //command.CommandText = "SELECT * FROM appointment WHERE start >= @startDate AND end <= @endDate";
            command.Parameters.AddWithValue("@startDate", startDate);
            command.Parameters.AddWithValue("@endDate", endDate);
            MySqlDataReader reader = command.ExecuteReader();

            var results = new List<Appointment>();
            while (reader.Read())
            {
                results.Add(ReadAppointment(reader));
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
            command.CommandText = "SELECT * FROM appointment " +
                                    "INNER JOIN customer ON appointment.customerId = customer.customerId " +
                                    "INNER JOIN address ON customer.addressId = address.addressId " +
                                    "INNER JOIN city on address.cityID = city.cityId " +
                                    "INNER JOIN country on city.countryID = country.countryId " +
                                    "WHERE userId = @userId AND start >= @startDate LIMIT 1";
            //command.CommandText = "SELECT * FROM appointment WHERE userId = @userId AND start >= @startDate LIMIT 1";
            command.Parameters.AddWithValue("@userId", userId);
            command.Parameters.AddWithValue("@startDate", DateTime.UtcNow);
            MySqlDataReader reader = command.ExecuteReader();

            Appointment result = null;
            while (reader.Read())
            {
                result = ReadAppointment(reader);
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
            command.CommandText = "SELECT * FROM appointment " +
                                    "INNER JOIN customer ON appointment.customerId = customer.customerId " +
                                    "INNER JOIN address ON customer.addressId = address.addressId " +
                                    "INNER JOIN city on address.cityID = city.cityId " +
                                    "INNER JOIN country on city.countryID = country.countryId " +
                                    "WHERE customer.customerId = @customerId";
            command.Parameters.AddWithValue("@customerId", customer.CustomerId);
            MySqlDataReader reader = command.ExecuteReader();

            var results = new List<Appointment>();
            while (reader.Read())
            {
                results.Add(ReadAppointment(reader));
            }

            CloseConnection();

            return results;
        }

        private static Appointment ReadAppointment(MySqlDataReader reader)
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
                LastUpdateBy = reader.GetString(14),

                Customer = new Customer()
                {
                    CustomerId = reader.GetInt32(15),
                    CustomerName = reader.GetString(16),
                    AddressId = reader.GetInt32(17),
                    CreateDate = reader.GetDateTime(19),
                    CreatedBy = reader.GetString(20),
                    LastUpdate = reader.GetDateTime(21),
                    LastUpdateBy = reader.GetString(22),

                    Address = new Address()
                    {
                        AddressId = reader.GetInt32(23),
                        Address1 = reader.GetString(24),
                        Address2 = reader.GetString(25),
                        CityId = reader.GetInt32(26),
                        PostalCode = reader.GetString(27),
                        Phone = reader.GetString(28),
                        CreateDate = reader.GetDateTime(29).ToLocalTime(),
                        CreatedBy = reader.GetString(30),
                        LastUpdate = reader.GetDateTime(31).ToLocalTime(),
                        LastUpdateBy = reader.GetString(32),

                        City = new City()
                        {
                            CityId = reader.GetInt32(33),
                            CityName = reader.GetString(34),
                            CountryId = reader.GetInt32(35),
                            CreateDate = reader.GetDateTime(36).ToLocalTime(),
                            CreatedBy = reader.GetString(37),
                            LastUpdate = reader.GetDateTime(38).ToLocalTime(),
                            LastUpdateBy = reader.GetString(39),

                            Country = new Country()
                            {
                                CountryId = reader.GetInt32(40),
                                CountryName = reader.GetString(41),
                                CreateDate = reader.GetDateTime(42).ToLocalTime(),
                                CreatedBy = reader.GetString(43),
                                LastUpdate = reader.GetDateTime(44).ToLocalTime(),
                                LastUpdateBy = reader.GetString(45)
                            }
                        },
                    },
                }
            };

            return appointment;
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
            command.Parameters.AddWithValue("@lastUpdateBy", NavigationService.MainViewModel.CurrentUser.UserName);
            var rowsAffected = command.ExecuteNonQuery();
            CloseConnection();

            return rowsAffected;
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
