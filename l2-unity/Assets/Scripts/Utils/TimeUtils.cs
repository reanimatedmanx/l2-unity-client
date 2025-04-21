using System;
using UnityEngine;

public class TimeUtils
{
    public static float ConvertMsToSec(float milliseconds)
    {
        return (float) milliseconds / 1000.0f;
    }

    public static string GetCurrentFullTime()
    {

        DateTime now = DateTime.Now;
        string formattedTime = now.ToString("yyyy-MM-dd HH:mm:ss.fff");

        return formattedTime;
    }

    public static void PrintFullTime(string text)
    {
        Debug.Log(text + "  " + "PrintFullTime :> " + GetCurrentFullTime());
    }


}
