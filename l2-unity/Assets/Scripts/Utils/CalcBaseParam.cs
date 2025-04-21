using System;
using UnityEngine;

public class CalcBaseParam
{
    public static  float CalculateTimeL2j(float patkSpeed)
    {
        return Math.Max(100, 500000 / patkSpeed);
    }

    //public float GetTime(int pAtkSpd)
   // {
    //    return Math.Max(100, 500000 / pAtkSpd);
   // }

    public static float GetAnimatedSpeed(int pAtkSpd, float timeAtck)
    {
        return pAtkSpd / timeAtck;
    }
}
