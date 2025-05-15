using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class RequestBuyItem : ClientPacket
{
    public RequestBuyItem(int listId , List<Product> buyList) : base((byte)GameInterludeClientPacketType.RequestBuyItem)
    {
        WriteI(listId);
        WriteI(buyList.Count);

        foreach (var item in buyList)
        {
            WriteI(item.ItemId);
            WriteI(item.Count);
        }

        BuildPacket();
    }
}
