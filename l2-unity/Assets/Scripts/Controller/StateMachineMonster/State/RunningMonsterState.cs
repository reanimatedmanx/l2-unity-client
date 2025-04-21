using System.Security.Cryptography;
using UnityEditorInternal;
using UnityEngine;
using static AttackingState;

public class RunningMonsterState : MonsterBase
{
    private MonsterEntity monsterEntity;
    public RunningMonsterState(MonsterStateMachine stateMachine) : base(stateMachine) { }
    public override void Enter()
    {
        monsterEntity = (MonsterEntity)_stateMachine.Entity;
        monsterEntity.StartRunAnim(true);
    }
    public override void Exit() { 
    }
    public override void Update() { }
    public override void HandleEvent(Event evt)
    {
        switch (evt)
        {
            case Event.ARRIVED:

                monsterEntity = (MonsterEntity)_stateMachine.Entity;
                monsterEntity.OnStopL2jMoving();
    
                 DebugLineDraw.RemoveDrawLineDebug(monsterEntity.IdentityInterlude.Id);

                break;
            case Event.CANCEL:
                Debug.Log("CANCEL Running MONSTER TO POINT!");
                monsterEntity = (MonsterEntity)_stateMachine.Entity;
                break;

        }
    }
}
