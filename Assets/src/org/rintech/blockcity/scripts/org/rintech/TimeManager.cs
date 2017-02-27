using System;
namespace org.rintech.blockcity
{
    public class TimeManager
    {
        public static long firstplay = -1;
        public static long totalplaytime = 0;
        public enum ConvPointType
        {
            day, sec
        }
        public static string convpoint(DateTime date, ConvPointType type)
        {
            string str = date.Year + "/" + date.Month + "/" + date.Day;
            if (type == ConvPointType.sec)
            {
                str += String.Format(" {0}:{1:00}:{2:00}", date.Hour, date.Minute, date.Second);
            }
            return str;
        }
        public static string convinterval(TimeSpan interval)
        {
            string str = "";
            if (interval.Days * 24 + interval.Hours == 0)
            {
                if (interval.Minutes != 0)
                {
                    str = interval.Minutes + "分";
                }
                str += interval.Seconds + "秒";
            }
            else
            {
                str = interval.Days * 24 + interval.Hours + "時間" + interval.Minutes + "分";
            }
            return str;
        }
    }
}
