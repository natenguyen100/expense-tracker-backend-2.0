namespace ExpenseTrackerAPI.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime AsUtc(this DateTime dateTime)
        {
            return dateTime.Kind == DateTimeKind.Unspecified 
                ? DateTime.SpecifyKind(dateTime, DateTimeKind.Utc) 
                : dateTime;
        }

        public static DateTime? AsUtc(this DateTime? dateTime)
        {
            return dateTime?.AsUtc();
        }
    }
}