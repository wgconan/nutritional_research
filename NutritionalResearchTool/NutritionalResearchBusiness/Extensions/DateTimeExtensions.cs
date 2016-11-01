using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NutritionalResearchBusiness.Extensions
{
    public static class DateTimeExtensions
    {
        public static int GetAge(this DateTime date, DateTime birth)
        {
            return (int.Parse(date.ToString("yyyymmdd")) - int.Parse(birth.ToString("yyyymmdd"))) / 10000;
        }
    }
}
