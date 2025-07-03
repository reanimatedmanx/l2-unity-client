using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;

public class IconManager
{
    private string _iconFolder = "Data\\SysTextures\\Icon";
    private string _cursorFolder = "Data\\UI\\Assets\\Cursor";
    private string _defaultNameIconBackground = "ItemWindow_DF_SlotBox_Default";


    private Texture2D _noImageIcon;

    private Dictionary<int, Texture2D> _icons = new Dictionary<int, Texture2D>();
    private Dictionary<int, Texture2D[]> _otherIcons = new Dictionary<int, Texture2D[]>();
    private Dictionary<string, Texture2D> _interfaceIcon = new Dictionary<string, Texture2D>();


    private static IconManager _instance;
    public static IconManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new IconManager();
            }

            return _instance;
        }
    }

    public void Initialize()
    {
        _noImageIcon = GetNoImageIcon();
    }

    public void CacheIcons()
    {
        var weaponValues = ItemTable.Instance.Weapons.Values;
        var armorValues = ItemTable.Instance.Armors.Values;
        var etcalues = ItemTable.Instance.EtcItems.Values;

        foreach (Weapon weapon in ItemTable.Instance.Weapons.Values)
        {
            Texture2D icon = LoadTextureByName(weapon.Icon);
            _icons.Add(weapon.Id, icon);
        }

        foreach (Armor armor in ItemTable.Instance.Armors.Values)
        {
            if(armor.Id == 1146)
            {
                Debug.Log("");
            }
            Texture2D icon = LoadTextureByName(armor.Icon);
            _icons.Add(armor.Id, icon);
        }

        foreach (EtcItem etcItem in ItemTable.Instance.EtcItems.Values)
        {
            Texture2D icon = LoadTextureByName(etcItem.Icon);
            if (!_icons.ContainsKey(etcItem.Id))
            {
                _icons.Add(etcItem.Id, icon);
            }
            
        }
    }

    public void CacheOtherIcons()
    {
        foreach (Armor armor in ItemTable.Instance.Armors.Values)
        {
            string[] otherIcon = armor.Itemgrp.OtherIcon;

            if(otherIcon != null)
            {
                Texture2D[] arrayIcon = new Texture2D[otherIcon.Length];

                if (otherIcon.Length > 1)
                {
                    for (int n = 0; n < otherIcon.Length; n++)
                    {
                        arrayIcon[n] = LoadTextureByName(otherIcon[n]);
                    }

                    _otherIcons.Add(armor.Id, arrayIcon);
                }
            }
   
        }
    }

    public void CacheInterfaceIcons()
    {
        var grade_a = LoadTextureByName("grade_a");
        var grade_b = LoadTextureByName("grade_b");
        var grade_c = LoadTextureByName("grade_c");
        var grade_d = LoadTextureByName("grade_d");
        var grade_s = LoadTextureByName("grade_s");
        _interfaceIcon.Add("grade_a", grade_a);
        _interfaceIcon.Add("grade_b", grade_b);
        _interfaceIcon.Add("grade_c", grade_c);
        _interfaceIcon.Add("grade_d", grade_d);
        _interfaceIcon.Add("grade_s", grade_s);
    }

    public Texture2D GetInterfaceIcon(string nameIcon)
    {
        if (_interfaceIcon.ContainsKey(nameIcon))
        {
            return _interfaceIcon[nameIcon];
        }
        return _noImageIcon;
    }

    public Texture2D LoadTextureByName(string name)
    {
        string icon = _iconFolder + "\\" + CleanIconName(name);
        var result = Resources.Load<Texture2D>(icon);

        //Debug.Log($"Loading icon {name}.");
        if (result != null)
        {
            return result;
        }

        Debug.LogWarning($"Missing icon: {name}.");

        return _noImageIcon;
    }

    public Texture2D LoadTextureOtherSources(string href)
    {
        var result = Resources.Load<Texture2D>(href);

        //Debug.Log($"Loading icon {name}.");
        if (result != null)
        {
            return result;
        }

        Debug.LogWarning($"Missing texture: {href}.");

        return _noImageIcon;
    }

    public Texture2D GetInvetoryDefaultBackground()
    {
        return LoadTextureByName(_defaultNameIconBackground);
    }



    public Texture2D LoadCursorByName(string name)
    {
        string icon = _cursorFolder + "\\" + name;
        var result = Resources.Load<Texture2D>(icon);
        return result;
    }

    public Texture2D GetIcon(int id)
    {
        Texture2D icon;
        _icons.TryGetValue(id, out icon);

        if (icon == null)
        {
            _icons.Add(id, _noImageIcon);
            return _noImageIcon;
        }

        return icon;
    }

    public Texture2D GetOtherIcon(int id , int indexIcon)
    {
        Texture2D[] alIcon;
        _otherIcons.TryGetValue(id, out alIcon);

        if (alIcon == null)
        {
            _icons.Add(id, _noImageIcon);
            return _noImageIcon;
        }

        if(ArrayUtils.IsValidIndexArray(alIcon, indexIcon))
        {
            return alIcon[indexIcon];
        }

        return null;
    }


    private Texture2D GetNoImageIcon()
    {
        return Resources.Load<Texture2D>(_iconFolder + "\\" + "NOIMAGE");
    }

    private string CleanIconName(string name)
    {
        if(name != null)
        {
            return name.Replace("icon.", "");
        }
        return "";
    }
}
