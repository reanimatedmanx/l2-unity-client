using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;


//		buffer.writeByte(0x0b);
//buffer.writeInt(_response);
//buffer.writeInt(0);
//buffer.writeInt(0);
//buffer.writeInt(0);
//buffer.writeInt(0);
public class GGAuth : ServerPacket
{
    private int sessionId;
    public int SessionId { get { return sessionId; } }
    public GGAuth(byte[] d) : base(d)
    {
        Parse();
    }


    public override void Parse()
    {
        var sessionId1 = ReadB();
        var response = ReadI();
        var sessionId3 = ReadI();
        var sessionId4 = ReadI();
        var sessionId5 = ReadI();
        var sessionId6 = ReadI();
        Debug.Log("GGAuth response " + response);
    }
}
