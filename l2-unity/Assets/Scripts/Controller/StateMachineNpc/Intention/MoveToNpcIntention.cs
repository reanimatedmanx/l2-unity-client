using UnityEditorInternal;
using UnityEngine;

public class MoveToNpcIntention : NpcIntentionBase
{
    public MoveToNpcIntention(NpcStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter(object arg0)
    {

        if (arg0.GetType() == typeof(CharMoveToLocation))
        {
            CharMoveToLocation packet = (CharMoveToLocation)arg0;
            Vector3 npcPos = _stateMachine.NpcObject.transform.position;
           
            //float dist = VectorUtils.Distance2D(_targetPos, monsterPos);
            NpcEntity npcEntity = (NpcEntity)_stateMachine.Entity;
            //if (dist >= 0.5)
            //{
                npcEntity.OnStartL2jMoving((npcEntity.Running) ? false : true);
                
                int id = _stateMachine.Entity.IdentityInterlude.Id;
                Debug.Log("Npc MoveToRequest " + npcEntity.name);

                DebugLineDraw.ShowDrawLineDebugNpc(PlayerEntity.Instance.IdentityInterlude.Id, packet.OldPosition, packet.NewPosition, Color.red);

                MovementTarget movementTarget = new MovementTarget(packet.NewPosition, 0.1f);
                MoveAllCharacters.Instance.AddMoveData(id, new MovementData(npcEntity, movementTarget));
                //_stateMachine.MoveNpc.MoveToTargetPosition(_targetPos);

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
