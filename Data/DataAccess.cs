using MySql.Data.MySqlClient;
using SchedulingApp.Models;
using SchedulingApp.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;

namespace SchedulingApp.Data
{
    public class DataAccess
    {
        private MySqlConnection _connection;
        private string _server = "127.0.0.1";
        private string _database = "client_schedule";
        private string _userId = "sqlUser";
        private string _password = "Passw0rd!";

        public DataAccess() => _connection = new MySqlConnection($"SERVER={_server};DATABASE={_database};UID={_userId};PASSWORD={_password};");

        private bool OpenConnection()
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

        private bool CloseConnection()
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

        #region Generic Queries
        public int Insert<T>(T obj)
        {
            if (!OpenConnection())
            {
                return 0;
            }

            MySqlCommand command = null;
            switch (obj.GetType())
            {
                case Type appointmentType when appointmentType == typeof(Appointment):
                    Debug.WriteLine($"Inserting appointment");
                    command = CreateInsertAppointmentCommand(obj as Appointment);
                    break;

                case Type customerType when customerType == typeof(Customer):
                    Debug.WriteLine($"Inserting customer");
                    command = CreateInsertCustomerCommand(obj as Customer);
                    break;

                case Type countryType when countryType == typeof(Country):
                    Debug.WriteLine($"Inserting country");
                    command = CreateInsertCountryCommand(obj as Country);
                    break;

                case Type cityType when cityType == typeof(City):
                    Debug.WriteLine($"Inserting city");
                    command = CreateInsertCityCommand(obj as City);
                    break;

                case Type addressType when addressType == typeof(Address):
                    Debug.WriteLine($"Inserting address");
                    command = CreateInsertAddressCommand(obj as Address);
                    break;

                case Type userType when userType == typeof(User):
                    Debug.WriteLine($"Inserting user");
                    command = CreateInsertUserCommand(obj as User);
                    break;
            };

            if (command is null)
            {
                CloseConnection();
                return 0;
            }

            command.ExecuteNonQuery();
            int idFromInsert = (int)command.LastInsertedId;
            CloseConnection();

            return idFromInsert;
        }

        public int Delete<T>(T obj)
        {
            if (!OpenConnection())
            {
                return 0;
            }

            MySqlCommand command = null;
            switch (obj.GetType())
            {
                case Type appointmentType when appointmentType == typeof(Appointment):
                    Debug.WriteLine($"Removing appointment");
                    command = CreateDeleteAppointmentCommand(obj as Appointment);
                    break;

                case Type customerType when customerType == typeof(Customer):
                    Debug.WriteLine($"Removing customer");
                    command = CreateDeleteCustomerCommand(obj as Customer);
                    break;
            };

            if (command is null)
            {
                CloseConnection();
                return 0;
            }

            var rowsAffected = command.ExecuteNonQuery();
            CloseConnection();

            return rowsAffected;
        }

        public int Update<T>(T obj)
        {
            if (!OpenConnection())
            {
                return 0;
            }

            MySqlCommand command = null;
            switch (obj.GetType())
            {
                case Type appointmentType when appointmentType == typeof(Appointment):
                    Debug.WriteLine($"Updating appointment");
                    command = CreateUpdateAppointmentCommand(obj as Appointment);
                    break;

                case Type customerType when customerType == typeof(Customer):
                    Debug.WriteLine($"Updating customer");
                    command = CreateUpdateCustomerCommand(obj as Customer);
                    break;
            };

            if (command is null)
            {
                CloseConnection();
                return 0;
            }

            var rowsAffected = command.ExecuteNonQuery();
            CloseConnection();

            return rowsAffected;
        }
        #endregion

