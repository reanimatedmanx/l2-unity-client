using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using static StorageVariable;


public class SystemMessageTable {
    private static SystemMessageTable _instance;
    public static SystemMessageTable Instance {
        get {
            if (_instance == null) {
                _instance = new SystemMessageTable();
            }

            return _instance;
        }
    }

    private Dictionary<int, SystemMessageDat> _systemMessages;
    public Dictionary<int, SystemMessageDat> SystemMessages { get { return _systemMessages; } }

    public void Initialize() {
        ReadDatFile();
    }

    private void ReadDatFile() {
        _systemMessages = new Dictionary<int, SystemMessageDat>();
        string dataPath = Path.Combine(Application.streamingAssetsPath, "Data/Meta/SystemMsg_Classic-eu.txt");
        if (!File.Exists(dataPath)) {
            Debug.LogWarning("File not found: " + dataPath);
            return;
        }

        using (StreamReader reader = new StreamReader(dataPath)) {
            string line;
            while ((line = reader.ReadLine()) != null) {
                SystemMessageDat systemMessage = new SystemMessageDat();

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
                            systemMessage.Id = int.Parse(value);
                            break;
                        case "message":
                            string message = value.Replace("[", "").Replace("]", "");
                            systemMessage.Message = message;
                            systemMessage.OriginalMessage = message;
                            break;
                        case "color":
                            systemMessage.Color = value;
                            string r = systemMessage.Color.Substring(0, 2);
                            string g = systemMessage.Color.Substring(2, 2);
                            string b = systemMessage.Color.Substring(4, 2);
                            string a = systemMessage.Color.Substring(6, 2);
                            systemMessage.Color =  b + g + r + "B0";
                            break;
                        case "group":
                            systemMessage.Group = int.Parse(value);
                            break;                 
                    }
                }

                _systemMessages.TryAdd(systemMessage.Id, systemMessage);
            }

            Debug.Log($"Successfully imported {_systemMessages.Count} system message(s)");
        }
    }

    public SystemMessageDat GetSystemMessage(int id) {
        SystemMessageDat message;
        _systemMessages.TryGetValue(id, out message);

        if (IsReplaceS1(id))
        {
            message.Message = GetVariableText(message.OriginalMessage);
        }

        return message;
    }


    private bool IsReplaceS1(int id)
    {
        if (id != (int)StorageVariable.MessageID.LEARNED_SKILL_S1)
        {
            return false;
        }

        return true;
    }
    private string GetVariableText(string text)
    {
        string finalStr = text;

        if (text.IndexOf("$s1") != -1)
        {
            string name = "";
            if (StorageVariable.getInstance().GetVariableByName("$s1") != null)
            {
                name = StorageVariable.getInstance().GetVariableByName("$s1").Name;
            }

            finalStr =  text.Replace("$s1", name).Trim();

        }
         if(text.IndexOf("$s2") != -1)
        {
            string name = "";
            if (StorageVariable.getInstance().GetVariableByName("$s2") != null)
            {
                name = StorageVariable.getInstance().GetVariableByName("$s2").Name;
            }

            finalStr = finalStr.Replace("$s2", name).Trim();
        }
        if (text.IndexOf("$c1") != -1)
        {
            string name = "";
            if (StorageVariable.getInstance().GetVariableByName("$c1") != null)
            {
                name = StorageVariable.getInstance().GetVariableByName("$c1").Name;
            }

            finalStr = finalStr.Replace("$c1", name).Trim();
        }
        return finalStr;
    }
}
