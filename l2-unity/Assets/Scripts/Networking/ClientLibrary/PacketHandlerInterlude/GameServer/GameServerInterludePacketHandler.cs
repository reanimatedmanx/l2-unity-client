using L2_login;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.GPUSort;

public class GameServerInterludePacketHandler : ServerPacketHandler
{
    private bool isKeyAuthСompleted = false;
    public override void HandlePacket(IData itemQueue)
    {
        ItemServer item = (ItemServer)itemQueue;

        switch (item.PaketType())
        {
            case GameInterludeServerPacketType.InterludeKeyPacket:
       
                OnKeyReceive(itemQueue.DecodeData());
                break;
            case GameInterludeServerPacketType.CharSelectionInfo:
            
                OnCharSelectionInfo(itemQueue.DecodeData());
                break;
            case GameInterludeServerPacketType.CharTemplate:
           
                OnCharTemplate(itemQueue.DecodeData());
                break;
            case GameInterludeServerPacketType.CharCreateOk:
         
                OnCharCreateOk(itemQueue.DecodeData());
                break;
            case GameInterludeServerPacketType.CharSelected:
       
                OnCharSelected((itemQueue.DecodeData()));
                break;
            case GameInterludeServerPacketType.CharCreateFail:
          
                OnCharCreateFail((itemQueue.DecodeData()));
                break;
            case GameInterludeServerPacketType.SkillList:
         
                OnCharSkillList(itemQueue.DecodeData());
                break;
            case GameInterludeServerPacketType.UserInfo:

                OnCharUserInfo(itemQueue.DecodeData());
                break;
            case GameInterludeServerPacketType.SkillCoolTime:
             
                OnCharSkillCoolTime(itemQueue.DecodeData());
                break;
            case GameInterludeServerPacketType.MacroList:
            
                OnCharMacroList(itemQueue.DecodeData());
                break;
            case GameInterludeServerPacketType.NpcHtmlMessage:
              
                OnNpcHtmlMessage(itemQueue.DecodeData());
                break;
            case GameInterludeServerPacketType.TutorialShowHtml:
            
                OnTutorialShowHtml(itemQueue.DecodeData());
                break;
            case GameInterludeServerPacketType.BuyList:
              
                OnBuyList(itemQueue.DecodeData());
                break;
            case GameInterludeServerPacketType.ShopPreviewList:
                OnShopPreviewList(itemQueue.DecodeData());
                break;
            case GameInterludeServerPacketType.ShopPreviewInfo:
                OnShopPreviewInfo(itemQueue.DecodeData());
                break;
            case GameInterludeServerPacketType.MultiSellList:
           
                OnMultisellList(itemQueue.DecodeData());
                break;
            case GameInterludeServerPacketType.SellList:
           
                OnSellList(itemQueue.DecodeData());
                break;
            case GameInterludeServerPacketType.WhDepositList:

                OnWhDepositList(itemQueue.DecodeData());
                break;
            case GameInterludeServerPacketType.ShortCutInit:
    
                OnCharShortCutInit(itemQueue.DecodeData());
                break;
            case GameInterludeServerPacketType.ShortCutRegister:
           
                OnCharShortCutRegister(itemQueue.DecodeData());
                break;
            case GameInterludeServerPacketType.SocialAction:
            
                OnSocialAction(itemQueue.DecodeData());
                break;
            case GameInterludeServerPacketType.TeleportToLocation:
             
                OnTeleportToLocation(itemQueue.DecodeData());
                break;
            case GameInterludeServerPacketType.Revive:
        
                OnRevive(itemQueue.DecodeData());
                break;
            case GameInterludeServerPacketType.HennaInfo:
          
                OnCharHennaInfo(itemQueue.DecodeData());
                break;
            case GameInterludeServerPacketType.QuestList:
           
                OnCharQuestList(itemQueue.DecodeData());
                break;
            case GameInterludeServerPacketType.NpcInfo:
        
                OnCharNpcInfo(itemQueue.DecodeData());
                break;
            case GameInterludeServerPacketType.DeleteObject:
          
                OnDeleteObject(itemQueue.DecodeData());
                break;
            case GameInterludeServerPacketType.CharMoveToLocation:
       
                OnMoveToLocation(itemQueue.DecodeData());
                break;
            case GameInterludeServerPacketType.ValidateLocation:
     
                OnValidateLocation(itemQueue.DecodeData());
                break;
            case GameInterludeServerPacketType.FriendList:
   
                OnFriendList(itemQueue.DecodeData());
                break;
            case GameInterludeServerPacketType.EtcStatusUpdate:

                OnEtcStatusUpdate(itemQueue.DecodeData());
                break;
            case GameInterludeServerPacketType.StatusUpdate:

                OnStatusUpdate(itemQueue.DecodeData());
                break;

            case GameInterludeServerPacketType.TargetUnselected:

                OnTargetUnselected(itemQueue.DecodeData());
                break;
            case GameInterludeServerPacketType.AbnormalStatusUpdate:

                OnAbnormalStatusUpdate(itemQueue.DecodeData());
                break;
            case GameInterludeServerPacketType.AcquireSkillList:

                OnAcquireSkillList(itemQueue.DecodeData());
                break;
            case GameInterludeServerPacketType.AcquireSkillInfo:

                OnAcquireSkillInfo(itemQueue.DecodeData());
                break;

        }

    }

