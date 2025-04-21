
using UnityEditorInternal;
using UnityEngine;

public class NewAttackState : StateBase
{
    public NewAttackState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }


    public override void Update()
    {

    }

    public override void HandleEvent(Event evt)
    {
        switch (evt)
        {
            case Event.READY_TO_ACT:
                Debug.Log(" Attack Sate to Intention Create " + PlayerEntity.Instance.CurrentAttackCount);
                PlayerEntity.Instance.RefreshRandomPAttack();
                AnimationManager.Instance.PlayAnimation(PlayerEntity.Instance.RandomName.ToString(), true);
                break;
            case Event.CANCEL:
                Debug.Log("NewAttackState > Cancel not implements ");
                break;

        }
    }
}