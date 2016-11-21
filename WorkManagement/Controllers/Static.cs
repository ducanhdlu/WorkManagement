using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkManagement.Models;

namespace WorkManagement.Controllers
{
    public class Static
    {
        

        //201611071500=>15:00:00 07/11/2016
        public static DateTime StringToDatetime(string s)
        {
            DateTime rs;
            rs = new DateTime(int.Parse(s.Substring(0, 4)), int.Parse(s.Substring(4, 2)),
                int.Parse(s.Substring(6, 2)), int.Parse(s.Substring(8, 2)), int.Parse(s.Substring(10, 2)), 0);
            return rs;
        }
        //15:00:00 07/11/2016 => 201611071500
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
        //07/11/2016 => 201611070800
        public static string ShortDatetimeToString(string dt)
        {
            string rs;
            if (dt.Length == 10)
            {
                string year = dt.Substring(6, 4);
                string mon = dt.Substring(3, 2);
                string day = dt.Substring(0, 2);
                string hou = "08";
                string min = "00";
                rs = year + mon + day + hou + min;
                return rs;
            }

            return null;
        }

        //07/11/2016 => 201611070800
        public static string ShortDatetimeToString(string dt, string hour, string minute)
        {
            string rs;
            if (dt.Length == 10)
            {
                string year = dt.Substring(6, 4);
                string mon = dt.Substring(3, 2);
                string day = dt.Substring(0, 2);
                string hou = int.Parse(hour) > 9 ? hour : "0" + hour;
                string min = int.Parse(minute) > 9 ? minute : "0" + minute;
                rs = year + mon + day + hou + min;
                return rs;
            }

            return null;
        }

        public static string ProcessStatus(string Status)
        {
            switch (Status)
            {
                case "0":
                    return "Chưa duyệt";
                case "1":
                    return "Đã chấp nhận";
                case "2":
                    return "Đã từ chối";
                case "3":
                    return "Đã hủy";
                case "4":
                    return "Chưa duyệt";
                case "5":
                    return "Đã chuyển lên";
                case "6":
                    return "QLCC đã chấp nhận";
                case "7":
                    return "QLCC đã từ chối";
                default:
                    return "Chưa duyệt";

            }
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
        /// <summary>
        /// hoaippt
        /// thông báo số tin nhắn nghỉ phép chưa được duyệt
        /// </summary>
        public static int New_Messeger_NghiPhep = 0;
        /// <summary>
        /// hoaippt
        /// thông báo số tin nhắn nghỉ sớm chưa được duyệt
        /// </summary>
        public static int New_Messeger_NghiSom = 0;
        /// <summary>
        /// hoaippt
        /// 
        /// </summary>
        /// <param name="db"></param>
        public static void setMesseger(QLNghiPhepEntities1 db)
        {
            List<AbsenceLetter> new_mesenger_NghiPhep = db.AbsenceLetters.Where(n => n.Status == "0").ToList();
            List<GoOutLetter> new_mesenger_NghiSom = db.GoOutLetters.Where(n => n.Status == "0").ToList();
            Static.New_Messeger_NghiPhep = new_mesenger_NghiPhep.Count;
            Static.New_Messeger_NghiSom = new_mesenger_NghiSom.Count;
        }
        public static void setMessegerSuper(QLNghiPhepEntities1 db)
        {
            List<AbsenceLetter> new_mesenger_NghiPhep = db.AbsenceLetters.Where(n => n.Status == "4" || n.Status == "5").ToList();
            List<GoOutLetter> new_mesenger_NghiSom = db.GoOutLetters.Where(n => n.Status == "4" || n.Status == "5").ToList();
            Static.New_Messeger_NghiPhep = new_mesenger_NghiPhep.Count;
            Static.New_Messeger_NghiSom = new_mesenger_NghiSom.Count;

        }
    }
}