    //3 repeated packages are coming ignoring
    private void OnKeyReceive(byte[] data)
    {
        InterludeKeyPacket packet = new InterludeKeyPacket(data);
        if (!isKeyAuthСompleted)
        {
            if (!packet.AuthAllowed)
            {

                //Debug.LogWarning("WAAAARRRNING  Gameserver connect not allowed.");
                EventProcessor.Instance.QueueEvent(() => GameClient.Instance.Disconnect());
                EventProcessor.Instance.QueueEvent(() => LoginClient.Instance.Disconnect());


                return;
            }
            if (packet.UseBlowfish)
            {
                byte[] equalsKey = BlowFishStaticKey.GetCreateFullKeyBlowFish(packet.BlowFishKey);
                GameClient.Instance.EnableCrypt(equalsKey);
            }
            
            //Debug.Log("Data1" + LoginClient.Instance.Account);
            //Debug.Log("Data2" + GameClient.Instance.PlayKey1);
            //Debug.Log("Data3" + GameClient.Instance.PlayKey2);
            //Debug.Log("Data4" + GameClient.Instance.SessionKey1);
            //Debug.Log("Data5" + GameClient.Instance.SessionKey2);

            var sendPaket = CreatorPacketsGameLobby.CreateAuthLogin(LoginClient.Instance.Account, GameClient.Instance.PlayKey1, GameClient.Instance.PlayKey2,
                GameClient.Instance.SessionKey1, GameClient.Instance.SessionKey2);

            bool enable = GameClient.Instance.IsCryptEnabled();

            SendGameDataQueue.Instance().AddItem(sendPaket, enable, enable);
        }
        
        isKeyAuthСompleted = true;
       //_eventProcessor.QueueEvent(() => ((GameClientPacketHandler)_clientPacketHandler).SendAuth());
        //_eventProcessor.QueueEvent(() => ((GameClientPacketHandler)_clientPacketHandler).SendPing());
    }

    private void OnCharSelectionInfo(byte[] data)
    {
        CharSelectionInfo packet = new CharSelectionInfo(data);
        MapClassId.Init();
        EventProcessor.Instance.QueueEvent(() => {
            CharSelectWindow.Instance.SetInterludeCharacterList(packet.Characters);
            CharSelectWindow.Instance.SelectInterludeSlot(packet.SelectedSlotId);
            CharacterSelector.Instance.SetCharacterInterludeList(packet.Characters);
            CharacterSelector.Instance.SelectInterludeCharacter(packet.SelectedSlotId);
            LoginClient.Instance.Disconnect();
            GameClient.Instance.OnAuthAllowed();
        });
    }

