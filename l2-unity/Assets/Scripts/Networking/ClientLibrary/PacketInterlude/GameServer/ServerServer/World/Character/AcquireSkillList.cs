using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class AcquireSkillList : ServerPacket
{
    private List<OtherModel> _acquireList;
    public List<OtherModel> AcquireList { get => _acquireList; }
    public AcquireSkillList(byte[] d) : base(d)
    {
        _acquireList = new List<OtherModel>();
        Parse();
    }


    public override void Parse()
    {
        int skillType = ReadI();
        int size = ReadI();

        switch (skillType)
        {
            case (int)AcquireSkillType.USUAL:
                ParceUsual(size , _acquireList);
                break;
            case (int)AcquireSkillType.FISHING:
                ParceFishing(size, _acquireList);
                break;
            case (int)AcquireSkillType.CLAN:
                ParceClan(size, _acquireList);
                break;
        }
    }
    //writeD(gsn.getId());
    //writeD(gsn.getValue());
    //writeD(gsn.getValue());
    //writeD(gsn.getCorrectedCost());
    //writeD(0);
    private void ParceUsual(int size , List<OtherModel> _acquireList)
    {
        for(int i = 0; i < size; i++)
        {
            int id = ReadI();
            int value1=  ReadI();
            int value2 = ReadI();
            int correctCost =  ReadI();
            int unk1 = ReadI();
            _acquireList.Add(new OtherModel(new AcquireData(id, value1, value2, correctCost)));
        }
    }

    private void ParceFishing(int size, List<OtherModel> _acquireList)
    {
        for (int i = 0; i < size; i++)
        {
            int id = ReadI();
            int value1 = ReadI();
            int value2 = ReadI();
            int unk1 = ReadI();
            int unk2 = ReadI();
            _acquireList.Add(new OtherModel(new AcquireData(id, value1, value2, unk1)));
        }
    }


    private void ParceClan(int size, List<OtherModel> _acquireList)
    {
        for (int i = 0; i < size; i++)
        {
            int id = ReadI();
            int value1 = ReadI();
            int value2 = ReadI();
            int cost = ReadI();
            int unk2 = ReadI();
            //OtherModel otherModel = new OtherModel(new AcquireData(id, value1, value2, cost));
            _acquireList.Add(new OtherModel(new AcquireData(id, value1, value2, cost)));
        }
    }
}

public class AcquireData
{
    private int _id;
    private int _value1;
    private int _value2;
    private int _correctCost;

    public AcquireData(int id , int value1 , int value2 , int correctCost)
    {
        _id = id;
        _value1 = value1;
        _value2 = value2;
        _correctCost = correctCost;
    }

    public int GetId()
    {
        return _id;
    }

    public int GetCost()
    {
        return _correctCost;
    }

    public int GetValue1()
    {
        return _value1;
    }

}
