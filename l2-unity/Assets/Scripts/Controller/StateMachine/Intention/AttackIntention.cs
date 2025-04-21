using UnityEngine;
using static AttackingState;

public class AttackIntention : IntentionBase
{
    public AttackIntention(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter(object arg0)
    {
        Transform target = TargetManager.Instance.Target.Data.ObjectTransform;

        //Attack attackpacket = (Attack)arg0;

        //Debug.Log("AttackIntention++++ HP " + TargetManager.Instance.Target.Status.Hp);\\
        //TimeUtils.PrintFullTime("Attack Packet Time Intention " + _stateMachine.State);
        if (target == null)
        {
            //Debug.Log("Target is null, CANCEL event sent");
             _stateMachine.NotifyEvent(Event.CANCEL);
            return;
        }
        PlayerEntity.Instance.isAccesNewAtk = true;
        //default combo 2 attack
        //Attack packet send time 1200ms and UpdatePacket 600ms;
        if (PlayerEntity.Instance.CountAtk >= 2)
        {
            PlayerEntity.Instance.isStop = true;
        }
        PlayerEntity.Instance.CurrentAttackCount = 0;
        PlayerEntity.Instance.CountAtk = 2;
        PlayerEntity.Instance.RefreshRandomPAttack();

        //Debug.Log("Attack Sate to Intention  " + _stateMachine.State);
        //PlayerAnimationController.Instance.SetBool("jatk01_1HS", true, false);
        if (_stateMachine.State != PlayerState.WAIT_RETURN)
        {
            _stateMachine.ChangeState(PlayerState.ATTACKING);

        }
        

        if (_stateMachine.State == PlayerState.ATTACKING)
        {
            if (TargetManager.Instance.IsAttackTargetSet())
            {
                // Already attacking target
                //PlayerStateMachine.Instance.ChangeState(PlayerState.IDLE);
                //PlayerStateMachine.Instance.ChangeState(PlayerState.ATTACKING);
                return;
            }
            else
            {
                //PlayerStateMachine.Instance.ChangeState(PlayerState.IDLE);
                //PlayerStateMachine.Instance.ChangeState(PlayerState.ATTACKING);

                return;
            }

        }

        

        AttackIntentionType type = (AttackIntentionType)arg0;

        Debug.LogWarning((AttackIntentionType)arg0);

        if (type != AttackIntentionType.TargetReached)
        {
            TargetManager.Instance.SetAttackTarget();
        }
        //Debug.Log("Attack Intention!!!!! comment code");
        if (type == AttackIntentionType.WaitReturn)
        {
            _stateMachine.ChangeState(PlayerState.ATTACKING);
           // _stateMachine.NotifyEvent(Event.WAIT_RETURN);
        }
        else
        {
            _stateMachine.ChangeState(PlayerState.IDLE);
            _stateMachine.NotifyEvent(Event.READY_TO_ACT);
        }



       // Vector3 targetPos = TargetManager.Instance.AttackTarget.Data.ObjectTransform.position;

        //float attackRange = ((PlayerInterludeStats)PlayerEntity.Instance.Stats).AttackRange;
       // float distance = Vector3.Distance(PlayerEntity.Instance.transform.position, targetPos);
       // Debug.Log($"target: {target} distance: {distance} range: {attackRange}");

        // Is close enough? Is player already waiting for server reply?
        //if (distance <= attackRange * 0.9f && !_stateMachine.WaitingForServerReply)
        //{
            //_stateMachine.ChangeState(PlayerState.IDLE);
            //_stateMachine.NotifyEvent(Event.READY_TO_ACT);
       // }
       // else
        //{
            // Move to target with a 10% error margin
            //PathFinderController.Instance.MoveTo(targetPos, ((PlayerStats)PlayerEntity.Instance.Stats).AttackRange * 0.9f);
       // }
    }

    public override void Exit() { }
    public override void Update()
    {

    }
}