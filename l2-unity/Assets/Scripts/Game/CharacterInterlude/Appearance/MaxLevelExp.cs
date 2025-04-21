using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class LevelServer
{
    public static Dictionary<int, long> dict;
    public static long GetExp(int lvl)
    {
        if(dict == null)
        {
            dict = new Dictionary<int , long> ();
            Init();
        }

        if(dict.ContainsKey(lvl)) return dict[lvl];

        return -1;
    }

    private static void Init()
    {
        dict.Add(1 , lvl1);
        dict.Add(2, lvl2);
        dict.Add(3, lvl3);
        dict.Add(4, lvl4);
        dict.Add(5, lvl5);
        dict.Add(6, lvl6);
        dict.Add(7, lvl7);
        dict.Add(8, lvl8);
        dict.Add(9, lvl9);
        dict.Add(10, lvl10);
        dict.Add(11, lvl11);
        dict.Add(12, lvl12);
        dict.Add(13, lvl13);
        dict.Add(14, lvl14);
        dict.Add(15, lvl15);
        dict.Add(16, lvl16);
        dict.Add(17, lvl17);
        dict.Add(18, lvl18);
        dict.Add(19, lvl19);
        dict.Add(20, lvl20);
        dict.Add(21, lvl21);
        dict.Add(22, lvl22);
        dict.Add(23, lvl23);
        dict.Add(24, lvl24);
        dict.Add(25, lvl25);
        dict.Add(26, lvl26);
        dict.Add(27, lvl27);
        dict.Add(28, lvl28);
        dict.Add(29, lvl29);
        dict.Add(30, lvl30);

    }

    public static int lvl1 = 0;
    public static int lvl2 = 68;
    public static int lvl3 = 363;
    public static int lvl4 = 1168;
    public static int lvl5 = 2884;
    public static int lvl6 = 6038;
    public static int lvl7 = 11287;
    public static int lvl8 = 19423;
    public static int lvl9 = 31378;
    public static int lvl10 = 48229;
    public static int lvl11 = 71201;
    public static int lvl12 = 101676;
    public static int lvl13 = 141192;
    public static int lvl14 = 191452;
    public static int lvl15 = 254327;
    public static int lvl16 = 331864;
    public static int lvl17 = 426284;
    public static int lvl18 = 539995;
    public static int lvl19 = 675590;
    public static int lvl20 = 835854;
    public static int lvl21 = 1023775;
    public static int lvl22 = 1242536;
    public static int lvl23 = 1495531;
    public static int lvl24 = 1786365;
    public static int lvl25 = 2118860;
    public static int lvl26 = 2497059;
    public static int lvl27 = 2925229;
    public static int lvl28 = 3407873;
    public static int lvl29 = 3949727;
    public static int lvl30 = 4555766;
}
