using HRMS.Api.Common;

namespace HRMS.Api.Tests;

public class ExperienceHelperTests
{
    [Fact]
    public void CalculateNVExperience_ActiveEmployee_ReturnsCorrectYears()
    {
        var doj = new DateTime(2025, 1, 1);
        var result = ExperienceHelper.CalculateNVExperience(doj, isActive: true, lwd: null);

        var expected = Math.Round((decimal)(DateTime.UtcNow.Date - doj).TotalDays / 365.25m, 1);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void CalculateNVExperience_InactiveEmployeeWithLWD_ReturnsLWDMinusDOJ()
    {
        var doj = new DateTime(2025, 1, 1);
        var lwd = new DateTime(2025, 10, 1);
        var result = ExperienceHelper.CalculateNVExperience(doj, isActive: false, lwd: lwd);

        var expected = Math.Round((decimal)(lwd - doj).TotalDays / 365.25m, 1);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void CalculateNVExperience_InactiveEmployeeNullLWD_FallsBackToToday()
    {
        var doj = new DateTime(2025, 1, 1);
        var result = ExperienceHelper.CalculateNVExperience(doj, isActive: false, lwd: null);

        var expected = Math.Round((decimal)(DateTime.UtcNow.Date - doj).TotalDays / 365.25m, 1);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void CalculateNVExperience_LWDBeforeDOJ_ReturnsZero()
    {
        var doj = new DateTime(2025, 6, 1);
        var lwd = new DateTime(2025, 1, 1);
        var result = ExperienceHelper.CalculateNVExperience(doj, isActive: false, lwd: lwd);

        Assert.Equal(0, result);
    }

    [Fact]
    public void CalculateNVExperience_ActiveEmployeeSameDayDOJ_ReturnsZero()
    {
        var doj = DateTime.UtcNow.Date;
        var result = ExperienceHelper.CalculateNVExperience(doj, isActive: true, lwd: null);

        Assert.Equal(0, result);
    }

    [Fact]
    public void CalculateTotalExperience_IncludesPriorExperience()
    {
        var doj = new DateTime(2025, 1, 1);
        var prior = 2.5m;
        var result = ExperienceHelper.CalculateTotalExperience(doj, prior, isActive: true, lwd: null);

        var nvOnly = Math.Round((decimal)(DateTime.UtcNow.Date - doj).TotalDays / 365.25m, 1);
        var expected = Math.Round(nvOnly + prior, 1);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void CalculateTotalExperience_WithPriorAndInactive_ReturnsCorrectSum()
    {
        var doj = new DateTime(2025, 1, 1);
        var lwd = new DateTime(2025, 10, 1);
        var prior = 3.0m;
        var result = ExperienceHelper.CalculateTotalExperience(doj, prior, isActive: false, lwd: lwd);

        var nvOnly = Math.Round((decimal)(lwd - doj).TotalDays / 365.25m, 1);
        var expected = Math.Round(nvOnly + prior, 1);
        Assert.Equal(expected, result);
    }
}
