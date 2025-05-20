using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameInterludeClientPacketType : byte
{
    ProtocolVersion = 0x00,
    AuthLogin = 0x08,
    NewCharacter = 0x0E,
    CharacterCreate = 0x0B,
    CharacterSelect = 0x0D,
    EnterWorld = 0x03,
    RequestSkillCoolTime = 0x9D,
    MoveBackwardToLocation = 0x01,
    RequestBypassToServer = 0x21,
    UseItem = 0x14,
    RequestDestroyItem = 0x59,
    RequestBuyItem = 0x1f,
    RequestSellItem = 0x1e,
    ValidatePosition = 0x48,
    Appearing = 0x30,
    RequestShortCutDel = 0x35,
    RequestShortCutReg = 0x33,
    Action = 0x04,
    RequestRestartPoint = 0x6D,
    RequestMagicSkillUse = 0x2F
}
