using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class EtcItemgrpTable {
    private static EtcItemgrpTable _instance;
    public static EtcItemgrpTable Instance {
        get {
            if (_instance == null) {
                _instance = new EtcItemgrpTable();
            }

            return _instance;
        }
    }

    private Dictionary<int, EtcItemgrp> _etcItemGrps;
    public Dictionary<int, EtcItemgrp> EtcItemGrps { get { return _etcItemGrps; } }

    public EtcItemgrp GetEtcItem(int id)
    {
        EtcItemgrp item;
        EtcItemGrps.TryGetValue(id, out item);
        return item;
    }


    public void Initialize() {
        ReadEtcItemgrpDat();
        ReadEtcItemInterlude();
    }

    private void ReadEtcItemgrpDat() {
        _etcItemGrps = new Dictionary<int, EtcItemgrp>();
        string dataPath = Path.Combine(Application.streamingAssetsPath, "Data/Meta/EtcItemgrp_Classic.txt");
        if (!File.Exists(dataPath)) {
            Debug.LogWarning("File not found: " + dataPath);
            return;
        }

        using (StreamReader reader = new StreamReader(dataPath)) {
            string line;
            while ((line = reader.ReadLine()) != null) {
                EtcItemgrp etcItemgrp = new EtcItemgrp();
                string[] keyvals = line.Split('\t');

                for (int i = 0; i < keyvals.Length; i++) {
                    if (!keyvals[i].Contains("=")) {
                        continue;
                    }

                    string[] keyval = keyvals[i].Split("=");
                    string key = keyval[0];
                    string value = keyval[1];

                    if (DatUtils.ParseBaseAbstractItemGrpDat(etcItemgrp, key, value)) {
                        continue;
                    }

                    switch (key) {
                        case "etcitem_type": 
                            etcItemgrp.EtcItemType = value;
                            break;
                        case "consume_type":
                            //Debug.Log("Consume Category " + value  + " ID "  + etcItemgrp.ObjectId);
                            etcItemgrp.ConsumeType = ConsumeType.ParceCategory(value);
                            break;
                    }
                }


                if (!ItemTable.Instance.ShouldLoadItem(etcItemgrp.ObjectId)) {
                    continue;
                }

                _etcItemGrps.TryAdd(etcItemgrp.ObjectId, etcItemgrp);
            }

            Debug.Log($"Successfully imported {_etcItemGrps.Count} etcItemgrp(s)");
        }
    }


    private int indexIcon = 13;

    public void ReadEtcItemInterlude()
    {
        //_actionsInterlude = new Dictionary<int, ActionData>();
        string dataPath = Path.Combine(Application.streamingAssetsPath, "Data/Meta/EtcItemgrp_interlude.txt");

        using (StreamReader reader = new StreamReader(dataPath))
        {
            string line;
            int index = 0;
            while ((line = reader.ReadLine()) != null)
            {
                //string[] test = line.Split('\t');
                if (index != 0)
                {
                    string[] ids = line.Split('\t');
                    int id = Int32.Parse(ids[1]);


                    if (!_etcItemGrps.ContainsKey(id))
                    {
                        EtcItemgrp etcItemgrp = new EtcItemgrp();

                        if (IsIndexValid(ids, indexIcon))
                        {

                            etcItemgrp.ObjectId = id;
                            etcItemgrp.Icon = ids[indexIcon];
                            _etcItemGrps.Add(id, etcItemgrp);
                        }
                    }
                }

                index++;
            }

        }
    }

    bool IsIndexValid<T>(T[] array, int index)
    {
        return index >= 0 && index < array.Length;
    }
}