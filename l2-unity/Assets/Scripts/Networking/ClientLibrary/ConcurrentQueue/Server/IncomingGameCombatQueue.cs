using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;


public class IncomingGameCombatQueue : IQueue
{
    private BlockingCollection<ItemServer> _queue;
    private static IncomingGameCombatQueue _instance;
    private ServerPacketHandler _serverPacketHandler;
    private static CancellationTokenSource _cancelTokenSource;
    private static CancellationToken _token;
    private static EventWaitHandle _ewh;
    private bool _isRunning = false;
    public static IncomingGameCombatQueue Instance()
    {
        if (_instance == null)
        {
            _cancelTokenSource = new CancellationTokenSource();
            _token = _cancelTokenSource.Token;
            _instance = new IncomingGameCombatQueue();
            _ewh = new EventWaitHandle(false, EventResetMode.ManualReset);
        }

        return _instance;
    }

    public void SetPacketHandler(ServerPacketHandler serverPacketHandler)
    {
        this._serverPacketHandler = serverPacketHandler;
    }

    public IncomingGameCombatQueue()
    {
        RestartQueue();
    }

    public ItemServer CreateItem(byte[] data, bool init, bool cryptEnbled)
    {
        return new ItemServer(data, init, cryptEnbled);
    }

    public void AddItem(ItemServer item)
    {
        if (FilterAccessType.IsAccessTypeCombat(item))
        {
            if (!InitPacketsLoadWord.getInstance().IsInit)
            {
                FastSinglExecuter.Instance.Execute(item);

            }
              
            _ewh.Set();
            _queue.Add(item);
        }
    }

    private void RestartQueue()
    {
        if (_queue == null)
        {
            _queue = new();
        }
        if (!_isRunning)
        {
            _isRunning = true;
            WaitData();
        }

    }

    public void WaitData()
    {
        Task.Run(() =>
        {
            try
            {
                while (true)
                {
                    _ewh.WaitOne();

                    if (_token.IsCancellationRequested)
                    {
                        break;
                    }
                    UseNormal();
                }
            }
            catch (Exception ex)
            {
                _isRunning = false;
                Debug.Log("Error in WaitData->CombatData " + ex.Message);
                RestartQueue();
            }
            finally
            {
                _isRunning = false;
            }
        
        });
    }

    private void UseNormal()
    {
        if (_queue.Count > 0)
        {
            ItemServer item = _queue.Take();
            if (item != null)
            {
                //we need something that doesn’t freeze the flow processing incoming messages
                //Task.Run(() =>
                //{
                    //Debug.Log("Запускаем извлеченный пакет " + item.ToString());
                    _serverPacketHandler.HandlePacket(item);
                //});

            }
        }
        else
        {
            //Thread.Sleep(10);
            _ewh.Reset();
        }
    }

    public void Dispose()
    {
        if (!_cancelTokenSource.IsCancellationRequested)
        {
            _ewh.Set();
            _cancelTokenSource.Cancel();
            _queue = null;
            _instance = null;
        }
    }

}
