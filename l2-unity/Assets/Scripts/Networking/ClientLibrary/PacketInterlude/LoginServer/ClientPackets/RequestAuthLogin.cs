using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using static UnityEngine.UI.Image;

public class RequestAuthLogin : ClientPacket
{
    public RequestAuthLogin(string account , string password) : base((byte)LoginInterludeClientPacketTYpe.RequestAuthLogin)
    {

        try
        {
            byte[] accountBytes = Encoding.UTF8.GetBytes(account);
            byte[] shaPass = Encoding.UTF8.GetBytes(password);
            // byte[] shaPass = SHACrypt.ComputeSha256HashToBytes("22222222");

            byte[] testRsa = new byte[113];

            //l2j server checks these bytes 79 login (size 14 bytes)
            //l2j server these bytes 93 password (size 14 bytes)
            Array.Copy(accountBytes, 0, testRsa, 79, accountBytes.Length);
            Array.Copy(shaPass, 0, testRsa, 93, shaPass.Length);

            var rsaBlock = LoginClient.Instance.RSACrypt.EncryptRSANoPadding(testRsa);

            WriteB(rsaBlock);

            BuildPacket();

        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
        
    }




}
