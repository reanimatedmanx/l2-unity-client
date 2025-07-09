using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class SkillNameTable
{
    private static SkillNameTable _instance;
    private static Dictionary<int, Dictionary<int, SkillNameData>> _names;
    public Dictionary<int, Dictionary<int, SkillNameData>> Names { get { return _names; } }
    public static SkillNameTable Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new SkillNameTable();
            }

            return _instance;
        }
    }

    

    public SkillNameData GetName(int id , int level)
    {
        if (Names.ContainsKey(id) != true) return null;

        Dictionary<int, SkillNameData> _skillNameLevel = Names[id];
        if (!_skillNameLevel.ContainsKey(level)) return null;

        return _skillNameLevel[level];
    }
    

    public void Initialize()
    {
        _names = new Dictionary<int, Dictionary<int, SkillNameData>>();
        ReadActions();
        ReadNameInterlude();
        Debug.Log("");
    }

    private void ReadActions()
    {
       
        string dataPath = Path.Combine(Application.streamingAssetsPath, "Data/Meta/SkillName_Classic-eu.txt");

        if (!File.Exists(dataPath))
        {
            Debug.LogWarning("File not found: " + dataPath);
            return;
        }

        using (StreamReader reader = new StreamReader(dataPath))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                SkillNameData nameData = new SkillNameData();

                string[] keyvals = line.Split('\t');

                for (int i = 0; i < keyvals.Length; i++)
                {
                    if (!keyvals[i].Contains("="))
                    {
                        continue;
                    }

                    string[] keyval = keyvals[i].Split("=");
                    string key = keyval[0];
                    string value = keyval[1];
                    
                    switch (key)
                    {
                        case "skill_id":
                            nameData.Id = int.Parse(value);
                            if (nameData.Id.Equals(410))
                            {
                                //Debug.Log("");
                            }
                           break;
                        case "skill_level":
                            nameData.Level = int.Parse(value);
                            break;
                        case "skill_sublevel":
                            nameData.SubLevel = int.Parse(value);
                            break;
                        case "name":
                            nameData.Name = DatUtils.CleanupString(value);
                            break;
                        case "desc":
                            nameData.Desc = DatUtils.CleanupString(value);
                            break;
                        case "prev_skill_id":
                            nameData.PrevSkillId = int.Parse(value);
                            break;
                    }
                }

                TryAdd(nameData);
            }

            Debug.Log($"Successfully imported {_names.Count} actionName(s)");
        }
    }

    private void TryAdd(SkillNameData skillName)
    {

        if (_names.ContainsKey(skillName.Id) == true)
        {
            Dictionary<int, SkillNameData> dataGrp = _names[skillName.Id];
            dataGrp.TryAdd(skillName.Level, skillName);
        }
        else
        {
            Dictionary<int, SkillNameData> dataGrp = CreateDict();
            AddDict(dataGrp, skillName);
            _names.TryAdd(skillName.Id, dataGrp);
        }
    }

    private Dictionary<int, SkillNameData> CreateDict()
    {
        return new Dictionary<int, SkillNameData>();
    }
    private void AddDict(Dictionary<int, SkillNameData> dataGrp, SkillNameData skillName)
    {
        dataGrp.TryAdd(skillName.Level, skillName);
    }

    private int indexDescription = 3;

    public void ReadNameInterlude()
    {
        string dataPath = Path.Combine(Application.streamingAssetsPath, "Data/Meta/Skillname_interlude-e.txt");

        using (StreamReader reader = new StreamReader(dataPath))
        {
            string line;
            int index = 0;
            while ((line = reader.ReadLine()) != null)
            {
                if (index != 0)
                {
                    string[] ids = line.Split('\t');


                    int id = int.Parse(ids[0]);
                    int level = int.Parse(ids[1]);
                    string description = DatUtils.CleanupStringOldData(ids[indexDescription]);
                    string description1 = description.Replace("Effect", "Power");

                    ReplaceDescription(description1, id, level);
                }

                index++;
            }

        }
    }

    private void ReplaceDescription(string description , int id , int level)
    {
        if (_names.ContainsKey(id) == true)
        {
            Dictionary<int, SkillNameData> dataGrp = _names[id];
            if (dataGrp.ContainsKey(level))
            {
                SkillNameData data = dataGrp[level];
                data.Desc = description;
            }
         
        }
    }
}
