using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class InitPacketsLoadWord
{
    private static InitPacketsLoadWord instance;

    private static List<ServerPacket> listPackets;

    private InitPacketsLoadWord()
    { }

    public static InitPacketsLoadWord getInstance()
    {
        if (instance == null)
        {
            instance = new InitPacketsLoadWord();
            listPackets = new List<ServerPacket>();
        }
            
        return instance;
    }

    public bool IsInit { get; set; }

    public void AddPacketsInit(ServerPacket packet)
    {
        listPackets.Add(packet);
    }

    public void UseInitPackets()
    {
        Task.Run(() =>
        {
            SpawnNpc();
            UpdateItemsInventory();
            UpdateShortCuts();
            UpdateInventoryBar();
            ShowMessage();
            ForEachDie();
            EtcStatusUpdate();
        });

    }
    private void SpawnNpc()
    {
        for (int i = 0; i < listPackets.Count; i++)
        {
            if (listPackets[i] is NpcInfo)
            {
                NpcInfo npcInfo = (NpcInfo)listPackets[i];
                EventProcessor.Instance.QueueEvent(() => { World.Instance.SpawnNpcInterlude(npcInfo.Identity, npcInfo.Status, npcInfo.Stats); });
                Thread.Sleep(10);
            }
        }
    }
    private void UpdateItemsInventory()
    {
        EventProcessor.Instance.QueueEvent(() => PlayerInventory.Instance.SetInventory(StorageItems.getInstance().GetItems(), StorageItems.getInstance().GetShowWindow()));
    }

    private void UpdateShortCuts()
    {
        //EventProcessor.Instance.QueueEvent(() => SkillbarWindow.Instance.UpdateAllShortcuts(StorageItems.getInstance().GetShortCuts()));
        EventProcessor.Instance.QueueEvent(() => PlayerShortcuts.Instance.SetShortcutList(StorageItems.getInstance().GetShortCuts()));
    }

    private void UpdateInventoryBar()
    {
        UserInfo info = StorageNpc.getInstance().GetFirstUser();
        if(info != null & info.PlayerInfoInterlude.Stats != null)
        {
            EventProcessor.Instance.QueueEvent(() => { InventoryWindow.Instance.UpdateStats(info.PlayerInfoInterlude.Stats); });
        }
       
    }
    private void ShowMessage()
    {
        ForEachMessage();
        ForEachCreatureSay();
        ForEachNpcSay();
    }

    private void EtcStatusUpdate()
    {
        for (int i = 0; i < listPackets.Count; i++)
        {
            if (listPackets[i] is EtcStatusUpdate)
            {
                var packet = (EtcStatusUpdate)listPackets[i];
                EventProcessor.Instance.QueueEvent(() => BufferPanel.Instance.RefreshPenalty(packet));
            }
        }
    }

    private void ForEachMessage()
    {
        for (int i = 0; i < listPackets.Count; i++)
        {
            if (listPackets[i] is SystemMessagePacket)
            {
                ShowMessage((SystemMessagePacket)listPackets[i]);
                //Thread.Sleep(10);
            }
        }
    }

    private void ForEachDie()
    {
        for (int i = 0; i < listPackets.Count; i++)
        {
            if (listPackets[i] is Die)
            {
                OnDie((Die)listPackets[i]);
            }
        }
    }

    private void ForEachCreatureSay()
    {
        for (int i = 0; i < listPackets.Count; i++)
        {
            if (listPackets[i] is CreatureSay)
            {
                var packet = (CreatureSay)listPackets[i];
                ShowMessagCreaturee(packet.Message);
                //Thread.Sleep(10);
            }
        }
    }

    private void ForEachNpcSay()
    {
        for (int i = 0; i < listPackets.Count; i++)
        {
            if (listPackets[i] is NpcSay)
            {
                var packet = (NpcSay)listPackets[i];
                ShowMessagCreaturee(packet.NpcMessage);
                //Thread.Sleep(10);
            }
        }
    }

    private void ShowMessagCreaturee(CreatureMessage creatureMessage)
    {

        if (creatureMessage != null)
        {

            EventProcessor.Instance.QueueEvent(() => ChatWindow.Instance.ReceiveSystemMessage(creatureMessage));
        }
        else
        {
            EventProcessor.Instance.QueueEvent(() => ChatWindow.Instance.ReceiveSystemMessage(new UnhandledMessage()));
        }
    }
    private void ShowMessage(SystemMessagePacket packet)
    {
        SMParam[] smParams = packet.Params;
        int messageId = packet.Id;

        SystemMessageDat messageData = SystemMessageTable.Instance.GetSystemMessage(messageId);

        if (messageData != null)
        {
            SystemMessage systemMessage = new SystemMessage(smParams, messageData);
            EventProcessor.Instance.QueueEvent(() => ChatWindow.Instance.ReceiveSystemMessage(systemMessage));
        }
        else
        {
            EventProcessor.Instance.QueueEvent(() => ChatWindow.Instance.ReceiveSystemMessage(new UnhandledMessage()));
        }
    }

    private void OnDie(Die packet)
    {
       EventProcessor.Instance.QueueEvent(() => PlayerStateMachine.Instance.ChangeIntention(Intention.INTENTION_DEAD , packet));
    }

}
