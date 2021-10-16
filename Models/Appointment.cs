using SchedulingApp.Utilities;
using SchedulingApp.ViewModels;
using System;
using System.Diagnostics;
using System.Windows;

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

        public string Consultant { get => DataAccess.SelectUser(UserId).UserName;  }
        public string CustomerName { get => DataAccess.SelectCustomer(CustomerId).CustomerName; }
        public string FullDate { get => Start.ToLocalTime().ToString("MMM dd, yyyy"); }
        public string FormattedDate { get => Start.ToLocalTime().ToString("MMM dd"); }
        public string FormattedTime { get => Start.ToLocalTime().ToShortTimeString(); }
        public string Duration { get => End.Subtract(Start).TotalMinutes.ToString(); }
        public string TimeTo { get => GetTimeTo(); }
        public string MonthYear { get => Start.ToString("MMMM yyyy"); }

        public RelayCommand CancelCommand { get; set; }
        public RelayCommand EditCommand { get; set; }

        public Appointment()
        {
            CancelCommand = new RelayCommand(o => CancelAppointment());
            EditCommand = new RelayCommand(o => NavigationService.NavigateTo<UpdateAppointmentViewModel>(this));
        }

        private void CancelAppointment()
        {
            var cancelPrompt = MessageBox.Show($"Are you sure you want to cancel this appointment?", "Cancel appointment?", MessageBoxButton.YesNo);
            if (cancelPrompt == MessageBoxResult.Yes)
            {
                DataAccess.RemoveAppointment(AppointmentId);
                NavigationService.NavigateTo<CalendarViewModel>();
            }
        }

        private string GetTimeTo()
        {
            var timeTo = Start.ToLocalTime().Subtract(DateTime.Now.ToLocalTime());
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
            if(timeTo.Minutes == 0)
            {
                return "Now";
            }

            return "Done";
        }
    }
}
