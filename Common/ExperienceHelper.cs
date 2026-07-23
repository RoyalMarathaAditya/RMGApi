namespace HRMS.Api.Common
{
    public static class ExperienceHelper
    {
        public static decimal CalculateNVExperience(DateTime doj, bool isActive, DateTime? lwd)
        {
            DateTime effectiveEndDate;

            if (isActive)
            {
                effectiveEndDate = DateTime.UtcNow.Date;
            }
            else
            {
                effectiveEndDate = lwd ?? DateTime.UtcNow.Date;
            }

            if (effectiveEndDate < doj)
            {
                return 0;
            }

            return Math.Round((decimal)(effectiveEndDate - doj).TotalDays / 365.25m, 1);
        }

        public static decimal CalculateTotalExperience(DateTime doj, decimal? priorExperience, bool isActive, DateTime? lwd)
        {
            var nvExp = CalculateNVExperience(doj, isActive, lwd);
            return Math.Round(nvExp + (priorExperience ?? 0), 1);
        }
    }
}
