using Xunit;
using TollCalculator;
using TollCalculator.Toll;

namespace TollCalculator.Tests;

public class TollCalculatorTest
{
    
    private static readonly TollFee[] TollFees = new TollFee[]
    {
        new TollFee(new TimeSpan(6, 0, 0), new TimeSpan(6, 30, 0), 8m),
        new TollFee(new TimeSpan(6, 30, 0), new TimeSpan(7, 0, 0), 13m),
        new TollFee(new TimeSpan(7, 0, 0), new TimeSpan(8, 0, 0), 18m),
        new TollFee(new TimeSpan(8, 0, 0), new TimeSpan(15, 0, 0), 8m),
        new TollFee(new TimeSpan(15, 0, 0), new TimeSpan(15, 30, 0), 13m),
        new TollFee(new TimeSpan(15, 30, 0), new TimeSpan(17, 0, 0), 18m),
        new TollFee(new TimeSpan(17, 0, 0), new TimeSpan(18, 0, 0), 13m),
        new TollFee(new TimeSpan(18, 0, 0), new TimeSpan(6, 0, 0), 0m),
    };

    private static readonly TollFeeTable TollFeeTable = TollFeeTable.CreateInstance(TollFees);
    private static readonly TollCalculator Sut = new TollCalculator(TollFeeTable);
    
    [Fact]
    public void Max_fine_should_be_60SEK()
    {
        
        var vehicle = new Vehicle("Car");
        var dateArray = new DateTime[]
        {
            new DateTime(2023, 5, 1),
            new DateTime(2023, 6, 15),
            new DateTime(2023, 7, 20)
        };
        var result = Sut.GetTollFee(vehicle, dateArray);

        Assert.Equal(60, result);

    }
    
        [Fact]
    public void Multiple_fees_per_day_get_summed_up()
    {
        var vehicle = new Vehicle("Car");
        var dates = new[]
        {
            new DateTime(2024, 5, 13, 14, 59, 0),
            new DateTime(2024, 5, 13, 15, 0, 0),
            new DateTime(2024, 5, 13, 16, 59, 0),
        };
        Assert.Equal(39, Sut.GetTollFee(vehicle, dates));
    }

    [Fact]
    public void Fee_over_multiple_days_sum_up()
    {
        var vehicle = new Vehicle("Car");
        var dates = new[]
        {
            new DateTime(2024, 5, 13, 14, 59, 0),
            new DateTime(2024, 5, 13, 15, 0, 0),
            new DateTime(2024, 5, 13, 16, 59, 0),
            new DateTime(2024, 5, 14, 6, 30, 0),
            new DateTime(2024, 5, 14, 7, 59, 0),
            new DateTime(2024, 5, 14, 14, 59, 0),
            new DateTime(2024, 5, 14, 15, 0, 0),
            new DateTime(2024, 5, 14, 16, 59, 0),
        };
        Assert.Equal(99, Sut.GetTollFee(vehicle, dates));
    }
    

    [Fact]
    public void Only_highest_fee_applies_per_hour()
    {
        var vehicle = new Vehicle("Car");
        var dates = new[]
        {
            new DateTime(2024, 5, 13, 15, 0, 0),
            new DateTime(2024, 5, 13, 15, 30, 0),
        };
        Assert.Equal(18, Sut.GetTollFee(vehicle, dates));
    }

    [Theory]
    [InlineData("Motorbike")]
    [InlineData("Tractor")]
    [InlineData("Emergency")]
    [InlineData("Diplomat")]
    [InlineData("Foreign")]
    [InlineData("Military")]
    public void No_fee_on_fee_free_vehicles(string vehicleType)
    {
        var vehicle = new Vehicle(vehicleType);
        var dates = new[] { new DateTime(2024, 5, 13, 6, 0, 0) };
        Assert.Equal(0, Sut.GetTollFee(vehicle, dates));
    }

    [Theory]
    [MemberData(nameof(GetFeeFreeDates))]
    public void No_fee_on_fee_free_days(DateTime[] dates)
    {
        var vehicle = new Vehicle("Car");
        Assert.Equal(0, Sut.GetTollFee(vehicle, dates));
    }

    public static IEnumerable<object[]> GetFeeFreeDates()
    {
        yield return new object[] { new DateTime[] { new DateTime(2024, 5, 11, 17, 0, 0) } };  // weekend
        yield return new object[] { new DateTime[] { new DateTime(2024, 5, 12, 17, 0, 0) } };  // weekend
        yield return new object[] { new DateTime[] { new DateTime(2024, 5, 9, 17, 0, 0) } };   // holiday
    }
}