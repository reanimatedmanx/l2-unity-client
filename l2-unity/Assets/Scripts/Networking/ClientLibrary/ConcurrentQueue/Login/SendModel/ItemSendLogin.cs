using L2_login;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSendLogin : IData
{
    private ClientPacket packet;
    private bool encrypt;
    public ItemSendLogin(ClientPacket packet , bool encrypt , bool blowfish)
    {
        this.packet = packet;
        this.encrypt = encrypt;
        Encrypt(packet, encrypt);
        Blowfish(packet.GetData(), blowfish);
    }
    public byte[] DecodeData()
    {
        throw new System.NotImplementedException();
    }

    private void Encrypt(ClientPacket packet , bool encrypt)
    {
        if (encrypt)
        {
            byte[] data = packet.GetData();
            NewCrypt.appendChecksum(data);
        }
    }

    private void Blowfish(byte[] data, bool blowfish)
    {
        if (blowfish)
        {
            LoginClient.Instance.EncryptBlowFish.processBigBlock(data, 0, data, 0, data.Length);
        }
        
    }

    public ClientPacket GetPacket()
    {
        return packet;
    }
}
