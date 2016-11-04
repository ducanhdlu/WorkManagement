using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkManagement.Controllers
{
    public class Static
    {
        public static DateTime StringToDatetime(string s)
        {
            DateTime rs;
            rs = new DateTime(int.Parse(s.Substring(0, 4)), int.Parse(s.Substring(4, 2)),
                int.Parse(s.Substring(6, 2)), int.Parse(s.Substring(8, 2)), int.Parse(s.Substring(10, 2)), 0);
            return rs;
        }

        public static string DatetimeToString(DateTime dt)
        {
            string rs;
            string mon = dt.Month > 9 ? dt.Month.ToString() : "0" + dt.Month.ToString();
            string day = dt.Day > 9 ? dt.Day.ToString() : "0" + dt.Day.ToString();
            string hou = dt.Hour > 9 ? dt.Hour.ToString() : "0" + dt.Hour.ToString();
            string min = dt.Minute > 9 ? dt.Minute.ToString() : "0" + dt.Minute.ToString();
            rs = dt.Year.ToString() + mon + day + hou + min;
            return rs;
        }

        public static int TakeDay(string s)
        {
            int rs = int.Parse(s.Substring(6, 2));
            return rs;
        }

        public static int TakeMonth(string s)
        {
            int rs = int.Parse(s.Substring(4, 2));
            return rs;
        }
    }
}