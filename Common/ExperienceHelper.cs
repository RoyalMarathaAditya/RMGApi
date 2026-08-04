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

            var years = effectiveEndDate.Year - doj.Year;
            var months = effectiveEndDate.Month - doj.Month;
            if (months < 0)
            {
                years--;
                months += 12;
            }

            return years + (decimal)months / 10m;
        }

        public static decimal CalculateTotalExperience(DateTime doj, decimal? priorExperience, bool isActive, DateTime? lwd)
        {
            var nvExp = CalculateNVExperience(doj, isActive, lwd);
            return Math.Round(nvExp + (priorExperience ?? 0), 1);
        }
    }
}
