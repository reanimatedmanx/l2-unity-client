using UnityEditorInternal;
using UnityEngine;

public class FollowMonsterIntention : MonsterIntentionBase
{
    public FollowMonsterIntention(MonsterStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter(object arg0)
    {

        if (arg0.GetType() == typeof(ModelMovePawn))
        {
            //PlayerEntity _target = (PlayerEntity)arg0;
            ModelMovePawn _target = (ModelMovePawn)arg0;
            //_stateMachine.SetTarget(_target);
            //Vector3 tarPos = _target.transform.position;
            Vector3 monsterPos = _stateMachine.MonsterObject.transform.position;
            //float dist = VectorUtils.Distance2D(tarPos, monsterPos);
            //10 metr distance tar and defens
           // if (dist == 0 | dist <= 50)
            //{
               
                _stateMachine.Entity.Running = true;
                _stateMachine.ChangeState(MonsterState.RUNNING);

                // _stateMachine.MoveMonster.FollowToTarget(_target.transform);
                _stateMachine.MoveMonster.MoveToPawn(_target);
           // }

        }
        Debug.Log("Debug EVENT =========== MoveMonsterIntention");
    }

    public override void Exit()
    {
        Debug.Log("Debug EVENT =========== MoveMonsterIntention  EXIT");
    }
    public override void Update()
    {
        Debug.Log("Debug EVENT MoveMonsterIntention ATTACK");
    }
}
