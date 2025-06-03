using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;


public class GameClientReceiving : MonoBehaviour
{
    private AsynchronousClient _asyncClient;
    private int header = 2;
    public GameClientReceiving(AsynchronousClient asyncClient)
    {

        this._asyncClient = asyncClient;
    }

    public void StartReceiving(Socket _socket)
    {
        Debug.Log("Start receiving GameClient");
        Task.Run(() => {
            Receiving(_socket);
        });
    }
   
    private void Receiving(Socket _socket)
    {
        using (NetworkStream stream = new NetworkStream(_socket))
        {
            //int lengthHi;
            //int lengthLo;
           // int length;
            try
            {
                int length = -1;
                //Debug.Log("");
                for (; ; )
                {
                    if (!_asyncClient.IsConnected)
                    {
                        Debug.LogWarning("Disconnected.");
                        break;
                    }


                    if (stream.DataAvailable == true)
                    {
                        if (length == -1)
                        {
                            length = GetLength(stream);
                        }

                        byte[] buffer = new byte[length];
                        buffer = ReadWholeArray(stream, buffer);
                        length = -1;

                        if (buffer.Length > 0)
                        {
                            //We convey one point to everyone. if the package id does not match, it will be ignored
                            ItemServer item = IncomingGameDataQueue.Instance().CreateItem(buffer, _asyncClient.InitPacket, _asyncClient.CryptEnabled);
                            if (item.ByteType() == 0xFE)
                            {

                                Debug.Log(" Size packet ExStorageMaxCount " + buffer.Length);
                                //46 ExStorageMaxCount
                                Debug.Log("пришел пакет ex  попытка игнорировать>>>>>>");
                            }
                            else
                            {
                                //test slowdown of packets if they fly too fast some of them are dropped by the blocking collection a fix may be needed
                                //if (item.ByteType() == 0x27)
                                //{
                                //    Debug.Log(" Inventory Update Server Get ");
                               // }

                                IncomingGameCombatQueue.Instance().AddItem(item);
                                IncomingGameDataQueue.Instance().AddItem(item);
                                IncomingGameMessageQueue.Instance().AddItem(item);
                                //Debug.Log("");
                            }
                        }
                        else
                        {
                            Debug.Log(" Пришел кривой пакет размер его 0! ");
                        }
                    }


                    //lengthLo = stream.ReadByte();
                    //lengthHi = stream.ReadByte();
                    //length = (lengthHi * 256) + lengthLo;
                    //if (lengthHi == -1 || !_asyncClient.IsConnected)
                    //{
                    //  Debug.Log("Server terminated the connection.");
                    //  _asyncClient.Disconnect();
                    //   break;
                    //}

                    //byte[] data = new byte[length];

                    //int bytesRead = 0;
                    //int chunkSize = 1;

                    // while (bytesRead < data.Length && chunkSize > 0)
                    // {

                    //if (stream.DataAvailable)
                    //{
                    //bytesRead += chunkSize = stream.Read(data, bytesRead, length - bytesRead);
                    //}
                    //else
                    //{
                    //     break;
                    // }
                    // }

                    // Array.Resize(ref data, bytesRead);



                    // }
                    //else
                    //{
                    //    Debug.Log("GameClientReceiving получили кривой пакет в нем нет данных!");
                    //}

                    //ReadWholeArray(stream, new byte[2]);
                    //var buffer = new List<byte>();

                   // while (_socket.Available > 0)
                    //{
                        //var currByte = new Byte[1];
                        //var byteCounter = _socket.Receive(currByte, currByte.Length, SocketFlags.None);

                       // if (byteCounter.Equals(1))
                       // {
                        //    buffer.Add(currByte[0]);
                       // }
                   // }

                   // buffer.ToArray();
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

        }
    }

    public static byte[] ReadWholeArray(Stream stream, byte[] data)
    {
        int offset = 0;
        int remaining = data.Length;
        while (remaining > 0)
        {
            int read = stream.Read(data, offset, remaining);
            if (read <= 0)
                throw new EndOfStreamException(String.Format("End of stream reached with {0} bytes left to read", remaining));
            remaining -= read;
            offset += read;
        }
        return data;
    }

   

    private int GetLength(Stream stream)
    {
          int length = 0;
          byte[] bytes = ReadWholeArray(stream, new byte[header]);
          length = ReadSh(bytes);

        if (length <= 0)
            throw new EndOfStreamException();
         

        if (length > 0)
          {
            length = length - header;
          }
        
         return length;
    }

   
    protected int ReadSh(byte[] _packetData)
    {
        byte[] data = new byte[2];
        Array.Copy(_packetData, 0, data, 0, 2);
        Array.Reverse(data);

        short value = ByteUtils.fromByteArrayShort(data);

        return value;
    }
}