    public void OnAbnormalStatusUpdate(byte[] data)
    {
        AbnormalStatusUpdate packet = new AbnormalStatusUpdate(data);
        EventProcessor.Instance.QueueEvent(() => {
            foreach (var item in packet.ListEffect)
            {
                BufferPanel.Instance.AddDataCellToTime(item._id, item._value, item._duration);
            }
        });
    }

    public void OnAcquireSkillList(byte[] data)
    {
        AcquireSkillList packet = new AcquireSkillList(data);
        EventProcessor.Instance.QueueEvent(() => {
            SkillLearnWindow.Instance.AddData(packet.AcquireList);
            SkillLearnWindow.Instance.ShowWindow();
        });
    }

    public void OnAcquireSkillInfo(byte[] data)
    {
        AcquireSkillInfo packet = new AcquireSkillInfo(data);
        EventProcessor.Instance.QueueEvent(() => {
            SkillLearnWindow.Instance.HideWindow();
            DescriptionSkillWindow.Instance.AddData(packet);
            DescriptionSkillWindow.Instance.ShowWindow();
        });
    }
    
    private void OnCharTemplate(byte[] data)
    {
        CharTemplates templates = new CharTemplates(data);
        var list = templates.PlayerTemplates;
        EventProcessor.Instance.QueueEvent(() => {
            GameManager.Instance.OnCreateUser(list);
        });
        
    }

    private void OnCharCreateOk(byte[] data)
    {
        CharCreateOk charOk = new CharCreateOk(data);
        bool create = charOk.IsCreate;
    }

    private void OnCharCreateFail(byte[] data)
    {
        CharCreateFail charOk = new CharCreateFail(data);
        string text = charOk.Text;
        EventProcessor.Instance.QueueEvent(() => {
            GameManager.Instance.OnCreateUserFail(text);
        });
    }

    private void OnCharSkillList(byte[] data)
    {
        SkillList skillListPacket = new SkillList(data);
        PlayerInfoInterlude info1 = GameClient.Instance.PlayerInfo;
        //if (info1.Skills == null) info1.Skills = new CharacterSkills();
        //info1.Skills.AddSkillsList(skillListPacket.Skills);
    }



    private void OnCharSelected(byte[] data)
    {
        Debug.Log("Char Selected event!!");
        CharSelected charOk = new CharSelected(data);
        GameClient.Instance.PlayerInfo = charOk.PlayeInfo;
        GameClient.Instance.SetDataPreparationCompleted(false);
        InitPacketsLoadWord.getInstance().IsInit = true;
       // Task.Run(() =>
        //{
            _eventProcessor.QueueEvent(() => {
                GameClient.Instance.PlayerInfo = charOk.PlayeInfo;
                GameManager.Instance.OnCharacterSelect();
            });
        //});


        var sendPaket = CreatorPacketsGameLobby.CreateEnterWorld();
        bool enable = GameClient.Instance.IsCryptEnabled();
        //Debug.Log("GameServerPacket OnCharUserInfo : Отправил серверу пакет EnterWorld!!!! ");
        SendGameDataQueue.Instance().AddItem(sendPaket, enable, enable);
    }

    private void OnCharUserInfo(byte[] data)
    {
        UserInfo userInfo = new UserInfo(data, GameClient.Instance.PlayerInfo);
        StorageNpc.getInstance().AddUserInfo(userInfo);
        if (InitPacketsLoadWord.getInstance().IsInit)
        {
            GameClient.Instance.SetDataPreparationCompleted(true);
            Debug.Log("GameServerPacket OnCharUserInfo : Завершили обработку пакета UserInfo Init Packet");
        }
        else
        {
            EventProcessor.Instance.QueueEvent(() =>
            {
                World.Instance.UserInfoUpdateCharacter(userInfo);
                UserInfoCharacterCombat(userInfo);
            });
            Debug.Log("GameServerPacket OnCharUserInfo : Завершили обработку пакета UserInfo noraml packet");
        }


    }

