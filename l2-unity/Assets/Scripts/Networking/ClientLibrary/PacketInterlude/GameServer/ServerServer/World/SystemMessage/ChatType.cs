using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum  ChatType: int
{
    GENERAL = 0,
    SHOUT = 1,
    WHISPER = 2,
    PARTY = 3,
    CLAN = 4 ,
    GM = 5,
    PETITION_PLAYER = 6,
    PETITION_GM = 7,
    TRADE = 8,
    ALLIANCE = 9,
    ANNOUNCEMENT = 10,
    BOAT = 11,
    FRIEND = 12,
    MSNCHAT = 13,
    PARTYMATCH_ROOM = 14,
    PARTYROOM_COMMANDER = 15,
    PARTYROOM_ALL =16,
    HERO_VOICE = 17,
    CRITICAL_ANNOUNCE = 18,
    SCREEN_ANNOUNCE = 19,
    BATTLEFIELD= 20,
    MPCC_ROOM = 21,
    //NPC_GENERAL(0), // Epilogue adjustment
    //NPC_SHOUT(1); // Epilogue adjustment
}
