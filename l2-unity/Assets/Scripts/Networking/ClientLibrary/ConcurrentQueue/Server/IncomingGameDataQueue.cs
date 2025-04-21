using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class IncomingGameDataQueue : IQueue
{
    private BlockingCollection<ItemServer> _queue;
    private BlockingCollection<ItemServer> _queueHigh;
    private static IncomingGameDataQueue _instance;
    private ServerPacketHandler _serverPacketHandler;
    private static CancellationTokenSource _cancelTokenSource;
    private static CancellationToken _token;
    public static IncomingGameDataQueue Instance()
    {
        if (_instance == null)
        {
            _cancelTokenSource = new CancellationTokenSource();
            _token = _cancelTokenSource.Token;
            _instance = new IncomingGameDataQueue();
        }

        return _instance;
    }

    public void SetPacketHandler(ServerPacketHandler serverPacketHandler)
    {
        this._serverPacketHandler = serverPacketHandler;
    }
    public IncomingGameDataQueue()
    {
        RestartQueue();
    }

    public ItemServer CreateItem(byte[] data, bool init, bool cryptEnbled)
    {
        return new ItemServer(data, init, cryptEnbled);
    }
    public void AddItem(ItemServer item )
    {
        if(FilterAccessType.IsAccessTypeGame(item))
        {
            if(item.PaketType() == GameInterludeServerPacketType.InventoryUpdate | item.PaketType() == GameInterludeServerPacketType.ShortCutInit)
            {
                _queueHigh.Add(item);
            }
            else
            {
                _queue.Add(item);
            }
            
        }      
    }

    
    
    private void RestartQueue()
    {
        if (_queue == null)
        {
            _queue = new();
            _queueHigh = new();
            WaitData();
        }
    }


    public void WaitData()
    {
        Task.Run(() =>
        {
            while (true)
            {
                if (_token.IsCancellationRequested)
                {
                    break;
                }
                UseHigh();
                UseNormal();
                //Thread.Sleep(10);
            }
        });
    }


    private void UseHigh()
    {
        if (_queueHigh.Count > 0)
        {
            ItemServer item = _queueHigh.Take();
            if (item != null)
            {
                //we need something that doesn’t freeze the flow processing incoming messages
                Task.Run(() =>
                {
                    Debug.Log("Âûñîêèé óðîâåíü íà îáðàáîòêó " + item.ByteType());
                    _serverPacketHandler.HandlePacket(item);
                });

            }
        }
    }

    private void UseNormal()
    {
        if (_queue.Count > 0)
        {
            ItemServer item = _queue.Take();
            if (item != null)
            {
                //we need something that doesn’t freeze the flow processing incoming messages
                Task.Run(() =>
                {
                    _serverPacketHandler.HandlePacket(item);
                });

            }
        }
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
