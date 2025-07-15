using System.Collections.Generic;
using UnityEngine;

public class PackageToList : ServerPacket
{

    private readonly Dictionary<int , string> _players;

    public PackageToList(byte[] d) : base(d)
    {
        Parse();
    }

    public override void Parse()
    {
        int size = ReadI();

        for(int i =0; i < size; i++)
        {
            int objectId = ReadI();
            string name = ReadOtherS();

            if (!_players.ContainsKey(objectId))
            {
                _players.Add(objectId, name);
            }
        }

    }
}
