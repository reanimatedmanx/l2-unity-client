using UnityEngine;

public class InitPacket : ServerPacket {
    private int sessionId;
    private int protocol;
    private int rsaKeyLength;
    private byte[] publicKey;
    private int blowfishKeyLength;
    private byte[] blowfishKey;
    public int gg1;
    public int gg2;
    public int gg3;
    public int gg4;
    //RSAKeyLength - interlude server info size 128
    //blowfishKeyLength - interlude server info size 16
    public int SessionId { get { return sessionId; } }

    public int Protocol { get { return protocol; } }
    public int RSAKeyLength { get { return rsaKeyLength; } }
    public byte[] PublicKey { get { return publicKey; } }
    public byte[] BlowfishKey { get { return blowfishKey; } }

    public int[] GG { get { return new int[4] { gg1,gg2,gg3,gg4}; } }
    public int GG1 { get { return gg1; } }
    public int GG2 { get { return gg2; } }
    public int GG3 { get { return gg3; } }
    public int GG4 { get { return gg4; } }

    public InitPacket(byte[] d) : base(d) {
        rsaKeyLength = 128;
        blowfishKeyLength = 16;
        Parse();
    }

    public override void Parse() {
        sessionId = ReadI();
        protocol = ReadI();

        //Debug.Log(sessionId);
       // Debug.Log(protocol);
        publicKey = ReadB(rsaKeyLength);
        Debug.Log("InitPacket Publick KEY: " + StringUtils.ByteArrayToString(publicKey));
        gg1 = ReadI();
        gg2 = ReadI();
        gg3 = ReadI();
        gg4 = ReadI();

        blowfishKey = ReadB(blowfishKeyLength);
        //Debug.Log("CLEAR: " + StringUtils.ByteArrayToString(blowfishKey));
    }
}