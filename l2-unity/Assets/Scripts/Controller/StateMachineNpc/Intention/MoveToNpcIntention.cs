using UnityEngine;

public class MoveToNpcIntention : NpcIntentionBase
{
    public MoveToNpcIntention(NpcStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter(object arg0)
    {

        if (arg0.GetType() == typeof(Vector3))
        {
            Vector3 _targetPos = (Vector3)arg0;
            Vector3 npcPos = _stateMachine.NpcObject.transform.position;
           
            //float dist = VectorUtils.Distance2D(_targetPos, monsterPos);
            NpcEntity npcEntity = (NpcEntity)_stateMachine.Entity;
            //if (dist >= 0.5)
            //{
                npcEntity.OnStartL2jMoving((npcEntity.Running) ? false : true);
                _stateMachine.MoveNpc.MoveToTargetPosition(_targetPos);

                if (npcEntity.Running)
                {
                    _stateMachine.ChangeState(NpcState.RUNNING);
                }
                else
                {
                    _stateMachine.ChangeState(NpcState.WALKING);
                }
           // }
        }
        //Debug.Log("Debug EVENT =========== MoveToMonsterIntention");
    }

    public override void Exit()
    {
        //Debug.Log("Debug EVENT =========== MoveToMonsterIntention  EXIT");
    }
    public override void Update()
    {
        //Debug.Log("Debug EVENT MoveToMonsterIntention ATTACK");
    }

}
