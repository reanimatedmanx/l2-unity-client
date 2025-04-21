using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class SendGameDataQueue
{
    private BlockingCollection<ItemSendServer> _queue;
    private static SendGameDataQueue _instance;
    protected ClientPacketHandler _clientPacketHandler;
    private static CancellationTokenSource _cancelTokenSource;
    private static CancellationToken _token;
    public static SendGameDataQueue Instance()
    {
        if (_instance == null)
        {
            _cancelTokenSource = new CancellationTokenSource();
            _token = _cancelTokenSource.Token;
            _instance = new SendGameDataQueue();
        }

        return _instance;
    }

    public void SetPacketHandler(ClientPacketHandler clientPacketHandler)
    {
        this._clientPacketHandler = clientPacketHandler;
    }
    public SendGameDataQueue()
    {
        if (_queue == null) _queue = new();
        WaitSendData();
    }
    public void AddItem(ClientPacket packet, bool encrypt, bool blowfish)
    {
        if (_queue == null) _queue = new();
        _queue.Add(new ItemSendServer(packet, encrypt, blowfish));
    }

    public void WaitSendData()
    {
        Debug.Log("Start Wait Send Data Game CLient");
        Task.Run(() =>
        {
            while (true)
            {
                if (_token.IsCancellationRequested)
                {
                    break;
                }

                if (_queue.Count > 0)
                {
                    ItemSendServer item = _queue.Take();
                    if (item != null)
                    {
                        //Debug.Log("Test Game Send Packet  " + item.GetPacket());
                        var sender = (GameClientInterludePacketHandler)_clientPacketHandler;
                        sender.SendPacket(item.GetPacket());
                        //Debug.Log("Test Game Send Packet 2");
                    }

                }
                //Thread.Sleep(100);
            }
        });
    }



    public void Dispose()
    {
        if (!_cancelTokenSource.IsCancellationRequested)
        {
            _cancelTokenSource.Cancel();
            _queue = null;
            _instance = null;
        }
    }
}
