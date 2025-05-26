using NUnit.Framework.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;


public class ItemNameTable {
    private static ItemNameTable _instance;
    public static ItemNameTable Instance {
        get {
            if (_instance == null) {
                _instance = new ItemNameTable();
            }

            return _instance;
        }
    }

    private Dictionary<int, ItemName> _itemNames;
    public Dictionary<int, ItemName> ItemNames { get { return _itemNames; } }

    public void Initialize() {
        ReadItemNameDat();
        ReadItemNameInterlude();
    }

    private void ReadItemNameDat() {
        _itemNames = new Dictionary<int, ItemName>();
        string dataPath = Path.Combine(Application.streamingAssetsPath, "Data/Meta/ItemName_Classic-eu.txt");
        if (!File.Exists(dataPath)) {
            Debug.LogWarning("File not found: " + dataPath);
            return;
        }

        using (StreamReader reader = new StreamReader(dataPath)) {
            string line;
            while ((line = reader.ReadLine()) != null) {
                ItemName itemName = new ItemName();

                string[] keyvals = line.Split('\t');

                for (int i = 0; i < keyvals.Length; i++) {
                    if (!keyvals[i].Contains("=")) {
                        continue;
                    }

                    string[] keyval = keyvals[i].Split("=");
                    string key = keyval[0];
                    string value = keyval[1];

                    switch (key) {
                        case "id":
                            itemName.Id = int.Parse(value);
                            break;
                        case "name": 
                            itemName.Name = DatUtils.CleanupString(value);
                            break;
                        case "description": 
                            itemName.Description = DatUtils.CleanupString(value);
                            break;
                        case "default_action":
                            itemName.DefaultAction = DatUtils.CleanupString(value);
                            break;
                        case "is_trade":
                            itemName.Tradeable = value == "1";
                            break;
                        case "is_drop":
                            itemName.Droppable = value == "1";
                            break;
                        case "is_destruct":
                            itemName.Destructible = value == "1";
                            break;
                        case "is_npctrade":
                            itemName.Sellable = value == "1";
                            break;                    
                    }
                }

                if (!ItemTable.Instance.ShouldLoadItem(itemName.Id)) {
                    continue;
                }

                _itemNames.TryAdd(itemName.Id, itemName);
            }

            Debug.Log($"Successfully imported {_itemNames.Count} itemNames(s)");
        }
    }

    public ItemName GetItemName(int id) {
        ItemName itemName;
        _itemNames.TryGetValue(id, out itemName);
        return itemName;
    }

    public void ReadItemNameInterlude()
    {
        //_actionsInterlude = new Dictionary<int, ActionData>();
        string dataPath = Path.Combine(Application.streamingAssetsPath, "Data/Meta/Itemname-e_interlude.txt");


        
        using (StreamReader reader = new StreamReader(dataPath))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (line.IndexOf(".\\0") > -1)
                {
                    ItemName itemData = new ItemName();

                    string[] ids = line.Split('\t');
                    //int s1 = line.IndexOf("a,") + 2;
                    //int s2 = line.IndexOf(".\\0");


                    string parts = DatUtils.CleanupStringOldData(ids[5]);


                    itemData.Id = Int32.Parse(ids[0]);
                    itemData.Name = ids[1];

                    var sets = GetSetsIds(itemData.Id, parts);
                    SetDescriptionText(itemData, parts, ids);

                    //itemData.Description = GetDesciptionText(line, s1, s2);

                    if (itemData.Id == 23)
                    {
                        Debug.Log("");
                    }
                    if (_itemNames.ContainsKey(itemData.Id))
                    {
                        if (string.IsNullOrEmpty(_itemNames[itemData.Id].Description))
                        {
                            _itemNames[itemData.Id].Description = itemData.Description;
                            _itemNames[itemData.Id].SetSets(sets);
                        }

                    }
                }

            }

        }
    }

    private void  SetDescriptionText(ItemName itemData , string parts , string[] ids)
    {
        if (string.IsNullOrEmpty(parts))
        {
            var oldDescript = DatUtils.CleanupStringOldData(ids[3]);
            itemData.Description = oldDescript;
        }
        else
        {
            string descript_parts = DatUtils.CleanupStringOldData(ids[6]);
            itemData.Description = descript_parts;
        }
    }

    private int[] GetSetsIds(int id , string text)
    {
        if (!string.IsNullOrEmpty(text))
        {
            //var text1 = text.Replace(id.ToString()+",", "");
            string[] stringArray = text.Split(',');

            return stringArray
                .Select(s => {
                    int number;
                    return int.TryParse(s, out number) ? number : 0; // или выбросьте исключение, если необходимо
                }).ToArray();

        }
        else
        {
            return null;
        }

    }
}
