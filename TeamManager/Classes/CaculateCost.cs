using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TeamManager.Classes
{
    public class CaculateCost
    {
        public int Caculate(DateTime start, DateTime end)
        {
            string result = string.Empty;
            int hours = 0;

            TimeSpan diff = end.Subtract(start);
            if ((end.Day - start.Day) == 0)
            {
                hours = diff.Hours;
                if (start.Hour <= 12 && end.Hour >= 13) hours--;
            }
            else
            {
                DateTime compareDateTime = new DateTime(start.Year, start.Month, start.Day, 18, 0, 0);
                //first day
                diff = compareDateTime.Subtract(start);
                hours = diff.Hours;
                if (start.Hour <= 12) hours--;

                //before end day
                compareDateTime = new DateTime(compareDateTime.Year, compareDateTime.Month, compareDateTime.Day).AddDays(1);
                while (end.Subtract(compareDateTime).Days > 0)
                {
                    compareDateTime = compareDateTime.AddDays(1);
                    hours += 8;
                }

                //end day
                compareDateTime = new DateTime(end.Year, end.Month, end.Day, 9, 0, 0);
                diff = end.Subtract(compareDateTime);
                hours += diff.Hours;
                if (end.Hour >= 13) hours--;
            }

            //result = hours.ToString() + "H";
            return hours;
        }
    }
}