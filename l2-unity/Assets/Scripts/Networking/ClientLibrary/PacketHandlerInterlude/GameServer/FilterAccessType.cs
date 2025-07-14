using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FilterAccessType
{
    public static bool IsAccessTypeGame(ItemServer item)
    {
        if(item.PaketType() == GameInterludeServerPacketType.InterludeKeyPacket)
        {
            return true;
        }else if (item.PaketType() ==  GameInterludeServerPacketType.CharSelectionInfo)
        {
            return true;
        }
        else if (item.PaketType() == GameInterludeServerPacketType.CharTemplate)
        {
            return true;
        }
        else if (item.PaketType() == GameInterludeServerPacketType.CharCreateOk)
        {
            return true;
        }
        else if (item.PaketType() == GameInterludeServerPacketType.SocialAction)
        {
            return true;
        }
        else if (item.PaketType() == GameInterludeServerPacketType.CharSelected)
        {
            return true;
        }
        else if (item.PaketType() == GameInterludeServerPacketType.CharCreateFail)
        {
            return true;
        }
        else if (item.PaketType() == GameInterludeServerPacketType.SkillList)
        {
            return true;
        }
        else if (item.PaketType() == GameInterludeServerPacketType.UserInfo)
        {
            return true;
        }
        else if (item.PaketType() == GameInterludeServerPacketType.SkillCoolTime)
        {
            return true;
        }
        else if (item.PaketType() == GameInterludeServerPacketType.MacroList)
        {
            return true;
        }
        else if (item.PaketType() == GameInterludeServerPacketType.NpcHtmlMessage)
        {
            return true;
        }
        else if (item.PaketType() == GameInterludeServerPacketType.TutorialShowHtml)
        {
            return true;
        }
        else if (item.PaketType() == GameInterludeServerPacketType.BuyList)
        {
            return true;
        }
        else if (item.PaketType() == GameInterludeServerPacketType.ShopPreviewList)
        {
            return true;
        }
        else if (item.PaketType() == GameInterludeServerPacketType.ShopPreviewInfo)
        {
            return true;
        }
        else if (item.PaketType() == GameInterludeServerPacketType.MultiSellList)
        {
            return true;
        }
        else if (item.PaketType() == GameInterludeServerPacketType.SellList)
        {
            return true;
        }
        else if (item.PaketType() == GameInterludeServerPacketType.WhDepositList)
        {
            return true;
        }
        else if (item.PaketType() == GameInterludeServerPacketType.WhWithdrawList)
        {
            return true;
        }

        else if (item.PaketType() == GameInterludeServerPacketType.ShortCutInit)
        {
            return true;
        }
        else if (item.PaketType() == GameInterludeServerPacketType.ShortCutRegister)
        {
            return true;
        }
        else if (item.PaketType() == GameInterludeServerPacketType.HennaInfo)
        {
            return true;
        }
        else if (item.PaketType() == GameInterludeServerPacketType.QuestList)
        {
            return true;
        }
        else if (item.PaketType() == GameInterludeServerPacketType.NpcInfo)
        {
            return true;
        }
        else if (item.PaketType() == GameInterludeServerPacketType.DeleteObject)
        {
            return true;
        }
        else if (item.PaketType() == GameInterludeServerPacketType.CharMoveToLocation)
        {
            return true;
        }
        else if (item.PaketType() == GameInterludeServerPacketType.ValidateLocation)
        {
            return true;
        }
        else if (item.PaketType() == GameInterludeServerPacketType.FriendList)
        {
            return true;
        }
        else if (item.PaketType() == GameInterludeServerPacketType.EtcStatusUpdate)
        {
            return true;
        }
        else if (item.PaketType() == GameInterludeServerPacketType.StatusUpdate)
        {
            return true;
        }
        else if (item.PaketType() == GameInterludeServerPacketType.TargetUnselected)
        {
            return true;
        }
        else if (item.PaketType() == GameInterludeServerPacketType.TeleportToLocation)
        {
            return true;
        }
        else if (item.PaketType() == GameInterludeServerPacketType.Revive)
        {
            return true;
        }
        else if (item.PaketType() == GameInterludeServerPacketType.AbnormalStatusUpdate)
        {
            return true;
        }
        else if (item.PaketType() == GameInterludeServerPacketType.AcquireSkillList)
        {
            return true;
        }
        else if (item.PaketType() == GameInterludeServerPacketType.AcquireSkillInfo)
        {
            return true;
        }
        return false;
    }
    public static bool IsAccessTypeCombat(ItemServer item)
    {
        GSInterludeCombatPacketType type = (GSInterludeCombatPacketType)item.ByteType();
         if (type == GSInterludeCombatPacketType.MyTargetSelected)
         {
            return true;
         }else if (type == GSInterludeCombatPacketType.MoveToPawn)
         {
            return true;
         }
        else if (type == GSInterludeCombatPacketType.STOP_MOVE)
        {
            return true;
        }
        else if (type == GSInterludeCombatPacketType.ATTACK)
        {
            return true;
        }
        else if (type == GSInterludeCombatPacketType.ActionFailed)
        {
            return true;
        }
        else if (type == GSInterludeCombatPacketType.AUTO_ATTACK_START)
        {
            return true;
        }
        else if (type == GSInterludeCombatPacketType.AUTO_ATTACK_STOP)
        {
            return true;
        }
        else if (type == GSInterludeCombatPacketType.DIE)
        {
            return true;
        }
        else if (type == GSInterludeCombatPacketType.MagicSkillUse)
        {
            return true;
        }
        else if (type == GSInterludeCombatPacketType.SetupGauge)
        {
            return true;
        }
        else if (type == GSInterludeCombatPacketType.ItemList)
        {
            return true;
        }
        else if (type == GSInterludeCombatPacketType.InventoryUpdate)
        {
            return true;
        }
        else if (type == GSInterludeCombatPacketType.ChooseInventoryItem)
        {
            return true;
        }
        else if (type == GSInterludeCombatPacketType.EnchantResult)
        {
            Debug.Log("Enchanted inventory 1");
            return true;
        }
        else if (type == GSInterludeCombatPacketType.MagicSkillLaunched)
        {
            return true;
        }
        //else if (type == GSInterludeCombatPacketType.CharMoveToLocation)
        // {
        //     return true;
        // }
        return false;
    }
    public static bool IsAccessTypeMessage(ItemServer item)
    {
        GSInterludeMessagePacketType type = (GSInterludeMessagePacketType)item.ByteType();

        if (type == GSInterludeMessagePacketType.SystemMessage)
        {
            Debug.Log("Enchanted inventory 2");
            return true;
        }
        else if (type == GSInterludeMessagePacketType.CreatureSay)
        {
            return true;
        }
        else if (type == GSInterludeMessagePacketType.NpcSay)
        {
            return true;
        }

        return false;
    }
}
