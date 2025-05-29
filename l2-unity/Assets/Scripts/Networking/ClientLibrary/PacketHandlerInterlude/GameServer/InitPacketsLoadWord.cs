using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            ForEachPackets();
            //ForEachDie();
            //EtcStatusUpdate();
        });

    }
    private void SpawnNpc()
    {
        //List<int> remove = new List<int>();
        for (int i = 0; i < listPackets.Count; i++)
        {
            if (listPackets[i] is NpcInfo)
            {
                NpcInfo npcInfo = (NpcInfo)listPackets[i];
                //remove.Add(i);
                EventProcessor.Instance.QueueEvent(() => { World.Instance.SpawnNpcInterlude(npcInfo.Identity, npcInfo.Status, npcInfo.Stats); });
                Thread.Sleep(10);
            }
        }

        //RemoveByListId(remove);
    }
    private void UpdateItemsInventory()
    {
        var items = StorageItems.getInstance().GetItems();
        var equipItems = StorageItems.getInstance().GetEquipItems();
        List<ItemInstance> itemInstances= items.Values.ToList();
        int adenaCount = GetAdenaCount(itemInstances);
        //EventProcessor.Instance.QueueEvent(() => PlayerInventory.Instance.SetInventory(items, equipItems , StorageItems.getInstance().GetShowWindow() , adenaCount));
        PlayerInventory.Instance.SetInventory(items, equipItems, StorageItems.getInstance().GetShowWindow(), adenaCount);
    }


    private int GetAdenaCount(List<ItemInstance> modified)
    {
        ItemInstance item = modified.FirstOrDefault(o => o.Category == ItemCategory.Adena);
        return (item != null) ? item.Count : 0;
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
    private void ForEachPackets()
    {
        List<int > remove = new List<int>();
        
        for (int i = 0; i < listPackets.Count; i++)
        {
            ServerPacket packet = listPackets[i];
            switch (packet)
            {
                case SystemMessagePacket systemMessage:
                    ShowMessage(systemMessage);
                    remove.Add(i);
                    break;
                case CreatureSay creatureSay:
                    ShowMessagCreaturee(creatureSay.Message);
                    remove.Add(i);
                    break;
                case NpcSay npcSay:
                    ShowMessagCreaturee(npcSay.NpcMessage);
                    remove.Add(i);
                    break;
                case Die die:
                    OnDie(die);
                    remove.Add(i);
                    break;
                case EtcStatusUpdate etcStatusUpdate:
                    Refreshpenalty(etcStatusUpdate);
                    remove.Add(i);
                    break;
            }
        }
        //RemoveByListId(remove);
    }

    private async Task RemoveByListId(List<int> remove)
    {
        await Task.Delay(3000);

        for (int i = 0; i < remove.Count; i++)
        {
            int index = remove[i];
            listPackets.RemoveAt(index);
        }

        Debug.Log("Init size array remove " + listPackets.Count);
        remove.Clear();
    }

    public CharMoveToLocation GetMoveToLocation(int id)
    {
        List<CharMoveToLocation> list = new List<CharMoveToLocation>();
        for (int i = 0; i < listPackets.Count; i++)
        {
            ServerPacket packet = listPackets[i];
            if(packet.GetType() == typeof(CharMoveToLocation))
            {
                CharMoveToLocation movepack = (CharMoveToLocation)packet;
                if (movepack.ObjId == id)
                {
                    list.Add(movepack);
                }
            }
        }
        return list.OrderByDescending(o => o.CreatedAt).FirstOrDefault();
    }
    private void Refreshpenalty(EtcStatusUpdate etcStatusUpdate)
    {
        EventProcessor.Instance.QueueEvent(() => BufferPanel.Instance.RefreshPenalty(etcStatusUpdate));
    }

    public async Task MoveTo(CharMoveToLocation moveToLocation)
    {
        Entity entity = await World.Instance.GetEntityNoLock(moveToLocation.ObjId);
        Debug.Log("Добавлен пакет перемещаемся MoveTo Init ");
        if(entity != null)
        {
            if (entity.GetType() == typeof(NpcEntity))
            {
                var npc = (NpcEntity)entity;
                Debug.Log("Добавлен пакет перемещаемся есть entity 1 Entity MoveTo Init ");
                NpcMove(npc, moveToLocation);
                
            }
        }
    }

    private async Task NpcMove(NpcEntity npc, CharMoveToLocation moveToLocation)
    {
       // var nsm = npc.GetComponent<NpcStateMachine>();
        //if (nsm != null)
        //{
            //nsm.ChangeIntention(NpcIntention.INTENTION_MOVE_TO, moveToLocation);
            npc.transform.position = moveToLocation.NewPosition;
            Debug.Log("Добавлен пакет перемещаемся есть entity 2 Entity MoveTo Init ");
        //}

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