        #region User Queries
        private MySqlCommand CreateInsertUserCommand(User user)
        {
            var command = _connection.CreateCommand();
            command.CommandText = "INSERT INTO user(userName, password, active, createDate, createdBy, lastUpdate, lastUpdateBy) VALUES(@userName, @password, @active, @createDate, @createdBy, @lastUpdate, @lastUpdateBy)";
            command.Parameters.AddWithValue("@userName", user.UserName);
            command.Parameters.AddWithValue("@password", user.Password);
            command.Parameters.AddWithValue("@active", 1);
            command.Parameters.AddWithValue("@createDate", $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}");
            command.Parameters.AddWithValue("@createdBy", user.UserName);
            command.Parameters.AddWithValue("@lastUpdate", $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}");
            command.Parameters.AddWithValue("@lastUpdateBy", user.UserName);

            return command;
        }

        public User SelectUser(User user)
        {
            if (!OpenConnection())
            {
                return null;
            }

            var command = _connection.CreateCommand();
            command.CommandText = "SELECT * FROM client_schedule.user WHERE userName = @username AND password = @password LIMIT 1";
            command.Parameters.AddWithValue("@username", user.UserName);
            command.Parameters.AddWithValue("@password", user.Password);
            MySqlDataReader reader = command.ExecuteReader();

            User result = null;
            while (reader.Read())
            {
                result = ReadUser(reader);
            }
            CloseConnection();

            return result;
        }

        public User SelectUser(string username)
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
                result = ReadUser(reader);
            }

            CloseConnection();

