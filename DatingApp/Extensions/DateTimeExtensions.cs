using System;

namespace DatingApp.Extensions
{
    public static class DateTimeExtensions
    {
        public static int CalculateAge(this DateTime birthDateTime)
        {
            var today = DateTime.Today;
            var age = today.Year - birthDateTime.Year;
            if (birthDateTime.Date > today.AddYears(-age)) age--;
            return age;
        }
    }
}