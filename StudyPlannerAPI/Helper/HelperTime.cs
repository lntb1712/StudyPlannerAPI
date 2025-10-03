namespace StudyPlannerAPI.Helper
{
    public static class HelperTime
    {
        public static DateTime NowVN()
        {
            var vnZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            return TimeZoneInfo.ConvertTime(DateTime.UtcNow, vnZone);
        }
    }
}
