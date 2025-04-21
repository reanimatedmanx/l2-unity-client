public class NewDeadState : StateBase
{
    public NewDeadState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        if(PlayerController.Instance.RunningToDestination) PlayerController.Instance.StopMove();
    }
    public override void HandleEvent(Event evt)
    {
        switch (evt)
        {
            case Event.DEAD:
                AnimationManager.Instance.PlayOriginalAnimation(AnimationNames.DEAD.ToString());
                break;
 
        }
    }
}