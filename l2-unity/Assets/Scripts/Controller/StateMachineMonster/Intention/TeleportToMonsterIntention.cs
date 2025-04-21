using UnityEngine;

public class TeleportToMonsterIntention : MonsterIntentionBase
{
    public TeleportToMonsterIntention(MonsterStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter(object arg0)
    {
        if(arg0 != null)
        {
            if (arg0.GetType() == typeof(Vector3))
            {

                Vector3 _targetPos = (Vector3)arg0;
                MonsterEntity monsterEntity = (MonsterEntity)_stateMachine.Entity;
                if (_stateMachine.MoveMonster.IsTeleport() == false & !_stateMachine.MoveMonster.IsFollow())
                {
                    _stateMachine.MoveMonster.TeleportToPosition(_targetPos);
                }
                

            }
        }


    }

    public override void Exit()
    {
     
    }
    public override void Update()
    {
  
    }

}
