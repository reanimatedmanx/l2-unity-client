using System.Collections.Generic;
using UnityEngine;

public class ChooseInventoryItem : ServerPacket
{

    private int _itemId;

    public int ItemId { get { return _itemId; } }
    public ChooseInventoryItem(byte[] d) : base(d)
    {
        Parse();
    }

    public override void Parse()
    {

        _itemId = ReadI();
    }
}
