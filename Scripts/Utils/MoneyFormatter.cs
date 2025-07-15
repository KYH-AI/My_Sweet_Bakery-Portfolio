public static class MoneyFormatter 
{
    public static string FormatMoney(long money)
    {
        if (money <= 0)
        {
            return "0";
        }
        
        if (money < 1000)
        {
            return money.ToString();
        }
        if (money < 1000000)
        {
            return (money / 1000f).ToString("0.##") + "K";
        }
        if (money < 1000000000)
        {
            return (money / 1000000f).ToString("0.##") + "M";
        }
        return (money / 1000000000f).ToString("0.##") + "T";
    }
}