            return result;
        }

        public User SelectUser(int userId)
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
                result = ReadUser(reader);
            }

            CloseConnection();

            return result;
        }

        public List<User> SelectAllUsers()
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
                var result = ReadUser(reader);

                results.Add(result);
            }

            CloseConnection();

            return results;
        }

        private User ReadUser(MySqlDataReader reader)
        {
            var user = new User(reader.GetString(1), reader.GetString(2))
            {
                UserId = reader.GetInt32(0),
                CreateDate = reader.GetDateTime(4).ToLocalTime(),
                CreatedBy = reader.GetString(5),
                LastUpdate = reader.GetDateTime(6).ToLocalTime(),
                LastUpdateBy = reader.GetString(7)
            };

            return user;
        }
        #endregion

        #region Address Queries
        private MySqlCommand CreateInsertCountryCommand(Country country)
        {
            var command = _connection.CreateCommand();
            command.CommandText = "INSERT INTO country (country, createDate, createdBy, lastUpdate, lastUpdateBy) VALUES (@country, @createDate, @createdBy, @lastUpdate, @lastUpdateBy)";
            command.Parameters.AddWithValue("@country", country.CountryName);
            command.Parameters.AddWithValue("@createDate", DateTime.UtcNow);
            command.Parameters.AddWithValue("@createdBy", NavigationService.MainViewModel.CurrentUser.UserName);
            command.Parameters.AddWithValue("@lastUpdate", DateTime.UtcNow);
            command.Parameters.AddWithValue("@lastUpdateBy", NavigationService.MainViewModel.CurrentUser.UserName);
            
            return command;
        }

        private MySqlCommand CreateInsertCityCommand(City city)
        {
            var command = _connection.CreateCommand();
            command.CommandText = "INSERT INTO city (city, countryId, createDate, createdBy, lastUpdate, lastUpdateBy) VALUES (@city, @countryId, @createDate, @createdBy, @lastUpdate, @lastUpdateBy)";
            command.Parameters.AddWithValue("@city", city.CityName);
            command.Parameters.AddWithValue("@countryId", city.CountryId);
            command.Parameters.AddWithValue("@createDate", DateTime.UtcNow);
            command.Parameters.AddWithValue("@createdBy", NavigationService.MainViewModel.CurrentUser.UserName);
            command.Parameters.AddWithValue("@lastUpdate", DateTime.UtcNow);
            command.Parameters.AddWithValue("@lastUpdateBy", NavigationService.MainViewModel.CurrentUser.UserName);

            return command;
        }

        private MySqlCommand CreateInsertAddressCommand(Address address)
        {
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

            return command;
        }
        #endregion

        #region Customer Queries
        private MySqlCommand CreateInsertCustomerCommand(Customer customer)
        {
            var command = _connection.CreateCommand();
            command.CommandText = "INSERT INTO customer (firstName, lastName, addressId, active, createDate, createdBy, lastUpdate, lastUpdateBy) VALUES (@firstName, @lastName, @addressId, @active, @createDate, @createdBy, @lastUpdate, @lastUpdateBy)";
            command.Parameters.AddWithValue("@firstName", customer.FirstName);
            command.Parameters.AddWithValue("@lastName", customer.LastName);
            command.Parameters.AddWithValue("@addressId", customer.AddressId);
            command.Parameters.AddWithValue("@active", Convert.ToInt32(customer.Active));
            command.Parameters.AddWithValue("@createDate", DateTime.UtcNow);
            command.Parameters.AddWithValue("@createdBy", NavigationService.MainViewModel.CurrentUser.UserName);
            command.Parameters.AddWithValue("@lastUpdate", DateTime.UtcNow);
            command.Parameters.AddWithValue("@lastUpdateBy", NavigationService.MainViewModel.CurrentUser.UserName);

            return command;
        }

        private MySqlCommand CreateUpdateCustomerCommand(Customer customer)
        {
            var command = _connection.CreateCommand();
            command.CommandText = "UPDATE customer " +
                                    "INNER JOIN address ON address.addressId = customer.addressId " +
                                    "INNER JOIN city ON address.cityId = city.cityId " +
                                    "INNER JOIN country ON city.countryId = country.countryId " +
                                    "SET customer.firstName = @firstName, " +
                                        "customer.lastName = @lastName, " +
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
            command.Parameters.AddWithValue("@firstName", customer.FirstName);
            command.Parameters.AddWithValue("@lastName", customer.LastName);
            command.Parameters.AddWithValue("@address", customer.Address.Address1);
            command.Parameters.AddWithValue("@address2", customer.Address.Address2);
            command.Parameters.AddWithValue("@postalCode", customer.Address.PostalCode);
            command.Parameters.AddWithValue("@phone", customer.Address.Phone);
            command.Parameters.AddWithValue("@city", customer.Address.City.CityName);
            command.Parameters.AddWithValue("@country", customer.Address.City.Country.CountryName);
            command.Parameters.AddWithValue("@lastUpdate", DateTime.UtcNow);
            command.Parameters.AddWithValue("@lastUpdateBy", NavigationService.MainViewModel.CurrentUser.UserName);

            return command;
        }

        private MySqlCommand CreateDeleteCustomerCommand(Customer customer)
        {
            var command = _connection.CreateCommand();
            command.CommandText = "DELETE customer, address, city, country FROM customer " +
                "INNER JOIN address ON customer.addressId = address.addressId " +
                "INNER JOIN city ON address.cityId = city.cityId " +
                "INNER JOIN country ON city.countryId = country.countryId " +
                "WHERE customerId = @customerId";
            command.Parameters.AddWithValue("@customerId", customer.CustomerId);

            return command;
        }

        public List<Customer> SelectAllCustomers()
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
            }

            CloseConnection();

            return results;
        }

        public Customer SelectCustomer(int customerId)
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

        public Customer SelectCustomer(string customerName)
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

        private Customer ReadCustomer(MySqlDataReader reader)
        {
            var customer = new Customer()
            {
                CustomerId = reader.GetInt32(0),
                FirstName = reader.GetString(1),
                LastName = reader.GetString(2),
                AddressId = reader.GetInt32(3),
                CreateDate = reader.GetDateTime(5),
                CreatedBy = reader.GetString(6),
                LastUpdate = reader.GetDateTime(7),
                LastUpdateBy = reader.GetString(8),

                Address = new Address()
                {
                    AddressId = reader.GetInt32(9),
                    Address1 = reader.GetString(10),
                    Address2 = reader.GetString(11),
                    CityId = reader.GetInt32(12),
                    PostalCode = reader.GetString(13),
                    Phone = reader.GetString(14),
                    CreateDate = reader.GetDateTime(15).ToLocalTime(),
                    CreatedBy = reader.GetString(16),
                    LastUpdate = reader.GetDateTime(17).ToLocalTime(),
                    LastUpdateBy = reader.GetString(18),

                    City = new City()
                    {
                        CityId = reader.GetInt32(19),
                        CityName = reader.GetString(20),
                        CountryId = reader.GetInt32(21),
                        CreateDate = reader.GetDateTime(22).ToLocalTime(),
                        CreatedBy = reader.GetString(23),
                        LastUpdate = reader.GetDateTime(24).ToLocalTime(),
                        LastUpdateBy = reader.GetString(25),

                        Country = new Country()
                        {
                            CountryId = reader.GetInt32(26),
                            CountryName = reader.GetString(27),
                            CreateDate = reader.GetDateTime(28).ToLocalTime(),
                            CreatedBy = reader.GetString(29),
                            LastUpdate = reader.GetDateTime(30).ToLocalTime(),
                            LastUpdateBy = reader.GetString(31)
                        }
                    },
                },
            };

            return customer;
        }
        #endregion

        #region Appointment Queries
        private MySqlCommand CreateInsertAppointmentCommand(Appointment appointment)
        {
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

            return command;
        }

        private MySqlCommand CreateDeleteAppointmentCommand(Appointment appointment)
        {
            var command = _connection.CreateCommand();
            command.CommandText = "DELETE FROM appointment WHERE appointmentId = @appointmentId";
            command.Parameters.AddWithValue("@appointmentId", appointment.AppointmentId);

            return command;
        }

        private MySqlCommand CreateUpdateAppointmentCommand(Appointment appointment)
        {
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

            return command;
        }

        public List<Appointment> SelectAllAppointments()
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

        public List<Appointment> SelectAllAppointmentsForCustomer(Customer customer)
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

        public List<Appointment> SelectAllAppointmentsInDateRange(DateTime startDate, DateTime endDate)
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

        public Appointment SelectNextAppointment(int userId)
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

        private Appointment ReadAppointment(MySqlDataReader reader)
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
                    FirstName = reader.GetString(16),
                    LastName = reader.GetString(17),
                    AddressId = reader.GetInt32(18),
                    CreateDate = reader.GetDateTime(20),
                    CreatedBy = reader.GetString(21),
                    LastUpdate = reader.GetDateTime(22),
                    LastUpdateBy = reader.GetString(23),

                    Address = new Address()
                    {
                        AddressId = reader.GetInt32(24),
                        Address1 = reader.GetString(25),
                        Address2 = reader.GetString(26),
                        CityId = reader.GetInt32(27),
                        PostalCode = reader.GetString(28),
                        Phone = reader.GetString(29),
                        CreateDate = reader.GetDateTime(30).ToLocalTime(),
                        CreatedBy = reader.GetString(31),
                        LastUpdate = reader.GetDateTime(32).ToLocalTime(),
                        LastUpdateBy = reader.GetString(33),

                        City = new City()
                        {
                            CityId = reader.GetInt32(34),
                            CityName = reader.GetString(35),
                            CountryId = reader.GetInt32(36),
                            CreateDate = reader.GetDateTime(37).ToLocalTime(),
                            CreatedBy = reader.GetString(38),
                            LastUpdate = reader.GetDateTime(39).ToLocalTime(),
                            LastUpdateBy = reader.GetString(40),

                            Country = new Country()
                            {
                                CountryId = reader.GetInt32(41),
                                CountryName = reader.GetString(42),
                                CreateDate = reader.GetDateTime(43).ToLocalTime(),
                                CreatedBy = reader.GetString(44),
                                LastUpdate = reader.GetDateTime(45).ToLocalTime(),
                                LastUpdateBy = reader.GetString(46)
                            }
                        },
                    },
                }
            };

            return appointment;
        }

        public int CountOverlappingAppointments(User user, DateTime startTime, DateTime endTime)
        {
            if (!OpenConnection())
            {
                return 0;
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
                return 0;
            }

            return Convert.ToInt32(count);
        }
        #endregion
    }
}
