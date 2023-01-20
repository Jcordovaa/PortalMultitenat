namespace ApiPortal.Services
{
    public static class DateUtils
    {
        public static DateTime StartOfWeek(this DateTime dt)
        {
            DayOfWeek startOfWeek = DayOfWeek.Monday;
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }

        public static DateTime FirstDayOfWeek(DateTime date)
        {
            DayOfWeek fdow = DayOfWeek.Monday;
            int offset = fdow - date.DayOfWeek;
            DateTime fdowDate = date.AddDays(offset);
            return fdowDate;
        }

        public static DateTime LastDayOfWeek(DateTime date)
        {
            DateTime ldowDate = FirstDayOfWeek(date).AddDays(6);
            return ldowDate;
        }

        
    }
}
