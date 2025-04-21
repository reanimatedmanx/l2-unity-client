using L2_login;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemServer : IData
{
    private byte[] _data;
    private GameInterludeServerPacketType packetType;
    private byte _byteType;
    public ItemServer(byte[] data, bool init, bool cryptEnbled)
    {

        Decrypt(data, cryptEnbled, init);
        packetType = (GameInterludeServerPacketType)data[0];
        _byteType = data[0];
        this._data = data;
    }

    public byte[] DecodeData() { return _data; }

    public byte ByteType() { return _byteType; }
    public GameInterludeServerPacketType PaketType() { return packetType; }

    private void Decrypt(byte[] data, bool cryptEnbled, bool init)
    {
        if (cryptEnbled)
        {
            GameClient.Instance.GameCrypt.GameServerDecrypt(data, 0, data.Length);
        }

    }

  

    public ClientPacket GetPacket()
    {
        throw new System.NotImplementedException();
    }

    public string ToString()
    {
        return "ItemServer{" +
            ", packetType=" + packetType +
            ", byteType=" + _byteType +
            '}';
    }
}
