using TollCalculator.Toll;

namespace TollCalculator;

public class TollCalculator(TollFeeTable feeTable)
{
    private readonly TollFeeTable _feeTable = feeTable;

    public int GetTollFee(Vehicle vehicle, DateTime[] dates)
    {
        return -1;
    }
}