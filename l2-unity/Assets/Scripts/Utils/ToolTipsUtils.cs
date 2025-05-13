using System.Globalization;
using UnityEngine;

public class ToolTipsUtils 
{
    public static Color GetColorPrice(string price)
    {
        if (price.Length == 6)
        {
            return new Color(255f / 255f, 126f / 255f, 255f / 255f);
            //return new Color(0, 251, 255);
        }
        else if (price.Length == 5)
        {
            return new Color(0, 251, 255);
        }
        return new Color(169, 180, 196);
    }
    public static string ConvertToPrice(int wholeNumber)
    {
        string formattedNumber = wholeNumber.ToString("N0", CultureInfo.InvariantCulture);
        return formattedNumber.Replace('.', ',');
    }
}
