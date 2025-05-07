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
            ForEachPackets();
            //ForEachDie();
            //EtcStatusUpdate();
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
    private void ForEachPackets()
    {
        for (int i = 0; i < listPackets.Count; i++)
        {
            ServerPacket packet = listPackets[i];
            switch (packet)
            {
                case SystemMessagePacket systemMessage:
                    ShowMessage(systemMessage);
                    break;
                case CreatureSay creatureSay:
                    ShowMessagCreaturee(creatureSay.Message);
                    break;
                case NpcSay npcSay:
                    ShowMessagCreaturee(npcSay.NpcMessage);
                    break;
                case Die die:
                    OnDie(die);
                    break;
                case EtcStatusUpdate etcStatusUpdate:
                    Refreshpenalty(etcStatusUpdate);
                    break;
                case TutorialShowHtml showTutorial:
                    Task.Run(() => ShowTutorial(700, showTutorial));
                    break;
                case CharMoveToLocation moveTo:
                    EventProcessor.Instance.QueueEvent(() => MoveTo(moveTo));
                    break;
            }
        }
        //ForEachMessage();
        //ForEachCreatureSay();
        //ForEachNpcSay();
    }

    private void Refreshpenalty(EtcStatusUpdate etcStatusUpdate)
    {
        EventProcessor.Instance.QueueEvent(() => BufferPanel.Instance.RefreshPenalty(etcStatusUpdate));
    }
    private async Task ShowTutorial(int delayMilliseconds , TutorialShowHtml showTutorial)
    {
        await Task.Delay(delayMilliseconds);

        EventProcessor.Instance.QueueEvent(() => {
            HtmlWindow.Instance.InjectToWindow(showTutorial.Elements());
            HtmlWindow.Instance.ShowWindow();
        });
    }

    public async Task MoveTo(CharMoveToLocation moveToLocation)
    {
        Entity entity = await World.Instance.GetEntityNoLock(moveToLocation.ObjId);
        if(entity != null)
        {
            if (entity.GetType() == typeof(NpcEntity))
            {
                var npc = (NpcEntity)entity;
                NpcMove(npc, moveToLocation);
            }
        }

    }

    private async Task NpcMove(NpcEntity npc, CharMoveToLocation moveToLocation)
    {
        var nsm = npc.GetComponent<NpcStateMachine>();
        if (nsm != null)
        {
            //nsm.ChangeIntention(NpcIntention.INTENTION_MOVE_TO, moveToLocation);
            npc.transform.position = moveToLocation.NewPosition;
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
