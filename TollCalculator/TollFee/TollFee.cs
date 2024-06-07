namespace TollCalculator.Toll;

public class TollFee(TimeSpan startTimeSpam, TimeSpan endTimeSpam, decimal fee)
{
    private TimeSpan StartTimeSpam { get; } = startTimeSpam;
    private TimeSpan EndTimeSpam { get; } = endTimeSpam;
    private decimal Fee { get; } = fee;
}