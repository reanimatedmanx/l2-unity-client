using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

public class SkillList : ServerPacket
{
    private List<SkillServer> list;
    public List<SkillServer> Skills { get; private set; }

    public SkillList(byte[] d) : base(d)
    {
        list = new List<SkillServer>();
        Parse();
    }

    public override void Parse()
    {
        
        int size = ReadI();
        for(int i = 0; i < size; i++)
        {
            bool passive = ReadI() == 1;
            int pLevel = ReadI();
            int pId = ReadI();
            int disabled = (int) ReadB();

            SkillServer skill = new SkillServer(pId, pLevel, passive, disabled == 1);
            list.Add(skill);
        }
    }


}