    private void UserInfoCharacterCombat(UserInfo userInfo)
    {
        World.Instance.UpdateUserInfo(PlayerEntity.Instance , userInfo);
    }

    private void OnCharSkillCoolTime(byte[] data)
    {
        //Debug.Log("GameServerPacket SkillCoolTime : Обработали но не сохранили т.к не реализован механизм ");
        SkillCoolTime skillCoolTime = new SkillCoolTime(data);
        //Debug.Log("GameServerPacket OnCharSkillCoolTime : Завершено ");

    }
    private void OnCharMacroList(byte[] data)
    {
        //Debug.Log("GameServerPacket OnCharMacroList : Обработали но не сохранили т.к не реализован механизм ");
        MacroList macroList = new MacroList(data);
        //Debug.Log("GameServerPacket OnCharMacroList : Завершено ");
    }

    private void OnNpcHtmlMessage(byte[] data)
    {

        NpcHtmlMessage npcHtmlMessage = new NpcHtmlMessage(data);

        EventProcessor.Instance.QueueEvent(() => {
            Entity npc = World.Instance.GetEntityNoLockSync(npcHtmlMessage.GetNpcId());

            if (npc == null) return;

            var nsm = npc.GetComponent<NpcStateMachine>();

            if(nsm != null)
            {
                Debug.Log("NpcHtmlMessage 2 ");
                nsm.ChangeIntention(NpcIntention.STARTED_TALKING, npcHtmlMessage);
            }
            else
            {
                Vector3 position = PlayerEntity.Instance.transform.position;
                Debug.Log("NpcHtmlMessage 1 ");
                ManualRotate(npc.transform, position);
                HtmlWindow.Instance.InjectToWindow(npcHtmlMessage.Elements());
                HtmlWindow.Instance.ShowWindowToCenter();
            }
           
        });
    }

    private void ManualRotate(Transform npc  , Vector3 player)
    {
        Vector3 direction = player - npc.position;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        npc.rotation = lookRotation;
    }

    private void OnTutorialShowHtml(byte[] data)
    {

        TutorialShowHtml htmlMessage = new TutorialShowHtml(data);

        if (InitPacketsLoadWord.getInstance().IsInit)
        {
            InitPacketsLoadWord.getInstance().AddPacketsInit(htmlMessage);

        }
        else
        {
            EventProcessor.Instance.QueueEvent(() => {
                HtmlWindow.Instance.InjectToWindow(htmlMessage.Elements());
                HtmlWindow.Instance.ShowWindowToCenter();
            });
        }

    }

    private void OnBuyList(byte[] data)
    {
        BuyList buyList = new BuyList(data);
       
        EventProcessor.Instance.QueueEvent(() => {
            UserInfo info = StorageNpc.getInstance().GetFirstUser();
            DealerWindow.Instance.SetWindowName("Shop");
            DealerWindow.Instance.SetHeaderNameSellPanel("Sell");
            DealerWindow.Instance.SetHeaderNameBuyPanel("Buy");
            DealerWindow.Instance.SetProductType(ProductType.BUY);
            DealerWindow.Instance.UpdateBuyData(buyList.Products, false , buyList.ListID);
            DealerWindow.Instance.UpdateDataForm(buyList.CurrentMoney, info.PlayerInfoInterlude.Stats.WeightPercent(), info.PlayerInfoInterlude.Stats.CurrWeight, info.PlayerInfoInterlude.Stats.MaxWeight);
            DealerWindow.Instance.ShowWindowToCenter();
        });

    }

