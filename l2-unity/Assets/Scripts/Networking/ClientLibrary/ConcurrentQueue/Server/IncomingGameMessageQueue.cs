using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;


public class IncomingGameMessageQueue: IQueue
{

    private BlockingCollection<ItemServer> _queue;
    private static IncomingGameMessageQueue _instance;
    private GSInterludeMessageHandler _serverPacketHandler;
    private static CancellationTokenSource _cancelTokenSource;
    private static CancellationToken _token;
    private static ManualResetEvent _ewh;
    public static IncomingGameMessageQueue Instance()
    {
        if (_instance == null)
        {
            _cancelTokenSource = new CancellationTokenSource();
            _token = _cancelTokenSource.Token;
            _instance = new IncomingGameMessageQueue();
            _ewh = new ManualResetEvent(false);
        }

        return _instance;
    }



    public void SetPacketHandler(ServerPacketHandler serverPacketHandler)
    {
        this._serverPacketHandler = (GSInterludeMessageHandler) serverPacketHandler;
    }

    public ServerPacketHandler GetPacketHandler()
    {
        return _serverPacketHandler;
    }

    public IncomingGameMessageQueue()
    {
        RestartQueue();
    }
    public void AddItem(ItemServer item)
    {
        if (FilterAccessType.IsAccessTypeMessage(item))
        {
            try
            {
                _ewh.Set();
                if (!_queue.TryAdd(item))
                {
                    Debug.Log("IncomingGameMessageQueue: Критическая ошибка не все пакеты были добавлен!!!!  TryAdd не сработал!");
                }
                //_queue.Add(item);
            }
            catch (Exception ex)
            {
                Debug.Log("IncomingGameMessageQueue: Критическая ошибка не все пакеты были добавлен!!!! " + ex);
            }
        
        }

    }

    private void RestartQueue()
    {
        if (_queue == null)
        {
            _queue = new();
            WaitData();
        }
    }


    public void WaitData()
    {
        Task.Run(() =>
        {
            while (true)
            {
                _ewh.WaitOne();

                if (_token.IsCancellationRequested)
                {
                    break;
                }

                if (_queue.Count > 0)
                {
                     ItemServer item;
                     bool ok = _queue.TryTake(out item);
                    if (ok == true)
                    {
                        //Task.Run(() =>
                        // {
                        //Debug.Log("Running Thread running");
                        _serverPacketHandler.HandlePacket(item);
                        //});
                    }
                    else
                    {
                        Debug.Log("IncomingGameMessageQueue: Критическая ошибка не все пакеты ,были обработаны!!!! ");
                    }
                }
                else
                {
                    _ewh.Reset();
                }
                //Debug.Log("Running Thread");
                //Thread.Sleep(10);
            }
        });
    }

    public void Dispose()
    {
        if (!_cancelTokenSource.IsCancellationRequested)
        {
            _ewh.Set();
            _cancelTokenSource.Cancel();
            //_queue.CompleteAdding();
            _queue = null;
            _instance = null;
            _serverPacketHandler.Dispose();
        }
    }
}

    
