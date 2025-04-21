using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameInterludeServerPacketType : byte
{
    InterludeKeyPacket = 0x00,
    CharSelectionInfo = 0x13,
    CharTemplate = 0x17,
    CharCreateOk = 0x19,
    CharCreateFail = 0x1A,
    CharSelected = 0x15,
    SkillList = 0x58,
    UserInfo = 0x04,
    SkillCoolTime = 0xC1,
    MacroList = 0xE7,
    BuyList = 0x11,
    NpcHtmlMessage = 0x0F,
    ItemList = 0x1B,
    ShortCutInit = 0x45,
    ShortCutRegister = 0x44,
    HennaInfo = 0xE4,
    QuestList = 0x80,
    NpcInfo = 0x16,
    DeleteObject = 0x12,
    CharMoveToLocation = 0x01,
    FriendList = 0xFA,
    ValidateLocation = 0x61,
    EtcStatusUpdate = 0xF3,
    StatusUpdate = 0x0E,
    InventoryUpdate = 0x27,
    TargetUnselected = 0x2A,
    SocialAction = 0x2D,
    TeleportToLocation = 0x28,
    Revive = 0x07
}