    public void OnShopPreviewList(byte[] data)
    {
        ShopPreviewList shopPreviewList = new ShopPreviewList(data);
        EventProcessor.Instance.QueueEvent(() => {
            UserInfo info = StorageNpc.getInstance().GetFirstUser();
            DealerWindow.Instance.SetHeaderNameSellPanel("Attempt");
            DealerWindow.Instance.SetHeaderNameBuyPanel("Selection list");
            DealerWindow.Instance.SetProductType(ProductType.WEAR);
            DealerWindow.Instance.UpdateBuyData(shopPreviewList.Products, false, shopPreviewList.ListID);
            DealerWindow.Instance.UpdateDataForm(shopPreviewList.CurrentMoney, info.PlayerInfoInterlude.Stats.WeightPercent(), info.PlayerInfoInterlude.Stats.CurrWeight, info.PlayerInfoInterlude.Stats.MaxWeight);
            DealerWindow.Instance.ShowWindowToCenter();
        });
    }

    public void OnShopPreviewInfo(byte[] data)
    {
        ShopPreviewInfo shopPreviewList = new ShopPreviewInfo(data);
        Debug.Log("There is no implementation of this package.> OnShopPreviewInfo");
    }

    private void OnMultisellList(byte[] data)
    {
        MultiSellList multiSellList = new MultiSellList(data);

        EventProcessor.Instance.QueueEvent(() => {
            MultiSellWindow.Instance.AddData(multiSellList.GetOnlyItems()  , multiSellList);
            MultiSellWindow.Instance.ShowWindow();
        });
    }


    private void OnSellList(byte[] data)
    {
        SellList sellList = new SellList(data);

        EventProcessor.Instance.QueueEvent(() => {
            UserInfo info = StorageNpc.getInstance().GetFirstUser();
            DealerWindow.Instance.SetWindowName("Shop");
            DealerWindow.Instance.SetHeaderNameSellPanel("Inventory");
            DealerWindow.Instance.SetHeaderNameBuyPanel("Sell");
            DealerWindow.Instance.SetProductType(ProductType.SELL);
            DealerWindow.Instance.UpdateBuyData(sellList.Products, true, sellList.ListID);
            DealerWindow.Instance.UpdateDataForm(sellList.CurrentMoney, info.PlayerInfoInterlude.Stats.WeightPercent(), info.PlayerInfoInterlude.Stats.CurrWeight, info.PlayerInfoInterlude.Stats.MaxWeight);
            DealerWindow.Instance.ShowWindow();
        });

    }

    private void OnWhDepositList(byte[] data)
    {
        WarehouseDepositList whList = new WarehouseDepositList(data);
        EventProcessor.Instance.QueueEvent(() => {
            UserInfo info = StorageNpc.getInstance().GetFirstUser();
            DealerWindow.Instance.SetWindowName("Personal storage");
            DealerWindow.Instance.SetHeaderNameSellPanel("Inventory");
            DealerWindow.Instance.SetHeaderNameBuyPanel("Items in stock");
            DealerWindow.Instance.SetProductType(ProductType.SELL);
            DealerWindow.Instance.UpdateBuyData(whList.Products, true, -1);
            DealerWindow.Instance.UpdateDataForm(whList.CurrentMoney, info.PlayerInfoInterlude.Stats.WeightPercent(), info.PlayerInfoInterlude.Stats.CurrWeight, info.PlayerInfoInterlude.Stats.MaxWeight);
            DealerWindow.Instance.ShowWindow();
        });
    }

   

    private void OnCharShortCutInit(byte[] data)
    {
        Debug.Log("GameServerPacket OnCharShortCutInit : Приняли пакет");
        ShortCutInit shortCutList = new ShortCutInit(data);
        if (InitPacketsLoadWord.getInstance().IsInit)
        {
            StorageItems.getInstance().AddShortCuts(shortCutList.ShortCuts);
            Debug.Log("GameServerPacket OnCharShortCutInit : Сохранили пока init не запущен!");
        }
        else
        {
            //EventProcessor.Instance.QueueEvent(() => SkillbarWindow.Instance.UpdateAllShortcuts(shortCutList.ShortCuts));
            EventProcessor.Instance.QueueEvent(() => PlayerShortcuts.Instance.SetShortcutList(shortCutList.ShortCuts));
            Debug.Log("GameServerPacket OnCharShortCutInit : Обновили на лету=========");
        }
            
        
        //Debug.Log("GameServerPacket OnCharShortCutInit : Завершено ");
    }

