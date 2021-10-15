using SchedulingApp.Utilities;
using System;

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
        public City City { get; set; }
        public Country Country { get; set; }

        public Customer()
        {

        }

        private string SplitName(string name)
        {
            var names = name.Split(' ');
            FirstName = names[0];
            LastName = names[1];
            return name;
        }
    }
}
