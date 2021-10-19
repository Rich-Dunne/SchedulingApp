using SchedulingApp.Utilities;
using System;
using System.Diagnostics;
using System.Windows;

namespace SchedulingApp.Models
{
    public class Customer
    {
        public int CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CustomerName { get => $"{FirstName} {LastName}"; set => SplitName(value); }
        public int AddressId { get; set; }
        public bool Active { get; } = true;
        public DateTime CreateDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime LastUpdate { get; set; }
        public string LastUpdateBy { get; set; }
        public Address Address { get; set; }

        public string Phone { get => Address.Phone; }
        public string FullAddress { get => $"{Address.FullAddress} {Address.City.CityName}, {Address.City.Country.CountryName}, {Address.PostalCode}"; }
        public string FirstLetter { get => CustomerName[0].ToString().ToUpper(); }


        public RelayCommand DeleteCommand { get; set; }
        public RelayCommand EditCommand { get; set; }
        public Customer()
        {
            DeleteCommand = new RelayCommand(o => Delete(false));
            EditCommand = new RelayCommand(o => NavigationService.NavigateTo(View.UpdateCustomer, this));
        }

        private string SplitName(string name)
        {
            var names = name.Split(' ');
            FirstName = names[0];
            for(int i = 1; i< names.Length; i++)
            {
                LastName += $"{names[i]} ";
            }
            LastName = LastName.Trim(' ');
            return name;
        }

        public void Delete(bool navigateBack)
        {
            var result = MessageBox.Show($"Are you sure you want to delete {CustomerName}?  This will remove all associated appointments.", $"Delete customer?", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.No)
            {
                return;
            }
            var appointments = DataAccess.SelectAppointmentsForCustomer(this);
            appointments.ForEach(x => DataAccess.RemoveAppointment(x.AppointmentId));

            var rows = DataAccess.RemoveCustomer(this);
            if (rows > 0)
            {
                Debug.WriteLine($"Customer deleted");
                if(navigateBack)
                {
                    NavigationService.NavigateTo(NavigationService.PreviousView);
                }
                else
                {
                    NavigationService.NavigateTo(NavigationService.CurrentView);
                }
            }
        }
    }
}