    private void OnCharShortCutRegister(byte[] data)
    {
        ShortCutRegister shortCutPacket = new ShortCutRegister(data);
        EventProcessor.Instance.QueueEvent(() => PlayerShortcuts.Instance.RegisterShortcut(shortCutPacket.Shortcut));
    }

    private void OnSocialAction(byte[] data)
    {
        SocialAction socialAction = new SocialAction(data);
        //Debug.Log("Пришел пакет не обрабатываем Social Action ObjectId +++" + socialAction.ObjectId + " ID +++" + socialAction.ActionId);
    }

    private void OnTeleportToLocation(byte[] data)
    {
        TeleportToLocation teleportLocation = new TeleportToLocation(data);
        //_eventProcessor.QueueEvent(() => {
        //    World.Instance.TeleportTo(teleportLocation.TarObjId, teleportLocation.TeleportPos);
        //});
        
    }

    public void OnRevive(byte[] data)
    {
        //Debug.Log("DoRevive 1");
        Revive revive = new Revive(data);
        EventProcessor.Instance.QueueEvent(() => World.Instance.Revive(revive.ObjectId));
        PlayerEntity.Instance.SetDead(false);
        //Debug.Log("DoRevive 2");
    }


    private void OnCharHennaInfo(byte[] data)
    {
        //Debug.Log("GameServerPacket HennaInfo : Обработали но не сохранили т.к не реализован механизм ");
        HennaInfo hennaInfo = new HennaInfo(data);
        //Debug.Log("GameServerPacket HennaInfo : Завершено ");
    }

    private void OnCharQuestList(byte[] data)
    {
        //Debug.Log("GameServerPacket QuestList : Обработали но не сохранили т.к не реализован механизм ");
        QuestList hennaInfo = new QuestList(data);
        //Debug.Log("GameServerPacket QuestList : Завершено ");
    }

    private void OnCharNpcInfo(byte[] data)
    {
        //Debug.Log("GameServerPacket NpcInfo  : начало обработки пакета ");
        NpcInfo npcInfo = new NpcInfo(data);
        StorageNpc.getInstance().AddNpcInfo(npcInfo);

        if (InitPacketsLoadWord.getInstance().IsInit)
        {
            InitPacketsLoadWord.getInstance().AddPacketsInit(npcInfo);
        }
        else
        {
             _eventProcessor.QueueEvent(() =>{
                 UpdateNpc(npcInfo);
             });
        }
    }

    private void UpdateNpc(NpcInfo npcInfo)
    {
        Entity entity = World.Instance.GetEntityNoLockSync(npcInfo.Identity.Id);

        if (entity == null)
        {
            World.Instance.SpawnNpcInterlude(npcInfo.Identity, npcInfo.Status, npcInfo.Stats);
        }
        else
        {
            World.Instance.UpdateNpcInfo(entity, npcInfo);
        }
    }


    public void OnDeleteObject(byte[] data)
    {
        DeleteObject deletepacket = new DeleteObject(data);
        _eventProcessor.QueueEvent(() => { World.Instance.DeleteObject(deletepacket.ObjectId); });
    }

    private void OnMoveToLocation(byte[] data)
    {
        //Debug.Log("GameServerPacket OnMoveToLocation  : Пришел пакет");
        CharMoveToLocation charMoveToLocation = new CharMoveToLocation(data);
        if (!InitPacketsLoadWord.getInstance().IsInit)
        {
                EventProcessor.Instance.QueueEvent(() => MoveTo(charMoveToLocation.ObjId, charMoveToLocation.NewPosition ,  charMoveToLocation.OldPosition , charMoveToLocation));
        }
        else
        {
            InitPacketsLoadWord.getInstance().AddPacketsInit(charMoveToLocation);
        }
    }

