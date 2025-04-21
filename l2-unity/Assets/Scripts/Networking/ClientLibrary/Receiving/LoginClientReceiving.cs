using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class LoginClientReceiving
{
    private AsynchronousClient _asyncClient;

    public LoginClientReceiving(AsynchronousClient asyncClient) { 

        this._asyncClient = asyncClient;
    }

    public void StartReceiving(Socket _socket)
    {
        Debug.Log("Start receiving LoginClient");
        Task.Run(() => {
            Receiving(_socket);
        });
    }
     
    private void Receiving(Socket _socket)
    {
        using (NetworkStream stream = new NetworkStream(_socket))
        {
            int lengthHi;
            int lengthLo;
            int length;
            try
            {

                for (; ; )
                {
                    if (!_asyncClient.IsConnected)
                    {
                        Debug.LogWarning("Disconnected.");
                        break;
                    }

                    lengthLo = stream.ReadByte();
                    lengthHi = stream.ReadByte();
                    length = (lengthHi * 256) + lengthLo;
                    if (lengthHi == -1 || !_asyncClient.IsConnected)
                    {
                        Debug.Log("Server terminated the connection.");
                        _asyncClient.Disconnect();
                        break;
                    }

                    byte[] data = new byte[length];

                    int bytesRead = 0;
                    int chunkSize = 1;
                    while (bytesRead < data.Length && chunkSize > 0)
                    {

                        if (stream.DataAvailable)
                        {
                            bytesRead += chunkSize = stream.Read(data, bytesRead, length - bytesRead);
                        }
                        else
                        {
                            break;
                        }
                    }

                    Array.Resize(ref data, bytesRead);


                    IncomingLoginDataQueue.Instance().AddItem(data , _asyncClient.InitPacket , _asyncClient.CryptEnabled);

                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

        }
    }
}
