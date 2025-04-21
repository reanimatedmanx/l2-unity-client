using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestList : ServerPacket
{
    public QuestList(byte[] d) : base(d)
    {
        Parse();
    }

    public override void Parse()
    {
        int size = ReadSh();

        for(int i = 0; i < size; i++)
        {
            int questId = ReadI();
            int read = ReadI();
            Debug.Log("");
        }
    }
}
