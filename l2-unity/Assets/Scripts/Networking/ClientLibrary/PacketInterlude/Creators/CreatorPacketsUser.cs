using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.UIElements;
using Unity.Burst.CompilerServices;

public class CreatorPacketsUser 
{
    public static RequestSkillCoolTime CreatSkillCoolTime()
    {
        return new RequestSkillCoolTime();
    }
    public static MoveBackwardToLocation CreateMoveToLocation(Vector3 position, Vector3 target)
    {
        return new MoveBackwardToLocation(position, target);
    }

    public static UseItem CreateUseItem(int _objectId, int _ctrlPressed)
    {
        return new UseItem(_objectId, _ctrlPressed);
    }

    public static RequestDestroyItem CreateDestroyItem(int _objectId, int _count)
    {
        return new RequestDestroyItem(_objectId, _count);
    }

    public static RequestEnchantItem CreateEnchantItem(int _objectId)
    {
        return new RequestEnchantItem(_objectId);
    }

    public static MultiSellChoose CreateMultiSellChoose(int listId, int entryId, int amoun)
    {
        return new MultiSellChoose(listId , entryId , amoun);
    }

    public static RequestBuyItem CreateRequestBuyItem(int listId, List<Product> buyList)
    {
        return new RequestBuyItem(listId,  buyList);
    }

    public static RequestPreviewItem CreateRequestPreviewList(int listId, List<Product> buyList)
    {
        return new RequestPreviewItem(listId, buyList);
    }

    public static RequestSellItem CreateRequestSellItem(int listId, List<Product> buyList)
    {
        return new RequestSellItem(listId, buyList);
    }

    public static RequestShortCutDel CreateDestroyShortCut(int slot)
    {
        return new RequestShortCutDel(slot);
    }

    public static RequestShortCutReg CreateRegShortCut(int typeId, int slot, int id, int level)
    {
        return new RequestShortCutReg(typeId, slot, id, level);
    }

    public static ClickAction CreateActiont(int objectId, int originX, int originY, int originZ, int actionId)
    {
        return new ClickAction(objectId, originX, originY, originZ , actionId);
    }

    public static RequestMagicSkillUse CreateMagicSkilluse(int skillId)
    {
        return new RequestMagicSkillUse(skillId , 0 , 0);
    }
    public static Appearing CreateAppearing()
    {
        return new Appearing();
    }
    public static ValidatePosition CreateValidatePosition(float x, float y, float z)
    {
        return new ValidatePosition(x, y, z);
    }

    public static RequestRestartPoint CreateRestartPoint()
    {
        return new RequestRestartPoint();
    }

    public static RequestBypassToServer CreateByPassPacket(string bypasscommand)
    {
        return new RequestBypassToServer(bypasscommand);
    }

}
