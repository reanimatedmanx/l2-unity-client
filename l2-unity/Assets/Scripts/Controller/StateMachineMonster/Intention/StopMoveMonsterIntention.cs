using UnityEditorInternal;
using UnityEngine;

public class StopMoveMonsterIntention : MonsterIntentionBase
{
    public StopMoveMonsterIntention(MonsterStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter(object arg0)
    {

        if (arg0.GetType() == typeof(StopMove))
        {
            StopMove stopMovePacket = (StopMove)arg0;
            _stateMachine.ChangeState(MonsterState.IDLE);
            _stateMachine.NotifyEvent(Event.ARRIVED);
            Debug.Log("StopMoveMonsterIntention>>>>StopMove !!!!");
        }

    }

    public override void Exit()
    {

    }
    public override void Update()
    {

    }

}
