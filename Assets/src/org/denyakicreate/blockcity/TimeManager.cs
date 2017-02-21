using System;
namespace org.denyakicreate.blockcity
{
    public class TimeManager
    {
        public static long firstplay = -1;
        public static long totalplaytime = 0;
        public enum ConvPointType
        {
            day
        }
        public static string convpoint(DateTime date, ConvPointType type)
        {
            return date.Year + "/" + date.Month + "/" + date.Day;
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
