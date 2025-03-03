namespace Common.BalanceValues;


public static class AppConstants
{
    public static readonly HashSet<int> TopUpValuesSet = new HashSet<int> { 50, 100, 200, 300, 400, 500 };

    public static string GetValidValuesString()
    {
        return string.Join(",", TopUpValuesSet);
    }
}