    public async Task MoveTo(int objId, Vector3 charMovePosition , Vector3 currentPosition  , CharMoveToLocation moveToLocation)
    {
            Entity entity = await World.Instance.GetEntityNoLock(objId);
            float distance = GetDistance(entity, charMovePosition);

            if (entity.name.Equals("Elpy")) return;

            if (entity.GetType() == typeof(PlayerEntity))
            {
                PlayerMove(moveToLocation);
            }
            else if (entity.GetType() == typeof(NpcEntity))
            {
                var npc = (NpcEntity)entity;
                NpcMove(npc, moveToLocation);
            }
            else if (entity.GetType() == typeof(MonsterEntity))
            {
                var monster = (MonsterEntity)entity;
                if (!monster.IsDead())
                {
                    DebugLineDraw.ShowDrawLineDebug(objId, charMovePosition, currentPosition , Color.red);
                    MonsterMove(monster, charMovePosition);
                }

            }
       //}
  
    }




    private float GetDistance(Entity entity, Vector3 charMovePosition)
    {
        //monsterStatemachine.MoveMonster.SetFollow(false);
        //var gravityOffTransform = new Vector3(entity.transform.position.x, 0, entity.transform.position.z);
        //var gravityOffTarget = new Vector3(charMovePosition.x, 0, charMovePosition.z);

        return VectorUtils.Distance2D(entity.transform.position, charMovePosition);
    }


    private async Task PlayerMove(CharMoveToLocation charMovePosition)
    {
      //PlayerController.Instance.StopMove();
      PlayerStateMachine.Instance.ChangeIntention(Intention.INTENTION_MOVE_TO, charMovePosition);
    }
    private async Task MonsterMove(MonsterEntity monster, Vector3 monsterMovePosition)
    {
        var msm = monster.GetComponent<MonsterStateMachine>();
        msm.ChangeIntention(MonsterIntention.INTENTION_MOVE_TO, monsterMovePosition);
    }

    private async Task NpcMove(NpcEntity npc, CharMoveToLocation moveToLocation)
    {
        var nsm = npc.GetComponent<NpcStateMachine>();
        if(nsm != null)
        {
            nsm.ChangeIntention(NpcIntention.INTENTION_MOVE_TO, moveToLocation);
        }
        
    }

    private void OnValidateLocation(byte[] data)
    {

        ValidateLocation validateLocation = new ValidateLocation(data);

        if (!InitPacketsLoadWord.getInstance().IsInit)
        {
            PositionValidationController.Instance.AddValidateLocation(validateLocation);
        }
    }

    private void OnFriendList(byte[] data)
    {

        Debug.Log("Friend List SUccess");
    }

    private void OnEtcStatusUpdate(byte[] data)
    {
        EtcStatusUpdate etcStatusUpdate = new EtcStatusUpdate(data);

        if (InitPacketsLoadWord.getInstance().IsInit)
        {
            InitPacketsLoadWord.getInstance().AddPacketsInit(etcStatusUpdate);
        }
        else
        {
            _eventProcessor.QueueEvent(() => {
                BufferPanel.Instance.RefreshPenalty(etcStatusUpdate);
            });
        }

    }

    private void OnStatusUpdate(byte[] data)
    {
        StatusUpdate packet = new StatusUpdate(data);

        if(EventProcessor.Instance != null)
        {
            if(World.Instance != null)
            {
                EventProcessor.Instance.QueueEvent(() => World.Instance.StatusUpdate(packet.ObjectId, packet.Attributes));
            }
           
        }
        
    }

    private void OnTargetUnselected(byte[] data)
    {
        TargetUnselected packet = new TargetUnselected(data);
        EventProcessor.Instance.QueueEvent(() => TargetManager.Instance.UnselectedTarget(packet.ObjId));
    }



    
  

    protected override byte[] DecryptPacket(byte[] data)
    {
        throw new System.NotImplementedException();
    }
}
