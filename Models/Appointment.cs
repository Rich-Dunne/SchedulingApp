using SchedulingApp.Utilities;
using System;

namespace SchedulingApp.Models
{
    public class Appointment
    {
        public int AppointmentId { get; set; }
        public int CustomerId { get; set; }
        public int UserId { get; set; }
        public string Title { get; } = "Not needed";
        public string Description { get; } = "Not needed";
        public string Location { get; } = "Not needed";
        public string Contact { get; } = "Not needed";
        public string Type { get; set; }
        public string URL { get; set; } = "Not needed";
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime LastUpdate { get; set; }
        public string LastUpdateBy { get; set; }

        public string CustomerName { get => DataAccess.SelectCustomer(CustomerId).CustomerName; }
        public string FormattedDate { get => Start.ToString("MMM dd"); }
        public string FormattedTime { get => Start.ToShortTimeString(); }
        public string Duration { get => End.Subtract(Start).ToString("mm"); }
        public string TimeTo { get => GetTimeTo(); }

        public Appointment()
        {

        }

        public string GetTimeTo()
        {
            var timeTo = End.Subtract(Start);
            if(timeTo.Minutes == 1)
            {
                return $"In {timeTo.Minutes} minute";
            }
            if(timeTo.Hours < 1 && timeTo.Minutes > 1)
            {
                return $"In {timeTo.Minutes} minutes";
            }
            if(timeTo.Hours == 1)
            {
                return $"In {timeTo.Hours} hour";
            }
            if(timeTo.Hours > 1 && timeTo.Hours < 24)
            {
                return $"In {timeTo.Hours} hours";
            }

            return "Now";
        }
    }
}
